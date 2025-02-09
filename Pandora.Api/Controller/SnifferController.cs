using System.Collections;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pandora.Api.Contract;

namespace Pandora.Api.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
public class SnifferController(IEnumerable<ISniffer> sniffers) : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> GetAllSource()
    {
        return sniffers.Select(p => p.SourceName);
    }

    [HttpGet("{source}")]
    public IEnumerable<string>? GetAllKeys(string source)
    {
        var sniffer = sniffers.FirstOrDefault(p => p.SourceName == source);
        return sniffer?.GetAllKeys() ?? [];
    }

    [HttpGet("{source}")]
    public Task<Dictionary<string, string>> GetAllConfigurationAsync(string source)
    {
        var sniffer = sniffers.FirstOrDefault(p => p.SourceName == source);
        return sniffer?.GetAllConfigurationAsync() ?? Task.FromResult(new Dictionary<string, string>());
    }

    [HttpPost("{source}")]
    public async Task<IActionResult>? SetConfigurationAsync([FromRoute] string source,
        [FromBody] Dictionary<string, string> config)
    {
        var sniffer = sniffers.FirstOrDefault(p => p.SourceName == source);
        await (sniffer?.SetConfigurationAsync(config) ?? Task.CompletedTask);
        return Ok();
    }
}