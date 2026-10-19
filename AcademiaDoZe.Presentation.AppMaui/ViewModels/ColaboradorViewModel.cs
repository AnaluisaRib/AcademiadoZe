// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;
    private readonly ILogradouroService _logradouroService;

    public IEnumerable<AppColaboradorTipo> ColaboradorTipos { get; } =
        Enum.GetValues<AppColaboradorTipo>();

    public IEnumerable<AppColaboradorVinculo> ColaboradorVinculos { get; } =
        Enum.GetValues<AppColaboradorVinculo>();

    private ColaboradorDto _colaborador = CriarColaboradorPadrao();

    public ColaboradorDto Colaborador
    {
        get => _colaborador;
        set
        {
            if (SetProperty(ref _colaborador, value))
            {
                OnPropertyChanged(nameof(DataNascimentoSelecionada));
                OnPropertyChanged(nameof(DataAdmissaoSelecionada));
                OnPropertyChanged(nameof(HasEnderecoVinculado));
                OnPropertyChanged(nameof(HasNoEnderecoVinculado));
                AtualizarFotoExibida();
            }
        }
    }

    private int _colaboradorId;

    public int ColaboradorId
    {
        get => _colaboradorId;
        set => SetProperty(ref _colaboradorId, value);
    }

    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    private string _confirmarSenha = string.Empty;

    public string ConfirmarSenha
    {
        get => _confirmarSenha;
        set => SetProperty(ref _confirmarSenha, value);
    }

    private string _cep = string.Empty;

    public string Cep
    {
        get => _cep;
        set => SetProperty(ref _cep, value);
    }

    private ImageSource? _fotoSelecionada;

    public ImageSource? FotoSelecionada
    {
        get => _fotoSelecionada;
        set => SetProperty(ref _fotoSelecionada, value);
    }

    public DateTime DataNascimentoSelecionada
    {
        get => Colaborador.DataNascimento.ToDateTime(TimeOnly.MinValue);
        set
        {
            var data = DateOnly.FromDateTime(value);

            if (Colaborador.DataNascimento != data)
            {
                Colaborador.DataNascimento = data;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DataAdmissaoSelecionada
    {
        get => Colaborador.DataAdmissao.ToDateTime(TimeOnly.MinValue);
        set
        {
            var data = DateOnly.FromDateTime(value);

            if (Colaborador.DataAdmissao != data)
            {
                Colaborador.DataAdmissao = data;
                OnPropertyChanged();
            }
        }
    }

    public bool HasEnderecoVinculado =>
        Colaborador.Endereco is not null &&
        Colaborador.Endereco.Id > 0;

    public bool HasNoEnderecoVinculado => !HasEnderecoVinculado;

    public ColaboradorViewModel(
        IColaboradorService colaboradorService,
        ILogradouroService logradouroService)
    {
        _colaboradorService = colaboradorService;
        _logradouroService = logradouroService;

        Title = "Cadastro de Colaborador";
    }

    public async Task InitializeAsync()
    {
        if (ColaboradorId > 0)
        {
            IsEditMode = true;
            await CarregarColaboradorAsync(ColaboradorId);
        }
        else
        {
            IsEditMode = false;
            Colaborador = CriarColaboradorPadrao();
            Cep = string.Empty;
            ConfirmarSenha = string.Empty;
            FotoSelecionada = null;
        }
    }

    public async Task CarregarColaboradorAsync(int id)
    {
        try
        {
            IsBusy = true;

            var resultado =
                await _colaboradorService.ObterPorIdAsync(id);

            if (resultado is null)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "Colaborador não encontrado.",
                    "OK");

                return;
            }

            Colaborador = resultado;
            Cep = resultado.Endereco?.Cep ?? string.Empty;
            ConfirmarSenha = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível carregar o colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelecionarFotoAsync()
    {
        try
        {
            var arquivo = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Selecione a foto do colaborador",
                    FileTypes = FilePickerFileType.Images
                });

            if (arquivo is null)
                return;

            await using var stream = await arquivo.OpenReadAsync();
            using var memoria = new MemoryStream();

            await stream.CopyToAsync(memoria);

            var conteudo = memoria.ToArray();

            if (conteudo.Length == 0)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "O arquivo selecionado está vazio.",
                    "OK");

                return;
            }

            Colaborador.Foto = new ArquivoDto
            {
                Conteudo = conteudo
            };

            FotoSelecionada = ImageSource.FromStream(
                () => new MemoryStream(conteudo));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível selecionar a foto: {ex.Message}",
                "OK");
        }
    }

    private void AtualizarFotoExibida()
    {
        var conteudo = Colaborador.Foto?.Conteudo;

        FotoSelecionada = conteudo is { Length: > 0 }
            ? ImageSource.FromStream(() => new MemoryStream(conteudo))
            : null;
    }

    [RelayCommand]
    private async Task PesquisarEnderecoPorCepAsync()
    {
        try
        {
            var cepLimpo =
                new string(Cep.Where(char.IsDigit).ToArray());

            if (cepLimpo.Length != 8)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Informe um CEP com 8 dígitos.",
                    "OK");

                return;
            }

            IsBusy = true;

            var endereco =
                await _logradouroService.ObterPorCepAsync(cepLimpo);

            if (endereco is null)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "Nenhum logradouro encontrado para esse CEP. Cadastre o logradouro primeiro.",
                    "OK");

                return;
            }

            Colaborador.Endereco = endereco;

            OnPropertyChanged(nameof(Colaborador));
            OnPropertyChanged(nameof(HasEnderecoVinculado));
            OnPropertyChanged(nameof(HasNoEnderecoVinculado));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível pesquisar o CEP: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Colaborador.Nome) ||
                string.IsNullOrWhiteSpace(Colaborador.Cpf) ||
                string.IsNullOrWhiteSpace(Colaborador.Telefone) ||
                string.IsNullOrWhiteSpace(Colaborador.Numero))
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Preencha nome, CPF, telefone e número do endereço.",
                    "OK");

                return;
            }

            if (Colaborador.Endereco is null ||
                Colaborador.Endereco.Id <= 0)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Pesquise o CEP e selecione um logradouro cadastrado antes de salvar.",
                    "OK");

                return;
            }

            if (Colaborador.Foto?.Conteudo is not { Length: > 0 })
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Selecione uma foto do colaborador antes de salvar.",
                    "OK");

                return;
            }

            if (!IsEditMode)
            {
                if (string.IsNullOrWhiteSpace(Colaborador.Senha) ||
                    Colaborador.Senha.Length < 6)
                {
                    await Shell.Current.DisplayAlert(
                        "Atenção",
                        "Informe uma senha com pelo menos 6 caracteres.",
                        "OK");

                    return;
                }

                if (Colaborador.Senha != ConfirmarSenha)
                {
                    await Shell.Current.DisplayAlert(
                        "Atenção",
                        "A senha e a confirmação são diferentes.",
                        "OK");

                    return;
                }
            }

            IsBusy = true;

            if (IsEditMode)
            {
                await _colaboradorService.AtualizarAsync(Colaborador);
            }
            else
            {
                await _colaboradorService.AdicionarAsync(Colaborador);
            }

            await Shell.Current.DisplayAlert(
                "Sucesso",
                IsEditMode
                    ? "Colaborador atualizado com sucesso!"
                    : "Colaborador cadastrado com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível salvar o colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ExcluirAsync()
    {
        if (!IsEditMode || Colaborador.Id <= 0)
        {
            await Shell.Current.DisplayAlert(
                "Atenção",
                "Abra um colaborador cadastrado para excluí-lo.",
                "OK");

            return;
        }

        var confirmar = await Shell.Current.DisplayAlert(
            "Confirmar exclusão",
            $"Deseja realmente excluir o colaborador {Colaborador.Nome}?",
            "Excluir",
            "Cancelar");

        if (!confirmar)
            return;

        try
        {
            IsBusy = true;

            var removido =
                await _colaboradorService.RemoverAsync(Colaborador.Id);

            if (!removido)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "O colaborador não foi encontrado ou já foi removido.",
                    "OK");

                return;
            }

            await Shell.Current.DisplayAlert(
                "Sucesso",
                "Colaborador excluído com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível excluir o colaborador: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static ColaboradorDto CriarColaboradorPadrao()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        return new ColaboradorDto
        {
            Nome = string.Empty,
            Cpf = string.Empty,
            DataNascimento = hoje,
            Telefone = string.Empty,
            Numero = string.Empty,
            DataAdmissao = hoje,
            Tipo = AppColaboradorTipo.Atendente,
            Vinculo = AppColaboradorVinculo.CLT
        };
    }
}
