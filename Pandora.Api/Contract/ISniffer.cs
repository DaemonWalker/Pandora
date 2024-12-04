using Pandora.Api.Model;

namespace Pandora.Api.Contract;

public interface ISniffer
{
    public string SourceName { get; }
    Task<IEnumerable<InfoModel>> SniffAsync(SearchModel searchModel);
}