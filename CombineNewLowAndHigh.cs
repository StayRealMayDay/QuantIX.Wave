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
                return 1;
            }
            if (IsANewLowOfSma)
            {
                return -1;
            }
            return 0;
        }

        public bool IsANewLowOfSma => NewLowOfSma[Idx] == Idx;

        public bool IsANewHighOfSma => NewHighOfSma[Idx] == Idx;
        

        private int Idx { get; set; }
        
        
        
        private NewHighOfSMA NewHighOfSma { get; set; }

        private NewLowOfSMA NewLowOfSma { get; set; }
    }
}