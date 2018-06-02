/**************************************************************************
 * 类名：MFElementClickEventArgs.cs
 * 描述：地图控件图元点击事件消息结构体
 * 作者：Allen
 * 日期：Dec 2,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFrame.Core.Interface;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 地图控件图元点击事件消息结构体
    /// </summary>
    public class MFElementClickEventArgs : EventArgs
    {
        /// <summary>
        /// 鼠标事件
        /// </summary>
        public MouseEventArgs MouseEventArgs;

        /// <summary>
        /// 图元对象
        /// </summary>
        public IMFElement Element;
    }
}
