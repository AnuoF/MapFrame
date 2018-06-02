/**************************************************************************
 * 类名：EditMarker.cs
 * 描述：编辑图元时的Marker
 * 作者：Allen
 * 日期：July 19,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing;

namespace MapFrame.GMap.Model
{
    /// <summary>
    /// 编辑图元时的Marker
    /// </summary>
    class EditMarker : GMapMarker
    {
        /// <summary>
        /// 画笔
        /// </summary>
        private Pen m_Pen = null;

        public EditMarker(PointLatLng pos)
            : base(pos)
        {
            this.Size = new Size(14, 14);
            m_Pen = new Pen(Brushes.Green, 2);
        }

        /// <summary>
        /// 重绘
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            Rectangle rect = new Rectangle(LocalPosition.X - Size.Width / 2, LocalPosition.Y - Size.Height / 2, Size.Width, Size.Height);
            if (m_Pen != null)
            {
                g.FillEllipse(m_Pen.Brush, rect);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (m_Pen != null)
            {
                m_Pen.Dispose();
                m_Pen = null;
            }

            base.Dispose();
        }

    }
}
