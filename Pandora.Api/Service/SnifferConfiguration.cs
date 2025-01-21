using Microsoft.EntityFrameworkCore;
using Pandora.Api.Contract;
using Pandora.Api.Data;
using Pandora.Api.Entity;

namespace Pandora.Api.Service;

public class SnifferConfigurationService(PandoraDbContext dbContext)
{
    public string? Get(ISniffer sniffer, string key)
    {
        var configuration = dbContext
            .Configurations.Where(c => c.Type == sniffer.SourceName && c.Key == key)
            .ToDictionary(p => p.Key, p => p.Value);
        return configuration.GetValueOrDefault(key);
    }

    public async Task UpdateConfigurationAsync(Dictionary<string, string> config, IEnumerable<string> keys, string sourceName)
    {
        var updateKeys = keys.Where(config.ContainsKey).ToList();
        var configurations = updateKeys.Select(p =>
        {
            if (string.Compare("url", p, true) != 0)
            {
                var url = config[p].EndsWith('/') ? config[p][..^1] : config[p];
                return new ConfigurationEntity(sourceName, p, url);
            }
            else
            {
                return new ConfigurationEntity(sourceName, p, config[p]);
            }
        });

        await dbContext
            .Configurations.Where(p => p.Type == sourceName && updateKeys.Contains(p.Key))
            .ExecuteDeleteAsync();
        await dbContext.Configurations.AddRangeAsync(configurations);
        await dbContext.SaveChangesAsync();
    }
}