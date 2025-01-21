using HtmlAgilityPack;
using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public abstract class CDNSnifferBase(
    PandoraHttpClient pandoraHttpClient,
    SnifferConfigurationService snifferConfiguration,
    ILogger<CDNSnifferBase> logger
) : ISniffer
{
    public abstract string SourceName { get; }

    public IEnumerable<string> GetAllKeys() => ["url", "cookie"];

    public Task SetConfigurationAsync(Dictionary<string, string> config) =>
        snifferConfiguration.UpdateConfigurationAsync(config, GetAllKeys(), SourceName);

    public async Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        var url = snifferConfiguration.Get(this, "url");
        var cookie = snifferConfiguration.Get(this, "cookie");

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(cookie))
        {
            logger.LogWarning("{source} URL or Cookie is not configured", SourceName);
            return [];
        }
        logger.LogTrace("Load configuration success, url: {url}, cookie: {cookie}", url, cookie);

        var searchUrl = $"{url}/search.php?mod=forum&searchid=1053&orderby=lastpost&ascdesc=desc&searchsubmit=yes&kw={searchModel.Text}";
        logger.LogDebug("Search {site} with URL: {url}", SourceName, searchUrl);
        var doc = await pandoraHttpClient.GetAsync(new(searchUrl, HttpMethod.Post, cookie));
        logger.LogTrace("Search result: {doc}", doc?.DocumentNode.OuterHtml);
        if (doc == null)
        {
            return [];
        }
        logger.LogDebug("Get search result success");

        var list = doc.DocumentNode.SelectNodes("//h3[@class='xs3']/a[1]").Take(5).ToList();
        var result = new List<InfoModel>(list.Count);
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

            var forumUrl = $"{url}/{HtmlEntity.DeEntitize(node.GetAttributeValue("href", ""))}";
            logger.LogDebug("Get forum link: {forumUrl}", forumUrl);

            var forum = await pandoraHttpClient.GetAsync(new(forumUrl, HttpMethod.Get, cookie));
            logger.LogTrace("Forum content html: {forum}", forum?.DocumentNode.InnerHtml);

            var torrentLink = forum
                ?.DocumentNode.SelectNodes("//ignore_js_op//span//a")
                .FirstOrDefault()
                ?.GetAttributeValue("href", "");
            logger.LogDebug("Get torrent link: {torrentLink}", torrentLink);
            if (torrentLink != null)
            {
                var torrentUrl = $"{url}/{torrentLink}";
                logger.LogDebug("Get torrent link: {torrentUrl}", torrentUrl);
                // var stream = await pandoraHttpClient.GetStreamAsync(
                //     new(torrentUrl, HttpMethod.Get, cookie)
                // );
                // var torrent = new Honoo.Text.BEncode.TorrentAnalysis(stream, true);
                // var magnet = torrent.GetMagnet(true, true, true, false);
                result.Add(new InfoModel(name, torrentUrl, size, LinkType.Torrent));
            }
            await Task.Delay(500);
        }
        return result;
    }
}
