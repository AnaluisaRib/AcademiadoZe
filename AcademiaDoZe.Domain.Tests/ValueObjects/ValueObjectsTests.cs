// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class ValueObjectsTests
{
    [Theory(DisplayName = "Cep: dígitos inválidos -> CEP_DIGITOS")]
    [InlineData("123")]
    [InlineData("12-345")]
    public void Deve_Falhar_Criacao_Quando_CepDigitosInvalidos(string input)
    {
        var result = Cep.Criar(input);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }


    [Theory(DisplayName = "Cep: formatos válidos (com e sem hífen)")]
    [InlineData("12345-678")]
    [InlineData("12345678")]
    public void Deve_Criar_Cep_Quando_Valido(string input)
    {
        var result = Cep.Criar(input);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            "12345678",
            result.Value!.Valor);
    }


    [Theory(DisplayName = "Cep: obrigatório -> CEP_OBRIGATORIO")]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_CepNuloOuVazio(string? input)
    {
        var result = Cep.Criar(input!);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CEP_OBRIGATORIO");
    }


    [Theory(DisplayName = "Endereco: criação válida com número e complemento")]
    [InlineData("10", "Bloco A")]
    [InlineData("1", "")]
    public void Deve_Criar_Endereco_Quando_Valido(
        string numero,
        string complemento)
    {
        var logradouro = Logradouro
            .Criar(
                1,
                "12345-678",
                "Rua Teste",
                "Bairro",
                "Cidade",
                "SP",
                "Brasil")
            .Value!;

        var result = Endereco.Criar(
            logradouro,
            numero,
            complemento);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            numero,
            result.Value!.Numero);

        Assert.Equal(
            complemento,
            result.Value!.Complemento);
    }


    [Theory(DisplayName = "Endereco: valida obrigatoriedade do logradouro e número")]
    [InlineData(null, "1", "LOGRADOURO_OBRIGATORIO")]
    [InlineData("valid", "", "NUMERO_OBRIGATORIO")]
    public void Deve_Falhar_Criacao_Quando_EnderecoInvalido(
        string? logradouroCase,
        string numero,
        string expected)
    {
        Logradouro? logradouro = null;

        if (logradouroCase == "valid")
        {
            logradouro = Logradouro
                .Criar(
                    1,
                    "12345-678",
                    "Rua Teste",
                    "Bairro",
                    "Cidade",
                    "SP",
                    "Brasil")
                .Value!;
        }

        var result = Endereco.Criar(
            logradouro!,
            numero,
            "");

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == expected);
    }


    [Theory(DisplayName = "Cep: formatos inválidos")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("abcde-678")]
    [InlineData("12345-67a")]
    public void Deve_Falhar_Criacao_Quando_CepFormatoInvalido(
        string input)
    {
        var result = Cep.Criar(input);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }


    [Theory(DisplayName = "Cep: quantidade de dígitos inválida")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("123")]
    [InlineData("123456")]
    public void Deve_Falhar_Criacao_Quando_QuantidadeDeDigitosInvalida(
        string input)
    {
        var result = Cep.Criar(input);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }


    [Theory(DisplayName = "Cpf: nulo/vazio/espaços -> CPF_OBRIGATORIO")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Deve_Falhar_Criacao_Quando_CpfNuloOuVazio(
        string? input)
    {
        var result = Cpf.Criar(input!);

        Assert.True(result.IsFailure);

        Assert.Single(result.Notifications);

        Assert.Equal(
            "CPF_OBRIGATORIO",
            result.Notifications.First().Mensagem);
    }


    [Theory(DisplayName = "Cpf: formatos válidos")]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void Deve_Criar_Cpf_Quando_ValorValido(
        string input)
    {
        var result = Cpf.Criar(input);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            "52998224725",
            result.Value!.Valor);
    }


    [Theory(DisplayName = "Cpf: formato inválido")]
    [InlineData("123456789")]
    [InlineData("123456789012")]
    public void Deve_Falhar_Criacao_Quando_CpfFormatoInvalido(
        string input)
    {
        var result = Cpf.Criar(input);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Notifications);
    }


    [Theory(DisplayName = "Cpf: sem dígitos -> CPF_DIGITOS")]
    [InlineData("dfgdf")]
    [InlineData("abc")]
    public void Deve_Falhar_Criacao_Quando_CpfSemDigitos(
        string input)
    {
        var result = Cpf.Criar(input);

        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "CPF_DIGITOS");
    }


    [Theory(DisplayName = "Telefone: dígitos inválidos")]
    [InlineData("1234")]
    [InlineData("(1)2345")]
    public void Deve_Falhar_Criacao_Quando_TelefoneDigitosInvalidos(
        string input)
    {
        var result = Telefone.Criar(input);

        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "TELEFONE_DIGITOS");
    }


    [Theory(DisplayName = "Telefone: formatos válidos")]
    [InlineData("(11) 91234-5678")]
    [InlineData("11912345678")]
    public void Deve_Criar_Telefone_Quando_Valido(
        string input)
    {
        var result = Telefone.Criar(input);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            "11912345678",
            result.Value!.Valor);
    }


    [Theory(DisplayName = "Telefone: obrigatório")]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_TelefoneNuloOuVazio(
        string? input)
    {
        var result = Telefone.Criar(input!);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "TELEFONE_OBRIGATORIO");
    }


    [Theory(DisplayName = "Senha: criação com tamanho válido")]
    [InlineData("Abcdef12")]
    [InlineData("12345678")]
    public void Deve_Criar_Senha_Quando_TamanhoValido(
        string senha)
    {
        var result = Senha.Criar(senha);

        Assert.True(result.IsSuccess);
    }


    [Theory(DisplayName = "Senha: nula ou vazia deve falhar")]
    [InlineData(null)]
    [InlineData("")]
    public void Deve_Falhar_Criacao_Quando_SenhaNulaOuVazia(
        string? input)
    {
        var result = Senha.Criar(input!);

        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "SENHA_OBRIGATORIA");
    }
}