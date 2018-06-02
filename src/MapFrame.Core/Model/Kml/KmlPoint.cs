/**************************************************************************
 * 类名：KmlPoint.cs
 * 描述：点定义
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

#define GMap
//#define ArcMap


using System;
using System.Drawing;
using System.Xml.Serialization;


namespace MapFrame.Core.Model
{
    /// <summary>
    /// 点定义
    /// </summary>
    [XmlType(TypeName="Point")]
    public class KmlPoint : KmlBaseGraph
    {
#if GMap
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 大小，格式（Width，Height）
        /// </summary>
        public MapSize Size { get; set; }

        /// <summary>
        /// 点的颜色
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Tip内容
        /// </summary>
        public string TipText { get; set; }

        /// <summary>
        /// 标牌内容
        /// </summary>
        public string LabelText { get; set; }

        /// <summary>
        /// 是否栅格化   栅格化之后贴近地图，未栅格为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }

        /// <summary>
        /// 点的样式  使用3最好
        /// </summary>
        public int PointStyle { get; set; }

        /// <summary>
        /// 点图片路径
        /// 方式①：直接传图片路径
        /// 方式②：字体文件*.ttf路径|字体代号|字体大小，如“D：\abc.ttf|80|16”
        /// </summary>
        public string IcoUrl { get; set; }

        /// <summary>
        /// 字体库路径
        /// </summary>
        public string FontPath { get; set; }

        /// <summary>
        /// 字体代码
        /// </summary>
        public int Code { get; set; }
#endif

#if ArcMap
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public double Size { get; set; }

         /// <summary>
        /// 点的颜色
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// 方位角
        /// </summary>
        public double Angle { get; set; }
#endif

    }
}
