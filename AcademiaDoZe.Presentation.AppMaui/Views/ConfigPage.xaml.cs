// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    private const string TemaPreferenceKey = "Tema";

    private const string BancoTipoPreferenceKey = "BancoTipo";
    private const string BancoServidorPreferenceKey = "BancoServidor";
    private const string BancoNomePreferenceKey = "BancoNome";
    private const string BancoUsuarioPreferenceKey = "BancoUsuario";
    private const string BancoSenhaPreferenceKey = "BancoSenha";
    private const string BancoComplementoPreferenceKey = "BancoComplemento";

    private readonly string _sqliteCaminhoPadrao =
        @"C:\Users\Ana Luisa\source\repos\AcademiadoZe\db_academia_do_ze.db";

    public ConfigPage()
    {
        InitializeComponent();

        CarregarTema();
        CarregarTiposBanco();
        CarregarConfiguracoes();
    }

    // =========================================================
    // TEMA
    // =========================================================

    private void CarregarTema()
    {
        var tema = Preferences.Get(
            TemaPreferenceKey,
            "system");

        TemaPicker.SelectedItem = tema;
    }

    private async void OnSalvarTemaClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            var tema =
                TemaPicker.SelectedItem?.ToString()
                ?? "system";

            Preferences.Set(
                TemaPreferenceKey,
                tema);

            WeakReferenceMessenger.Default.Send(
                new TemaPreferencesUpdatedMessage(tema));

            await DisplayAlertAsync(
                "Sucesso",
                "Tema salvo com sucesso!",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Erro",
                $"Não foi possível salvar o tema: {ex.Message}",
                "OK");
        }
    }

    // =========================================================
    // BANCO DE DADOS
    // =========================================================

    private void CarregarTiposBanco()
    {
        DatabaseTypePicker.ItemsSource =
            Enum.GetValues<AppDatabaseType>()
                .Select(tipo => tipo.ToString())
                .ToList();

        var tipoSalvo = Preferences.Get(
            BancoTipoPreferenceKey,
            AppDatabaseType.Sqlite.ToString());

        var tipos =
            DatabaseTypePicker.ItemsSource
                .Cast<string>()
                .ToList();

        var index =
            tipos.FindIndex(x =>
                string.Equals(
                    x,
                    tipoSalvo,
                    StringComparison.OrdinalIgnoreCase));

        DatabaseTypePicker.SelectedIndex =
            index >= 0
                ? index
                : 0;
    }

    private void CarregarConfiguracoes()
    {
        SqliteCaminhoEntry.Text =
            Preferences.Get(
                BancoComplementoPreferenceKey,
                _sqliteCaminhoPadrao);

        ServidorEntry.Text =
            Preferences.Get(
                BancoServidorPreferenceKey,
                string.Empty);

        BancoEntry.Text =
            Preferences.Get(
                BancoNomePreferenceKey,
                string.Empty);

        UsuarioEntry.Text =
            Preferences.Get(
                BancoUsuarioPreferenceKey,
                string.Empty);

        SenhaEntry.Text =
            Preferences.Get(
                BancoSenhaPreferenceKey,
                string.Empty);

        ComplementoEntry.Text =
            Preferences.Get(
                BancoComplementoPreferenceKey,
                string.Empty);

        AtualizarInterfacePorTipoBanco();
    }

    private void OnDatabaseTypeChanged(
        object? sender,
        EventArgs e)
    {
        AtualizarInterfacePorTipoBanco();
    }

    private void AtualizarInterfacePorTipoBanco()
    {
        if (DatabaseTypePicker.SelectedItem is null)
            return;

        var tipoSelecionado =
            DatabaseTypePicker.SelectedItem.ToString();

        var isSqlite =
            string.Equals(
                tipoSelecionado,
                AppDatabaseType.Sqlite.ToString(),
                StringComparison.OrdinalIgnoreCase);

        SqliteInfoCard.IsVisible = isSqlite;
        SqliteContainer.IsVisible = isSqlite;

        ServidorBancoGrid.IsVisible = !isSqlite;
        CredenciaisGrid.IsVisible = !isSqlite;

        if (isSqlite)
        {
            ComplementoLabel.Text =
                "Caminho do banco";

            SqliteCaminhoEntry.Text =
                string.IsNullOrWhiteSpace(
                    SqliteCaminhoEntry.Text)
                    ? _sqliteCaminhoPadrao
                    : SqliteCaminhoEntry.Text;
        }
        else
        {
            ComplementoLabel.Text =
                "Complemento";
        }
    }

    private async void OnSalvarBdClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            var tipoTexto =
                DatabaseTypePicker.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(tipoTexto))
            {
                await DisplayAlertAsync(
                    "Atenção",
                    "Selecione o tipo de banco de dados.",
                    "OK");

                return;
            }

            Preferences.Set(
                BancoTipoPreferenceKey,
                tipoTexto);

            Preferences.Set(
                BancoServidorPreferenceKey,
                ServidorEntry.Text ?? string.Empty);

            Preferences.Set(
                BancoNomePreferenceKey,
                BancoEntry.Text ?? string.Empty);

            Preferences.Set(
                BancoUsuarioPreferenceKey,
                UsuarioEntry.Text ?? string.Empty);

            Preferences.Set(
                BancoSenhaPreferenceKey,
                SenhaEntry.Text ?? string.Empty);

            var complemento =
                string.Equals(
                    tipoTexto,
                    AppDatabaseType.Sqlite.ToString(),
                    StringComparison.OrdinalIgnoreCase)
                    ? SqliteCaminhoEntry.Text?.Trim()
                    : ComplementoEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(complemento))
            {
                complemento = _sqliteCaminhoPadrao;
            }

            Preferences.Set(
                BancoComplementoPreferenceKey,
                complemento);

            var databaseType =
                Enum.Parse<AppDatabaseType>(
                    tipoTexto,
                    ignoreCase: true);

            var infrastructureType =
                databaseType.ToInfrastructure();

            var mensagem =
                $"{infrastructureType}|{complemento}";

            WeakReferenceMessenger.Default.Send(
                new BancoPreferencesUpdatedMessage(
                    mensagem));

            await DisplayAlertAsync(
                "Sucesso",
                "Configuração do banco salva com sucesso!",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Erro",
                $"Não foi possível salvar a configuração do banco: {ex.Message}",
                "OK");
        }
    }

    // =========================================================
    // CANCELAR
    // =========================================================

    private async void OnCancelarClicked(
        object? sender,
        EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(
                "//dashboard");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Erro",
                $"Não foi possível voltar: {ex.Message}",
                "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}