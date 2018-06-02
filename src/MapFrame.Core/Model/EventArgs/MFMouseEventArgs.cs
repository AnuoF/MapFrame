/**************************************************************************
 * 类名：MFMouseMoveEventArgs.cs
 * 描述：地图控件鼠标移动事件消息结构体
 * 作者：Allen
 * 日期：Dec 2,2016
 * 
 * ************************************************************************/

using System;
using System.Windows.Forms;
using System.Drawing;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 地图控件鼠标移动事件消息结构体
    /// </summary>
    public class MFMouseEventArgs : EventArgs
    {
        /// <summary>
        /// 当前位置经纬度
        /// </summary>
        public MapLngLat Position { get; private set; }

        /// <summary>
        /// 获取曾按下的是哪个鼠标按钮
        /// </summary>
        public MouseButtons Button { get; private set; }

        /// <summary>
        /// 获取鼠标在产生鼠标事件时的位置
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// 获取鼠标在产生鼠标事件时的 x 坐标
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 获取鼠标在产生鼠标事件时的 y 坐标
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// 地图鼠标移动事件构造函数
        /// </summary>
        /// <param name="position">经纬度</param>
        /// <param name="button">鼠标按键</param>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        public MFMouseEventArgs(MapLngLat position, MouseButtons button, int x, int y)
        {
            Position = position;
            Button = button;
            Location = new Point(x, y);
            X = x;
            Y = y;
        }
    }
}
