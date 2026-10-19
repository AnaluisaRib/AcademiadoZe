// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorListViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;

    public ObservableCollection<string> FilterTypes { get; } =
    [
        "Nome",
        "CPF",
        "Id",
        "Email",
        "Tipo",
        "Vínculo"
    ];

    private ObservableCollection<ColaboradorDto> _colaboradores = [];

    public ObservableCollection<ColaboradorDto> Colaboradores
    {
        get => _colaboradores;
        set => SetProperty(ref _colaboradores, value);
    }

    private ColaboradorDto? _selectedColaborador;

    public ColaboradorDto? SelectedColaborador
    {
        get => _selectedColaborador;
        set
        {
            if (SetProperty(ref _selectedColaborador, value) &&
                value is not null)
            {
                _ = EditarColaboradorAsync(value);
            }
        }
    }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private string _selectedFilterType = "Nome";

    public string SelectedFilterType
    {
        get => _selectedFilterType;
        set => SetProperty(ref _selectedFilterType, value);
    }

    public ColaboradorListViewModel(
        IColaboradorService colaboradorService)
    {
        _colaboradorService = colaboradorService;
        Title = "Colaboradores";
    }

    public async Task CarregarAsync()
    {
        try
        {
            IsBusy = true;

            var resultado = await _colaboradorService.ObterTodosAsync();

            Colaboradores.Clear();

            foreach (var colaborador in resultado)
            {
                Colaboradores.Add(colaborador);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível carregar os colaboradores: {ex.Message}",
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

            var resultado = await _colaboradorService.ObterTodosAsync();
            var pesquisa = resultado.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var termo = SearchText.Trim();

                pesquisa = SelectedFilterType switch
                {
                    "Nome" => pesquisa.Where(c =>
                        c.Nome.Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "CPF" => pesquisa.Where(c =>
                        c.Cpf.Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "Id" => pesquisa.Where(c =>
                        c.Id.ToString().Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "Email" => pesquisa.Where(c =>
                        (c.Email ?? string.Empty).Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "Tipo" => pesquisa.Where(c =>
                        c.Tipo.ToString().Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    "Vínculo" => pesquisa.Where(c =>
                        c.Vinculo.ToString().Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)),

                    _ => pesquisa
                };
            }

            Colaboradores.Clear();

            foreach (var colaborador in pesquisa)
            {
                Colaboradores.Add(colaborador);
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
    private async Task NovoColaborador()
    {
        await Shell.Current.GoToAsync("colaborador");
    }

    private async Task EditarColaboradorAsync(ColaboradorDto colaborador)
    {
        try
        {
            SelectedColaborador = null;

            await Shell.Current.GoToAsync(
                $"colaborador?Id={colaborador.Id}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível abrir o colaborador: {ex.Message}",
                "OK");
        }
    }
}
