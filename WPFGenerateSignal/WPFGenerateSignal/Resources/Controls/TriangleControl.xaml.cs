using OxyPlot;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFGenerateSignal.ViewModels;

namespace WPFGenerateSignal.Resources.Controls
{
    public partial class TriangleControl : UserControl
    {
        private PlotModel _plotModel;
        private LineSeries _dataSeries;
        private SignalViewModel _viewModel;
        private DispatcherTimer _animationTimer;
        private CancellationTokenSource _generationTokenSource;
        private double _currentTime = 0;
        private const double AnimationSpeed = 0.05;
        private const double TwoPI = 2 * Math.PI;

        private double _currentAmplitude = 1.0;
        private double _currentFrequency = 1.0;
        private double _currentPhase = 0.0;
        private int _currentPointsCount = 1000;

        public TriangleControl()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;

            InitializeAnimationTimer();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializePlot();
            StartAnimation();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            StopAnimation();
            CancelGeneration();
        }

        private void InitializeAnimationTimer()
        {
            _animationTimer = new DispatcherTimer();
            _animationTimer.Interval = TimeSpan.FromMilliseconds(33); // ~30 FPS
            _animationTimer.Tick += OnAnimationTick;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SignalViewModel oldViewModel)
            {
                oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            if (e.NewValue is SignalViewModel newViewModel)
            {
                _viewModel = newViewModel;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;

                _currentAmplitude = _viewModel.Amplitude;
                _currentFrequency = _viewModel.Frequency;
                _currentPhase = _viewModel.Phase;
                _currentPointsCount = _viewModel.PointsCount;

                UpdateSignalAsync().ConfigureAwait(false);
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            if (e.PropertyName == nameof(SignalViewModel.Amplitude) ||
                e.PropertyName == nameof(SignalViewModel.Frequency) ||
                e.PropertyName == nameof(SignalViewModel.Phase) ||
                e.PropertyName == nameof(SignalViewModel.PointsCount))
            {
                System.Diagnostics.Debug.WriteLine($"Параметр {e.PropertyName} изменен, но график треугольного сигнала не перестроен");
            }

            if (e.PropertyName == nameof(SignalViewModel.GenerationProgress) &&
                _viewModel.GenerationProgress == 100 &&
                !_viewModel.ParametersChanged)
            {
                ApplyNewParameters();
                UpdateSignalAsync().ConfigureAwait(false);
            }
        }

        private void ApplyNewParameters()
        {
            _currentAmplitude = _viewModel.Amplitude;
            _currentFrequency = _viewModel.Frequency;
            _currentPhase = _viewModel.Phase;
            _currentPointsCount = _viewModel.PointsCount;
        }

        private void InitializePlot()
        {
            _plotModel = new PlotModel { Title = "Треугольный сигнал" };
            _dataSeries = new LineSeries
            {
                StrokeThickness = 1.5,
                Color = OxyColors.Green,
                LineStyle = LineStyle.Solid
            };
            _plotModel.Series.Add(_dataSeries);

            _plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "Время",
                Minimum = 0,
                Maximum = 4 * Math.PI
            });
            _plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Амплитуда",
                Minimum = -6,
                Maximum = 6
            });

            PlotViewControl.Model = _plotModel;
        }

        private void OnAnimationTick(object sender, EventArgs e)
        {
            _currentTime += AnimationSpeed;

            if (_currentTime > 4 * Math.PI)
            {
                _currentTime = 0;
            }

            UpdateSignalAsync().ConfigureAwait(false);
        }

        private async Task UpdateSignalAsync()
        {
            if (_dataSeries == null || _viewModel == null) return;

            CancelGeneration();
            _generationTokenSource = new CancellationTokenSource();

            try
            {
                await GenerateSignalDataAsync(_generationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
               
            }
        }

        private async Task GenerateSignalDataAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                try
                {
                    double amplitude = _currentAmplitude;
                    double frequency = _currentFrequency;
                    double phase = _currentPhase;
                    int pointsCount = Math.Min(_currentPointsCount, 2000);
                    double viewRange = 4 * Math.PI;

                    var points = new DataPoint[pointsCount];

                    for (int i = 0; i < pointsCount; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        double x = i * viewRange / pointsCount;
                        double animatedX = x + _currentTime;

                        double value = GenerateTriangleWave(amplitude, frequency, phase, animatedX);
                        points[i] = new DataPoint(x, value);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _dataSeries.Points.Clear();
                            foreach (var point in points)
                            {
                                _dataSeries.Points.Add(point);
                            }
                            _plotModel?.InvalidatePlot(true);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка генерации треугольного сигнала: {ex.Message}");
                    });
                }
            }, cancellationToken);
        }

        private double GenerateTriangleWave(double amplitude, double frequency, double phase, double x)
        {
            double transformedX = frequency * x + phase;
            double triangleValue = 2 * Math.Abs(2 * (transformedX / (2 * Math.PI) - Math.Floor(transformedX / (2 * Math.PI) + 0.5))) - 1;
            return amplitude * triangleValue;
        }

        private void StartAnimation()
        {
            _animationTimer?.Start();
        }

        private void StopAnimation()
        {
            _animationTimer?.Stop();
        }

        private void CancelGeneration()
        {
            _generationTokenSource?.Cancel();
            _generationTokenSource?.Dispose();
            _generationTokenSource = null;
        }
    }
}