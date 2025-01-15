using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public sealed class CDN1998Sniffer(
    PandoraHttpClient pandoraHttpClient,
    SnifferConfigurationService snifferConfiguration,
    ILogger<CDN1998Sniffer> logger
) : CDNSnifferBase(pandoraHttpClient, snifferConfiguration, logger)
{
    public override string SourceName => "CDN1998";
}
