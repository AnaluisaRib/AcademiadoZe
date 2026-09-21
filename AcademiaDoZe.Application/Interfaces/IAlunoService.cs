// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAlunoService
{
    Task<AlunoDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AlunoDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default);

    Task<AlunoDto?> ObterPorCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default);

    Task<AlunoDto?> ObterPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> CpfJaExisteAsync(
        string cpf,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailJaExisteAsync(
        string email,
        int? id = null,
        CancellationToken cancellationToken = default);

    Task<AlunoDto> AdicionarAsync(
        AlunoDto dto,
        CancellationToken cancellationToken = default);

    Task<AlunoDto> AtualizarAsync(
        AlunoDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default);
}