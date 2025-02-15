using System.Text;

namespace Pandora.Api.Model;

public record PandoraHttpClientRequestModel(in string Url, in HttpMethod Method,
    in string? Cookie = null, in Encoding? Encoding1 = null)
{
    public Encoding Encoding => Encoding1 ?? Encoding.UTF8;
}
