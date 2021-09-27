using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace texture_packer
{
    class Program
    {
        static readonly string SOURCE = @"/home/ivan/Downloads/sprites for open arena/sprite pack/";
        static readonly string DEST = @"/home/ivan/Downloads/sprites for open arena/atlases/";

        static readonly int ATLAS_SPRITE_COUNT = 1024;
        static readonly int SPRITE_PIXEL_WIDTH = 32;
        static readonly int ATLAS_SPRITES_PER_ROW = 32;
        static readonly int ATLAS_PIXEL_WIDTH = ATLAS_SPRITES_PER_ROW * SPRITE_PIXEL_WIDTH;

        static void Main(string[] args)
        {
            var unsortedSourceImages = new List<string>();
            unsortedSourceImages.AddRange(Directory.GetFiles(SOURCE));
            unsortedSourceImages.Sort();
            var sourceImages = unsortedSourceImages.ToArray();
            var atlasesCount = Math.Ceiling(sourceImages.Length / (float)ATLAS_SPRITE_COUNT);

            //lets loop for each atlas to create it
            for (int atlasIndex = 0; atlasIndex < atlasesCount; atlasIndex++)
            {
                var offset = (ATLAS_SPRITE_COUNT * atlasIndex);
                using (var atlas = new Image<Rgba32>(ATLAS_PIXEL_WIDTH, ATLAS_PIXEL_WIDTH, Rgba32.ParseHex("00000000")))
                {
                    //loop through the images that we will save in this atlas
                    for (int i = 0; i < ATLAS_SPRITE_COUNT && (i + offset) < sourceImages.Length; i++)
                    {
                        var sourceImageIndex = i + offset;
                        var atlasRow = (int)Math.Floor(i / (float)ATLAS_SPRITES_PER_ROW);
                        var atlasCol = i % ATLAS_SPRITES_PER_ROW;

                        using (var sourceImage = Image.Load<Rgba32>(sourceImages[sourceImageIndex]))
                        {
                            for (int row = 0; row < sourceImage.Height; row++)
                            {
                                for (int col = 0; col < sourceImage.Width; col++)
                                {
                                    var x = (atlasCol * SPRITE_PIXEL_WIDTH) + col;
                                    var y = (atlasRow * SPRITE_PIXEL_WIDTH) + row;

                                    var color = sourceImage[col, row];

                                    if (color.R == 255 && color.G == 0 && color.B == 255)
                                    {
                                        color.A = 0;
                                    }

                                    atlas[x, y] = color;
                                }
                            }
                        }
                    }

                    atlas.SaveAsPng(Path.Combine(DEST, $"atlas-{atlasIndex + 1}.png"));
                }
            }
        }
    }
}
