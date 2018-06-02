/**************************************************************************
 * 类名：KmlIconStyle.cs
 * 描述：图标样式
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 图标样式
    /// </summary>
    [XmlType(TypeName = "IconStyle")]
    class KmlIconStyle
    {
        /// <summary>
        /// 大小
        /// </summary>
        public double scale { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }

        ///// <summary>
        ///// 图标
        ///// </summary>
        //public KmlIcon Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public KmlhotSpot hotSpot { get; set; }
        
        /// <summary>
        /// 样式
        /// </summary>
        public KmlIconStyle() 
        {
           
        }
    }
}
