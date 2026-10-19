
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class AlunoListViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;

    public ObservableCollection<AlunoDto> Alunos { get; } = [];

    private AlunoDto? _selectedAluno;

    public AlunoDto? SelectedAluno
    {
        get => _selectedAluno;
        set
        {
            if (SetProperty(ref _selectedAluno, value) && value is not null)
            {
                var aluno = value;
                SelectedAluno = null;
                _ = EditarAlunoAsync(aluno);
            }
        }
    }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public AlunoListViewModel(IAlunoService alunoService)
    {
        _alunoService = alunoService;
        Title = "Alunos";
    }

    public async Task CarregarAsync()
    {
        try
        {
            IsBusy = true;

            var resultado = await _alunoService.ObterTodosAsync();

            Alunos.Clear();

            foreach (var aluno in resultado)
                Alunos.Add(aluno);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível carregar os alunos: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PesquisarAsync()
    {
        try
        {
            IsBusy = true;

            var resultado = await _alunoService.ObterTodosAsync();

            var pesquisa = resultado.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var cpf = new string(
                    SearchText.Where(char.IsDigit).ToArray());

                pesquisa = pesquisa.Where(aluno =>
                    new string((aluno.Cpf ?? string.Empty)
                        .Where(char.IsDigit)
                        .ToArray())
                    .Contains(cpf));
            }

            Alunos.Clear();

            foreach (var aluno in pesquisa)
                Alunos.Add(aluno);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível pesquisar os alunos: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NovoAlunoAsync()
    {
        await Shell.Current.GoToAsync("aluno");
    }

    private async Task EditarAlunoAsync(AlunoDto aluno)
    {
        try
        {
            await Shell.Current.GoToAsync($"aluno?Id={aluno.Id}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível abrir o aluno: {ex.Message}",
                "OK");
        }
    }
}
