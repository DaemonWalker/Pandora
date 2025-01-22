using System.Text.Json;
using HtmlAgilityPack;
using RequestModel = Pandora.Api.Model.PandoraHttpClientRequestModel;

namespace Pandora.Api.Service;

public sealed class PandoraHttpClient(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<HtmlDocument?> GetAsync(RequestModel request)
    {
        return await HtmlDocumentAsync(request);
    }

    public Task<Stream?> GetStreamAsync(RequestModel request)
    {
        return StreamAsync(request);
    }

    public async Task<HtmlDocument?> PostJsonAsync<T>(RequestModel request, T t)
    {
        var json = JsonContent.Create(t, options: _jsonSerializerOptions);
        return await HtmlDocumentAsync(request, json);
    }

    public async Task<HtmlDocument?> PostFormAsync(RequestModel request, Dictionary<string, string> formData)
    {
        var content = new FormUrlEncodedContent(formData);
        return await HtmlDocumentAsync(request, content);
    }

    private async Task<HtmlDocument?> HtmlDocumentAsync(RequestModel requestInfo, HttpContent? content = null)
    {
        var response = await RequestAsync(requestInfo, content);
        if (response?.IsSuccessStatusCode == true)
        {
            var doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync());
            return doc;
        }
        return null;
    }

    private async Task<Stream?> StreamAsync(RequestModel requestInfo, HttpContent? content = null)
    {
        var response = await RequestAsync(requestInfo, content);
        if (response?.IsSuccessStatusCode == true)
        {
            return await response.Content.ReadAsStreamAsync();
        }
        return null;
    }

    private async Task<HttpResponseMessage?> RequestAsync(RequestModel requestInfo, HttpContent? content = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(requestInfo.Method, requestInfo.Url);
        if (content != null)
        {
            request.Content = content;
        }
        if (!string.IsNullOrEmpty(requestInfo.Cookie))
        {
            request.Headers.Add("Cookie", requestInfo.Cookie);
        }
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 Edg/132.0.0.0");
        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode ? response : null;
    }
}
