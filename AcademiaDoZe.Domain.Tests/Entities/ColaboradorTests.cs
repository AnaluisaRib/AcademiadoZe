using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class ColaboradorTests
{
    private static Logradouro GetValidLogradouro() =>
        Logradouro.Criar(
            1,
            "12345-678",
            "Rua Teste",
            "Bairro",
            "Cidade",
            "SP",
            "Brasil").Value!;

    private static Arquivo GetValidArquivo() =>
        Arquivo.Criar(
            new byte[] { 1, 2, 3 }).Value!;


    [Theory(DisplayName = "Colaborador: data admissão obrigatória")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deve_Validar_Criacao_Quando_DataAdmissao(
        bool useDefault)
    {
        var dataAdmissao =
            useDefault
                ? default(DateOnly)
                : DateOnly.FromDateTime(
                    DateTime.Today.AddYears(-1));

        var result = Colaborador.Criar(
            1,
            "Fulano",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-30)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo(),
            ColaboradorTipo.Atendente,
            ColaboradorVinculo.CLT,
            dataAdmissao);

        if (useDefault)
        {
            Assert.True(result.IsFailure);

            Assert.NotEmpty(result.Notifications);

            Assert.Contains(
                result.Notifications,
                n => n.Mensagem == "DATA_ADMISSAO_OBRIGATORIA");
        }
        else
        {
            Assert.True(result.IsSuccess);
        }
    }


    [Theory(DisplayName = "Colaborador: criação com tipo e vínculo")]
    [InlineData(
        ColaboradorTipo.Administrador,
        ColaboradorVinculo.Estagiario)]
    [InlineData(
        ColaboradorTipo.Administrador,
        ColaboradorVinculo.CLT)]
    public void Deve_Criar_Com_Sucesso_Quando_TipoEVinculoInformados(
        ColaboradorTipo tipo,
        ColaboradorVinculo vinc)
    {
        var result = Colaborador.Criar(
            1,
            "Fulano",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-30)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo(),
            tipo,
            vinc,
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-1)));

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);

        Assert.Equal(
            tipo,
            result.Value!.Tipo);

        Assert.Equal(
            vinc,
            result.Value.Vinculo);
    }


    [Theory(DisplayName = "Colaborador: data admissão futura é aceita")]
    [InlineData(1)]
    [InlineData(-1)]
    public void Deve_Criar_Com_Sucesso_Quando_DataAdmissaoForFuturaOuAnterior(
        int daysOffset)
    {
        var date =
            DateOnly.FromDateTime(
                DateTime.Today.AddDays(daysOffset));

        var result = Colaborador.Criar(
            1,
            "Fulano",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-30)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo(),
            ColaboradorTipo.Atendente,
            ColaboradorVinculo.CLT,
            date);

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);

        Assert.Equal(
            date,
            result.Value!.DataAdmissao);
    }


    [Theory(DisplayName = "Colaborador: tipo e vínculo podem ser informados")]
    [InlineData(999, 1)]
    [InlineData(1, 999)]
    public void Deve_Criar_Com_Sucesso_Quando_TipoOuVinculoNaoForemValidados(
        int tipoValue,
        int vincValue)
    {
        var tipo = (ColaboradorTipo)tipoValue;
        var vinc = (ColaboradorVinculo)vincValue;

        var result = Colaborador.Criar(
            1,
            "Fulano",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-30)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo(),
            tipo,
            vinc,
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-1)));

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);

        Assert.Equal(
            tipo,
            result.Value!.Tipo);

        Assert.Equal(
            vinc,
            result.Value.Vinculo);
    }
}