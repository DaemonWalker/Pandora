using System.Text;
using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public sealed class CDN1998Sniffer(
    PandoraHttpClient pandoraHttpClient,
    ISnifferConfigurationService<CDN1998Sniffer> snifferConfiguration,
    ILogger<CDN1998Sniffer> logger
) : CDNSnifferBase(pandoraHttpClient, snifferConfiguration, logger)
{
    public override string SourceName => "CDN1998";
    protected override Encoding Encoding => Encoding.UTF8;
}
