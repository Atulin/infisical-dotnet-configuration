using System.Text.Json;
using InfisicalConfiguration;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Enter Infisical Client ID:");
var clientId = Console.ReadLine() ?? throw new Exception("Client ID is required");

Console.WriteLine("Enter Infisical Client Secret:");
var clientSecret = Console.ReadLine() ?? throw new Exception("Client Secret is required");

Console.WriteLine("Enter Infisical Project ID:");
var projectId = Console.ReadLine() ?? throw new Exception("Project ID is required");

Console.WriteLine("Enter Infisical Environment (optional, default: dev):");
var environment = Console.ReadLine() is { Length: > 0 } env ? env : "dev";

var auth = new InfisicalAuthBuilder()
	.SetUniversalAuth(clientId, clientSecret)
	.Build();

var config = new InfisicalConfigBuilder()
	.SetProjectId(projectId)
	.SetEnvironment(environment)
	.SetInfisicalUrl("https://eu.infisical.com")
	.SetAuth(auth)
	.Build();

var cfg = new ConfigurationBuilder()
	.AddInfisical(config)
	.Build();

Console.WriteLine(JsonSerializer.Serialize(cfg));