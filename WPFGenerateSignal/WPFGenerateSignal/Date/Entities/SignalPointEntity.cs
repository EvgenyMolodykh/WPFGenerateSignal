using System.ComponentModel.DataAnnotations;

namespace WPFGenerateSignal.Date.Entities
{
    public class SignalPointEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SignalId { get; set; }
        public double Time { get; set; }
        public double Value { get; set; }
        public virtual SignalEntity Signal { get; set; }
    }
}