// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class MatriculaService : IMatriculaService
{
    private readonly Func<IMatriculaRepository> _matriculaRepoFactory;
    private readonly Func<IAlunoRepository> _alunoRepoFactory;

    public MatriculaService(
        Func<IMatriculaRepository> matriculaRepoFactory,
        Func<IAlunoRepository> alunoRepoFactory)
    {
        _matriculaRepoFactory = matriculaRepoFactory
            ?? throw new ArgumentNullException(nameof(matriculaRepoFactory));

        _alunoRepoFactory = alunoRepoFactory
            ?? throw new ArgumentNullException(nameof(alunoRepoFactory));
    }

    public async Task<MatriculaDto?> ObterPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var matricula = await _matriculaRepoFactory()
            .ObterPorId(id, cancellationToken);

        if (matricula is null)
            return null;

        var aluno = await _alunoRepoFactory()
            .ObterPorId(matricula.AlunoId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"Aluno associado à matrícula {matricula.Id} não foi encontrado.");

        return matricula.ToDto(aluno);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterTodasAsync(
        CancellationToken cancellationToken = default)
    {
        var matriculas = await _matriculaRepoFactory()
            .ObterTodos(cancellationToken);

        return await EnriquecerComAlunosAsync(
            matriculas,
            cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorAlunoIdAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var aluno = await _alunoRepoFactory()
            .ObterPorId(alunoId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"Aluno com ID {alunoId} não foi encontrado.");

        var matriculas = await _matriculaRepoFactory()
            .ObterPorAluno(alunoId, cancellationToken);

        return matriculas
            .Select(matricula => matricula.ToDto(aluno))
            .ToList();
    }

    public async Task<MatriculaDto?> ObterMatriculaAtivaPorAlunoAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var matricula = await _matriculaRepoFactory()
            .ObterMatriculaAtivaPorAluno(
                alunoId,
                cancellationToken);

        if (matricula is null)
            return null;

        var aluno = await _alunoRepoFactory()
            .ObterPorId(alunoId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"Aluno com ID {alunoId} não foi encontrado.");

        return matricula.ToDto(aluno);
    }

    public async Task<bool> PossuiMatriculaAtivaAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return await _matriculaRepoFactory()
            .PossuiMatriculaAtiva(alunoId, cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterAtivasAsync(
        int alunoId = 0,
        CancellationToken cancellationToken = default)
    {
        var matriculas = await _matriculaRepoFactory()
            .ObterAtivas(alunoId, cancellationToken);

        return await EnriquecerComAlunosAsync(
            matriculas,
            cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterVencendoEmDiasAsync(
        int dias,
        CancellationToken cancellationToken = default)
    {
        if (dias < 0)
        {
            throw new ArgumentException(
                "A quantidade de dias não pode ser negativa.",
                nameof(dias));
        }

        var matriculas = await _matriculaRepoFactory()
            .ObterVencendoEmDias(dias, cancellationToken);

        return await EnriquecerComAlunosAsync(
            matriculas,
            cancellationToken);
    }

    public async Task<MatriculaDto> AdicionarAsync(
        MatriculaDto matriculaDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);

        if (matriculaDto.AlunoMatricula is null ||
            matriculaDto.AlunoMatricula.Id <= 0)
        {
            throw new InvalidOperationException(
                "Aluno não informado ou com ID inválido para a matrícula.");
        }

        var aluno = await _alunoRepoFactory()
            .ObterPorId(
                matriculaDto.AlunoMatricula.Id,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"Aluno com ID {matriculaDto.AlunoMatricula.Id} não foi encontrado.");

        if (await _matriculaRepoFactory()
            .PossuiMatriculaAtiva(aluno.Id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Já existe uma matrícula ativa para este aluno.");
        }

        bool menorDe16 = aluno.DataNascimento >
            DateOnly.FromDateTime(DateTime.Today.AddYears(-16));

        bool possuiLaudo =
            matriculaDto.LaudoMedico?.Conteudo is { Length: > 0 };

        if (menorDe16 && !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos menores de 16 anos devem apresentar um laudo médico.");
        }

        if (matriculaDto.RestricoesMedicas != AppMatriculaRestricoes.None &&
            !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos com restrições médicas devem apresentar um laudo médico.");
        }

        var laudoMedico = matriculaDto.LaudoMedico is null
            ? null
            : Arquivo.Criar(
                matriculaDto.LaudoMedico.Conteudo).Value;

        var resultado = Matricula.Criar(
            matriculaDto.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            laudoMedico,
            matriculaDto.ObservacoesRestricoes ?? string.Empty);

        if (resultado.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível criar a matrícula.");
        }

        var matricula = resultado.Value!;

        var adicionada = await _matriculaRepoFactory()
            .Adicionar(matricula, cancellationToken);

        return adicionada.ToDto(aluno);
    }

    public async Task<MatriculaDto> AtualizarAsync(
        MatriculaDto matriculaDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);

        var matriculaExistente = await _matriculaRepoFactory()
            .ObterPorId(matriculaDto.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Matrícula com ID {matriculaDto.Id} não foi encontrada.");

        var aluno = await _alunoRepoFactory()
            .ObterPorId(
                matriculaExistente.AlunoId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"Aluno associado à matrícula {matriculaDto.Id} não foi encontrado.");

        bool menorDe16 = aluno.DataNascimento >
            DateOnly.FromDateTime(DateTime.Today.AddYears(-16));

        bool possuiLaudo =
            matriculaDto.LaudoMedico?.Conteudo is { Length: > 0 } ||
            matriculaExistente.LaudoMedico is not null;

        if (menorDe16 && !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos menores de 16 anos devem apresentar um laudo médico.");
        }

        if (matriculaDto.RestricoesMedicas != AppMatriculaRestricoes.None &&
            !possuiLaudo)
        {
            throw new InvalidOperationException(
                "Alunos com restrições médicas devem apresentar um laudo médico.");
        }

        var laudoMedico = matriculaDto.LaudoMedico is null
            ? matriculaExistente.LaudoMedico
            : Arquivo.Criar(
                matriculaDto.LaudoMedico.Conteudo).Value;

        var resultadoAtualizacao = Matricula.Criar(
            matriculaExistente.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            laudoMedico,
            matriculaDto.ObservacoesRestricoes ?? string.Empty);

        if (resultadoAtualizacao.IsFailure)
        {
            throw new InvalidOperationException(
                "Não foi possível atualizar a matrícula.");
        }

        var matriculaAtualizada = resultadoAtualizacao.Value!;

        var atualizada = await _matriculaRepoFactory()
            .Atualizar(
                matriculaAtualizada,
                cancellationToken);

        return atualizada.ToDto(aluno);
    }

    public async Task<bool> RemoverAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var matricula = await _matriculaRepoFactory()
            .ObterPorId(id, cancellationToken);

        if (matricula is null)
            return false;

        return await _matriculaRepoFactory()
            .Remover(id, cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorPlanoAsync(
        AppMatriculaPlano plano,
        CancellationToken cancellationToken = default)
    {
        var matriculas = await _matriculaRepoFactory()
            .ObterPorPlano(
                plano.ToDomain(),
                cancellationToken);

        return await EnriquecerComAlunosAsync(
            matriculas,
            cancellationToken);
    }

    private async Task<IEnumerable<MatriculaDto>> EnriquecerComAlunosAsync(
        IEnumerable<Matricula> matriculas,
        CancellationToken cancellationToken)
    {
        var listaMatriculas = matriculas.ToList();

        if (listaMatriculas.Count == 0)
            return [];

        var alunos = new Dictionary<int, Aluno>();

        foreach (var matricula in listaMatriculas)
        {
            if (!alunos.ContainsKey(matricula.AlunoId))
            {
                var aluno = await _alunoRepoFactory()
                    .ObterPorId(
                        matricula.AlunoId,
                        cancellationToken);

                if (aluno is not null)
                    alunos[matricula.AlunoId] = aluno;
            }
        }

        return listaMatriculas
            .Select(matricula =>
            {
                if (!alunos.TryGetValue(
                        matricula.AlunoId,
                        out var aluno))
                {
                    throw new InvalidOperationException(
                        $"Aluno associado à matrícula {matricula.Id} não foi encontrado.");
                }

                return matricula.ToDto(aluno);
            })
            .ToList();
    }
}