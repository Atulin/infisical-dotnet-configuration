using System.Text.Json;

namespace InfisicalConfiguration;

public class MachineIdentityLogin
{
	public required string AccessToken { get; init; }

	public static MachineIdentityLogin Deserialize(string content)
	{
		var result = JsonSerializer.Deserialize(content, InfisicalJsonContext.Default.MachineIdentityLogin);

		return result ?? throw new InvalidOperationException("Failed to deserialize MachineIdentityLogin");
	}
}

public class SecretsList
{
	public List<Secret> Secrets { get; init; } = [];

	public static SecretsList Deserialize(string content)
	{
		var result = JsonSerializer.Deserialize(content, InfisicalJsonContext.Default.SecretsList);
		return result ?? new();
	}
}

public record Secret(string Key, string Value);