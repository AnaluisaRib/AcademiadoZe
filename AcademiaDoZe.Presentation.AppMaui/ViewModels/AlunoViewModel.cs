
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(AlunoId), "Id")]
public partial class AlunoViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private readonly ILogradouroService _logradouroService;

    private int _alunoId;

    public int AlunoId
    {
        get => _alunoId;
        set => SetProperty(ref _alunoId, value);
    }

    private AlunoDto _aluno = CriarAlunoPadrao();

    public AlunoDto Aluno
    {
        get => _aluno;
        set
        {
            if (SetProperty(ref _aluno, value))
            {
                OnPropertyChanged(nameof(DataNascimentoSelecionada));
                OnPropertyChanged(nameof(HasEnderecoVinculado));
                AtualizarFotoExibida();
            }
        }
    }

    private string _cep = string.Empty;

    public string Cep
    {
        get => _cep;
        set => SetProperty(ref _cep, value);
    }

    private string _confirmarSenha = string.Empty;

    public string ConfirmarSenha
    {
        get => _confirmarSenha;
        set => SetProperty(ref _confirmarSenha, value);
    }

    private ImageSource? _fotoSelecionada;

    public ImageSource? FotoSelecionada
    {
        get => _fotoSelecionada;
        set => SetProperty(ref _fotoSelecionada, value);
    }

    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public DateTime DataNascimentoSelecionada
    {
        get => Aluno.DataNascimento.ToDateTime(TimeOnly.MinValue);
        set
        {
            var data = DateOnly.FromDateTime(value);

            if (Aluno.DataNascimento != data)
            {
                Aluno.DataNascimento = data;
                OnPropertyChanged();
            }
        }
    }

    public bool HasEnderecoVinculado =>
        Aluno.Endereco is not null && Aluno.Endereco.Id > 0;

    public AlunoViewModel(
        IAlunoService alunoService,
        ILogradouroService logradouroService)
    {
        _alunoService = alunoService;
        _logradouroService = logradouroService;

        Title = "Cadastro de aluno";
    }

    public async Task InitializeAsync()
    {
        if (AlunoId > 0)
        {
            IsEditMode = true;
            await CarregarAlunoAsync(AlunoId);
        }
        else
        {
            IsEditMode = false;
            Aluno = CriarAlunoPadrao();
            Cep = string.Empty;
            ConfirmarSenha = string.Empty;
            FotoSelecionada = null;
        }
    }

    private async Task CarregarAlunoAsync(int id)
    {
        try
        {
            IsBusy = true;

            var resultado = await _alunoService.ObterPorIdAsync(id);

            if (resultado is null)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "Aluno não encontrado.",
                    "OK");

                return;
            }

            Aluno = resultado;
            Cep = resultado.Endereco?.Cep ?? string.Empty;
            ConfirmarSenha = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível carregar o aluno: {ex.Message}",
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
            var foto = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Selecione a foto do aluno",
                    FileTypes = FilePickerFileType.Images
                });

            if (foto is null)
                return;

            await using var stream = await foto.OpenReadAsync();
            using var memoria = new MemoryStream();

            await stream.CopyToAsync(memoria);

            var conteudo = memoria.ToArray();

            Aluno.Foto = new ArquivoDto
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
        if (Aluno.Foto?.Conteudo is { Length: > 0 } conteudo)
        {
            FotoSelecionada = ImageSource.FromStream(
                () => new MemoryStream(conteudo));
        }
        else
        {
            FotoSelecionada = null;
        }
    }

    [RelayCommand]
    private async Task PesquisarEnderecoPorCepAsync()
    {
        try
        {
            var cep = new string(
                Cep.Where(char.IsDigit).ToArray());

            if (cep.Length != 8)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Digite um CEP com 8 números.",
                    "OK");

                return;
            }

            IsBusy = true;

            var endereco = await _logradouroService.ObterPorCepAsync(cep);

            if (endereco is null)
            {
                await Shell.Current.DisplayAlert(
                    "Aviso",
                    "CEP não encontrado. Cadastre o logradouro primeiro.",
                    "OK");

                return;
            }

            Aluno.Endereco = endereco;
            OnPropertyChanged(nameof(Aluno));
            OnPropertyChanged(nameof(HasEnderecoVinculado));

            await Shell.Current.DisplayAlert(
                "Sucesso",
                "Endereço localizado.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível consultar o CEP: {ex.Message}",
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
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(Aluno.Nome) ||
                string.IsNullOrWhiteSpace(Aluno.Cpf) ||
                string.IsNullOrWhiteSpace(Aluno.Telefone))
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Preencha nome, CPF e telefone.",
                    "OK");

                return;
            }

            if (!HasEnderecoVinculado)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Informe um CEP cadastrado antes de salvar.",
                    "OK");

                return;
            }

            if (Aluno.Foto?.Conteudo is not { Length: > 0 })
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Selecione uma foto para o aluno.",
                    "OK");

                return;
            }

            if (!IsEditMode)
            {
                if (string.IsNullOrWhiteSpace(Aluno.Senha) ||
                    Aluno.Senha.Length < 6)
                {
                    await Shell.Current.DisplayAlert(
                        "Atenção",
                        "A senha deve ter pelo menos 6 caracteres.",
                        "OK");

                    return;
                }

                if (Aluno.Senha != ConfirmarSenha)
                {
                    await Shell.Current.DisplayAlert(
                        "Atenção",
                        "As senhas não coincidem.",
                        "OK");

                    return;
                }

                await _alunoService.AdicionarAsync(Aluno);
            }
            else
            {
                await _alunoService.AtualizarAsync(Aluno);
            }

            await Shell.Current.DisplayAlert(
                "Sucesso",
                IsEditMode
                    ? "Aluno atualizado com sucesso!"
                    : "Aluno cadastrado com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível salvar o aluno: {ex.Message}",
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
        if (!IsEditMode || Aluno.Id <= 0)
            return;

        try
        {
            var confirmar = await Shell.Current.DisplayAlert(
                "Confirmar exclusão",
                $"Deseja realmente excluir o aluno {Aluno.Nome}?",
                "Excluir",
                "Cancelar");

            if (!confirmar)
                return;

            IsBusy = true;

            await _alunoService.RemoverAsync(Aluno.Id);

            await Shell.Current.DisplayAlert(
                "Sucesso",
                "Aluno excluído com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível excluir o aluno: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static AlunoDto CriarAlunoPadrao()
    {
        return new AlunoDto
        {
            Nome = string.Empty,
            Cpf = string.Empty,
            Telefone = string.Empty,
            Email = string.Empty,
            Numero = string.Empty,
            Complemento = string.Empty,
            Senha = string.Empty,
            DataNascimento = DateOnly.FromDateTime(DateTime.Today)
        };
    }
}
