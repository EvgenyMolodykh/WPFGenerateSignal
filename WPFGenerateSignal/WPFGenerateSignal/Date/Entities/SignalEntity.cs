using System.ComponentModel.DataAnnotations;

namespace WPFGenerateSignal.Date.Entities
{
    public class SignalEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SignalType { get; set; }
        public double Amplitude { get; set; }
        public double Frequency { get; set; }
        public double Phase { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<SignalPointEntity> Points { get; set; } = new List<SignalPointEntity>();
    }
}