namespace QuantIX.Wave
{
    public class Wave
    {
        public Wave(int startPoint, int stopPoint, double startPointprice, double stopPointPrice)
        {
            StartPoint = startPoint;
            StopPoint = stopPoint;
            StartPointprice = startPointprice;
            StopPointPrice = stopPointPrice;
        }

        public int StartPoint { get; }

        public int StopPoint { get; }

        public double StartPointprice { get; }

        public double StopPointPrice { get; }
    }
}