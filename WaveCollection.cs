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
        public WaveCollection(IIndicator<IPrice> source, int cycle, double priceDiffThreshold, double positionThreshold)
        {
            Source = source;
            Cycle = cycle;
            PriceDiffThreshold = priceDiffThreshold;
            PositionThreshold = positionThreshold;
            OldPosition = -1;
            Close = Source.LiveSelect((arg, i) => arg.Close);
            TenSMA = Close.SMA(10);
            SixtySMA = Close.SMA(60);
            Wave = new List<Wave>();
            WaveSequence = new List<char>();
        }

        /// <summary>
        /// identify the waves
        /// choose the high point or the low point to identify the wave
        /// </summary>
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

        /// <summary>
        /// with compare wave to get the relationship between the waves
        /// </summary>
        private void CreateTheSequence()
        {
            for (int i = 1; i < Wave.Count; i++)
            {
                WaveSequence.Add(CompareWave(Wave[i - 1], Wave[i]));
            }
        }

//        private char CompareWave(Wave wave1, Wave wave2, double threshold)
//        {
//            if (wave1.Tendency != wave2.Tendency)
//            {
//                if (CompareSlope(wave1.Slope, wave2.Slope, threshold))
//                {
//                    var compareLength = wave2.Length / wave1.Length;
//                    if (compareLength > 1.5)
//                    {
//                        return 'L';
//                    }
//
//                    if (compareLength < 0.5)
//                    {
//                        return 'j';
//                    }
//
//                    return 'v';
//                }
//
//                return 'y';
//            }
//
//            return 'i';
//        }

        private char CompareWave(Wave wave1, Wave wave2)
        {
            if (wave1.StopPoint != wave2.StartPoint)
            {
                return '$';
            }
            if (wave1.Tendency != wave2.Tendency)
            {
                var priceDiff = ComparePriceDiff(wave1, wave2, PriceDiffThreshold);
                if (priceDiff > 1 - PriceDiffThreshold)
                {
                    if (priceDiff < 1 + PriceDiffThreshold)
                    {
                        var position = Math.Abs(wave2.StopPoint - wave1.StartPoint) * PositionThreshold;
                        if (wave1.StopPoint > wave1.StartPoint + position)
                        {
                            if (wave1.StopPoint < wave2.StopPoint - position)
                            {
                                return 'V';
                            }
                            // R indicate the connect point is in the right position,closer to the wave2
                            return 'R';
                        }
                        // L indicate the connect point is in the left position
                        return 'L';
                    }
                    // r indicate wave2 price is beyond the threshold
                    return 'r';
                }
                //l indicate wave2 price is below the threshold
                return 'l';
            }

            return 'i';
        }

        private double ComparePriceDiff(Wave wave1, Wave wave2, double priceDiffThreshold)
        {
            var wave1PriceDiff = Math.Abs(wave1.StartPointprice - wave1.StopPointPrice);
            var waveBetweenPriceDiff = Math.Abs(wave1.StartPointprice - wave2.StopPointPrice);
            return (waveBetweenPriceDiff / wave1PriceDiff);
        }

        /// <summary>
        /// compare the slope of the two wave and make a decision whether they are
        /// </summary>
        /// <param name="wave1Slope"></param>
        /// <param name="wave2Slope"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private bool CompareSlope(double wave1Slope, double wave2Slope, double threshold)
        {
            var Slope =  Math.Abs(Math.Min(wave1Slope, wave2Slope) / Math.Max(wave1Slope, wave2Slope));
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

        private double PriceDiffThreshold { get; set; }

        private double PositionThreshold { get; set; }
        
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