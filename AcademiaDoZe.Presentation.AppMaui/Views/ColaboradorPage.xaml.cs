
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(ColaboradorId), "Id")]
public partial class ColaboradorPage : ContentPage
{
    private readonly ColaboradorViewModel _viewModel;
    private bool _inicializado;

    public int ColaboradorId
    {
        set => _viewModel.ColaboradorId = value;
    }

    public ColaboradorPage(ColaboradorViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_inicializado)
            return;

        _inicializado = true;

        await _viewModel.InitializeAsync();
    }
}
