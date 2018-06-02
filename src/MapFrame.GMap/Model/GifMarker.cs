/**************************************************************************
 * 类名：GifMarker.cs
 * 描述：GIF图标图元
 * 作者：Allen
 * 日期：Aug 31,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing;

namespace MapFrame.GMap.Model
{
    /// <summary>
    /// GIF图标图元
    /// </summary>
    public class GifMarker : GMapMarker
    {

        private Image m_Bitmap = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pos"></param>
        public GifMarker(PointLatLng pos)
            : base(pos)
        {
            this.Size = new Size(50, 50);

            string imagePath = @"D:\gif.gif";
            m_Bitmap = Bitmap.FromFile(imagePath);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            if (!this.Overlay.Control.ViewArea.Contains(this.Position)) return;

            Rectangle bitmapRct = new Rectangle(LocalPosition.X - Size.Width / 2, LocalPosition.Y - Size.Height / 2, Size.Width, Size.Height);

            if (m_Bitmap != null)
                g.DrawImage(m_Bitmap, bitmapRct);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (m_Bitmap != null)
            {
                m_Bitmap.Dispose();
                m_Bitmap = null;
            }

            base.Dispose();
        }


    }
}
