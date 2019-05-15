namespace TbiDesktop.Models
{
    public class Result
    {
        public Value headLost { get; set; } = new Value();
        public Value previousHeadLost { get; set; } = new Value();
        public Value savingPotentialMin { get; set; } = new Value();
        public Value savingPotentialMax { get; set; } = new Value();
        public string advise { get; set; }
        public double[] co2 { get; set; } = new[] { 0d, 0d, 0d, 0d };
        public double annual_saving_from { get; set; }
        public double annual_saving_to { get; set; }
    }

    public class Value
    {
        public double power { get; set; }
        public double money { get; set; }
    }
}