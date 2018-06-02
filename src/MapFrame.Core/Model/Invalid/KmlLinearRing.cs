/**************************************************************************
 * 类名：KmlLinearRing.cs
 * 描述：
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
    /// 线
    /// </summary>
    [XmlType("LinearRing")]
    class KmlLinearRing
    {
        /// <summary>
        /// 点集合
        /// </summary>
        public string coordinates { get; set; }
    }
}
