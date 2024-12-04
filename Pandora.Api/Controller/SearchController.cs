using Microsoft.AspNetCore.Mvc;
using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Controller;

[ApiController]
public class SearchController(IEnumerable<ISniffer> sniffers, ILogger<SearchController> logger) : ControllerBase
{
    public async Task<IActionResult> SearchAsync(string text)
    {
        var searchModel = new SearchModel() { Text = text };
        var tasks = sniffers.ToDictionary(p => p.SourceName, p => p.SniffAsync(searchModel));
        var result = await Task.WhenAll(tasks.Values);
    }
}