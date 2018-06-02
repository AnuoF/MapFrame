using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 工具操作类型
    /// </summary>
    public enum ToolTypeEnum
    {
        /// <summary>
        /// 绘制编辑工具
        /// </summary>
        Draw = 0,

        /// <summary>
        /// 测量工具
        /// </summary>
        Measure=1,

        /// <summary>
        /// 选择工具
        /// </summary>
        Select = 2,

        /// <summary>
        /// 编辑工具
        /// </summary>
        Edit = 3
    }
}
