using Pandora.Api.Data;

namespace Pandora.Api.Service;

public class SnifferConfigurationService<T>
{
    private IReadOnlyDictionary<string, string> configuration;

    public SnifferConfigurationService(PandoraDbContext dbContext)
    {
        var type = typeof(T).FullName;
        var list = dbContext.Configurations.Where(p => p.Type == type).ToList();
        configuration = list.ToDictionary(p => p.Key, p => p.Value);
    }
    

    public string Get(string key)
    {
        return configuration[key];
    }

    public string this[string key]
    {
        get => configuration[key];
    }
}