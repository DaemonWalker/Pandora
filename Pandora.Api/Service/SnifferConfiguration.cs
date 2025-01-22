using Microsoft.EntityFrameworkCore;
using Pandora.Api.Contract;
using Pandora.Api.Data;
using Pandora.Api.Entity;

namespace Pandora.Api.Service;

public class SnifferConfigurationService<T>(PandoraDbContext dbContext) : ISnifferConfigurationService<T> where T : class
{
    private readonly string SourceName = typeof(T).FullName!;
    public string? Get(string key)
    {
        var configuration = dbContext
            .Configurations.Where(c => c.Type == SourceName && c.Key == key)
            .ToDictionary(p => p.Key, p => p.Value);
        return configuration.GetValueOrDefault(key);
    }

    public async Task UpdateConfigurationAsync(Dictionary<string, string> config, IEnumerable<string> keys)
    {
        var updateKeys = keys.Where(config.ContainsKey).ToList();
        var configurations = updateKeys.Select(p =>
        {
            if (string.Compare("url", p, true) != 0)
            {
                var url = config[p].EndsWith('/') ? config[p][..^1] : config[p];
                return new ConfigurationEntity(SourceName, p, url);
            }
            else
            {
                return new ConfigurationEntity(SourceName, p, config[p]);
            }
        });

        await dbContext
            .Configurations.Where(p => p.Type == SourceName && updateKeys.Contains(p.Key))
            .ExecuteDeleteAsync();
        await dbContext.Configurations.AddRangeAsync(configurations);
        await dbContext.SaveChangesAsync();
    }
}