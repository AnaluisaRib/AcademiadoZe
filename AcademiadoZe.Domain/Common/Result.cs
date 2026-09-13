// Ana Luisa Ribeiro de Araujo

namespace AcademiaDoZe.Domain.Common;

public class Result<T>
{
    public T? Value { get; }

    public IReadOnlyCollection<Notification> Notifications { get; }

    public bool IsSuccess => Notifications.Count == 0;

    public bool IsFailure => Notifications.Count != 0;

    private Result(
        T? value,
        IEnumerable<Notification> notifications)
    {
        Value = value;
        Notifications =
            notifications.ToList().AsReadOnly();
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, []);
    }

    public static Result<T> Failure(
        IEnumerable<Notification> notifications)
    {
        return new Result<T>(
            default,
            notifications);
    }

    public static Result<T> Failure(
        string propriedade,
        string mensagem)
    {
        return new Result<T>(
            default,
            [new Notification(
                propriedade,
                mensagem)]);
    }

    public static Result<T> Failure(
        Notification notification)
    {
        return new Result<T>(
            default,
            [notification]);
    }
}