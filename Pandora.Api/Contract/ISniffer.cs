using Pandora.Api.Model;

namespace Pandora.Api.Contract;

public interface ISniffer
{
    public string SourceName { get; }
    Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel);
    public IEnumerable<string> GetAllKeys();
    public Task<Dictionary<string, string>> GetAllConfigurationAsync();
    public Task SetConfigurationAsync(Dictionary<string, string> config);
    public Task<string?> GetMegnetAsync(string url);
    public Task<Stream?> GetTorrentAsync(string url);
}