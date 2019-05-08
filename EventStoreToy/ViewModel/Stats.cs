namespace EventStoreToy.ViewModel
{
    public class Stats
    {
        public Stats()
        {
        }

        public Stats(int sumValue, double averageValue, double standardDeviationValue)
        {
            Sum = sumValue;
            Average = averageValue;
            StandardDeviation = standardDeviationValue;
        }

        public int Sum { get; }
        public double Average { get; }
        public double StandardDeviation { get; }
    }
}