/**************************************************************************
 * 类名：KmlPolygon.cs
 * 描述：面的Kml类
 * 作者：CJ
 * 日期：July 14,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Drawing;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 面
    /// </summary>
    [XmlType(TypeName = "Point")]
    public class KmlPolygon : KmlBaseGraph
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 点集合（经纬度）
        /// </summary>
        public List<MapLngLat> PositionList { get; set; }

        private Color _outLineColor = Color.Blue;
        /// <summary>
        /// 面的轮廓颜色
        /// </summary>
        public Color OutLineColor
        {
            get { return _outLineColor; }
            set { _outLineColor = value; }
        }

        private Color _fillColor = Color.Gray;
        /// <summary>
        /// 面的填充颜色
        /// </summary>
        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        /// <summary>
        /// 默认设置线宽为2
        /// </summary>
        private float _outLineSize = 2;
        /// <summary>
        /// 轮廓线大小
        /// </summary>
        public float OutLineSize
        {
            get { return _outLineSize; }
            set { _outLineSize = value; }
        }

        /// <summary>
        /// 是否栅格化   栅格化之后贴近地图，未栅格为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public KmlPolygon() 
        {
            PositionList = new List<MapLngLat>();
        }
    }
}
