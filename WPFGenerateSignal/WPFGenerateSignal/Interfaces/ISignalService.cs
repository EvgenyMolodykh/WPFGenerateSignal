using System.Collections.Generic;
using WPFGenerateSignal.Enums;
using WPFGenerateSignal.Models;

namespace WPFGenerateSignal.Interfaces
{
    public interface ISignalService
    {
        IEnumerable<DataPoint> GenerateSignal(SignalType signalType, SignalParameters parameters, double time, int bufferSize);
        double CalculateSignalValue(SignalType signalType, double x, double time, SignalParameters parameters);
    }
}