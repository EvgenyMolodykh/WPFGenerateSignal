using System;
using System.Collections.Generic;
using WPFGenerateSignal.Enums;
using WPFGenerateSignal.Interfaces;
using WPFGenerateSignal.Models;

namespace WPFGenerateSignal.Services
{
    public class SignalService : ISignalService
    {
        public IEnumerable<DataPoint> GenerateSignal(SignalType signalType, SignalParameters parameters, double time, int bufferSize)
        {
            var points = new List<DataPoint>();
            double viewRange = 4 * Math.PI;

            for (int i = 0; i < bufferSize; i++)
            {
                double x = time + i * viewRange / bufferSize;
                double y = CalculateSignalValue(signalType, x, time, parameters);
                points.Add(new DataPoint(x, y));
            }

            return points;
        }

        public double CalculateSignalValue(SignalType signalType, double x, double time, SignalParameters parameters)
        {
            double transformedX = parameters.Frequency * x + parameters.Phase;

            return signalType switch
            {
                SignalType.Sinusoid => parameters.Amplitude * Math.Sin(transformedX),
                SignalType.Meandr => parameters.Amplitude * (Math.Sin(transformedX) >= 0 ? 1.0 : -1.0),
                SignalType.Triangle => parameters.Amplitude * CalculateTriangleValue(transformedX),
                SignalType.Sawtooth => parameters.Amplitude * CalculateSawtoothValue(transformedX),
                _ => 0
            };
        }

        private double CalculateTriangleValue(double x)
        {
            double phase = (x % (2 * Math.PI)) / (2 * Math.PI);

            if (phase < 0.25) return 4 * phase;
            else if (phase < 0.75) return 2 - 4 * phase;
            else return 4 * phase - 4;
        }

        private double CalculateSawtoothValue(double x)
        {
            double phase = (x % (2 * Math.PI)) / (2 * Math.PI);
            return 2 * phase - 1;
        }
    }
}