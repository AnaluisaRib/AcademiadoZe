// Ana Luisa Ribeiro de Araujo

using CommunityToolkit.Mvvm.ComponentModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    private bool isBusy;

    // Indica se uma operação está em andamento.
    // Pode ser utilizado para mostrar indicadores de carregamento na UI.
    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    private string title = string.Empty;

    // Título da ViewModel.
    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    private bool isRefreshing;

    // Indica se a ViewModel está em estado de atualização.
    // É utilizado pelo RefreshView da tela de Logradouros.
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }
}