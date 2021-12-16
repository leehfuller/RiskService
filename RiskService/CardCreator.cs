using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using System.Text.RegularExpressions;

namespace RiskService
{
    public class CardCreator
    {
		private int cardHeight = 400;
		private int cardWidth = 300;

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

		public SKBitmap getSkiaImage(string loadFile)
		{
			byte[] FileBytesCard = getFileBytes(loadFile);
			MemoryStream skiaCard = new MemoryStream(FileBytesCard);
			SKBitmap cardBitmap = SKBitmap.Decode(skiaCard);
			return (cardBitmap);
		}

		public string createMissionImage(string baseCard, string missionCard, string missionText)
        {
			SKPaint missionPaint = new SKPaint { TextSize = 12.0f, IsAntialias = true, Color = SKColors.Black, IsStroke = false, Typeface = SKTypeface.FromFamilyName("Courier New") };
			SKImage finalCardImage = null;

			SKBitmap cardBitmap = getSkiaImage(baseCard);
			SKBitmap missionBitmap = getSkiaImage(missionCard);

			SKSurface drawSurface = SKSurface.Create(new SKImageInfo(cardWidth, cardHeight));
			SKCanvas drawCanvas = drawSurface.Canvas;
			drawCanvas.Clear(SKColors.Transparent);

			drawCanvas.DrawImage(SKImage.FromBitmap(cardBitmap), SKRect.Create(0, 0, cardWidth, cardHeight/2));
			drawCanvas.DrawImage(SKImage.FromBitmap(missionBitmap), SKRect.Create(0, cardHeight/2, cardWidth, cardHeight));

			int textWidth = 0, textHeight = 0;
			string longestText = "";
			(textWidth, textHeight, longestText) = calculateTextBounds(missionText);
			drawTextLines(missionText, 0, 0, missionPaint, drawCanvas);

			finalCardImage = drawSurface.Snapshot();
			SKData missionPNG = finalCardImage.Encode(SKEncodedImageFormat.Png, 100);
			byte[] missionBytes = missionPNG.ToArray();

			return (Convert.ToBase64String(missionBytes));
        }

		private static void drawTextLines(string str, float x, float y, SKPaint paint, SKCanvas canvas)
		{
			string[] lines = str.Split("\n");
			float txtSize = paint.TextSize;

			for (int i = 0; i < lines.Length; i++)
			{
				canvas.DrawText(TrimNonAscii(lines[i]), x, y + (txtSize * i), paint);
			}
		}

		private static string TrimNonAscii(string value)
		{
			string pattern = "[^ -~]+";
			Regex reg_exp = new Regex(pattern);
			return reg_exp.Replace(value, "");
		}

		private static (int, int, string) calculateTextBounds(string str)
		{
			string[] lines = str.Split("\n");
			int maxWidth = 0;
			int maxHeight = 0;
			string longestString = "";

			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Length > maxWidth)
				{
					maxWidth = lines[i].Length;
					longestString = lines[i];
				}
				maxHeight++;
			}

			return (maxWidth, maxHeight, longestString);
		}
	}
}
