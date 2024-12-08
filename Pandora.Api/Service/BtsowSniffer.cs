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
    SnifferConfigurationService snifferConfiguration,
    PandoraDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    ILogger<BtsowSniffer> logger)
    : ISniffer
{
    public string SourceName { get; } = "BTSOW";

    public Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        return SearchAsync(searchModel).AsTask();
    }

    public IEnumerable<string> GetAllKeys()
    {
        return ["url"];
    }

    public async Task SetConfigurationAsync(Dictionary<string, string> config)
    {
        var updateKeys = GetAllKeys().Where(config.ContainsKey).ToList();
        var configurations = updateKeys
            .Select(p => new ConfigurationEntity(SourceName, p, config[p]));

        await dbContext.Configurations.Where(p => p.Type == SourceName && updateKeys.Contains(p.Key))
            .ExecuteDeleteAsync();
        await dbContext.Configurations.AddRangeAsync(configurations);
        await dbContext.SaveChangesAsync();
    }

    private async ValueTask<IEnumerable<InfoModel>> SearchAsync(SearchModel searchModel)
    {
        var url = string.Format(snifferConfiguration.Get(this, "url"), searchModel.Text);
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
        }).ToList();
    }
}