using DbUp;
using Microsoft.Extensions.Configuration;
using DbUp.Support;
using System.Data.SqlClient;

var argsList = args.ToList();
bool dropDb = argsList.Contains("--drop-db");

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

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
