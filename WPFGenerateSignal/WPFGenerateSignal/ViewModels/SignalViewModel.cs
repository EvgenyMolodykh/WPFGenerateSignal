using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFGenerateSignal.Services;
using WPFGenerateSignal.Models;
using System.Collections.Generic;
using System;
using System.Windows;

namespace WPFGenerateSignal.ViewModels
{
    public class SignalViewModel : INotifyPropertyChanged
    {
        private readonly ISignalStorageService _storageService;
        private string _selectedSignalType = "Синусоида";
        private double _amplitude = 1.0;
        private double _frequency = 1.0;
        private double _phase = 0.0;
        private int _pointsCount = 1000;
        private bool _isGenerating = false;
        private double _generationProgress = 0;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _parametersChanged = false;
        private SignalMetadata _currentSignal;
        private string _statusMessage = "Готов";
        private List<SignalMetadata> _loadedSignals = new List<SignalMetadata>();

        public SignalViewModel(ISignalStorageService storageService)
        {
            _storageService = storageService;

            StartGenerationCommand = new RelayCommand(async () => await StartSignalGenerationAsync(), () => CanGenerate);
            CancelGenerationCommand = new RelayCommand(() => CancelSignalGeneration(), () => IsGenerating);
            SaveSignalToDatabaseCommand = new RelayCommand(async () => await SaveSignalToDatabaseAsync(), () => CanSaveSignal);
            LoadSignalsFromDatabaseCommand = new RelayCommand(async () => await LoadSignalsFromDatabaseAsync());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand StartGenerationCommand { get; }
        public ICommand CancelGenerationCommand { get; }
        public ICommand SaveSignalToDatabaseCommand { get; }
        public ICommand LoadSignalsFromDatabaseCommand { get; }

        public List<SignalMetadata> LoadedSignals
        {
            get => _loadedSignals;
            set
            {
                _loadedSignals = value;
                OnPropertyChanged();
            }
        }

        public SignalMetadata CurrentSignal
        {
            get => _currentSignal;
            set
            {
                _currentSignal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSaveSignal));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool CanSaveSignal => CurrentSignal != null && !IsGenerating;

        public string SelectedSignalType
        {
            get => _selectedSignalType;
            set
            {
                if (_selectedSignalType != value)
                {
                    _selectedSignalType = value;
                    _parametersChanged = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ParametersChanged));
                    OnPropertyChanged(nameof(CanGenerate));

                 
                    UpdateSignalVisibility();
                }
            }
        }

