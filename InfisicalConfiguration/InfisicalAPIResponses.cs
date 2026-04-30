using System.Text.Json;
using JetBrains.Annotations;

namespace InfisicalConfiguration;

internal sealed class MachineIdentityLogin
{
	public required string AccessToken { get; init; }

	public static MachineIdentityLogin Deserialize(string content)
	{
		var result = JsonSerializer.Deserialize(content, InfisicalJsonContext.Default.MachineIdentityLogin);

		return result ?? throw new InvalidOperationException("Failed to deserialize MachineIdentityLogin");
	}
}

internal sealed class SecretsList
{
	public List<Secret> Secrets { get; init; } = [];

	public static SecretsList Deserialize(string content)
	{
		var result = JsonSerializer.Deserialize(content, InfisicalJsonContext.Default.SecretsList);
		return result ?? new();
	}
}

[UsedImplicitly]
internal sealed record Secret(string SecretKey, string SecretValue);