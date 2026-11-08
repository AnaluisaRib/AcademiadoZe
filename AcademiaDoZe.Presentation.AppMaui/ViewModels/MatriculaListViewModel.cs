
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class MatriculaListViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;

    public ObservableCollection<string> FilterTypes { get; } =
    [
        "Aluno",
        "CPF",
        "ID",
        "Plano",
        "Objetivo"
    ];

    private ObservableCollection<MatriculaDto> _matriculas = [];

    public ObservableCollection<MatriculaDto> Matriculas
    {
        get => _matriculas;
        set => SetProperty(ref _matriculas, value);
    }

    private MatriculaDto? _selectedMatricula;

    public MatriculaDto? SelectedMatricula
    {
        get => _selectedMatricula;
        set
        {
            if (SetProperty(ref _selectedMatricula, value) &&
                value is not null)
            {
                _ = EditarMatriculaAsync(value);
            }
        }
    }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string _selectedFilterType = "Aluno";

    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    public MatriculaListViewModel(
        IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
        Title = "Matrículas";
    }

    public async Task CarregarAsync()
    {
        try
        {
            IsBusy = true;

            var resultado =
                await _matriculaService.ObterTodasAsync();

            Matriculas.Clear();

            foreach (var matricula in resultado)
            {
                Matriculas.Add(matricula);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível carregar as matrículas: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Pesquisar()
    {
        await PesquisarAsync();
    }

    public async Task PesquisarAsync()
    {
        try
        {
            IsBusy = true;

            var resultado =
                await _matriculaService.ObterTodasAsync();

            var pesquisa = resultado.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var termo = SearchText.Trim();

                pesquisa = SelectedFilterType switch
                {
                    "Aluno" => pesquisa.Where(m =>
                        m.AlunoMatricula.Nome.Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "CPF" => pesquisa.Where(m =>
                        m.AlunoMatricula.Cpf.Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "ID" => pesquisa.Where(m =>
                        m.Id.ToString().Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "Plano" => pesquisa.Where(m =>
                        m.Plano.ToString().Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "Objetivo" => pesquisa.Where(m =>
                        m.Objetivo.Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    _ => pesquisa
                };
            }

            Matriculas.Clear();

            foreach (var matricula in pesquisa)
            {
                Matriculas.Add(matricula);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível pesquisar: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NovaMatricula()
    {
        await Shell.Current.GoToAsync("matricula");
    }

    private async Task EditarMatriculaAsync(
        MatriculaDto matricula)
    {
        try
        {
            SelectedMatricula = null;

            await Shell.Current.GoToAsync(
                $"matricula?Id={matricula.Id}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível abrir a matrícula: {ex.Message}",
                "OK");
        }
    }
}
