using System.Linq;
using QuantTC.Data;
using QuantTC.Indicators;
using QuantTC.Indicators.Generic;
using static QuantTC.X;
namespace QuantIX.Wave
{
    public class NewHighOfSMA : Indicator<int>
    {
        public NewHighOfSMA(IIndicator<IPrice> source, int cycle)
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
            RangeL(Count, Close.Count).ForEach(i => Data.Add(FindNewHigh(i)));
            FollowUp();
        }

        private int FindNewHigh(int i)
        {
            var max = RangeR(0, i + 1).Take(Cycle).Select(ii => SixtySMA[ii]).Max();
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return RangeR(0, i + 1).FirstOrDefault(ii => SixtySMA[ii] == max );//&& TenSMA[ii] - SixtySMA[ii] >= Edge);
        }

        private SMA TenSMA { get; }

        private SMA SixtySMA { get; }

        public LiveSelector<IPrice, double> Close { get; set; }

        public IIndicator<IPrice> Source { get; }

        private int Cycle { get; }
    }
}