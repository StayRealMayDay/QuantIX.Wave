using System;

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
            Slope = (StopPointPrice - StartPointprice) / (StopPoint - StartPoint);
            Length = Math.Sqrt(Math.Pow(StopPointPrice - StartPointprice, 2) + Math.Pow(StopPoint - StartPoint, 2));
        }



        public int Tendency => StartPointprice > StopPointPrice ? -1 : 1;

        public double Length { get; set; }

        public double Slope { get; set; }

        public int StartPoint { get; }

        public int StopPoint { get; }

        public double StartPointprice { get; }

        public double StopPointPrice { get; }
    }
}