using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public sealed class CDN1998Sniffer(
    PandoraHttpClient pandoraHttpClient,
    SnifferConfigurationService snifferConfiguration,
    ILogger<CDN1998Sniffer> logger
) : ISniffer
{
    public string SourceName => "CDN1998";

    public IEnumerable<string> GetAllKeys() => ["url", "cookie"];

    public Task SetConfigurationAsync(Dictionary<string, string> config) =>
        snifferConfiguration.UpdateConfigurationAsync(config, GetAllKeys(), SourceName);

    public async Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel)
    {
        var url = snifferConfiguration.Get(this, "url");
        var cookie = snifferConfiguration.Get(this, "cookie");
        var hash = snifferConfiguration.Get(this, "hash") ?? "50a6a055";

        logger.LogDebug(
            "Search CDN1998 with URL: {url}, name: {name} hash: {hash}",
            url,
            searchModel.Text,
            hash
        );
        var doc = await pandoraHttpClient.PostFormAsync(
            new($"{url}/search.php?searchsubmit=yes", HttpMethod.Post, cookie),
            new()
            {
                { "formhash", hash },
                { "srchtxt", searchModel.Text },
                { "searchsubmit", "yes" },
            }
        );
        if (doc == null)
        {
            return [];
        }

        var list = doc.DocumentNode.SelectNodes("//h3[@class='xs3']/a[1]");
        var result = new List<InfoModel>(list.Count);
        foreach (var node in list)
        {
            var text = node.InnerText.Split('\n');
            var name = text[0];
            var size = text[1].Replace("\t", "");

            var forumUrl = $"{url}/{node.GetAttributeValue("href", "")}";
            var forum = await pandoraHttpClient.GetAsync(new(forumUrl, HttpMethod.Get, cookie));

            result.Add(new InfoModel(name, "", size));
        }
    }
}
