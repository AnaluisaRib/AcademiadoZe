// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity, IAggregateRoot
{
    public int AlunoId { get; private set; }

    public DateTime DataHora { get; private set; }

    private AcessoAluno(
        int id,
        int alunoId,
        DateTime dataHora)
        : base(id)
    {
        AlunoId = alunoId;
        DataHora = dataHora;
    }

    public static Result<AcessoAluno> Criar(
        int id,
        int alunoId,
        DateTime dataHora)
    {
        var notifications =
            new List<Notification>();

        if (alunoId <= 0)
        {
            notifications.Add(
                new Notification(
                    "AlunoId",
                    "ALUNO_ID_OBRIGATORIO"));
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
            return Result<AcessoAluno>.Failure(
                notifications);
        }

        return Result<AcessoAluno>.Success(
            new AcessoAluno(
                id,
                alunoId,
                dataHora));
    }
}