        public double Amplitude
        {
            get => _amplitude;
            set
            {
                if (_amplitude != value && value >= 0.1 && value <= 5.0)
                {
                    _amplitude = value;
                    _parametersChanged = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ParametersChanged));
                    OnPropertyChanged(nameof(CanGenerate));
                }
            }
        }

        public double Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency != value && value >= 0.1 && value <= 10.0)
                {
                    _frequency = value;
                    _parametersChanged = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ParametersChanged));
                    OnPropertyChanged(nameof(CanGenerate));
                }
            }
        }

        public double Phase
        {
            get => _phase;
            set
            {
                if (_phase != value && value >= 0 && value <= 6.28)
                {
                    _phase = value;
                    _parametersChanged = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ParametersChanged));
                    OnPropertyChanged(nameof(CanGenerate));
                }
            }
        }

        public int PointsCount
        {
            get => _pointsCount;
            set
            {
                if (_pointsCount != value && value >= 100 && value <= 10000) 
                {
                    _pointsCount = value;
                    _parametersChanged = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ParametersChanged));
                    OnPropertyChanged(nameof(CanGenerate));
                }
            }
        }

        public bool IsGenerating
        {
            get => _isGenerating;
            set
            {
                _isGenerating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ParametersChanged));
                OnPropertyChanged(nameof(CanGenerate));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public double GenerationProgress
        {
            get => _generationProgress;
            set
            {
                _generationProgress = value;
                OnPropertyChanged();
            }
        }

        public bool ParametersChanged => _parametersChanged && !IsGenerating;

        public bool CanGenerate => _parametersChanged && !IsGenerating;

     
        public bool IsSinusoidSelected => SelectedSignalType == "Синусоида";
        public bool IsMeandrSelected => SelectedSignalType == "Меандр";
        public bool IsTriangleSelected => SelectedSignalType == "Треугольный";
        public bool IsSawtoothSelected => SelectedSignalType == "Пилообразный";

        public bool IsSinusoidVisible => IsSinusoidSelected;
        public bool IsMeandrVisible => IsMeandrSelected;
        public bool IsTriangleVisible => IsTriangleSelected;
        public bool IsSawtoothVisible => IsSawtoothSelected;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSignalVisibility()
        {
           
            OnPropertyChanged(nameof(IsSinusoidSelected));
            OnPropertyChanged(nameof(IsMeandrSelected));
            OnPropertyChanged(nameof(IsTriangleSelected));
            OnPropertyChanged(nameof(IsSawtoothSelected));
            OnPropertyChanged(nameof(IsSinusoidVisible));
            OnPropertyChanged(nameof(IsMeandrVisible));
            OnPropertyChanged(nameof(IsTriangleVisible));
            OnPropertyChanged(nameof(IsSawtoothVisible));
        }

        public async Task StartSignalGenerationAsync()
        {
            if (IsGenerating || !_parametersChanged) return;

            _cancellationTokenSource = new CancellationTokenSource();
            IsGenerating = true;
            GenerationProgress = 0;

            try
            {
                await Task.Run(async () =>
                {
                    int totalSteps = 20;
                    for (int i = 0; i < totalSteps; i++)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            break;

                        await Task.Delay(100, _cancellationTokenSource.Token);
                        GenerationProgress = (i + 1) * 100.0 / totalSteps;
                    }
                }, _cancellationTokenSource.Token);

                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _parametersChanged = false;
                    OnPropertyChanged(nameof(ParametersChanged));
                    OnPropertyChanged(nameof(CanGenerate));

                    CurrentSignal = new SignalMetadata
                    {
                        Name = $"{SelectedSignalType} сигнал",
                        SignalType = SelectedSignalType,
                        Amplitude = Amplitude,
                        Frequency = Frequency,
                        Phase = Phase,
                        Points = GenerateSignalPoints(),
                        CreatedAt = DateTime.Now
                    };

                    StatusMessage = "Сигнал сгенерирован";
                }
            }
            catch (TaskCanceledException)
            {
                GenerationProgress = 0;
                StatusMessage = "Генерация отменена";
            }
            finally
            {
                IsGenerating = false;
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    GenerationProgress = 100;
                }
            }
        }

        private List<DataPoint> GenerateSignalPoints()
        {
            var points = new List<DataPoint>();
            double duration = 1.0 / Frequency * 2; 
            double timeStep = duration / PointsCount;

            for (int i = 0; i < PointsCount; i++)
            {
                double time = i * timeStep;
                double value = CalculateSignalValue(time);
                points.Add(new DataPoint(time, value));
            }
            return points;
        }

        private double CalculateSignalValue(double time)
        {
            double angularFrequency = 2 * Math.PI * Frequency;
            double angle = angularFrequency * time + Phase;

            return SelectedSignalType switch
            {
                "Синусоида" => Amplitude * Math.Sin(angle),
                "Меандр" => Amplitude * Math.Sign(Math.Sin(angle)),
                "Треугольный" => Amplitude * (2 * Math.Abs(2 * (time * Frequency - Math.Floor(time * Frequency + 0.5))) - 1),
                "Пилообразный" => Amplitude * (2 * (time * Frequency - Math.Floor(time * Frequency + 0.5))),
                _ => 0
            };
        }

        public void CancelSignalGeneration()
        {
            _cancellationTokenSource?.Cancel();
            GenerationProgress = 0;
            StatusMessage = "Генерация отменена";
        }

        private async Task SaveSignalToDatabaseAsync()
        {
            if (CurrentSignal == null) return;

            try
            {
                IsGenerating = true;
                StatusMessage = "Сохранение в БД...";

                var progress = new Progress<double>(p => GenerationProgress = p);
                await _storageService.SaveSignalAsync(CurrentSignal, progress);

                StatusMessage = "Сохранено в БД";

          
                await LoadSignalsFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private async Task LoadSignalsFromDatabaseAsync()
        {
            try
            {
                IsGenerating = true;
                StatusMessage = "Загрузка из БД...";

                var progress = new Progress<double>(p => GenerationProgress = p);
                var signals = await _storageService.LoadAllSignalsAsync(progress);

                LoadedSignals = signals;
                StatusMessage = $"Загружено {signals.Count} сигналов";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsGenerating = false;
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();
    }
}