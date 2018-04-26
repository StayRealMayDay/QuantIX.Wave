using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using QuantTC.Data;
using QuantTC.Indicators;
using QuantTC.Indicators.Generic;
using static QuantTC.X;

namespace QuantIX.Wave
{
    public class WaveCollection
    {
        public WaveCollection(IIndicator<IPrice> source, int cycle, double threshold)
        {
            Source = source;
            Cycle = cycle;
            Threshold = threshold;
            OldPosition = -1;
            Close = Source.LiveSelect((arg, i) => arg.Close);
            TenSMA = Close.SMA(10);
            SixtySMA = Close.SMA(60);
        }


        private void IdentifyWaves()
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


        private void CreateTheSequence()
        {
            for (int i = 1; i < Wave.Count; i++)
            {
                WaveSequence.Add(CompareWave(Wave[i - 1], Wave[i], Threshold));
            }
        }

        private char CompareWave(Wave wave1, Wave wave2, double threshold)
        {
            if (wave1.Tendency != wave2.Tendency)
            {
                if (CompareSlpoe(wave1.Slope, wave2.Slope, threshold))
                {
                    var compareLength = wave2.Length / wave1.Length;
                    if (compareLength > 1.5)
                    {
                        return 'L';
                    }

                    if (compareLength < 0.5)
                    {
                        return 'j';
                    }

                    return 'v';
                }

                return 'y';
            }

            return 'i';
        }

        private bool CompareSlpoe(double wave1Slope, double wave2Slope, double threshold)
        {
            var Slope = Math.Min(wave1Slope, wave2Slope) / Math.Max(wave1Slope, wave2Slope);
            return Slope > threshold;
        }


        public List<char> GetData()
        {
            IdentifyWaves();
            CreateTheSequence();
            return WaveSequence;
        }

        public bool IsALowPoint => RangeR(0, (Idx + Cycle) > SixtySMA.Count ? SixtySMA.Count : Idx + Cycle)
            .Take(Cycle * 2)
            .All(j => SixtySMA[j] >= SixtySMA[Idx]);

        public bool IsAHighPoint => RangeR(0, (Idx + Cycle) > SixtySMA.Count ? SixtySMA.Count : Idx + Cycle)
            .Take(Cycle * 2)
            .All(j => SixtySMA[j] <= SixtySMA[Idx]);

        private double Threshold { get; set; }
        
        private List<char> WaveSequence { get; set; }

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