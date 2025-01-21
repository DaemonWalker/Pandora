using Pandora.Api.Contract;
using Pandora.Api.Model;

namespace Pandora.Api.Service;

public sealed class CDN1999Sniffer(
    PandoraHttpClient pandoraHttpClient,
    SnifferConfigurationService snifferConfiguration,
    ILogger<CDN1999Sniffer> logger
) : CDNSnifferBase(pandoraHttpClient, snifferConfiguration, logger)
{
    public override string SourceName => "CDN1999";
}
