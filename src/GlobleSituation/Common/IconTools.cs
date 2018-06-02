using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GlobleSituation.Common
{
    public static class IconTools
    {
        /// <summary>
        /// 传入图片格式文件获取ICON
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Icon GetIcon(string path)
        {
            Size size = new Size(32, 32);
            using (Bitmap bm = new Bitmap(path))
            {
                using (Bitmap iconBm = new Bitmap(bm, size))
                {
                    return Icon.FromHandle(iconBm.GetHicon());
                }
            }
        }

        /// <summary>
        /// 将内存中的图片转为ICON
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Icon GetIcon(Image image)
        {
            Size size = new Size(32, 32);
            using (Bitmap bm = new Bitmap(image))
            {
                using (Bitmap iconBm = new Bitmap(bm, size))
                {
                    return Icon.FromHandle(iconBm.GetHicon());
                }
            }
        }
    }
}
