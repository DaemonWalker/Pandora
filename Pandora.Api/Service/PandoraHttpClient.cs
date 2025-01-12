using System.Text.Json;
using HtmlAgilityPack;
using RequestModel = Pandora.Api.Model.PandoraHttpClientRequestModel;

namespace Pandora.Api.Service;

public sealed class PandoraHttpClient(IHttpClientFactory httpClientFactory, HtmlWeb htmlWeb)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<HtmlDocument?> GetAsync(RequestModel request)
    {
        return await GetHtmlDocumentAsync(request);
    }

    public async Task<HtmlDocument?> PostJsonAsync<T>(RequestModel request, T t)
    {
        var json = JsonContent.Create(t, options: _jsonSerializerOptions);
        return await GetHtmlDocumentAsync(request, json);
    }

    public async Task<HtmlDocument?> PostFormAsync(
        RequestModel request,
        Dictionary<string, string> formData
    )
    {
        var content = new FormUrlEncodedContent(formData);
        return await GetHtmlDocumentAsync(request, content);
    }

    private async Task<HtmlDocument?> GetHtmlDocumentAsync(
        RequestModel requestInfo,
        HttpContent? content = null
    )
    {
        var httpClient = httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(requestInfo.Method, requestInfo.Url);
        if (content != null)
        {
            request.Content = content;
        }
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var html = await response.Content.ReadAsStringAsync();
            var doc = htmlWeb.Load(html);
            return doc;
        }
        return null;
    }
}
