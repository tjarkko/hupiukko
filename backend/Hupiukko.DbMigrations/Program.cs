using DbUp;
using Microsoft.Extensions.Configuration;
using DbUp.Support;
using System.Data.SqlClient;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var argsList = args.ToList();
bool dropDb = argsList.Contains("--drop-db");

// Check for connection string parameter (for local development)
var connectionStringIndex = argsList.IndexOf("--connection-string");
string? connectionString = null;

if (connectionStringIndex >= 0 && connectionStringIndex + 1 < argsList.Count)
{
    connectionString = argsList[connectionStringIndex + 1];
}

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

// Try to get connection string from Key Vault if not provided
if (string.IsNullOrEmpty(connectionString))
{
    var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
    var secretName = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING_SECRET_NAME") ?? "sql-connection-string";

    if (!string.IsNullOrEmpty(keyVaultName))
    {
        try
        {
            Console.WriteLine($"🔐 Retrieving connection string from Key Vault: {keyVaultName}");

            // Use DefaultAzureCredential (supports managed identity, Azure CLI, etc.)
            var credential = new DefaultAzureCredential();
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            var secretClient = new SecretClient(keyVaultUri, credential);

            var secret = secretClient.GetSecret(secretName);
            connectionString = secret.Value.Value;

            Console.WriteLine("✅ Successfully retrieved connection string from Key Vault");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Failed to get connection string from Key Vault: {ex.Message}");
            Console.WriteLine("Falling back to configuration file...");
            Console.ResetColor();
        }
    }
}

// Fallback to configuration file if Key Vault failed or not configured
if (string.IsNullOrEmpty(connectionString))
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    connectionString = configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: Connection string not found. Options:");
    Console.WriteLine("1. Provide --connection-string parameter");
    Console.WriteLine("2. Set KEY_VAULT_NAME and SQL_CONNECTION_STRING_SECRET_NAME environment variables");
    Console.WriteLine("3. Ensure DefaultConnection is configured in appsettings");
    Console.ResetColor();
    return -3;
}

if (dropDb)
{
    if (!string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: --drop-db is only allowed in Development environment. Current environment: {environment}");
        Console.ResetColor();
        return -2;
    }
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Dropping database as requested by --drop-db (Environment: {environment})...");
    Console.ResetColor();

    // Build a connection string to master DB
    var builder = new SqlConnectionStringBuilder(connectionString);
    var dbName = builder.InitialCatalog;
    builder.InitialCatalog = "master";
    using (var conn = new SqlConnection(builder.ConnectionString))
    {
        conn.Open();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"IF DB_ID('{dbName}') IS NOT NULL DROP DATABASE [{dbName}]";
            cmd.ExecuteNonQuery();
        }
    }
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Database {dbName} dropped.");
    Console.ResetColor();
}

// Ensure the database exists before running migrations
EnsureDatabase.For.SqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;
