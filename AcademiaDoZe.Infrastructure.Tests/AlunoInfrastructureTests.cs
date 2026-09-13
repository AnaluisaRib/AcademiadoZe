// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    private readonly AlunoRepository _alunoRepo;
    private readonly LogradouroRepository _logradouroRepo;

    public AlunoInfrastructureTests()
    {
        _alunoRepo = new AlunoRepository(
            ConnectionString,
            DatabaseType);

        _logradouroRepo = new LogradouroRepository(
            ConnectionString,
            DatabaseType);
    }

    private async Task<Aluno> CriarEInserirAlunoAsync()
    {
        var logradouro =
            await LogradouroInfrastructureTests
                .CriarEInserirLogradouroAsync(
                    _logradouroRepo);

        var result = Aluno.Criar(
            id: 0,
            nome: "Ana Luisa",
            cpf: GerarCpf(),
            dataNascimento: new DateOnly(2000, 1, 1),
            telefone: GerarTelefone(),
            email: GerarEmail(),
            logradouro: logradouro,
            numero: "123",
            complemento: "Casa",
            senha: "SenhaAluno123",
            foto: Arquivo.Criar(
                new byte[] { 1, 2, 3 }).Value!);

        Assert.True(
            result.IsSuccess,
            string.Join(
                " | ",
                result.Notifications.Select(
                    n => n.Mensagem)));

        Assert.NotNull(result.Value);

        return await _alunoRepo.Adicionar(
            result.Value!);
    }

    [Fact]
    public async Task Aluno_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var resultado =
            await _alunoRepo.ObterPorId(
                aluno.Id);

        Assert.NotNull(resultado);

        Assert.Equal(
            aluno.Id,
            resultado.Id);

        Assert.Equal(
            aluno.Nome,
            resultado.Nome);

        Assert.Equal(
            aluno.Cpf.Valor,
            resultado.Cpf.Valor);
    }

    [Fact]
    public async Task Aluno_ObterTodos_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var resultados =
            await _alunoRepo.ObterTodos();

        Assert.NotNull(resultados);

        Assert.Contains(
            resultados,
            x => x.Id == aluno.Id);
    }

    [Fact]
    public async Task Aluno_Atualizar_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var atualizado =
            await _alunoRepo.Atualizar(aluno);

        Assert.NotNull(atualizado);

        Assert.Equal(
            aluno.Id,
            atualizado.Id);

        var consulta =
            await _alunoRepo.ObterPorId(
                aluno.Id);

        Assert.NotNull(consulta);

        Assert.Equal(
            aluno.Cpf.Valor,
            consulta.Cpf.Valor);

        Assert.Equal(
            aluno.Email.Valor,
            consulta.Email.Valor);
    }

    [Fact]
    public async Task Aluno_Atualizar_Inexistente_DeveFalhar()
    {
        var logradouro =
            await LogradouroInfrastructureTests
                .CriarEInserirLogradouroAsync(
                    _logradouroRepo);

        var result = Aluno.Criar(
            id: 999999,
            nome: "Aluno Inexistente",
            cpf: GerarCpf(),
            dataNascimento: new DateOnly(2000, 1, 1),
            telefone: GerarTelefone(),
            email: GerarEmail(),
            logradouro: logradouro,
            numero: "123",
            complemento: "Casa",
            senha: "SenhaAluno123",
            foto: Arquivo.Criar(
                new byte[] { 1, 2, 3 }).Value!);

        Assert.True(
            result.IsSuccess,
            string.Join(
                " | ",
                result.Notifications.Select(
                    n => n.Mensagem)));

        await Assert.ThrowsAsync<InfrastructureException>(
            async () =>
                await _alunoRepo.Atualizar(
                    result.Value!));
    }

    [Fact]
    public async Task Aluno_Remover_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var removido =
            await _alunoRepo.Remover(
                aluno.Id);

        Assert.True(removido);

        var resultado =
            await _alunoRepo.ObterPorId(
                aluno.Id);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Aluno_ObterPorCpf_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var resultado =
            await _alunoRepo.ObterPorCpf(
                aluno.Cpf);

        Assert.NotNull(resultado);

        Assert.Equal(
            aluno.Id,
            resultado.Id);

        Assert.Equal(
            aluno.Cpf.Valor,
            resultado.Cpf.Valor);
    }

    [Fact]
    public async Task Aluno_ObterPorEmail_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var resultado =
            await _alunoRepo.ObterPorEmail(
                aluno.Email);

        Assert.NotNull(resultado);

        Assert.Equal(
            aluno.Id,
            resultado.Id);

        Assert.Equal(
            aluno.Email.Valor,
            resultado.Email.Valor);
    }

    [Fact]
    public async Task Aluno_CpfJaExiste_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var existe =
            await _alunoRepo.CpfJaExiste(
                aluno.Cpf);

        Assert.True(existe);

        var mesmoAluno =
            await _alunoRepo.CpfJaExiste(
                aluno.Cpf,
                aluno.Id);

        Assert.False(mesmoAluno);

        var naoExiste =
            await _alunoRepo.CpfJaExiste(
                Cpf.Criar(
                    GerarCpf()).Value!);

        Assert.False(naoExiste);
    }

    [Fact]
    public async Task Aluno_EmailJaExiste_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var existe =
            await _alunoRepo.EmailJaExiste(
                aluno.Email);

        Assert.True(existe);

        var mesmoAluno =
            await _alunoRepo.EmailJaExiste(
                aluno.Email,
                aluno.Id);

        Assert.False(mesmoAluno);

        var naoExiste =
            await _alunoRepo.EmailJaExiste(
                Email.Criar(
                    GerarEmail()).Value!);

        Assert.False(naoExiste);
    }

    [Fact]
    public async Task Aluno_ObterPorNome_Sucesso()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var resultados =
            await _alunoRepo.ObterPorNome(
                aluno.Nome);

        Assert.NotNull(resultados);

        Assert.Contains(
            resultados,
            x => x.Id == aluno.Id);
    }

    [Fact]
    public async Task Aluno_TrocarSenha_SucessoEFalha()
    {
        var aluno =
            await CriarEInserirAlunoAsync();

        var novaSenha =
            Senha.Criar(
                "NovaSenhaAluno123").Value!;

        var alterou =
            await _alunoRepo.TrocarSenha(
                aluno.Id,
                novaSenha);

        Assert.True(alterou);

        var atualizado =
            await _alunoRepo.ObterPorId(
                aluno.Id);

        Assert.NotNull(atualizado);

        Assert.Equal(
            "NovaSenhaAluno123",
            atualizado.Senha.Valor);

        var alterouInexistente =
            await _alunoRepo.TrocarSenha(
                999999,
                novaSenha);

        Assert.False(alterouInexistente);
    }
}