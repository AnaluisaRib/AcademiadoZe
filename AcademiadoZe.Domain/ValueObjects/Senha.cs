// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_OBRIGATORIA");

        var senhaNormalizada =
            NormalizadoService.LimparEspacos(valor);

        if (senhaNormalizada.Length < 8)
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_MINIMO_8_CARACTERES");

        return Result<Senha>.Success(
            new Senha(senhaNormalizada));
    }

    public override string ToString()
    {
        return Valor;
    }
}