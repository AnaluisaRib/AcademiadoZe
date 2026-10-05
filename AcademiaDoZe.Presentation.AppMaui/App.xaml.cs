// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(
            this,
            (_, message) =>
            {
                AplicarTema(message.Value);
            });

        AplicarTema(
            Preferences.Default.Get(
                "Tema",
                "system"));
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private void AplicarTema(string tema)
    {
        UserAppTheme = tema.ToLowerInvariant() switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}