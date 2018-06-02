/**************************************************************************
 * 类名：KmlIcon.cs
 * 描述：图标类
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 图标类
    /// </summary>
    [XmlType(TypeName="Icon")]
    public class KmlIcon
    {
        /// <summary>
        /// 图标超链接
        /// </summary>
        public string href { get; set; }
    }
}
