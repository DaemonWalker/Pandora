using Pandora.Api.Model;

namespace Pandora.Api.Contract;

public interface ISnifferConfigurationService<out T>
{
    Task UpdateConfigurationAsync(Dictionary<string, string> config, IEnumerable<string> keys);
    string? Get(string key);
    Task<Dictionary<string, string>> GetAllConfigurationAsync();
}