/**************************************************************************
 * 类名：KmlOuterBoundaryIs.cs
 * 描述：点集合
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 点集合
    /// </summary>
    [XmlType("outerBoundaryIs")]
    class KmlOuterBoundaryIs
    {
        /// <summary>
        /// 线
        /// </summary>
        public KmlLinearRing LinearRing { get; set; }
    }
}
