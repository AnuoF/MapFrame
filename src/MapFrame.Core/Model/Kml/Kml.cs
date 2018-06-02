/**************************************************************************
 * 类名：Kml.cs
 * 描述：kml根节点类
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System;
using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// kml根节点类
    /// </summary>
    [XmlType(TypeName = "kml")]
    [Serializable]
    public class Kml
    {

        /// <summary>
        /// Placemark
        /// </summary>
        public KmlPlacemark Placemark { get; set; }

        /// <summary>
        /// Kml对象
        /// </summary>
        public Kml()
        {
            Placemark = new KmlPlacemark();
        }
    }
}
