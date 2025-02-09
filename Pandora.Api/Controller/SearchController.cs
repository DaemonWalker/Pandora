using Microsoft.AspNetCore.Mvc;
using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
public class SearchController(IEnumerable<ISniffer> sniffers, ILogger<SearchController> logger)
    : ControllerBase
{
    [HttpGet("{text}")]
    public async Task<IEnumerable<ResultModel>> SearchAsync(string text)
    {
        var searchModel = new SearchModel() { Text = text };
        var result = await Task.WhenAll(sniffers.Select(p => p.SniffAsync(searchModel)));
        return result.Select((p, idx) => new ResultModel(sniffers.ElementAt(idx).SourceName, p));
    }

    [HttpGet("{source}/{text}")]
    public async Task<IEnumerable<ResultModel>> SearchBySourceAsync([FromRoute] string source, [FromRoute] string text)
    {
        logger.LogDebug("Search {text} from {source}", text, source);
        var searchModel = new SearchModel() { Text = text };
        var sniffer = GetSniffer(source);

        var result = await sniffer.SniffAsync(searchModel);
        return [new(source, result)];
    }

    [HttpPost("{source}")]
    public async Task<IActionResult> GetMagnetAsync([FromRoute] string source, [FromBody] string url)
    {
        logger.LogDebug("Get magnet for {input} from {source}", url, source);
        var sniffer = GetSniffer(source);

        var magnet = await sniffer.GetMegnetAsync(url);
        return Ok(magnet);
    }

    private ISniffer GetSniffer(string source)
    {
        var sniffer = sniffers.FirstOrDefault(p => p.SourceName == source);
        if (sniffer == null)
        {
            logger.LogWarning("No such source: {name}", source);
            throw new LogicException("No such source");
        }
        return sniffer;
    }
}
