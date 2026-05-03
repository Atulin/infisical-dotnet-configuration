namespace InfisicalConfiguration;

/// <summary>
/// Represents the configuration for connecting to an Infisical project.
/// Instances are created using <see cref="InfisicalConfigBuilder"/>.
/// </summary>
public class InfisicalConfig
{
	// Required properties
	internal string Environment { get; }
	internal string ProjectId { get; }
	internal InfisicalAuth Auth { get; }

	// Optional properties with defaults
	internal string SecretPath { get; }
	internal string InfisicalUrl { get; }
	internal string Prefix { get; }
	internal bool ExpandSecretReferences { get; set; }

	internal InfisicalConfig(
		string environment,
		string projectId,
		InfisicalAuth auth,
		string secretPath,
		string infisicalUrl,
		string prefix, 
		bool expandSecretReferences)
	{
		Environment = environment;
		ProjectId = projectId;
		Auth = auth;
		SecretPath = secretPath;
		InfisicalUrl = infisicalUrl;
		Prefix = prefix;
		ExpandSecretReferences = expandSecretReferences;
	}
}

/// <summary>
/// A fluent builder for constructing <see cref="InfisicalConfig"/> instances.
/// <see cref="SetProjectId"/>, <see cref="SetEnvironment"/>, and <see cref="SetAuth"/>
/// are required before calling <see cref="Build"/>.
/// </summary>
public class InfisicalConfigBuilder
{
	private string? _environment;
	private string? _projectId;
	private string? _prefix;
	private InfisicalAuth? _auth;
	private string _secretPath = "/";
	private string _infisicalUrl = "https://app.infisical.com";
	private bool _expandSecretReferences = true;

	/// <summary>
	/// Sets the authentication configuration. Required.
	/// </summary>
	/// <param name="auth">The authentication details built with <see cref="InfisicalAuthBuilder"/>.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetAuth(InfisicalAuth auth)
	{
		_auth = auth;
		return this;
	}

	/// <summary>
	/// Sets a prefix to prepend to all secret keys when they are added to the configuration. Optional.
	/// </summary>
	/// <param name="prefix">The prefix string to prepend to secret keys.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetPrefix(string prefix)
	{
		_prefix = prefix;
		return this;
	}

	/// <summary>
	/// Sets the environment slug to fetch secrets from (e.g. <c>"dev"</c>, <c>"staging"</c>, <c>"prod"</c>). Required.
	/// </summary>
	/// <param name="environment">The Infisical environment slug.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetEnvironment(string environment)
	{
		_environment = environment;
		return this;
	}

	/// <summary>
	/// Sets the Infisical project ID to fetch secrets from. Required.
	/// </summary>
	/// <param name="projectId">The Infisical project ID.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetProjectId(string projectId)
	{
		_projectId = projectId;
		return this;
	}

	/// <summary>
	/// Sets the secret path to fetch secrets from. Optional, defaults to <c>"/"</c>.
	/// </summary>
	/// <param name="secretPath">The path within the Infisical project.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetSecretPath(string secretPath)
	{
		_secretPath = secretPath;
		return this;
	}

	/// <summary>
	/// Sets the base URL of your Infisical instance. Optional, defaults to <c>"https://app.infisical.com"</c>.
	/// A trailing <c>/api</c> segment, if present, is automatically removed.
	/// </summary>
	/// <param name="infisicalUrl">The base URL of the Infisical instance.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetInfisicalUrl(string infisicalUrl)
	{
		if (infisicalUrl.EndsWith("/api"))
		{
			infisicalUrl = infisicalUrl[..^4];
		}
		_infisicalUrl = infisicalUrl;
		return this;
	}
	
	/// <summary>
	/// Sets whether secret references should be expanded by the Infisical server. Optional, defaults to <c>true</c>.
	/// </summary>
	/// <param name="expandSecretReferences"><c>true</c> to expand secret references; <c>false</c> to return raw values.</param>
	/// <returns>This builder instance for method chaining.</returns>
	public InfisicalConfigBuilder SetExpandSecretReferences(bool expandSecretReferences)
	{
		_expandSecretReferences = expandSecretReferences;
		return this;
	}

	/// <summary>
	/// Validates the configuration and builds an <see cref="InfisicalConfig"/> instance.
	/// </summary>
	/// <returns>A configured <see cref="InfisicalConfig"/> instance.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when required fields (<see cref="SetEnvironment"/>, <see cref="SetProjectId"/>,
	/// <see cref="SetAuth"/>) have not been set.
	/// </exception>
	public InfisicalConfig Build()
	{
		ValidateRequiredFields();

		return new InfisicalConfig(
			environment: _environment!,
			projectId: _projectId!,
			auth: _auth!,
			secretPath: _secretPath,
			infisicalUrl: _infisicalUrl,
			prefix: _prefix ?? "",
			expandSecretReferences: _expandSecretReferences
		);
	}

	private void ValidateRequiredFields()
	{
		if (string.IsNullOrEmpty(_environment) || string.IsNullOrEmpty(_projectId))
		{
			throw new InvalidOperationException("Environment and ProjectId must be set");
		}

		if (_auth is null)
		{
			throw new InvalidOperationException("Auth must be set");
		}

		if (string.IsNullOrEmpty(_infisicalUrl))
		{
			throw new InvalidOperationException("InfisicalUrl must be set");
		}

		if (string.IsNullOrEmpty(_secretPath))
		{
			throw new InvalidOperationException("SecretPath must be set");
		}
	}
}