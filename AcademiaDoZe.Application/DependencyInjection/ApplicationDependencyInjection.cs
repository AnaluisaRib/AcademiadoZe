// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Registra os serviços da camada de aplicação.

        services.AddTransient<ILogradouroService, LogradouroService>();

        //services.AddTransient<IColaboradorService, ColaboradorService>();

        services.AddTransient<IAlunoService, AlunoService>();

        services.AddTransient<IMatriculaService, MatriculaService>();

        // Registra a fábrica do repositório de Logradouro.

        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<ILogradouroRepository>)(() =>
                new LogradouroRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Registra a fábrica do repositório de Colaborador.

        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<IColaboradorRepository>)(() =>
                new ColaboradorRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Registra a fábrica do repositório de Aluno.

        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<IAlunoRepository>)(() =>
                new AlunoRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        // Registra a fábrica do repositório de Matrícula.

        services.AddTransient(provider =>
        {
            var config = provider.GetRequiredService<RepositoryConfig>();

            return (Func<IMatriculaRepository>)(() =>
                new MatriculaRepository(
                    config.ConnectionString,
                    config.DatabaseType));
        });

        return services;
    }
}