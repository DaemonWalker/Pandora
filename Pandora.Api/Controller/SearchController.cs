using Microsoft.AspNetCore.Mvc;
using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Controller;

[ApiController]
[Route("[controller]/[action]")]
public class SearchController(IEnumerable<ISniffer> sniffers, ILogger<SearchController> logger) : ControllerBase
{
    [HttpGet("{text}")]
    public async Task<IEnumerable<ResultModel>> SearchAsync(string text)
    {
        var searchModel = new SearchModel() { Text = text };
        var result = await Task.WhenAll(sniffers.Select(p => p.SniffAsync(searchModel)));
        return result.Select((p, idx) => new ResultModel(sniffers.ElementAt(idx).SourceName, p)).ToList();
    }
}