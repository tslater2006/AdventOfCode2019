using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AdventOfCode.Utilities
{
    class ElfImage
    {
        public List<int[,]> Layers = new List<int[,]>();
        public int Width;
        public int Height;

        public ElfImage(int[] imageData, int width, int height)
        {
            Width = width;
            Height = height;
            int layerSize = width * height;
            int[,] curLayer = null;
            for (var x = 0; x < imageData.Length; x++)
            {
                if (x % layerSize == 0)
                {
                    /* time for a new layer array */
                    curLayer = new int[height, width];
                    Layers.Add(curLayer);
                }

                var layerIndex = x % layerSize;

                var row = layerIndex / width;
                var col = layerIndex % width;

                curLayer[row, col] = imageData[x];

            }
        }

        public int[,] FlattenLayers()
        {
            int[,] final = new int[Height, Width];

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    for (var z = 0; z < Layers.Count; z++)
                    {
                        if (Layers[z][y, x] != 2)
                        {
                            final[y, x] = Layers[z][y, x];
                            break;
                        }
                    }
                }
            }

            return final;
        }

        public Bitmap RenderImage()
        {
            int[,] finalImage = FlattenLayers();

            Bitmap bmp = new Bitmap(Width, Height);

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    bmp.SetPixel(x, y, finalImage[y, x] == 0 ? Color.Black : Color.White);
                }
            }

            return bmp;
        }

    }

}
