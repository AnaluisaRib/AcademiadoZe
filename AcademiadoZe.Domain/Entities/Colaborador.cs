// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa, IAggregateRoot
{
    public ColaboradorTipo Tipo { get; private set; }

    public ColaboradorVinculo Vinculo { get; private set; }

    public DateOnly DataAdmissao { get; private set; }

    private Colaborador(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto,
        ColaboradorTipo tipo,
        ColaboradorVinculo vinculo,
        DateOnly dataAdmissao)
        : base(
            id,
            nome,
            cpf,
            dataNascimento,
            telefone,
            email,
            endereco,
            senha,
            foto)
    {
        Tipo = tipo;
        Vinculo = vinculo;
        DataAdmissao = dataAdmissao;
    }

    public static Result<Colaborador> Criar(
        int id,
        string nome,
        string cpf,
        DateOnly dataNascimento,
        string telefone,
        string email,
        Logradouro logradouro,
        string numero,
        string complemento,
        string senha,
        Arquivo foto,
        ColaboradorTipo tipo,
        ColaboradorVinculo vinculo,
        DateOnly dataAdmissao)
    {
        var notifications = new List<Notification>();

        if (NormalizadoService.TextoVazioOuNulo(nome))
        {
            notifications.Add(
                new Notification(
                    "Nome",
                    "NOME_OBRIGATORIO"));
        }
        else
        {
            nome = NormalizadoService.LimparEspacos(nome);
        }

        var cpfResult = Cpf.Criar(cpf);

        if (cpfResult.IsFailure)
            notifications.AddRange(cpfResult.Notifications);

        var telefoneResult = Telefone.Criar(telefone);

        if (telefoneResult.IsFailure)
            notifications.AddRange(telefoneResult.Notifications);

        var emailResult = Email.Criar(email);

        if (emailResult.IsFailure)
            notifications.AddRange(emailResult.Notifications);

        var senhaResult = Senha.Criar(senha);

        if (senhaResult.IsFailure)
            notifications.AddRange(senhaResult.Notifications);

        var enderecoResult = Endereco.Criar(
            logradouro,
            numero,
            complemento);

        if (enderecoResult.IsFailure)
            notifications.AddRange(enderecoResult.Notifications);

        if (foto == null)
        {
            notifications.Add(
                new Notification(
                    "Foto",
                    "FOTO_OBRIGATORIA"));
        }

        if (dataNascimento == default)
        {
            notifications.Add(
                new Notification(
                    "DataNascimento",
                    "DATA_NASCIMENTO_OBRIGATORIO"));
        }

        if (dataAdmissao == default)
        {
            notifications.Add(
                new Notification(
                    "DataAdmissao",
                    "DATA_ADMISSAO_OBRIGATORIA"));
        }

        if (dataAdmissao != default &&
            dataNascimento != default &&
            dataAdmissao < dataNascimento)
        {
            notifications.Add(
                new Notification(
                    "DataAdmissao",
                    "DATA_ADMISSAO_INVALIDA"));
        }

        if (notifications.Count != 0)
        {
            return Result<Colaborador>.Failure(
                notifications);
        }

        var colaborador = new Colaborador(
            id,
            nome,
            cpfResult.Value!,
            dataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            foto,
            tipo,
            vinculo,
            dataAdmissao);

        return Result<Colaborador>.Success(
            colaborador);
    }
}