using AdventOfCode.Utilities;
using System;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day16 : ASolution {
        FFT fft;
        public Day16() : base(16, 2019, "Flawed Frequency Transmission") {
        }

        protected override string SolvePartOne() {
            fft = new FFT(Input[0].Select(c => int.Parse(c.ToString())).ToArray());
            for (var x = 0; x < 100; x++)
            {
                fft.ApplyPhase();
            }

            var ans = string.Join("", fft.Digits.Take(8).Select(i => i.ToString()));

            return ans.ToString(); 
        }

        protected override string SolvePartTwo() {
            var originalInput = Input[0].Select(c => int.Parse(c.ToString())).ToArray();

            var part2Input = new int[originalInput.Length * 10000];

            for (var x = 0; x < originalInput.Length * 10000; x++)
            {
                part2Input[x] = originalInput[x % originalInput.Length];
            }

            fft = new FFT(part2Input);

            for(var x =0;x < 100; x++)
            {
                fft.ApplyPhaseBackHalf();
            }

            var offset = int.Parse(string.Join("",originalInput.Take(7).Select(i => i.ToString())));
            var ans = int.Parse(string.Join("", fft.Digits.Skip(offset).Take(8).Select(i => i.ToString())));
            return ans.ToString(); 
        }
    }
}
