using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Camera
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Extenisonnnnnnnnnnkasdfn;asvnk.zvgnwjfopewajgp'
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ToWpfSource(this Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                var result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = ms;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
