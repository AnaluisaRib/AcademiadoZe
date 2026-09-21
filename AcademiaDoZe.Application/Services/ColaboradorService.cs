// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class ColaboradorService : IColaboradorService
{
    private readonly Func<IColaboradorRepository> _repoFactory;

    public ColaboradorService(
        Func<IColaboradorRepository> repoFactory)
    {
        _repoFactory = repoFactory
            ?? throw new ArgumentNullException(nameof(repoFactory));
    }

    public async Task<ColaboradorDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var colaborador = await _repoFactory()
            .ObterPorId(id, cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var colaboradores = await _repoFactory()
            .ObterTodos(cancellationToken);

        return colaboradores
            .Select(colaborador => colaborador.ToDto())
            .ToList();
    }

    public async Task<ColaboradorDto?> ObterPorCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            throw new ArgumentException(
                "O CPF deve ser informado.",
                nameof(cpf));
        }

        var cpfResult = Cpf.Criar(cpf);

        if (cpfResult.IsFailure)
        {
            throw new ArgumentException(
                "CPF inválido.",
                nameof(cpf));
        }

        var colaborador = await _repoFactory()
            .ObterPorCpf(
                cpfResult.Value!,
                cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<ColaboradorDto?> ObterPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "O e-mail deve ser informado.",
                nameof(email));
        }

        var emailResult = Email.Criar(email);

        if (emailResult.IsFailure)
        {
            throw new ArgumentException(
                "E-mail inválido.",
                nameof(email));
        }

        var colaborador = await _repoFactory()
            .ObterPorEmail(
                emailResult.Value!,
                cancellationToken);

        return colaborador?.ToDto();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterPorTipoAsync(
        AppColaboradorTipo tipo,
        CancellationToken cancellationToken = default)
    {
        var colaboradores = await _repoFactory()
            .ObterPorTipo(
                tipo.ToDomain(),
                cancellationToken);

        return colaboradores
            .Select(colaborador => colaborador.ToDto())
            .ToList();
    }

    public async Task<IEnumerable<ColaboradorDto>> ObterPorVinculoAsync(
        AppColaboradorVinculo vinculo,
        CancellationToken cancellationToken = default)
    {
        var colaboradores = await _repoFactory()
            .ObterPorVinculo(
                vinculo.ToDomain(),
                cancellationToken);

        return colaboradores
            .Select(colaborador => colaborador.ToDto())
            .ToList();
    }

    public async Task<bool> CpfJaExisteAsync(
        string cpf,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfResult = Cpf.Criar(cpf);

        if (cpfResult.IsFailure)
            return false;

        return await _repoFactory()
            .CpfJaExiste(
                cpfResult.Value!,
                id,
                cancellationToken);
    }

    public async Task<bool> EmailJaExisteAsync(
        string email,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailResult = Email.Criar(email);

        if (emailResult.IsFailure)
            return false;

        return await _repoFactory()
            .EmailJaExiste(
                emailResult.Value!,
                id,
                cancellationToken);
    }

    public async Task<ColaboradorDto> AdicionarAsync(
        ColaboradorDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (await CpfJaExisteAsync(
                dto.Cpf,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe um colaborador cadastrado com este CPF.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            await EmailJaExisteAsync(
                dto.Email,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe um colaborador cadastrado com este e-mail.");
        }

        var colaborador = dto.ToEntity();

        await _repoFactory().Adicionar(
            colaborador,
            cancellationToken);

        return colaborador.ToDto();
    }

    public async Task<ColaboradorDto> AtualizarAsync(
        ColaboradorDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var colaboradorExistente = await _repoFactory()
            .ObterPorId(
                dto.Id,
                cancellationToken);

        if (colaboradorExistente is null)
        {
            throw new InvalidOperationException(
                "Colaborador não encontrado.");
        }

        if (await CpfJaExisteAsync(
                dto.Cpf,
                dto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe outro colaborador com este CPF.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            await EmailJaExisteAsync(
                dto.Email,
                dto.Id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe outro colaborador com este e-mail.");
        }

        colaboradorExistente.UpdateFromDto(dto);

        await _repoFactory().Atualizar(
            colaboradorExistente,
            cancellationToken);

        return colaboradorExistente.ToDto();
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var colaborador = await _repoFactory()
            .ObterPorId(
                id,
                cancellationToken);

        if (colaborador is null)
            return false;

        return await _repoFactory().Remover(
            id,
            cancellationToken);
    }

    public Task<bool> TrocarSenhaAsync(
        int id,
        string novaSenha,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(novaSenha))
        {
            throw new ArgumentException(
                "A nova senha deve ser informada.",
                nameof(novaSenha));
        }

        throw new NotSupportedException(
            "A alteração de senha ainda não foi implementada na entidade Colaborador.");
    }
}