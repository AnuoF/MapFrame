/**************************************************************************
 * 类名：KmlDocument.cs
 * 描述：Kml文本类
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// Kml文本类
    /// </summary>
    class KmlDocument
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否可见：1-可见；0-不可见
        /// </summary>
        public int visibility { get; set; }

        /// <summary>
        /// 对象
        /// </summary>
        [XmlElement("Placemark")]
        public KmlPlacemark Placemark { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public KmlDocument()
        {
            Placemark = new KmlPlacemark();
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="name">文件名</param>
        public KmlDocument(string name)
        {
            this.name = name;
            Placemark = new KmlPlacemark();
        }
    }
}
