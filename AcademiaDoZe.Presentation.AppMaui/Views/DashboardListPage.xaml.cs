// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class DashboardListPage : ContentPage
{
    private DashboardListViewModel? ViewModel =>
        BindingContext as DashboardListViewModel;

    public DashboardListPage()
    {
        InitializeComponent();

        var services =
            Microsoft.Maui.Controls.Application.Current?
            .Handler?
            .MauiContext?
            .Services;

        if (services is not null)
        {
            BindingContext =
                services.GetRequiredService<DashboardListViewModel>();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ViewModel is not null)
        {
            await ViewModel.LoadDashboardDataCommand.ExecuteAsync(null);
        }
    }
}