/**************************************************************************
 * 类名：KmlhotSpot.cs
 * 描述：
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 热点
    /// </summary>
    [XmlType(TypeName="hotSpot")]
    class KmlhotSpot
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string x { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string xunits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string yunits { get; set; }
    }
}
