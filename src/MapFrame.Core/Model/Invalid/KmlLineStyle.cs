/**************************************************************************
 * 类名：KmlLineStyle.cs
 * 描述：线样式
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 线样式
    /// </summary>
    [XmlType(TypeName = "LineStyle")]
    class KmlLineStyle
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int width { get; set; }
    }
}
