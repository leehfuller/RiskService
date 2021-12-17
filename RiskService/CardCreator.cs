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
		private int cardHeight = 300;
		private int cardWidth = 200;
		private int cardTextWidth = 30;
		private string qrCode = "QRLink.png";

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
			SKPaint missionPaint = new SKPaint { TextSize = 12.0f, IsAntialias = true, Color = SKColors.DarkGray, IsStroke = false, Typeface = SKTypeface.FromFamilyName("Calibri") };
			SKImage finalCardImage = null;

			SKBitmap cardBitmap = getSkiaImage(baseCard);
			SKBitmap missionBitmap = getSkiaImage(missionCard);
			SKBitmap qrlinkBitmap = getSkiaImage(qrCode);

			SKSurface drawSurface = SKSurface.Create(new SKImageInfo(cardWidth, cardHeight));
			SKCanvas drawCanvas = drawSurface.Canvas;
			drawCanvas.Clear(SKColors.Transparent);

			drawCanvas.DrawImage(SKImage.FromBitmap(cardBitmap), SKRect.Create(0, 0, cardWidth, cardHeight));
			drawCanvas.DrawImage(SKImage.FromBitmap(missionBitmap), SKRect.Create(20, 20, cardWidth-40, (cardHeight/2)));
			drawCanvas.DrawImage(SKImage.FromBitmap(qrlinkBitmap), SKRect.Create(cardWidth-65, cardHeight-70, 60, 60));

			int textWidth = 0, textHeight = 0;
			string longestText = "";
			(textWidth, textHeight, longestText) = calculateTextBounds(missionText);
			drawTextLines(missionText, 20, (cardHeight/2)+40, missionPaint, drawCanvas);
			drawTextLines(DateTime.Now.ToString("MMMM dd yyyy"), 20, cardHeight - 32, missionPaint, drawCanvas);
			drawTextLines(DateTime.Now.ToString("T"), 20, cardHeight - 22, missionPaint, drawCanvas);

			finalCardImage = drawSurface.Snapshot();
			SKData missionPNG = finalCardImage.Encode(SKEncodedImageFormat.Png, 100);
			byte[] missionBytes = missionPNG.ToArray();

			#if DEBUG
			using (var filestream = File.OpenWrite("debug.png"))
				{
				missionPNG.SaveTo(filestream);
				}
			#endif

			return (Convert.ToBase64String(missionBytes));
        }

		private void drawTextLines(string str, float x, float y, SKPaint paint, SKCanvas canvas)
		{
			string prepLines = SplitToLines(str, cardTextWidth);

			string[] lines = prepLines.Split("\n");
			float txtSize = paint.TextSize;

			for (int i = 0; i < lines.Length; i++)
			{
				canvas.DrawText(TrimNonAscii(lines[i]), x, y + (txtSize * i), paint);
			}
		}

		public string SplitToLines(string stringToSplit, int maximumLineLength)
		{
			return Regex.Replace(stringToSplit, @"(.{1," + maximumLineLength + @"})(?:\s|$)", "$1\n");
		}

		private static string TrimNonAscii(string value)
		{
			string pattern = "[^ -~]+";
			Regex reg_exp = new Regex(pattern);
			return reg_exp.Replace(value, "");
		}

		private (int, int, string) calculateTextBounds(string str)
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
