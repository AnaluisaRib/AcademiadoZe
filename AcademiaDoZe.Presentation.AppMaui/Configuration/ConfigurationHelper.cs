// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var databaseType = AppDatabaseType.Sqlite;

        string connectionString;

        if (databaseType == AppDatabaseType.Sqlite)
        {
            var dbPath = DeviceInfo.Platform == DevicePlatform.WinUI
                ? @"C:\Users\Ana Luisa\source\repos\AcademiadoZe\db_academia_do_ze.db"
                : Path.Combine(
                    FileSystem.AppDataDirectory,
                    "db_academia_do_ze.db");

            connectionString =
                $"Data Source={dbPath};Default Timeout=5;";
        }
        else
        {
            throw new NotSupportedException(
                "Apenas SQLite está configurado para este projeto.");
        }

        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = databaseType.ToInfrastructure()
        });

        services.AddApplicationServices();
    }
}