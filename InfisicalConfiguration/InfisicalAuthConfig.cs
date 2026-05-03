namespace InfisicalConfiguration;

internal sealed record UniversalAuthCredentials(string ClientId, string ClientSecret);

internal class AzureCustomProviderAuthCredentials
{
	public string IdentityId { get; }
	public Func<Task<string>> TokenProvider { get; }

	public AzureCustomProviderAuthCredentials(string identityId, Func<Task<string>> tokenProvider)
	{
		if (string.IsNullOrEmpty(identityId))
		{
			throw new ArgumentNullException(nameof(identityId));
		}

		IdentityId = identityId;
		TokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
	}
}

internal enum InfisicalAuthType
{
	Universal,
	AzureCustomProvider,
}

/// <summary>
/// Represents the authentication configuration for connecting to Infisical.
/// Instances are created using <see cref="InfisicalAuthBuilder"/>.
/// </summary>
public class InfisicalAuth
{
	internal InfisicalAuthType AuthType { get; private set; }
	private UniversalAuthCredentials? _universalAuthCredentials;
	private AzureCustomProviderAuthCredentials? _azureCustomProviderAuthCredentials;
	
	internal InfisicalAuth() {}

	internal UniversalAuthCredentials GetUniversalAuth()
	{
		if (_universalAuthCredentials == null)
		{
			throw new InvalidOperationException("UniversalAuth must be set");
		}

		if (AuthType == InfisicalAuthType.Universal)
		{
			return _universalAuthCredentials;
		}

		throw new InvalidOperationException("AuthType must be set. Are you missing a call to SetUniversalAuth?");
	}

	internal AzureCustomProviderAuthCredentials GetAzureCustomProviderAuth()
	{
		if (_azureCustomProviderAuthCredentials == null)
		{
			throw new InvalidOperationException("Azure auth must be set");
		}

		if (AuthType == InfisicalAuthType.AzureCustomProvider)
		{
			return _azureCustomProviderAuthCredentials;
		}

		throw new InvalidOperationException("AuthType must be set. Are you missing a call to SetAzureAuth?");
	}

	internal void SetUniversalAuthCredentials(UniversalAuthCredentials credentials)
	{
		_universalAuthCredentials = credentials;
		AuthType = InfisicalAuthType.Universal;
	}

	internal void SetAzureAuthCredentials(AzureCustomProviderAuthCredentials credentials)
	{
		_azureCustomProviderAuthCredentials = credentials;
		AuthType = InfisicalAuthType.AzureCustomProvider;
	}
}

/// <summary>
/// A fluent builder for constructing <see cref="InfisicalAuth"/> instances.
/// Exactly one authentication method must be configured before calling <see cref="Build"/>.
/// </summary>
public class InfisicalAuthBuilder
{
	private readonly InfisicalAuth _auth = new();

	/// <summary>
	/// Configures Universal Auth as the authentication method.
	/// </summary>
	/// <param name="clientId">The client ID of your universal auth machine identity.</param>
	/// <param name="clientSecret">The client secret of your universal auth machine identity.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalAuthBuilder SetUniversalAuth(string clientId, string clientSecret)
	{
		_auth.SetUniversalAuthCredentials(new UniversalAuthCredentials(clientId, clientSecret));
		return this;
	}

	/// <summary>
	/// Configures Azure (Entra ID) as the authentication method using a custom token provider.
	/// </summary>
	/// <param name="identityId">The ID of the Infisical identity to authenticate with.</param>
	/// <param name="tokenProvider">An async function that returns an Entra ID JWT token.
	/// This token will be exchanged with Infisical for an access token.</param>
	/// <returns>This builder instance for method chaining.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when <paramref name="identityId"/> is null or empty, or <paramref name="tokenProvider"/> is null.
	/// </exception>
	public InfisicalAuthBuilder SetAzureAuth(string identityId, Func<Task<string>> tokenProvider)
	{
		if (string.IsNullOrEmpty(identityId))
		{
			throw new InvalidOperationException("IdentityId must be set");
		}

		if (tokenProvider == null)
		{
			throw new InvalidOperationException("TokenProvider must be set");
		}

		_auth.SetAzureAuthCredentials(new AzureCustomProviderAuthCredentials(identityId, tokenProvider));
		return this;
	}

	/// <summary>
	/// Validates the configured authentication and builds an <see cref="InfisicalAuth"/> instance.
	/// </summary>
	/// <returns>A configured <see cref="InfisicalAuth"/> instance.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when no authentication method has been configured, or when the configured
	/// credentials are incomplete.
	/// </exception>
	public InfisicalAuth Build()
	{
		switch (_auth.AuthType)
		{
			case InfisicalAuthType.Universal:
				var universalAuth = _auth.GetUniversalAuth();
				if (string.IsNullOrEmpty(universalAuth.ClientId) || string.IsNullOrEmpty(universalAuth.ClientSecret))
				{
					throw new InvalidOperationException("ClientId and ClientSecret must be set");
				}
				break;
			case InfisicalAuthType.AzureCustomProvider:
				var azureCustomProviderAuth = _auth.GetAzureCustomProviderAuth();
				if (string.IsNullOrEmpty(azureCustomProviderAuth.IdentityId))
				{
					throw new InvalidOperationException("IdentityId must be set");
				}
				if (azureCustomProviderAuth.TokenProvider == null)
				{
					throw new InvalidOperationException("TokenProvider must be set");
				}

				break;
			default:
				throw new InvalidOperationException("AuthType must be set. Are you missing a call to SetUniversalAuth or SetAzureAuth?");
		}

		return _auth;
	}
}