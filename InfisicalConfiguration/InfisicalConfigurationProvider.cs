using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace InfisicalConfiguration;

internal class InfisicalConfigurationProvider : ConfigurationProvider, IDisposable
{
	private readonly HttpClient _httpClient;
	private readonly Dictionary<string, string> _secretsCache = new();

	private readonly InfisicalConfig _config;


	public InfisicalConfigurationProvider(InfisicalConfig config)
	{
		_config = config;

		_httpClient = new HttpClient
		{
			BaseAddress = new Uri(_config.InfisicalUrl)
		};

		var accessToken = Authenticate();

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
	}

	private string Authenticate()
	{
		var auth = _config.Auth;

		if (auth is null)
		{
			throw new InvalidOperationException("Auth details not provided");
		}

		return auth.AuthType switch
		{
			InfisicalAuthType.Universal => UniversalAuthLogin(),
			InfisicalAuthType.AzureCustomProvider => AzureAuthLogin(),
			_ => throw new InvalidOperationException("AuthType must be set. Are you missing a call to SetUniversalAuth?")
		};

	}

	private string UniversalAuthLogin()
	{
		var auth = _config.Auth;

		if (auth is null)
		{
			throw new InvalidOperationException("Auth details not provided");
		}
		var universalAuth = auth.GetUniversalAuth();

		var body = new
		{
			clientId = universalAuth.ClientId,
			clientSecret = universalAuth.ClientSecret
		};

		const string url = "/api/v1/auth/universal-auth/login";

		var response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Post, url)
		{
			Content = JsonContent.Create(body)
		});

		response.EnsureSuccessStatusCode();

		var machineIdentityLogin = MachineIdentityLogin.Deserialize(
			response.Content.ReadAsString()
		);

		return machineIdentityLogin.AccessToken;
	}

	private string AzureAuthLogin()
	{
		var auth = _config.Auth;

		if (auth is null)
		{
			throw new InvalidOperationException("Auth details not provided");
		}
		var azureAuth = auth.GetAzureCustomProviderAuth();

		var body = new
		{
			identityId = azureAuth.IdentityId,
			jwt = azureAuth.TokenProvider().GetAwaiter().GetResult()
		};

		var response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/azure-auth/login")
		{
			Content = JsonContent.Create(body)
		});

		response.EnsureSuccessStatusCode();

		var machineIdentityLogin = MachineIdentityLogin.Deserialize(
			response.Content.ReadAsString()
		);

		return machineIdentityLogin.AccessToken;
	}

	public override void Load()
	{
		try
		{
			var prefix = _config.Prefix;

			var url = $"/api/v3/secrets/raw/?environment={_config.Environment}&workspaceId={_config.ProjectId}&secretPath={_config.SecretPath}&include_imports=true&expandSecretReferences={_config.ExpandSecretReferences.ToString().ToLower()}";

			// ReSharper disable once MethodHasAsyncOverload
			var response = _httpClient.Send(new HttpRequestMessage(HttpMethod.Get, url));
			var content = response.Content.ReadAsString();
			response.EnsureSuccessStatusCode();
			var secrets = SecretsList.Deserialize(content);
	
			secrets.Secrets.Reverse();
			_secretsCache.Clear();

			foreach (var secret in secrets.Secrets)
			{
				_secretsCache[secret.SecretKey] = secret.SecretValue;
			}

			foreach (var secret in _secretsCache)
			{
				var key = prefix + secret.Key.Replace("__", ":");
				Data[key] = secret.Value;
			}
		}
		catch
		{
			foreach (var secret in _secretsCache)
			{
				Data[secret.Key] = secret.Value;
			}

			throw;
		}
	}

	public void Dispose()
	{
		_httpClient.Dispose();
	}
}