using Microsoft.Extensions.Configuration;

namespace InfisicalConfiguration;

/// <summary>
/// Extension methods for adding Infisical as a configuration source.
/// </summary>
public static class InfisicalConfigurationExtensions
{
	/// <summary>
	/// Adds Infisical as a configuration source to the <see cref="IConfigurationBuilder"/>.
	/// </summary>
	/// <param name="builder">The configuration builder to add Infisical to.</param>
	/// <param name="config">The Infisical configuration built with <see cref="InfisicalConfigBuilder"/>.</param>
	/// <returns>The <see cref="IConfigurationBuilder"/> for further chaining.</returns>
	public static IConfigurationBuilder AddInfisical(
		this IConfigurationBuilder builder,
		InfisicalConfig config
	)
	{
		return builder.Add(new InfisicalConfigurationSource(config));
	}
}