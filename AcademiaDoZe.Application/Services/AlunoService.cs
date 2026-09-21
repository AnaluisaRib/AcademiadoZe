// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class AlunoService : IAlunoService
{
    private readonly Func<IAlunoRepository> _repoFactory;
    private readonly Func<ILogradouroRepository>? _logradouroRepoFactory;

    public AlunoService(
        Func<IAlunoRepository> repoFactory,
        Func<ILogradouroRepository>? logradouroRepoFactory = null)
    {
        _repoFactory = repoFactory
            ?? throw new ArgumentNullException(nameof(repoFactory));

        _logradouroRepoFactory = logradouroRepoFactory;
    }

    public async Task<AlunoDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var aluno = await _repoFactory().ObterPorId(
            id,
            cancellationToken);

        if (aluno is null)
            return null;

        return aluno.ToDto();
    }

    public async Task<IEnumerable<AlunoDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var alunos = await _repoFactory().ObterTodos(
            cancellationToken);

        return alunos
            .Select(aluno => aluno.ToDto())
            .ToList();
    }

    public async Task<AlunoDto?> ObterPorCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException(
                "O CPF deve ser informado.",
                nameof(cpf));

        var cpfResult = Cpf.Criar(cpf);

        if (cpfResult.IsFailure)
            throw new ArgumentException(
                "CPF inválido.",
                nameof(cpf));

        var aluno = await _repoFactory().ObterPorCpf(
            cpfResult.Value!,
            cancellationToken);

        return aluno?.ToDto();
    }

    public async Task<AlunoDto?> ObterPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "O e-mail deve ser informado.",
                nameof(email));

        var emailResult = Email.Criar(email);

        if (emailResult.IsFailure)
            throw new ArgumentException(
                "E-mail inválido.",
                nameof(email));

        var aluno = await _repoFactory().ObterPorEmail(
            emailResult.Value!,
            cancellationToken);

        return aluno?.ToDto();
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

        return await _repoFactory().CpfJaExiste(
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

        return await _repoFactory().EmailJaExiste(
            emailResult.Value!,
            id,
            cancellationToken);
    }

    public async Task<AlunoDto> AdicionarAsync(
        AlunoDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (await CpfJaExisteAsync(dto.Cpf, null, cancellationToken))
            throw new InvalidOperationException(
                "Já existe um aluno cadastrado com este CPF.");

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            await EmailJaExisteAsync(dto.Email, null, cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe um aluno cadastrado com este e-mail.");
        }

        var aluno = dto.ToEntity();

        await _repoFactory().Adicionar(
            aluno,
            cancellationToken);

        return aluno.ToDto();
    }

    public async Task<AlunoDto> AtualizarAsync(
        AlunoDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var alunoExistente = await _repoFactory().ObterPorId(
            dto.Id,
            cancellationToken);

        if (alunoExistente is null)
            throw new InvalidOperationException(
                "Aluno não encontrado.");

        if (await CpfJaExisteAsync(dto.Cpf, dto.Id, cancellationToken))
            throw new InvalidOperationException(
                "Já existe outro aluno cadastrado com este CPF.");

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            await EmailJaExisteAsync(dto.Email, dto.Id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe outro aluno cadastrado com este e-mail.");
        }

        alunoExistente.UpdateFromDto(dto);

        await _repoFactory().Atualizar(
            alunoExistente,
            cancellationToken);

        return alunoExistente.ToDto();
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var aluno = await _repoFactory().ObterPorId(
            id,
            cancellationToken);

        if (aluno is null)
            return false;

        return await _repoFactory().Remover(
            id,
            cancellationToken);
    }
}