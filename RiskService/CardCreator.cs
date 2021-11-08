using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;

namespace RiskService
{
    public class CardCreator
    {
        public byte[] getFileBytes(string loadFile)
        {
            FileStream fs = new FileStream(loadFile, FileMode.Open, FileAccess.Read);
            MemoryStream memoryFile = new MemoryStream();
            fs.CopyTo(memoryFile);
            byte[] FileBytes = memoryFile.ToArray();

            return (FileBytes);
        }

        public string getFromImage(string loadFile)
        {
            byte[] FileBytes = getFileBytes(loadFile);
            return (Convert.ToBase64String(FileBytes));
        }

        public string createMissionImage(string currentFile)
        {
            byte[] FileBytes = getFileBytes(currentFile);

            MemoryStream skiaMS = new MemoryStream(FileBytes);
            SKBitmap asciiBitmap = SKBitmap.Decode(skiaMS);

            SKColor[] pixels1 = asciiBitmap.Pixels;


            return ("");
        }


		void NotMain(string[] args)
		{
			//get all the files in a directory
			string[] files = Directory.GetFiles("images");

			//combine them into one image
			SKImage stitchedImage = Combine(files);

			//make sure the output folder exists
			if (!Directory.Exists("output"))
			{
				Directory.CreateDirectory("output");
			}

			//save the new image
			using (SKData encoded = stitchedImage.Encode(SKEncodedImageFormat.Png, 100))
			using (Stream outFile = File.OpenWrite("output/stitchedImage.png"))
			{
				encoded.SaveTo(outFile);
			}
		}

		public static SKImage Combine(string[] files)
		{
			//read all images into memory
			List<SKBitmap> images = new List<SKBitmap>();
			SKImage finalImage = null;

			try
			{
				int width = 0;
				int height = 0;

				foreach (string image in files)
				{
					//create a bitmap from the file and add it to the list
					SKBitmap bitmap = SKBitmap.Decode(image);

					//update the size of the final bitmap
					width += bitmap.Width;
					height += bitmap.Height;

					images.Add(bitmap);
				}

				//get a surface so we can draw an image
				using (var tempSurface = SKSurface.Create(new SKImageInfo(width, height)))
				{
					//get the drawing canvas of the surface
					var canvas = tempSurface.Canvas;

					//set background color
					canvas.Clear(SKColors.Transparent);

					//go through each image and draw it on the final image
					int offset = 0;
					int offsetTop = 0;
					foreach (SKBitmap image in images)
					{
						canvas.DrawBitmap(image, SKRect.Create(offset, offsetTop, image.Width, image.Height));
						offsetTop = offsetTop > 0 ? 0 : image.Height / 2;
						offset += (int)(image.Width / 1.6);
					}

					// return the surface as a manageable image
					finalImage = tempSurface.Snapshot();
				}

				//return the image that was just drawn
				return finalImage;
			}
			finally
			{
				//clean up memory
				foreach (SKBitmap image in images)
				{
					image.Dispose();
				}
			}

		}
	}
}
