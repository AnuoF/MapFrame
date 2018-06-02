/**************************************************************************
 * 类名：KmlCircle.cs
 * 描述：圆的Kml类
 * 作者：Allen
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using System.Drawing;
using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 圆
    /// </summary>
    [XmlType(TypeName = "Circle")]
    public class KmlCircle : KmlBaseGraph
    {
        /// <summary>
        /// 默认设置线宽为2
        /// </summary>
        private float _outLineSize = 2;
        /// <summary>
        /// 轮廓线颜色，默认为黑色
        /// </summary>
        private Color _outLineColor = Color.Black;
        /// <summary>
        /// 圆的填充色，默认为无填充
        /// </summary>
        private Color _fillColor = Color.FromArgb(0);

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 圆心位置（和圆上任意一点不能同时使用）
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 圆上任意一点（和圆心不能同时使用）
        /// </summary>
        public MapLngLat RandomPosition { get; set; }

        /// <summary>
        /// 半径,米
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// 面的轮廓颜色
        /// </summary>
        public Color StrokeColor
        {
            get { return _outLineColor; }
            set { _outLineColor = value; }
        }

        /// <summary>
        /// 面的填充颜色
        /// </summary>
        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        /// <summary>
        /// 轮廓线大小
        /// </summary>
        public float StrokeWidth
        {
            get { return _outLineSize; }
            set { _outLineSize = value; }
        }

        /// <summary>
        /// 是否栅格化   栅格化之后贴近地图，未栅格为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }


        public KmlCircle()
        {
            Position = new MapLngLat();
        }
    }
}
