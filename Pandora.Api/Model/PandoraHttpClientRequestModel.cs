namespace Pandora.Api.Model;

public record PandoraHttpClientRequestModel(string Url, HttpMethod Method, string? Cookie);
