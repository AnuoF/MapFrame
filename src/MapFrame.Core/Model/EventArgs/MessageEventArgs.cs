using System;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 工具操作完成事件消息结构体
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe
        {
            get;
            set;
        }

        /// <summary>
        /// 数据对象，不同的工具对象返回有所不同
        /// 测量：距离、面积、方位角      double
        /// 框选：图元集合                List<IMFElement>
        /// 画图：图元对象                IMFElement
        /// etc:
        /// </summary>
        public Object Data
        {
            get;
            set;
        }

        /// <summary>
        /// 其他
        /// </summary>
        public Object Other
        {
            get;
            set;
        }

        /// <summary>
        /// 工具类型
        /// </summary>
        public ToolTypeEnum ToolType
        {
            get;
            set;
        }

    }
}
