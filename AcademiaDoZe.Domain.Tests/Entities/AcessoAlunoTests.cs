// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoAlunoTests
{
    [Fact(DisplayName = "AcessoAluno: criação válida")]
    public void Deve_Criar_AcessoAluno_Quando_DadosValidos()
    {
        var dataHora = new DateTime(
            2026,
            8,
            24,
            10,
            0,
            0);

        var result = AcessoAluno.Criar(
            1,
            1,
            dataHora);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            1,
            result.Value!.Id);

        Assert.Equal(
            1,
            result.Value.AlunoId);

        Assert.Equal(
            dataHora,
            result.Value.DataHora);
    }


    [Theory(DisplayName = "AcessoAluno: diferentes alunos válidos")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(100)]
    public void Deve_Criar_AcessoAluno_Quando_AlunoIdValido(
        int alunoId)
    {
        var dataHora = new DateTime(
            2026,
            8,
            24,
            10,
            0,
            0);

        var result = AcessoAluno.Criar(
            1,
            alunoId,
            dataHora);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            alunoId,
            result.Value!.AlunoId);
    }


    [Theory(DisplayName = "AcessoAluno: aluno inválido")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Deve_Falhar_Criacao_Quando_AlunoIdInvalido(
        int alunoId)
    {
        var dataHora = new DateTime(
            2026,
            8,
            24,
            10,
            0,
            0);

        var result = AcessoAluno.Criar(
            1,
            alunoId,
            dataHora);

        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "ALUNO_ID_OBRIGATORIO");
    }


    [Fact(DisplayName = "AcessoAluno: data e hora obrigatórias")]
    public void Deve_Falhar_Criacao_Quando_DataHoraForPadrao()
    {
        var result = AcessoAluno.Criar(
            1,
            1,
            default);

        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "DATA_HORA_OBRIGATORIA");
    }


    [Theory(DisplayName = "AcessoAluno: diferentes horários válidos")]
    [InlineData(6, 0)]
    [InlineData(8, 30)]
    [InlineData(12, 0)]
    [InlineData(18, 45)]
    [InlineData(22, 0)]
    public void Deve_Criar_AcessoAluno_Quando_HorarioValido(
        int hora,
        int minuto)
    {
        var dataHora = new DateTime(
            2026,
            8,
            24,
            hora,
            minuto,
            0);

        var result = AcessoAluno.Criar(
            1,
            1,
            dataHora);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            dataHora,
            result.Value!.DataHora);
    }
    [Theory(DisplayName = "AcessoAluno: diferentes datas válidas")]
    [InlineData(2026, 1, 1)]
    [InlineData(2026, 6, 15)]
    [InlineData(2026, 12, 31)]
    public void Deve_Criar_AcessoAluno_Quando_DataValida(
        int ano,
        int mes,
        int dia)
    {
        var dataHora = new DateTime(
            ano,
            mes,
            dia,
            10,
            0,
            0);

        var result = AcessoAluno.Criar(
            1,
            1,
            dataHora);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            dataHora,
            result.Value!.DataHora);
    }
}