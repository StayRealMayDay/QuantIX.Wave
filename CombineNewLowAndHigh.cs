using System.Collections.Generic;
using System.Linq;
using QuantTC;
using QuantTC.Indicators.Generic;
using static QuantTC.X;
namespace QuantIX.Wave
{
    public class CombineNewLowAndHigh : Indicator<int>
    {
        public CombineNewLowAndHigh(NewHighOfSMA newHighOfSma, NewLowOfSMA newLowOfSma)
        {
            NewHighOfSma = newHighOfSma;
            NewLowOfSma = newLowOfSma;
            NewLowOfSma.Update += NewLowOfSmaOnUpdate;
            OldIdx = -1;
        }

        private void NewLowOfSmaOnUpdate()
        {
            Data.FillRange(Count, NewLowOfSma.Count, Conbine);
            if (Data[Count -1] == Data[Count - 2])
            {
                Data[Count - 2] = 0;
            }
            FollowUp();
        }

        public bool NotAWave => RangeR(Count - 4, Count - 1).Any(i => Data[i] == Data[Count - 1]);

        private int Conbine(int arg)
        {
            Idx = arg;
            if (IsANewHighOfSma)
            {
                if (OldIdx == -1)
                {
                    OldIdx = Idx;
                    Waves.Add(new Wave(0, Idx, NewLowOfSma.SixtySMA[0], NewLowOfSma.SixtySMA[Idx]));
                    return 1;
                }
                Waves.Add(new Wave(OldIdx, Idx, NewLowOfSma.SixtySMA[OldIdx], NewLowOfSma.SixtySMA[Idx]));
                OldIdx = Idx;
                return 0;
            }
            if (IsANewLowOfSma)
            {
                if (OldIdx == -1)
                {
                    OldIdx = Idx;
                    Waves.Add(new Wave(0, Idx, NewLowOfSma.SixtySMA[0], NewLowOfSma.SixtySMA[Idx]));
                    return 1;
                }
                Waves.Add(new Wave(OldIdx, Idx, NewLowOfSma.SixtySMA[OldIdx], NewLowOfSma.SixtySMA[Idx]));
                OldIdx = Idx;
                return 0;
            }
            return 0;
        }

    

        public bool IsANewLowOfSma => NewLowOfSma[Idx] == Idx;

        public bool IsANewHighOfSma => NewHighOfSma[Idx] == Idx;

        private List<Wave> Waves { get; set; }

        private int Idx { get; set; }

        private int OldIdx { get; set; }

        private NewHighOfSMA NewHighOfSma { get; set; }

        private NewLowOfSMA NewLowOfSma { get; set; }
    }
}