// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class MatriculaTests
{
    private static Aluno GetValidAluno(
        DateOnly? dataNascimento = null)
    {
        var logradouroResult =
            Logradouro.Criar(
                1,
                "89000000",
                "Rua Teste",
                "Centro",
                "Lages",
                "SC",
                "Brasil");

        Assert.True(logradouroResult.IsSuccess);

        var alunoResult =
            Aluno.Criar(
                id: 1,
                nome: "Ana Luisa",
                cpf: "12345678909",
                dataNascimento:
                    dataNascimento ??
                    new DateOnly(1995, 5, 15),
                telefone: "49999999999",
                email: "ana@test.com",
                logradouro:
                    logradouroResult.Value!,
                numero: "123",
                complemento: "",
                senha: "Senha123",
                foto:
                    Arquivo.Criar(
                        new byte[]
                        {
                            1,
                            2,
                            3
                        }).Value!);

        Assert.True(alunoResult.IsSuccess);
        Assert.NotNull(alunoResult.Value);

        return alunoResult.Value!;
    }

    private static Arquivo GetValidArquivo()
    {
        var result =
            Arquivo.Criar(
                new byte[]
                {
                    1,
                    2,
                    3,
                    4
                });

        Assert.True(result.IsSuccess);
        return result.Value!;
    }

    [Fact]
    public void Deve_Criar_Matricula_Com_DadosValidos()
    {
        var aluno = GetValidAluno();

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Objetivo",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            aluno.Id,
            result.Value!.AlunoId);

        Assert.Equal(
            MatriculaPlano.Mensal,
            result.Value.Plano);

        Assert.Equal(
            "Objetivo",
            result.Value.Objetivo);

        Assert.Equal(
            MatriculaRestricoes.None,
            result.Value.RestricoesMedicas);
    }

    [Fact]
    public void Deve_Falhar_Criacao_Quando_AlunoForNulo()
    {
        var result =
            Matricula.Criar(
                id: 1,
                aluno: null!,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Objetivo",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n =>
                n.Mensagem ==
                "ALUNO_INVALIDO");
    }

    [Fact]
    public void Deve_Falhar_Criacao_Quando_PlanoForInvalido()
    {
        var aluno = GetValidAluno();

        var planoInvalido =
            (MatriculaPlano)999;

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: planoInvalido,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Objetivo",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n =>
                n.Mensagem ==
                "PLANO_INVALIDO");
    }

    [Fact]
    public void Deve_Falhar_Criacao_Quando_DataInicioForObrigatoria()
    {
        var aluno = GetValidAluno();

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio: default,
                objetivo: "Objetivo",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n =>
                n.Mensagem ==
                "DATA_INICIO_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Falhar_Criacao_Quando_ObjetivoForVazio()
    {
        var aluno = GetValidAluno();

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "   ",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n =>
                n.Mensagem ==
                "OBJETIVO_OBRIGATORIO");
    }

    [Theory]
    [InlineData(
        MatriculaPlano.Mensal,
        1)]
    [InlineData(
        MatriculaPlano.Trimestral,
        3)]
    [InlineData(
        MatriculaPlano.Semestral,
        6)]
    [InlineData(
        MatriculaPlano.Anual,
        12)]
    public void Deve_Criar_Com_Sucesso_E_Calcular_DataFim(
        MatriculaPlano plano,
        int meses)
    {
        var aluno = GetValidAluno();

        var inicio =
            DateOnly.FromDateTime(
                DateTime.Today);

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: plano,
                dataInicio: inicio,
                objetivo:
                    "Melhorar condicionamento",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            inicio.AddMonths(meses),
            result.Value!.DataFim);
    }

    [Theory]
    [InlineData(
        MatriculaRestricoes.None,
        true)]
    [InlineData(
        MatriculaRestricoes.Diabetes,
        true)]
    [InlineData(
        MatriculaRestricoes.Diabetes |
        MatriculaRestricoes.Alergias,
        true)]
    public void Deve_Tratar_Restricoes_ComOuSemLaudo(
        MatriculaRestricoes restricoes,
        bool expectSuccess)
    {
        var aluno = GetValidAluno();

        Arquivo? laudo = null;
        string observacoes = "";

        if (restricoes !=
            MatriculaRestricoes.None)
        {
            laudo = GetValidArquivo();
            observacoes = "Observacoes";
        }

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Objetivo",
                restricoesMedicas:
                    restricoes,
                laudoMedico: laudo,
                observacoesRestricoes:
                    observacoes);

        Assert.Equal(
            expectSuccess,
            result.IsSuccess);
    }

    [Theory]
    [InlineData(15, true)]
    [InlineData(20, false)]
    public void Deve_Falhar_Criacao_Quando_Menor16_ExigeLaudo(
        int age,
        bool expectFailure)
    {
        var nascimento =
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-age));

        var aluno =
            GetValidAluno(nascimento);

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo:
                    "Melhorar condicionamento",
                restricoesMedicas:
                    MatriculaRestricoes.None,
                laudoMedico: null);

        Assert.Equal(
            expectFailure,
            result.IsFailure);

        if (expectFailure)
        {
            Assert.NotEmpty(
                result.Notifications);

            Assert.Contains(
                result.Notifications,
                n =>
                    n.Mensagem ==
                    "MENOR_16_LAUDO_OBRIGATORIO");
        }
    }

    [Fact]
    public void Deve_Falhar_Criacao_Quando_RestricaoNaoPossuirLaudo()
    {
        var aluno = GetValidAluno();

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Objetivo",
                restricoesMedicas:
                    MatriculaRestricoes.Diabetes,
                laudoMedico: null);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n =>
                n.Mensagem ==
                "RESTRICOES_LAUDO_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Aceitar_Restricoes_Com_Laudo()
    {
        var aluno = GetValidAluno();

        var laudo = GetValidArquivo();

        var result =
            Matricula.Criar(
                id: 1,
                aluno: aluno,
                plano: MatriculaPlano.Mensal,
                dataInicio:
                    DateOnly.FromDateTime(
                        DateTime.Today),
                objetivo: "Objetivo",
                restricoesMedicas:
                    MatriculaRestricoes.Diabetes |
                    MatriculaRestricoes.Alergias,
                laudoMedico: laudo,
                observacoesRestricoes:
                    "Observacoes");

        Assert.True(result.IsSuccess);

        Assert.NotNull(
            result.Value);

        Assert.Equal(
            MatriculaRestricoes.Diabetes |
            MatriculaRestricoes.Alergias,
            result.Value!.RestricoesMedicas);

        Assert.NotNull(
            result.Value.LaudoMedico);

        Assert.Equal(
            "Observacoes",
            result.Value.ObservacoesRestricoes);
    }
}