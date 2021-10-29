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


    }
}
