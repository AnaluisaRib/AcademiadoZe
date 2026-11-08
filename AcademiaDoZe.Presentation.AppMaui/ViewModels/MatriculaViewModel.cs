
// Ana Luisa Ribeiro de Araujo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(MatriculaId), "Id")]
public partial class MatriculaViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;
    private readonly IAlunoService _alunoService;

    private int _matriculaId;

    public int MatriculaId
    {
        get => _matriculaId;
        set => SetProperty(ref _matriculaId, value);
    }

    public ObservableCollection<AlunoDto> Alunos { get; } = [];

    public ObservableCollection<AppMatriculaPlano> Planos { get; } =
    [
        AppMatriculaPlano.Mensal,
        AppMatriculaPlano.Trimestral,
        AppMatriculaPlano.Semestral,
        AppMatriculaPlano.Anual
    ];

    private AlunoDto? _alunoSelecionado;

    public AlunoDto? AlunoSelecionado
    {
        get => _alunoSelecionado;
        set => SetProperty(ref _alunoSelecionado, value);
    }

    private AppMatriculaPlano _planoSelecionado =
        AppMatriculaPlano.Mensal;

    public AppMatriculaPlano PlanoSelecionado
    {
        get => _planoSelecionado;
        set
        {
            if (SetProperty(ref _planoSelecionado, value))
                OnPropertyChanged(nameof(DataFimCalculada));
        }
    }

    private DateTime _dataInicioSelecionada = DateTime.Today;

    public DateTime DataInicioSelecionada
    {
        get => _dataInicioSelecionada;
        set
        {
            if (SetProperty(ref _dataInicioSelecionada, value))
                OnPropertyChanged(nameof(DataFimCalculada));
        }
    }

    public DateTime DataFimCalculada
    {
        get
        {
            return PlanoSelecionado switch
            {
                AppMatriculaPlano.Mensal =>
                    DataInicioSelecionada.AddMonths(1),

                AppMatriculaPlano.Trimestral =>
                    DataInicioSelecionada.AddMonths(3),

                AppMatriculaPlano.Semestral =>
                    DataInicioSelecionada.AddMonths(6),

                AppMatriculaPlano.Anual =>
                    DataInicioSelecionada.AddMonths(12),

                _ => DataInicioSelecionada
            };
        }
    }

    private string _objetivo = string.Empty;

    public string Objetivo
    {
        get => _objetivo;
        set => SetProperty(ref _objetivo, value);
    }

    private string _observacoesRestricoes = string.Empty;

    public string ObservacoesRestricoes
    {
        get => _observacoesRestricoes;
        set => SetProperty(ref _observacoesRestricoes, value);
    }

    private byte[]? _laudoMedicoConteudo;

    public string NomeLaudoMedico { get; set; } =
        "Nenhum arquivo selecionado";

    private bool _diabetes;

    public bool Diabetes
    {
        get => _diabetes;
        set => SetProperty(ref _diabetes, value);
    }

    private bool _pressaoAlta;

    public bool PressaoAlta
    {
        get => _pressaoAlta;
        set => SetProperty(ref _pressaoAlta, value);
    }

    private bool _labirintite;

    public bool Labirintite
    {
        get => _labirintite;
        set => SetProperty(ref _labirintite, value);
    }

    private bool _alergias;

    public bool Alergias
    {
        get => _alergias;
        set => SetProperty(ref _alergias, value);
    }

    private bool _problemasRespiratorios;

    public bool ProblemasRespiratorios
    {
        get => _problemasRespiratorios;
        set => SetProperty(ref _problemasRespiratorios, value);
    }

    private bool _remedioContinuo;

    public bool RemedioContinuo
    {
        get => _remedioContinuo;
        set => SetProperty(ref _remedioContinuo, value);
    }

    private bool _isEditMode;

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public MatriculaViewModel(
        IMatriculaService matriculaService,
        IAlunoService alunoService)
    {
        _matriculaService = matriculaService;
        _alunoService = alunoService;

        Title = "Cadastro de Matrícula";
    }

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;

            var alunos = await _alunoService.ObterTodosAsync();

            Alunos.Clear();

            foreach (var aluno in alunos)
                Alunos.Add(aluno);

            if (MatriculaId > 0)
            {
                IsEditMode = true;
                await CarregarMatriculaAsync(MatriculaId);
            }
            else
            {
                IsEditMode = false;
                LimparFormulario();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível inicializar a matrícula: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CarregarMatriculaAsync(int id)
    {
        var matricula =
            await _matriculaService.ObterPorIdAsync(id);

        if (matricula is null)
        {
            await Shell.Current.DisplayAlert(
                "Aviso",
                "Matrícula não encontrada.",
                "OK");

            return;
        }

        AlunoSelecionado = Alunos.FirstOrDefault(
            a => a.Id == matricula.AlunoMatricula.Id);

        PlanoSelecionado = matricula.Plano;

        DataInicioSelecionada =
            matricula.DataInicio.ToDateTime(TimeOnly.MinValue);

        Objetivo = matricula.Objetivo;

        ObservacoesRestricoes =
            matricula.ObservacoesRestricoes ?? string.Empty;

        Diabetes = matricula.RestricoesMedicas.HasFlag(
            AppMatriculaRestricoes.Diabetes);

        PressaoAlta = matricula.RestricoesMedicas.HasFlag(
            AppMatriculaRestricoes.PressaoAlta);

        Labirintite = matricula.RestricoesMedicas.HasFlag(
            AppMatriculaRestricoes.Labirintite);

        Alergias = matricula.RestricoesMedicas.HasFlag(
            AppMatriculaRestricoes.Alergias);

        ProblemasRespiratorios =
            matricula.RestricoesMedicas.HasFlag(
                AppMatriculaRestricoes.ProblemasRespiratorios);

        RemedioContinuo = matricula.RestricoesMedicas.HasFlag(
            AppMatriculaRestricoes.RemedioContinuo);

        _laudoMedicoConteudo =
            matricula.LaudoMedico?.Conteudo;

        NomeLaudoMedico = _laudoMedicoConteudo is { Length: > 0 }
            ? "Laudo médico já cadastrado"
            : "Nenhum arquivo selecionado";

        OnPropertyChanged(nameof(NomeLaudoMedico));
    }

    private void LimparFormulario()
    {
        AlunoSelecionado = null;
        PlanoSelecionado = AppMatriculaPlano.Mensal;
        DataInicioSelecionada = DateTime.Today;
        Objetivo = string.Empty;
        ObservacoesRestricoes = string.Empty;

        Diabetes = false;
        PressaoAlta = false;
        Labirintite = false;
        Alergias = false;
        ProblemasRespiratorios = false;
        RemedioContinuo = false;

        _laudoMedicoConteudo = null;
        NomeLaudoMedico = "Nenhum arquivo selecionado";

        OnPropertyChanged(nameof(NomeLaudoMedico));
    }

    [RelayCommand]
    private async Task SelecionarLaudoAsync()
    {
        try
        {
            var arquivo = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Selecione o laudo médico"
                });

            if (arquivo is null)
                return;

            await using var stream = await arquivo.OpenReadAsync();

            using var memoria = new MemoryStream();

            await stream.CopyToAsync(memoria);

            _laudoMedicoConteudo = memoria.ToArray();

            if (_laudoMedicoConteudo.Length == 0)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "O arquivo selecionado está vazio.",
                    "OK");

                _laudoMedicoConteudo = null;
                return;
            }

            NomeLaudoMedico = arquivo.FileName;

            OnPropertyChanged(nameof(NomeLaudoMedico));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível selecionar o laudo: {ex.Message}",
                "OK");
        }
    }

    private AppMatriculaRestricoes ObterRestricoesSelecionadas()
    {
        var restricoes = AppMatriculaRestricoes.None;

        if (Diabetes)
            restricoes |= AppMatriculaRestricoes.Diabetes;

        if (PressaoAlta)
            restricoes |= AppMatriculaRestricoes.PressaoAlta;

        if (Labirintite)
            restricoes |= AppMatriculaRestricoes.Labirintite;

        if (Alergias)
            restricoes |= AppMatriculaRestricoes.Alergias;

        if (ProblemasRespiratorios)
            restricoes |=
                AppMatriculaRestricoes.ProblemasRespiratorios;

        if (RemedioContinuo)
            restricoes |=
                AppMatriculaRestricoes.RemedioContinuo;

        return restricoes;
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (AlunoSelecionado is null)
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Selecione um aluno para a matrícula.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(Objetivo))
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Informe o objetivo da matrícula.",
                    "OK");

                return;
            }

            var restricoes = ObterRestricoesSelecionadas();

            if (restricoes != AppMatriculaRestricoes.None &&
                _laudoMedicoConteudo is not { Length: > 0 })
            {
                await Shell.Current.DisplayAlert(
                    "Atenção",
                    "Alunos com restrições médicas devem apresentar um laudo médico.",
                    "OK");

                return;
            }

            var matricula = new MatriculaDto
            {
                Id = MatriculaId,
                AlunoMatricula = AlunoSelecionado,
                Plano = PlanoSelecionado,
                DataInicio = DateOnly.FromDateTime(
                    DataInicioSelecionada),
                DataFim = DateOnly.FromDateTime(DataFimCalculada),
                Objetivo = Objetivo.Trim(),
                RestricoesMedicas = restricoes,
                ObservacoesRestricoes = ObservacoesRestricoes,
                LaudoMedico = _laudoMedicoConteudo is { Length: > 0 }
                    ? new ArquivoDto
                    {
                        Conteudo = _laudoMedicoConteudo
                    }
                    : null
            };

            IsBusy = true;

            if (IsEditMode)
            {
                await _matriculaService.AtualizarAsync(matricula);
            }
            else
            {
                await _matriculaService.AdicionarAsync(matricula);
            }

            await Shell.Current.DisplayAlert(
                "Sucesso",
                IsEditMode
                    ? "Matrícula atualizada com sucesso!"
                    : "Matrícula cadastrada com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Erro",
                $"Não foi possível salvar a matrícula: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VoltarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
