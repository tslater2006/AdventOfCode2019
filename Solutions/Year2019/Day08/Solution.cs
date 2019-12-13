using AdventOfCode.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

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

        protected override string SolvePartTwo()
        {
            /* my %key = (
  'A' => [[0,1,1,1,1,1],[1,0,0,1,0,0],[1,0,0,1,0,0],[0,1,1,1,1,1],[0,0,0,0,0,0]],
  'B' => [[1,1,1,1,1,1],[1,0,1,0,0,1],[1,0,1,0,0,1],[0,1,0,1,1,0],[0,0,0,0,0,0]],
  'C' => [[0,1,1,1,1,0],[1,0,0,0,0,1],[1,0,0,0,0,1],[0,1,0,0,1,0],[0,0,0,0,0,0]],
  'E' => [[1,1,1,1,1,1],[1,0,1,0,0,1],[1,0,1,0,0,1],[1,0,0,0,0,1],[0,0,0,0,0,0]],
  'F' => [[1,1,1,1,1,1],[1,0,1,0,0,0],[1,0,1,0,0,0],[1,0,0,0,0,0],[0,0,0,0,0,0]],
  'G' => [[0,1,1,1,1,0],[1,0,0,0,0,1],[1,0,0,1,0,1],[0,1,0,1,1,1],[0,0,0,0,0,0]],
  'H' => [[1,1,1,1,1,1],[0,0,1,0,0,0],[0,0,1,0,0,0],[1,1,1,1,1,1],[0,0,0,0,0,0]],
  'J' => [[0,0,0,0,1,0],[0,0,0,0,0,1],[1,0,0,0,0,1],[1,1,1,1,1,0],[0,0,0,0,0,0]],
  'K' => [[1,1,1,1,1,1],[0,0,1,0,0,0],[0,1,0,1,1,0],[1,0,0,0,0,1],[0,0,0,0,0,0]],
  'L' => [[1,1,1,1,1,1],[0,0,0,0,0,1],[0,0,0,0,0,1],[0,0,0,0,0,1],[0,0,0,0,0,0]],
  'P' => [[1,1,1,1,1,1],[1,0,0,1,0,0],[1,0,0,1,0,0],[0,1,1,0,0,0],[0,0,0,0,0,0]],
  'R' => [[1,1,1,1,1,1],[1,0,0,1,0,0],[1,0,0,1,1,0],[0,1,1,0,0,1],[0,0,0,0,0,0]],
  'U' => [[1,1,1,1,1,0],[0,0,0,0,0,1],[0,0,0,0,0,1],[1,1,1,1,1,0],[0,0,0,0,0,0]],
  'Y' => [[1,1,0,0,0,0],[0,0,1,0,0,0],[0,0,0,1,1,1],[0,0,1,0,0,0],[1,1,0,0,0,0]],
  'Z' => [[1,0,0,0,1,1],[1,0,0,1,0,1],[1,0,1,0,0,1],[1,1,0,0,0,1],[0,0,0,0,0,0]],
); */
            var bmp = image.RenderImage();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y).ToArgb() == Color.White.ToArgb())
                    {
                        sb.Append('█');
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }


            // bmp.Save("Day8Part2.png", ImageFormat.Png);

            return sb.ToString();
        }
    }
}
