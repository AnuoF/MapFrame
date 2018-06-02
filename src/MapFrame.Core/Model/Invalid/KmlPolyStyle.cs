/**************************************************************************
 * 类名：KmlPolyStyle.cs
 * 描述：填充图形的样式
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 填充图形的样式
    /// </summary>
    [XmlType("PolyStyle")]
    class KmlPolyStyle
    {
        /// <summary>
        /// 多边形填充颜色
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 多边形轮廓
        /// </summary>
        public int outline { get; set; }

        /// <summary>
        /// 不透明度
        /// </summary>
        //public int fill { get; set; }
    }
}
