// Ana Luisa Ribeiro de Araujo

using System.Net.Mail;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Result<Email> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Email>.Failure(
                "Email",
                "EMAIL_OBRIGATORIO");

        var emailNormalizado =
            NormalizadoService.LimparEspacos(valor)
                .ToLowerInvariant();

        try
        {
            var endereco = new MailAddress(emailNormalizado);

            if (endereco.Address != emailNormalizado)
                return Result<Email>.Failure(
                    "Email",
                    "EMAIL_INVALIDO");
        }
        catch
        {
            return Result<Email>.Failure(
                "Email",
                "EMAIL_INVALIDO");
        }

        return Result<Email>.Success(
            new Email(emailNormalizado));
    }

    public override string ToString()
    {
        return Valor;
    }
}