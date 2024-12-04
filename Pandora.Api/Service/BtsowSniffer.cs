using System.Text;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public class BtsowSniffer(
    SnifferConfigurationService<BtsowSniffer> snifferConfiguration,
    DbContext dbContext,
    IHttpClientFactory httpClientFactory,
    ILogger<BtsowSniffer> logger)
    : ISniffer
{
    public string SourceName { get; } = "BTSOW";

    public Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        return SearchAsync(searchModel).AsTask();
    }

    private async ValueTask<IEnumerable<InfoModel>> SearchAsync(SearchModel searchModel)
    {
        var url = string.Format(snifferConfiguration.Get("url"), searchModel.Text);
        logger.LogDebug("Search Btsow with URL: {url}", url);

        url = Uri.EscapeDataString(url);

        var web = new HtmlWeb();
        var page = await web.LoadFromWebAsync(url, Encoding.UTF8);
        var nodes = page.DocumentNode.SelectNodes("//*[@class='data-list']//*[@class='row']");
        return nodes.Select(node =>
        {
            var a = node.Descendants("a").First();
            var href = a.GetAttributeValue("href", "");
            var title = a.GetAttributeValue("title", "");
            var magnet = href.Split('/').Last();
            var sizeBlock = a.SelectNodes("//div[contains(@class, 'size')]").First();
            var size = sizeBlock.InnerText;
            return new InfoModel(title, $"magnet:?xt=urn:btih:{magnet}", size);
        });
    }
}