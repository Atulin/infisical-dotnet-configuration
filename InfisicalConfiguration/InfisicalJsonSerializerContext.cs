using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace InfisicalConfiguration;

[JsonSerializable(typeof(InfisicalConfig))]
[JsonSerializable(typeof(MachineIdentityLogin))]
[JsonSerializable(typeof(UniversalAuthCredentials))]
[JsonSerializable(typeof(AzureCustomProviderAuthCredentials))]
[JsonSerializable(typeof(SecretsList))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[UsedImplicitly]
internal sealed partial class InfisicalJsonContext : JsonSerializerContext;