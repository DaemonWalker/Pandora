namespace Pandora.Api.Model;

public record InfoModel
{
    public InfoModel(string name, string magnetLink, string? size)
    {
        Name = name;
        MagnetLink = magnetLink;
        Size = size;
    }

    public string Name { get; init; }
    public string MagnetLink { get; init; }
    public string? Size { get; init; }
}