// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class LogradouroPage : ContentPage
{
    public LogradouroPage(LogradouroViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is LogradouroViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    private async void OnCancelButtonClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Erro",
                $"Não foi possível cancelar: {ex.Message}",
                "OK");
        }
    }

    private async void OnSaveButtonClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            if (BindingContext is LogradouroViewModel viewModel)
            {
                await viewModel.SaveLogradouroCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Erro",
                $"Não foi possível salvar o logradouro: {ex.Message}",
                "OK");
        }
    }
}
