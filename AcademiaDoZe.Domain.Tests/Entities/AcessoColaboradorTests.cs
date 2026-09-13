using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoColaboradorTests
{
    [Fact(DisplayName = "AcessoColaborador: criação válida")]
    public void Deve_Criar_AcessoColaborador_Quando_DadosValidos()
    {
        // Arrange
        var dataHora = new DateTime(
            2026,
            8,
            24,
            10,
            0,
            0);

        // Act
        var result = AcessoColaborador.Criar(
            1,
            1,
            dataHora);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(
            1,
            result.Value!.Id);

        Assert.Equal(
            1,
            result.Value.ColaboradorId);

        Assert.Equal(
            dataHora,
            result.Value.DataHora);
    }


    [Theory(DisplayName = "AcessoColaborador: colaborador obrigatório")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_Falhar_Criacao_Quando_ColaboradorIdInvalido(
        int colaboradorId)
    {
        // Arrange
        var dataHora = new DateTime(
            2026,
            8,
            24,
            10,
            0,
            0);

        // Act
        var result = AcessoColaborador.Criar(
            1,
            colaboradorId,
            dataHora);

        // Assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "COLABORADOR_ID_OBRIGATORIO");
    }


    [Fact(DisplayName = "AcessoColaborador: data e hora obrigatórias")]
    public void Deve_Falhar_Criacao_Quando_DataHoraForPadrao()
    {
        // Arrange e Act
        var result = AcessoColaborador.Criar(
            1,
            1,
            default);

        // Assert
        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "DATA_HORA_OBRIGATORIA");
    }
}