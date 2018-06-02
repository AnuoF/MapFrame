/**************************************************************************
 * 类名：MFElementLeaveEventArgs.cs
 * 描述：鼠标离开图元事件消息结构体
 * 作者：Allen
 * 日期：Dec 2,2016
 * 
 * ************************************************************************/


using MapFrame.Core.Interface;
using System;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 鼠标离开图元事件消息结构体
    /// </summary>
    public class MFElementLeaveEventArgs : EventArgs
    {
        public IMFElement Element;
    }
}
