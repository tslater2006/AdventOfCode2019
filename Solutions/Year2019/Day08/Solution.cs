using AdventOfCode.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day08 : ASolution {
        ElfImage image;
        public Day08() : base(8, 2019, "Space Image Format") {
            int[] data = Input[0].ToCharArray().Select(c => int.Parse(c.ToString())).ToArray() ;

            image = new ElfImage(data, 25, 6);
        }

        protected override string SolvePartOne() {

            var minLayer = image.Layers.OrderBy(l => l.Cast<int>().Count(p => p == 0)).First();

            var oneDigits = minLayer.Cast<int>().Count(p => p == 1);
            var twoDigits = minLayer.Cast<int>().Count(p => p == 2);

            var ans = oneDigits * twoDigits;

            return ans.ToString(); 
        }

        protected override string SolvePartTwo() {
            var bmp = image.RenderImage();
            bmp.Save("Day8Part2.png", ImageFormat.Png);

            return "Answer saved as Day8Part2.png"; 
        }
    }
}
