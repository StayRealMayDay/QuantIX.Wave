using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using QuantTC.Data;
using QuantTC.Indicators;
using QuantTC.Indicators.Generic;
using static QuantTC.X;
namespace QuantIX.Wave
{
    public class WaveCollection
    {
        public WaveCollection(IIndicator<IPrice> source, int cycle)
        {
            Source = source;
            Cycle = cycle;
            OldPosition = -1;
            Close = Source.LiveSelect((arg, i) => arg.Close);
            TenSMA = Close.SMA(10);
            SixtySMA = Close.SMA(60);
        }


        public void IdentifyWaves()
        {
            for (int i = 0; i < SixtySMA.Count; i++)
            {
                Idx = i;
                if (IsAHighPoint)
                {
                    if (OldPosition != -1)
                    {
                        Wave.Add(new Wave(OldPosition, i, SixtySMA[OldPosition], SixtySMA[i], 1));
                    }
                    OldPosition = i;
                }

                if (IsALowPoint)
                {
                    if (OldPosition != -1)
                    {
                        Wave.Add(new Wave(OldPosition, i, SixtySMA[OldPosition], SixtySMA[i], -1));
                    }
                    OldPosition = i;
                }
            }
        }
        
        
        

        public bool IsALowPoint => RangeR(0, (Idx + Cycle) > SixtySMA.Count ? SixtySMA.Count : Idx + Cycle)
            .Take(Cycle * 2)
            .All(j => SixtySMA[j] >= SixtySMA[Idx]);

        public bool IsAHighPoint => RangeR(0, (Idx + Cycle) > SixtySMA.Count ? SixtySMA.Count : Idx + Cycle)
            .Take(Cycle * 2)
            .All(j => SixtySMA[j] <= SixtySMA[Idx]);


        private List<int> WaveSequence { get; set; }

        private int OldPosition { get; set; }

        private List<Wave> Wave { get; set; }

        public SMA SixtySMA { get; set; }

        public SMA TenSMA { get; set; }

        private int Idx { set; get; }
    
        public LiveSelector<IPrice, double> Close { get; set; }

        public int Cycle { get; set; }

        public IIndicator<IPrice> Source { get; set; }

    }
}