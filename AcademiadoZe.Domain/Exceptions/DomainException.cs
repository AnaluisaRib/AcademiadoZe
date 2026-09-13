// Ana Luisa Ribeiro de Araujo

namespace AcademiaDoZe.Domain.Exceptions;

public sealed class DomainException(string message)
    : Exception(message)
{
}