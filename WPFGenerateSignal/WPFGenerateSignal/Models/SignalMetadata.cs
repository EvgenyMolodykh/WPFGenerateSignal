using System;
using System.Collections.Generic;

namespace WPFGenerateSignal.Models
{
    public class SignalMetadata
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SignalType { get; set; }
        public double Amplitude { get; set; }
        public double Frequency { get; set; }
        public double Phase { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DataPoint> Points { get; set; } = new List<DataPoint>();
    }
}