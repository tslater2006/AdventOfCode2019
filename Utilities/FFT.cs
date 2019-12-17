using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utilities
{
    class FFT
    {
        public int[] Digits;
        int[] BasePattern = new int[] { 0, 1, 0, -1 };
        public int[] ExpandedPattern;
        public FFT(int[] digitList)
        {
            Digits = digitList;
            ExpandedPattern = new int[Digits.Length + 1];
        }

        public void BuildPattern(int outIndex)
        {
            int baseIndex = 0;
            int repeatCount = 0;
            for (var x = 0; x < ExpandedPattern.Length; x++)
            {

                ExpandedPattern[x] = BasePattern[baseIndex];
                repeatCount++;
                if (repeatCount > outIndex)
                {
                    repeatCount = 0;
                    baseIndex++;
                }

                if (baseIndex > BasePattern.Length -1)
                {
                    baseIndex = 0;
                }

            }
        }

        public void ApplyPhase()
        {
            int[] output = new int[Digits.Length];


            for (var x = 0; x < output.Length; x++)
            {
                /* for each output char ... */
                BuildPattern(x);
                int sum = 0;
                for (var y = 0; y < Digits.Length; y++)
                {
                    sum += Digits[y] * ExpandedPattern[y + 1];
                }

                output[x] = Math.Abs(sum % 10);
            }
            output.CopyTo(Digits, 0);
        }

        public void ApplyPhaseBackHalf()
        {
            for (var x = Digits.Length - 2; x >= Digits.Length/2; x--)
            {
                /* for each output char ... */
                Digits[x] = (Digits[x] + Digits[x + 1]) % 10;
            }
        }

    }
}
