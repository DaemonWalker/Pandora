namespace Pandora.Api.Entity;

public class ConfigurationEntity
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}