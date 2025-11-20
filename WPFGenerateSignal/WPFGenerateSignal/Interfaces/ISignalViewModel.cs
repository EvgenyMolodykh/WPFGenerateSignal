using System.ComponentModel;
using WPFGenerateSignal.Enums;
using WPFGenerateSignal.Models;

namespace WPFGenerateSignal.Interfaces
{
    public interface ISignalViewModel : INotifyPropertyChanged
    {
        SignalType SelectedSignalType { get; set; }
        double Amplitude { get; set; }
        double Frequency { get; set; }
        double Phase { get; set; }
        int PointsCount { get; set; }

        SignalParameters Parameters { get; }
        bool IsSinusoidVisible { get; }
        bool IsMeandrVisible { get; }
        bool IsTriangleVisible { get; }
        bool IsSawtoothVisible { get; }

        event System.Action SignalParametersChanged;
    }
}