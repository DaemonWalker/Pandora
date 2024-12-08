using System.Text.Json.Serialization;

namespace Pandora.Api.Model;

public record ResultModel
{
    public string Source { get; init; }
    public IEnumerable<InfoModel> Infos { get; init; }

    public ResultModel(string source, IEnumerable<InfoModel> infos)
    {
        this.Source = source;
        this.Infos = infos;
    }
}