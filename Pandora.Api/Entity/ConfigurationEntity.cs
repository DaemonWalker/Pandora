using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Api.Entity;

[Table("configurations")]
public class ConfigurationEntity
{
    [Key] public int Id { get; set; }
    [Column("type")] public string Type { get; set; } = null!;
    [Column("key")] public string Key { get; set; } = null!;
    [Column("value")] public string Value { get; set; } = null!;

    public ConfigurationEntity()
    {
    }

    public ConfigurationEntity(string type, string key, string value)
    {
        Type = type;
        Key = key;
        Value = value;
    }
}