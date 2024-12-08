using Pandora.Api.Contract;
using Pandora.Api.Data;

namespace Pandora.Api.Service;

public class SnifferConfigurationService(PandoraDbContext dbContext)
{
    public string Get(ISniffer sniffer, string key)
    {
        var configuration = dbContext.Configurations.Where(
                c => c.Type == sniffer.SourceName && c.Key == key)
            .ToDictionary(p => p.Key, p => p.Value);
        return configuration[key];
    }
}