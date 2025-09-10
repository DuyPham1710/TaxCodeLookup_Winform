using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace TestWinform.utils
{
    internal class Helper
    {
        public static string ReadCaptchaFromImage(Image image)
        {
            string resultText = "";
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        using (var page = engine.Process(pix))
                        {
                            resultText = page.GetText().Trim();
                        }
                    }
                }
            }
            return resultText;
        }
    }
}
