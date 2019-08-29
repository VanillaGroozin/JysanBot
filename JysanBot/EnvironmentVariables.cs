using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JysanBot
{
    public static class EnvironmentVariables
    {
        public const string BotToken =
            "938339701:AAEwiQPszzLIIuYek_2-HSFnM-82lFKfN8w";
        public const string DatabaseUrl =
            "JysanBot.db";
        public static string LastPrintedMessage = string.Empty;
        public static string LastInputMessage = string.Empty;
        public static bool ShowLastPrintedMessage = false;
        public static string MessagePath = string.Empty;

       
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
