// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class MatriculaMappingExtensions
{
    public static MatriculaDto ToDto(
     this Matricula matricula,
     Aluno aluno,
     Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(matricula);

        return new MatriculaDto
        {
            Id = matricula.Id,
            AlunoMatricula = aluno.ToDto(logradouro),
            Plano = matricula.Plano.ToApplication(),
            DataInicio = matricula.DataInicio,
            DataFim = matricula.DataFim,
            Objetivo = matricula.Objetivo,
            RestricoesMedicas = matricula.RestricoesMedicas.ToApplication(),
            ObservacoesRestricoes = matricula.ObservacoesRestricoes,
            LaudoMedico = matricula.LaudoMedico?.Conteudo != null
                ? new ArquivoDto
                {
                    Conteudo = matricula.LaudoMedico.Conteudo
                }
                : null
        };
    }

    public static Matricula ToEntity(
        this MatriculaDto matriculaDto,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(matriculaDto);

        var aluno = matriculaDto.AlunoMatricula.ToEntity(logradouro);

        Arquivo? laudoMedico = null;

        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult =
                Arquivo.Criar(matriculaDto.LaudoMedico.Conteudo);

            if (laudoResult.IsSuccess)
            {
                laudoMedico = laudoResult.Value;
            }
        }

        var result = Matricula.Criar(
            matriculaDto.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            laudoMedico,
            matriculaDto.ObservacoesRestricoes ?? string.Empty
        );

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação ao converter Matrícula: " +
                $"{string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
        }

        return result.Value!;
    }

    public static Matricula UpdateFromDto(
        this Matricula matricula,
        MatriculaDto matriculaDto,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(matriculaDto);

        var aluno = matriculaDto.AlunoMatricula.ToEntity(logradouro);

        Arquivo? laudoMedico = matricula.LaudoMedico;

        if (matriculaDto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult =
                Arquivo.Criar(matriculaDto.LaudoMedico.Conteudo);

            if (laudoResult.IsSuccess)
            {
                laudoMedico = laudoResult.Value;
            }
        }

        var result = Matricula.Criar(
            matricula.Id,
            aluno,
            matriculaDto.Plano.ToDomain(),
            matriculaDto.DataInicio,
            matriculaDto.Objetivo,
            matriculaDto.RestricoesMedicas.ToDomain(),
            laudoMedico,
            matriculaDto.ObservacoesRestricoes ?? string.Empty
        );

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação ao atualizar Matrícula: " +
                $"{string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
        }

        return result.Value!;
    }
}