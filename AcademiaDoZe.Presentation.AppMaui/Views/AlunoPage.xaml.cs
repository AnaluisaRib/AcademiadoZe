
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoPage : ContentPage
{
    private readonly AlunoViewModel _viewModel;
    private bool _inicializado;

    public AlunoPage(AlunoViewModel viewModel)
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
