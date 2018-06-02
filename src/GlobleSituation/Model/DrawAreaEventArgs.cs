using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 绘图事件消息结构体
    /// </summary>
    public class DrawAreaEventArgs : EventArgs
    {
        /// <summary>
        /// 区域类型
        /// </summary>
        public AreaType Type;

        public string AreaName;

    }
}
