/**************************************************************************
 * 类名：KmlLineString.cs
 * 描述：线
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Drawing;
using System;

namespace MapFrame.Core.Model
{

    /// <summary>
    /// 线
    /// </summary>
    [XmlType(TypeName = "LineString")]
    public class KmlLineString : KmlBaseGraph
    {
        private float outLineSize = 2;
        private Color _lineColor = Color.Blue;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 位置，格式（经度，纬度）
        /// </summary>
        public List<MapLngLat> PositionList { get; set; }

        /// <summary>
        /// 线宽
        /// </summary>
        public float Width
        {
            get { return outLineSize; }
            set { outLineSize = value; }
        }

        /// <summary>
        /// 线的颜色
        /// </summary>
        [XmlIgnore]
        public Color Color
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// 线条样式
        /// </summary>
        public int LineStyle { get; set; }

        /// <summary>
        /// 是否栅格化   栅格化之后贴近地图，未栅格为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }


        public KmlLineString()
        {
            PositionList = new List<MapLngLat>();
        }
    }
}
