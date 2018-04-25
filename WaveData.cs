using System.Collections.Generic;
using System.Linq;
using QuantTC;
using QuantTC.Data;
using QuantTC.Indicators;
using QuantTC.Indicators.Generic;
using static QuantTC.X;

namespace QuantIX.Wave
{
    public class WaveData : Indicator<int>
    {
         public WaveData(IIndicator<IPrice> source, int cycle)
        {
            Source = source;
            Close = Source.LiveSelect((arg, i) => arg.Close);
            Waves = new List<Wave>();
            MACD = Close.MACD(12, 26, 9);
            High = Source.High();
            Low = Source.Low();
            HHV = High.HighestValue(cycle);
            LLV = Low.LowestValue(cycle);
            Source.Update += SourceOnUpdate;
        }

        private void SourceOnUpdate()
        {
            Data.FillRange(Count, Source.Count, FillWaves);
            FollowUp();
        }

        private int FillWaves(int arg)
        {
            if (IsMacdDeathXAt(arg))
            {
                int higtestPointIndex = RangeR(0, arg + 1).First(ii => HHV[ii] == High[ii]);
                int lowestPointIndex = RangeR(0, higtestPointIndex).First(ii => LLV[ii] == Low[ii]);
               Waves.Add(new Wave(lowestPointIndex, higtestPointIndex, Source[lowestPointIndex].Low, Source[higtestPointIndex].High, 0));
            }
            if (IsMacdGoldenXAt(arg))
            {
                int LowestPointIndex = RangeR(0, arg + 1).First(ii => LLV[ii] == Low[ii]);
                int highestPointIndex = RangeR(0, LowestPointIndex).First(ii => HHV[ii] == High[ii]);
                Waves.Add(new Wave(highestPointIndex, LowestPointIndex, Source[highestPointIndex].High, Source[LowestPointIndex].Low, 0));
            }
            return 0;
        }

        public LiveSelector<IPrice, double> Close { get; }

        public IIndicator<IPrice> Source { get; set; }

        public MACD MACD { get; }

        public List<Wave> Waves { get; set; }

        public IIndicator<double> High { get; }

        public IIndicator<double> Low { get; }

        public MovingMostValue<double> HHV { get; }

        public MovingMostValue<double> LLV { get; }

        private bool IsMacdDeathXAt(int i) => MACD.Macd.IsDxAt(0.0, i);

        private bool IsMacdGoldenXAt(int i) => MACD.Macd.IsUxAt(0.0, i);
    }
}