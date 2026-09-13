using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class LogradouroTests
{
    [Fact(DisplayName = "Logradouro: criação válida")]
    public void Deve_Criar_Logradouro_Quando_DadosValidos()
    {
        var result = Logradouro.Criar(
            1,
            "12345-678",
            "Rua Teste",
            "Centro",
            "Bom Retiro",
            "sc",
            "Brasil");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(1, result.Value!.Id);
        Assert.Equal("12345678", result.Value.Cep.Valor);
        Assert.Equal("Rua Teste", result.Value.Nome);
        Assert.Equal("Centro", result.Value.Bairro);
        Assert.Equal("Bom Retiro", result.Value.Cidade);
        Assert.Equal("SC", result.Value.Estado);
        Assert.Equal("Brasil", result.Value.Pais);
    }

    [Theory(DisplayName = "Logradouro: campos obrigatórios")]
    [InlineData("", "Rua Teste", "Centro", "Cidade", "SC", "Brasil", "CEP_OBRIGATORIO")]
    [InlineData("12345-678", "", "Centro", "Cidade", "SC", "Brasil", "NOME_OBRIGATORIO")]
    [InlineData("12345-678", "Rua Teste", "", "Cidade", "SC", "Brasil", "BAIRRO_OBRIGATORIO")]
    [InlineData("12345-678", "Rua Teste", "Centro", "", "SC", "Brasil", "CIDADE_OBRIGATORIO")]
    [InlineData("12345-678", "Rua Teste", "Centro", "Cidade", "", "Brasil", "ESTADO_OBRIGATORIO")]
    [InlineData("12345-678", "Rua Teste", "Centro", "Cidade", "SC", "", "PAIS_OBRIGATORIO")]
    public void Deve_Falhar_Logradouro_Quando_CampoObrigatorioNaoInformado(
        string cep,
        string nome,
        string bairro,
        string cidade,
        string estado,
        string pais,
        string codigoEsperado)
    {
        var result = Logradouro.Criar(
            1,
            cep,
            nome,
            bairro,
            cidade,
            estado,
            pais);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == codigoEsperado);
    }

    [Fact(DisplayName = "Logradouro: normalização dos dados")]
    public void Deve_Normalizar_Dados_Do_Logradouro()
    {
        var result = Logradouro.Criar(
            1,
            "12345-678",
            "  Rua   Teste  ",
            "  Centro  ",
            "  Bom   Retiro  ",
            " s c ",
            "  Brasil  ");

        Assert.True(result.IsSuccess);

        Assert.Equal(
            "Rua Teste",
            result.Value!.Nome);

        Assert.Equal(
            "Centro",
            result.Value.Bairro);

        Assert.Equal(
            "Bom Retiro",
            result.Value.Cidade);

        Assert.Equal(
            "SC",
            result.Value.Estado);

        Assert.Equal(
            "Brasil",
            result.Value.Pais);
    }
}