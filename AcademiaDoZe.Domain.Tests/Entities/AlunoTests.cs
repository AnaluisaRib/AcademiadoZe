// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AlunoTests
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


    [Theory(DisplayName = "Aluno: criação bem-sucedida com nomes válidos (trim aplicado)")]
    [InlineData(" João da Silva ")]
    [InlineData("Maria")]
    [InlineData(" Ana ")]
    [InlineData("Carlos da Silva")]
    public void Deve_Criar_Com_Sucesso_Quando_NomeValido(string nome)
    {
        var result = Aluno.Criar(
            1,
            nome,
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-25)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo());

        Assert.True(result.IsSuccess);

        Assert.Equal(
            nome.Trim(),
            result.Value!.Nome);
    }


    [Theory(DisplayName = "Aluno: nome vazio -> NOME_OBRIGATORIO")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Deve_Falhar_Criacao_Quando_NomeVazio(string nome)
    {
        var result = Aluno.Criar(
            1,
            nome,
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-25)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo());

        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "NOME_OBRIGATORIO");
    }


    [Theory(DisplayName = "Aluno: data de nascimento obrigatória")]
    [InlineData("default")]
    public void Deve_Falhar_Criacao_Quando_DataNascimentoObrigatoria(
        string scenario)
    {
        var result = Aluno.Criar(
            1,
            "João",
            "529.982.247-25",
            default,
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo());

        Assert.True(result.IsFailure);

        Assert.NotEmpty(result.Notifications);

        Assert.Contains(
            result.Notifications,
            n => n.Mensagem == "DATA_NASCIMENTO_OBRIGATORIO");
    }


    [Theory(DisplayName = "Aluno: criação válida com diferentes idades")]
    [InlineData(-10)]
    [InlineData(-18)]
    [InlineData(-30)]
    [InlineData(-50)]
    public void Deve_Criar_Com_Sucesso_Quando_DataNascimentoInformada(
        int anos)
    {
        var dataNascimento =
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(anos));

        var result = Aluno.Criar(
            1,
            "João",
            "529.982.247-25",
            dataNascimento,
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo());

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);

        Assert.Equal(
            dataNascimento,
            result.Value!.DataNascimento);
    }


    [Theory(DisplayName = "Aluno: telefone válido")]
    [InlineData("(11) 91234-5678")]
    [InlineData("11912345678")]
    public void Deve_Criar_Com_Sucesso_Quando_TelefoneValido(
        string telefone)
    {
        var result = Aluno.Criar(
            1,
            "João",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-25)),
            telefone,
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo());

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
    }


    [Theory(DisplayName = "Aluno: e-mail válido")]
    [InlineData("user@example.com")]
    [InlineData("ana@example.com")]
    public void Deve_Criar_Com_Sucesso_Quando_EmailValido(
        string email)
    {
        var result = Aluno.Criar(
            1,
            "João",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-25)),
            "(11) 91234-5678",
            email,
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            GetValidArquivo());

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
    }


    [Theory(DisplayName = "Aluno: foto válida")]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Deve_Criar_Com_Sucesso_Quando_FotoValida(
        int tamanho)
    {
        var foto = new byte[tamanho];

        var arquivoResult = Arquivo.Criar(foto);

        Assert.True(arquivoResult.IsSuccess);

        var result = Aluno.Criar(
            1,
            "João",
            "529.982.247-25",
            DateOnly.FromDateTime(
                DateTime.Today.AddYears(-25)),
            "(11) 91234-5678",
            "user@example.com",
            GetValidLogradouro(),
            "123",
            "",
            "Abcdef1!",
            arquivoResult.Value!);

        Assert.True(result.IsSuccess);

        Assert.NotNull(result.Value);
    }
}