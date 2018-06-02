/**************************************************************************
 * 类名：MFElementEnterEventArgs.cs
 * 描述：鼠标进入图元事件消息结构体
 * 作者：Allen
 * 日期：Dec 2,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Interface;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 鼠标进入图元事件消息结构体
    /// </summary>
    public class MFElementEnterEventArgs : EventArgs
    {
        public IMFElement Element;
    }
}
