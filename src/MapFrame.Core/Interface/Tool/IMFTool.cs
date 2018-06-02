/**************************************************************************
 * 类名：ITool.cs
 * 描述：工具操作接口
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 工具操作接口
    /// </summary>
    public interface IMFTool : IDisposable
    {   
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        event EventHandler<MessageEventArgs> CommondExecutedEvent;

        /// <summary>
        /// 执行操作
        /// </summary>
        void RunCommond();

        /// <summary>
        /// 释放工具
        /// </summary>
        void ReleaseCommond();

    }
}
