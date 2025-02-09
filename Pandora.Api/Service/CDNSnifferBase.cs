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
    private int PageSize => snifferConfiguration.Get("pageSize").ToInt(5);
    private int FetchDelay => snifferConfiguration.Get("delay").ToInt(500);

    private void CheckParameter()
    {
        if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Cookie))
        {
            logger.LogWarning("{source} URL or Cookie is not configured", SourceName);
            throw new LogicException($"{SourceName} URL or Cookie is not configured");
        }
    }

    public abstract string SourceName { get; }

    public IEnumerable<string> GetAllKeys() => ["url", "cookie", "pageSize", "delay"];

    public async Task<string?> GetMegnetAsync(string url)
    {
        var stream = await GetTorrentAsync(url);
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

    public Task SetConfigurationAsync(Dictionary<string, string> config) =>
        snifferConfiguration.UpdateConfigurationAsync(config, GetAllKeys());

    public async Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        CheckParameter();

        logger.LogTrace("Load configuration success, url: {url}, cookie: {cookie}", Url, Cookie);

        var searchUrl = $"{Url}/search.php?mod=forum&searchid=1053&orderby=lastpost&ascdesc=desc&searchsubmit=yes&kw={searchModel.Text}";
        logger.LogDebug("Search {site} with URL: {url}", SourceName, searchUrl);
        var doc = await pandoraHttpClient.GetAsync(new(searchUrl, HttpMethod.Post, Cookie)) ?? throw new LogicException("Get search result failed");
        logger.LogDebug("Get search result success");

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
            string size;
            if (text.Length > 1)
            {
                size = text[1];
            }
            else
            {
                size = node.InnerText.Split(' ').Last();
            }

            logger.LogTrace("Link outer html: {node}", node.OuterHtml);

            var forumUrl = $"{Url}/{HtmlEntity.DeEntitize(node.GetAttributeValue("href", ""))}";
            logger.LogDebug("Get forum link: {forumUrl}", forumUrl);

            var forum = await pandoraHttpClient.GetAsync(new(forumUrl, HttpMethod.Get, Cookie));
            logger.LogTrace("Forum content html: {forum}", forum?.DocumentNode.InnerHtml);

            var torrentLink = forum
                ?.DocumentNode.SelectNodes("//ignore_js_op//span//a")
                .FirstOrDefault()
                ?.GetAttributeValue("href", "");
            logger.LogDebug("Get torrent link: {torrentLink}", torrentLink);
            if (torrentLink != null)
            {
                var torrentUrl = $"{Url}/{torrentLink}";
                logger.LogDebug("Get torrent link: {torrentUrl}", torrentUrl);

                result.Add(new InfoModel(name, torrentUrl, size, LinkType.Torrent));
            }
            await Task.Delay(FetchDelay);
        }
        return result;
    }

    public Task<Dictionary<string, string>> GetAllConfigurationAsync()
    {
        logger.LogDebug("Start getting all configuration information for {SourceName}", SourceName);
        return snifferConfiguration.GetAllConfigurationAsync();
    }
}
