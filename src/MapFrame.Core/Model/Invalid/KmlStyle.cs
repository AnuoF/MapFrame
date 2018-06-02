/**************************************************************************
 * 类名：KmlStyle.cs
 * 描述：样式
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 样式
    /// </summary>
    [XmlType(TypeName="Style")]
    class KmlStyle
    {
        /// <summary>
        /// ID
        /// </summary>
        [XmlAttribute]
        public string id { get; set;}

        /// <summary>
        /// Icon样式
        /// </summary>
        public KmlIconStyle IconStyle { get; set; }

        /// <summary>
        /// 线样式
        /// </summary>
        public KmlLineStyle LineStyle { get; set; }

        /// <summary>
        /// 多边形样式
        /// </summary>
        public KmlPolyStyle PolyStyle { get; set; }
    }
}
