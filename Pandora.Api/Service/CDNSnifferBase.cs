using System.Text;
using System.Web;
using HtmlAgilityPack;
using Pandora.Api.Contract;
using Pandora.Api.Extension;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public abstract class CDNSnifferBase(
    PandoraHttpClient pandoraHttpClient,
    ISnifferConfigurationService<CDNSnifferBase> snifferConfiguration,
    ILogger<CDNSnifferBase> logger
) : ISniffer
{
    private string? Url => snifferConfiguration.Get("url");
    private string? Cookie => snifferConfiguration.Get("cookie");
    private int PageSize => snifferConfiguration.Get("pageSize").ToInt(10);
    private string? FormHash => snifferConfiguration.Get("formhash");

    private void CheckParameter()
    {
        if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Cookie) || string.IsNullOrEmpty(FormHash))
        {
            logger.LogWarning("{source} URL, Cookie, FormHash is not configured", SourceName);
            throw new LogicException($"{SourceName} URL, Cookie, FormHash is not configured");
        }
    }

    public abstract string SourceName { get; }
    protected abstract Encoding Encoding { get; }

    public IEnumerable<string> GetAllKeys() => ["url", "cookie", "formhash", "pageSize"];

    public async Task<string?> GetMegnetAsync(string url)
    {
        var forum = await pandoraHttpClient.GetAsync(new(url, HttpMethod.Get, Cookie, Encoding));
        logger.LogTrace("Forum content html: {forum}", forum?.DocumentNode.InnerHtml);

        var torrentLink = forum
            ?.DocumentNode.SelectNodes("//ignore_js_op//span//a")
            .FirstOrDefault()
            ?.GetAttributeValue("href", "");
        logger.LogDebug("Get torrent link: {torrentLink}", torrentLink);
        if (string.IsNullOrEmpty(torrentLink))
        {
            return null;
        }

        var stream = await GetTorrentAsync($"{Url}/{HttpUtility.HtmlDecode(torrentLink)}");
        if (stream == null)
        {
            return null;
        }
        var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream, true);
        var magnet = torrent.GetMagnet(true, true, true, false);
        return magnet;

    }

    public async Task<Stream?> GetTorrentAsync(string url)
    {
        CheckParameter();
        var stream = await pandoraHttpClient.GetStreamAsync(new(url, HttpMethod.Get, Cookie)) ?? throw new LogicException("");
        return stream;
    }

    public async Task SetConfigurationAsync(Dictionary<string, string> config) =>
        await snifferConfiguration.UpdateConfigurationAsync(config, GetAllKeys());

    public async Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        CheckParameter();
        logger.LogDebug("Search {site} with URL: {url}", SourceName, Url);
        var doc = await pandoraHttpClient.PostFormAsync(new($"{Url}/search.php?mod=forum", HttpMethod.Post, Cookie, Encoding),
            new KeyValuePair<string, string>("formhash", FormHash!),
            new KeyValuePair<string, string>("srchtxt", searchModel.Text),
            new KeyValuePair<string, string>("searchsubmit", "yes")) ?? throw new LogicException("Get search result failed");

        logger.LogDebug("Get search result success");
        logger.LogTrace("Get search result: {html}", doc.DocumentNode.OuterHtml);
        var hrefCollection = doc.DocumentNode.SelectNodes("//h3[@class='xs3']/a[1]");
        if (hrefCollection == null || hrefCollection.Count == 0)
        {
            logger.LogWarning("No search result found");
            throw new LogicException("Cookie is invalid");
        }
        var list = hrefCollection.Skip(PageSize * (searchModel.Page ?? 1 - 1)).Take(PageSize);
        var result = new List<InfoModel>(PageSize);

        foreach (var node in list)
        {
            var text = node.InnerText.Split('\n');
            var name = text[0];
            var size = text.Length > 1 ? text[1] : text.Last();

            logger.LogTrace("Link outer html: {node}", node.OuterHtml);

            var forumUrl = $"{Url}/{HttpUtility.HtmlDecode(node.GetAttributeValue("href", ""))}";
            logger.LogDebug("Get forum link: {forumUrl}", forumUrl);

            result.Add(new InfoModel(name, forumUrl, size, LinkType.Torrent));
        }

        return result;
    }

    public Task<Dictionary<string, string>> GetAllConfigurationAsync()
    {
        logger.LogDebug("Start getting all configuration information for {SourceName}", SourceName);
        return snifferConfiguration.GetAllConfigurationAsync();
    }
}
