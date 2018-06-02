/**************************************************************************
 * 类名：KmlPlacemark.cs
 * 描述：目标对象
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 目标对象
    /// </summary>
    [XmlType(TypeName="Placemark")]
    public class KmlPlacemark
    {

        /// <summary>
        /// 对象名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 绘制图形的基本类
        /// </summary>
        [XmlElement(typeof(KmlPoint))]
        [XmlElement(typeof(KmlLineString))]
        [XmlElement(typeof(KmlPolygon))]
        [XmlElement(typeof(KmlText))]
        public KmlBaseGraph Graph { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public KmlPlacemark() 
        {
        }
    }
}
