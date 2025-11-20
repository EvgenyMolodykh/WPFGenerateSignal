using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFGenerateSignal.Models
{
    public class SignalParameters : INotifyPropertyChanged
    {
        private double _amplitude = 1.0;
        private double _frequency = 1.0;
        private double _phase = 0.0;
        private int _pointsCount = 1000;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Amplitude
        {
            get => _amplitude;
            set
            {
                if (_amplitude != value)
                {
                    _amplitude = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Phase
        {
            get => _phase;
            set
            {
                if (_phase != value)
                {
                    _phase = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PointsCount
        {
            get => _pointsCount;
            set
            {
                if (_pointsCount != value)
                {
                    _pointsCount = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}