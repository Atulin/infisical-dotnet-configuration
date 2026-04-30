using System.Text.Json.Serialization;

namespace InfisicalConfiguration;

[JsonSerializable(typeof(InfisicalConfig))]
[JsonSerializable(typeof(MachineIdentityLogin))]
[JsonSerializable(typeof(UniversalAuthCredentials))]
[JsonSerializable(typeof(AzureCustomProviderAuthCredentials))]
[JsonSerializable(typeof(SecretsList))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public sealed partial class InfisicalJsonContext : JsonSerializerContext;