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
        return await HtmlDocumentAsync(request);
    }

    public Task<Stream> GetStreamAsync(RequestModel request)
    {
        return StreamAsync(request);
    }

    public async Task<HtmlDocument?> PostJsonAsync<T>(RequestModel request, T t)
    {
        var json = JsonContent.Create(t, options: _jsonSerializerOptions);
        return await HtmlDocumentAsync(request, json);
    }

    public async Task<HtmlDocument?> PostFormAsync(
        RequestModel request,
        Dictionary<string, string> formData
    )
    {
        var content = new FormUrlEncodedContent(formData);
        return await HtmlDocumentAsync(request, content);
    }

    private async Task<HtmlDocument?> HtmlDocumentAsync(
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

    private async Task<Stream> StreamAsync(RequestModel requestInfo, HttpContent? content = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(requestInfo.Method, requestInfo.Url)
        {
            Content = content,
        };
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStreamAsync();
    }
}
