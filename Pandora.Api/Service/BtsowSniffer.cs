using System.Text;
using System.Web;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Pandora.Api.Contract;
using Pandora.Api.Data;
using Pandora.Api.Entity;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public class BtsowSniffer(
    ISnifferConfigurationService<BtsowSniffer> snifferConfiguration,
    ILogger<BtsowSniffer> logger
) : ISniffer
{
    public string SourceName { get; } = "BTSOW";

    public Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        return SearchAsync(searchModel).AsTask();
    }

    public IEnumerable<string> GetAllKeys() => ["url"];

    public Task SetConfigurationAsync(Dictionary<string, string> config)
    {
        return snifferConfiguration.UpdateConfigurationAsync(config, GetAllKeys());
    }

    private async ValueTask<IEnumerable<InfoModel>> SearchAsync(SearchModel searchModel)
    {
        var config = snifferConfiguration.Get("url");
        if (string.IsNullOrEmpty(config))
        {
            logger.LogWarning("{source} URL is not configured", SourceName);
            return [];
        }
        var url = string.Format(config, searchModel.Text);
        logger.LogDebug("Search Btsow with URL: {url}", url);

        //url = HttpUtility.UrlEncode(url);

        var web = new HtmlWeb();
        var page = await web.LoadFromWebAsync(url, Encoding.UTF8);
        var nodes = page.DocumentNode.SelectNodes("//*[@class='data-list']//*[@class='row']");

        logger.LogDebug("Get {count} result of searching {url}", nodes.Count, url);
        return nodes.Select(node =>
        {
            var a = node.Descendants("a").First();
            var href = a.GetAttributeValue("href", "");
            var title = a.GetAttributeValue("title", "");
            var magnet = href.Split('/').Last();
            var sizeBlock = node.SelectNodes("//div[contains(@class, 'size')]").Last();
            var size = sizeBlock.InnerText;
            return new InfoModel(title, $"magnet:?xt=urn:btih:{magnet}", size);
        });
    }

    public Task<string?> GetMegnetAsync(string url)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> GetTorrentAsync(string url)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> GetAllConfigurationAsync()
    {
        logger.LogDebug("Start getting all configuration information for {SourceName}", SourceName);
        return snifferConfiguration.GetAllConfigurationAsync();
    }
}
