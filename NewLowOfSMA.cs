using System.Linq;
using QuantTC.Data;
using QuantTC.Indicators;
using QuantTC.Indicators.Generic;
using static QuantTC.X;
namespace QuantIX.Wave
{
    public class NewLowOfSMA : Indicator<int>
    {
        public NewLowOfSMA(IIndicator<IPrice> source, int cycle)
        {
            Source = source;
            Cycle = cycle;
            Close = Source.LiveSelect((arg, i) => arg.Close);
            TenSMA = Close.SMA(10);
            SixtySMA = Close.SMA(60);
            Source.Update += SixtySmaOnUpdate;
        }

        private void SixtySmaOnUpdate()
        {
            RangeL(Count, Close.Count).ForEach(i => Data.Add(FindNewLow(i)));
            FollowUp();
        }

        private int FindNewLow(int i)
        {
            var min = RangeR(0, i + 1).Take(Cycle).Select(ii => SixtySMA[ii]).Min();
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return RangeR(0, i + 1)
                .FirstOrDefault(ii => SixtySMA[ii] == min); //&& SixtySMA[ii] - TenSMA[ii] >= Edge);
        }


        private SMA TenSMA { get; }

        public SMA SixtySMA { get; set; }

        public LiveSelector<IPrice, double> Close { get; set; }

        public IIndicator<IPrice> Source { get; }

        private int Cycle { get; }
    }
}