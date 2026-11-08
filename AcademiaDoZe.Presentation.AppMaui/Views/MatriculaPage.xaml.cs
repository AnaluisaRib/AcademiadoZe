
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(MatriculaId), "Id")]
public partial class MatriculaPage : ContentPage
{
    private readonly MatriculaViewModel _viewModel;
    private bool _inicializado;

    public int MatriculaId
    {
        set => _viewModel.MatriculaId = value;
    }

    public MatriculaPage(MatriculaViewModel viewModel)
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
