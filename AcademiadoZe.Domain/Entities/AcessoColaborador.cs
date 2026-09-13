// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity, IAggregateRoot
{
    public int ColaboradorId { get; private set; }

    public DateTime DataHora { get; private set; }

    private AcessoColaborador(
        int id,
        int colaboradorId,
        DateTime dataHora)
        : base(id)
    {
        ColaboradorId = colaboradorId;
        DataHora = dataHora;
    }

    public static Result<AcessoColaborador> Criar(
        int id,
        int colaboradorId,
        DateTime dataHora)
    {
        var notifications =
            new List<Notification>();

        if (colaboradorId <= 0)
        {
            notifications.Add(
                new Notification(
                    "ColaboradorId",
                    "COLABORADOR_ID_OBRIGATORIO"));
        }

        if (dataHora == default)
        {
            notifications.Add(
                new Notification(
                    "DataHora",
                    "DATA_HORA_OBRIGATORIA"));
        }

        if (notifications.Count != 0)
        {
            return Result<AcessoColaborador>.Failure(
                notifications);
        }

        return Result<AcessoColaborador>.Success(
            new AcessoColaborador(
                id,
                colaboradorId,
                dataHora));
    }
}