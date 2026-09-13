// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;
    private readonly MatriculaRepository _matriculaRepo;

    public MatriculaInfrastructureTests()
    {
        _logradouroRepo =
            new LogradouroRepository(
                ConnectionString,
                DatabaseType);

        _alunoRepo =
            new AlunoRepository(
                ConnectionString,
                DatabaseType);

        _matriculaRepo =
            new MatriculaRepository(
                ConnectionString,
                DatabaseType);
    }

    internal static async Task<Aluno> CriarEInserirAlunoAsync(
        AlunoRepository alunoRepo,
        LogradouroRepository logradouroRepo)
    {
        var logradouro =
            await LogradouroInfrastructureTests
                .CriarEInserirLogradouroAsync(
                    logradouroRepo);

        var foto =
            Arquivo.Criar(
                new byte[] { 5, 6, 7, 8 }).Value!;

        var alunoResult =
            Aluno.Criar(
                id: 0,
                nome:
                    "Ana Luisa " +
                    Guid.NewGuid()
                        .ToString("N")[..5],
                cpf: GerarCpf(),
                dataNascimento:
                    new DateOnly(1995, 5, 15),
                telefone: GerarTelefone(),
                email: GerarEmail(),
                logradouro: logradouro,
                numero: "200",
                complemento: "Sala 2",
                senha: "SenhaValida123",
                foto: foto);

        if (alunoResult.IsFailure)
        {
            throw new Exception(
                "Falha ao criar Aluno: " +
                string.Join(
                    ", ",
                    alunoResult.Notifications
                        .Select(n => n.Mensagem)));
        }

        return await alunoRepo.Adicionar(
            alunoResult.Value!);
    }

    private async Task<Matricula>
        CriarEInserirMatriculaAsync(
            Aluno aluno,
            MatriculaPlano plano =
                MatriculaPlano.Mensal,
            DateOnly? dataInicio = null,
            MatriculaRestricoes restricoes =
                MatriculaRestricoes.None,
            string obsRestricao = "",
            Arquivo? laudo = null)
    {
        var inicio =
            dataInicio ??
            DateOnly.FromDateTime(
                DateTime.Today);

        if (restricoes != MatriculaRestricoes.None
            && laudo == null)
        {
            laudo =
                Arquivo.Criar(
                    new byte[] { 1, 2, 3, 4 })
                .Value;
        }

        var matriculaResult =
            Matricula.Criar(
                id: 0,
                aluno: aluno,
                plano: plano,
                dataInicio: inicio,
                objetivo: "Ana Luisa",
                restricoesMedicas: restricoes,
                laudoMedico: laudo,
                observacoesRestricoes:
                    obsRestricao);

        if (matriculaResult.IsFailure)
        {
            throw new Exception(
                "Falha ao criar Matricula: " +
                string.Join(
                    ", ",
                    matriculaResult.Notifications
                        .Select(n => n.Mensagem)));
        }

        return await _matriculaRepo.Adicionar(
            matriculaResult.Value!);
    }

    [Fact]
    public async Task
        Matricula_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var restricoes =
            MatriculaRestricoes.Diabetes |
            MatriculaRestricoes.PressaoAlta;

        var laudo =
            Arquivo.Criar(
                new byte[] { 100, 101, 102 })
            .Value!;

        var inserida =
            await CriarEInserirMatriculaAsync(
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                restricoes: restricoes,
                obsRestricao: "SQLite",
                laudo: laudo);

        Assert.NotNull(inserida);
        Assert.True(inserida.Id > 0);
        Assert.Equal(
            aluno.Id,
            inserida.AlunoId);

        Assert.Equal(
            MatriculaPlano.Mensal,
            inserida.Plano);

        Assert.Equal(
            restricoes,
            inserida.RestricoesMedicas);

        var obtida =
            await _matriculaRepo.ObterPorId(
                inserida.Id);

        Assert.NotNull(obtida);

        Assert.Equal(
            inserida.Id,
            obtida.Id);

        Assert.Equal(
            aluno.Id,
            obtida.AlunoId);

        Assert.Equal(
            MatriculaPlano.Mensal,
            obtida.Plano);

        Assert.Equal(
            restricoes,
            obtida.RestricoesMedicas);

        Assert.Equal(
            "SQLite",
            obtida.ObservacoesRestricoes);

        Assert.NotNull(
            obtida.LaudoMedico);

        Assert.Equal(
            laudo.Conteudo,
            obtida.LaudoMedico!.Conteudo);
    }

    [Fact]
    public async Task
        Matricula_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtida =
            await _matriculaRepo.ObterPorId(
                999999);

        Assert.Null(obtida);
    }

    [Fact]
    public async Task
        Matricula_ObterTodos_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        await CriarEInserirMatriculaAsync(
            aluno);

        var todas =
            await _matriculaRepo.ObterTodos();

        Assert.NotNull(todas);
        Assert.NotEmpty(todas);
    }

    [Fact]
    public async Task
        Matricula_Atualizar_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var inserida =
            await CriarEInserirMatriculaAsync(
                aluno,
                MatriculaPlano.Mensal);

        var nova =
            Matricula.Criar(
                id: inserida.Id,
                aluno: aluno,
                plano: MatriculaPlano.Anual,
                dataInicio:
                    inserida.DataInicio,
                objetivo: "Ana Luisa",
                restricoesMedicas:
                    MatriculaRestricoes.Diabetes |
                    MatriculaRestricoes.Alergias,
                laudoMedico:
                    Arquivo.Criar(
                        new byte[]
                        {
                            99,
                            88,
                            77
                        }).Value!,
                observacoesRestricoes:
                    "SQLite");

        Assert.True(nova.IsSuccess);

        var resultado =
            await _matriculaRepo.Atualizar(
                nova.Value!);

        Assert.NotNull(resultado);

        Assert.Equal(
            MatriculaPlano.Anual,
            resultado.Plano);

        Assert.Equal(
            "Ana Luisa",
            resultado.Objetivo);

        Assert.Equal(
            "SQLite",
            resultado.ObservacoesRestricoes);

        var noBanco =
            await _matriculaRepo.ObterPorId(
                inserida.Id);

        Assert.NotNull(noBanco);

        Assert.Equal(
            MatriculaPlano.Anual,
            noBanco.Plano);

        Assert.Equal(
            "Ana Luisa",
            noBanco.Objetivo);

        Assert.Equal(
            MatriculaRestricoes.Diabetes |
            MatriculaRestricoes.Alergias,
            noBanco.RestricoesMedicas);
    }

    [Fact]
    public async Task
        Matricula_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var matriculaInexistente =
            Matricula.Criar(
                id: 999999,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Ana Luisa",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null,
                observacoesRestricoes:
                    "SQLite");

        Assert.True(
            matriculaInexistente.IsSuccess);

        var ex =
            await Assert.ThrowsAsync<
                InfrastructureException>(
                () =>
                    _matriculaRepo.Atualizar(
                        matriculaInexistente.Value!));

        Assert.Equal(
            "REGISTRO_NAO_ENCONTRADO",
            ex.ErrorCode);
    }

    [Fact]
    public async Task
        Matricula_Remover_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var inserida =
            await CriarEInserirMatriculaAsync(
                aluno);

        var removida =
            await _matriculaRepo.Remover(
                inserida.Id);

        Assert.True(removida);

        var noBanco =
            await _matriculaRepo.ObterPorId(
                inserida.Id);

        Assert.Null(noBanco);
    }

    [Fact]
    public async Task
        Matricula_Remover_RetornaFalseQuandoInexistente()
    {
        var removida =
            await _matriculaRepo.Remover(
                999999);

        Assert.False(removida);
    }

    [Fact]
    public async Task
        Matricula_ObterPorAluno_FiltragemCorreta()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        await CriarEInserirMatriculaAsync(
            aluno);

        var matriculas =
            await _matriculaRepo.ObterPorAluno(
                aluno.Id);

        Assert.NotNull(matriculas);
        Assert.NotEmpty(matriculas);

        Assert.All(
            matriculas,
            m =>
                Assert.Equal(
                    aluno.Id,
                    m.AlunoId));
    }

    [Fact]
    public async Task
        Matricula_PossuiMatriculaAtiva_E_ObterAtiva_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var possuiAntes =
            await _matriculaRepo
                .PossuiMatriculaAtiva(
                    aluno.Id);

        Assert.False(possuiAntes);

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Mensal,
            DateOnly.FromDateTime(
                DateTime.Today));

        var possuiDepois =
            await _matriculaRepo
                .PossuiMatriculaAtiva(
                    aluno.Id);

        Assert.True(possuiDepois);

        var ativa =
            await _matriculaRepo
                .ObterMatriculaAtivaPorAluno(
                    aluno.Id);

        Assert.NotNull(ativa);

        Assert.Equal(
            aluno.Id,
            ativa.AlunoId);
    }

    [Fact]
    public async Task
        Matricula_ObterAtivas_FiltragemCorreta()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Semestral,
            DateOnly.FromDateTime(
                DateTime.Today));

        var ativas =
            await _matriculaRepo.ObterAtivas();

        Assert.NotNull(ativas);
        Assert.NotEmpty(ativas);

        var ativasAluno =
            await _matriculaRepo.ObterAtivas(
                aluno.Id);

        Assert.NotNull(ativasAluno);
        Assert.NotEmpty(ativasAluno);

        Assert.All(
            ativasAluno,
            m =>
                Assert.Equal(
                    aluno.Id,
                    m.AlunoId));
    }

    [Fact]
    public async Task
        Matricula_ObterVencendoEmDias_RetornaCorretamente()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        var inicio =
            DateOnly.FromDateTime(
                DateTime.Today.AddDays(-25));

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Mensal,
            inicio);

        var vencendo =
            await _matriculaRepo
                .ObterVencendoEmDias(30);

        Assert.NotNull(vencendo);

        Assert.Contains(
            vencendo,
            m =>
                m.AlunoId == aluno.Id);
    }

    [Fact]
    public async Task
        Matricula_ObterPorPlano_FiltragemCorreta()
    {
        var aluno =
            await CriarEInserirAlunoAsync(
                _alunoRepo,
                _logradouroRepo);

        await CriarEInserirMatriculaAsync(
            aluno,
            MatriculaPlano.Trimestral);

        var trimestrais =
            await _matriculaRepo.ObterPorPlano(
                MatriculaPlano.Trimestral);

        Assert.NotNull(trimestrais);

        Assert.Contains(
            trimestrais,
            m =>
                m.AlunoId == aluno.Id &&
                m.Plano ==
                    MatriculaPlano.Trimestral);
    }
}