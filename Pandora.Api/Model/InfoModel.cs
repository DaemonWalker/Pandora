namespace Pandora.Api.Model;

public record InfoModel
{
    public InfoModel(string name, string link, string? size = null, LinkType linkType = LinkType.Magnet)
    {
        Name = name;
        Link = link;
        Size = size;
        LinkType = linkType;
    }

    public string Name { get; init; }
    public string? Link { get; init; }
    public string? Size { get; init; }
    public LinkType LinkType { get; init; }
}