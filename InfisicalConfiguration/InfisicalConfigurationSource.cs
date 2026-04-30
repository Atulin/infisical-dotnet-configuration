using Microsoft.Extensions.Configuration;

namespace InfisicalConfiguration;

internal class InfisicalConfigurationSource(InfisicalConfig config) : IConfigurationSource
{
  public IConfigurationProvider Build(IConfigurationBuilder builder)
  {
    return new InfisicalConfigurationProvider(config);
  }
}