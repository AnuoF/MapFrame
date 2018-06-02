using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 添加图元后的委托
    /// </summary>
    public class ElementAddEventArgs : EventArgs
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ElementAddEventArgs() { }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="elementName">图元名称</param>
        public ElementAddEventArgs(string layerName, string elementName)
        {
            LayerName = layerName;
            ElementName = elementName;
        }

        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName { get; set; }
    }
}
