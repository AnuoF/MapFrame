using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 显示书签列表事件消息结构体
    /// </summary>
    public class ShowBookmarkEventArgs : EventArgs
    {
        /// <summary>
        /// 书签名称列表
        /// </summary>
        public List<string> NameList = new List<string>();

        /// <summary>
        /// 是否追加
        /// </summary>
        public bool Append { get; set; }
    }
}
