/**************************************************************************
 * 类名：IMFMapControl.cs
 * 描述：地图控件接口
 * 作者：Allen
 * 日期：Dec 2,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 地图控件接口
    /// </summary>
    public interface IMFMap
    {

        #region 鼠标事件
        
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        event EventHandler<MFMouseEventArgs> MouseMoveEvent;

        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        event EventHandler<MFMouseEventArgs> MouseClickEvent;

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        event EventHandler<MFMouseEventArgs> MouseDownEvent;

        /// <summary>
        /// 鼠标起来事件
        /// </summary>
        event EventHandler<MFMouseEventArgs> MouseUpEvent;

        /// <summary>
        /// 鼠标双击事件
        /// </summary>
        event EventHandler<MFMouseEventArgs> MouseDbClickEvent;

        #endregion

        #region 图元事件
        /// <summary>
        /// 鼠标进入图元事件
        /// </summary>
        event EventHandler<MFElementEnterEventArgs> ElementEnterEvent;
        /// <summary>
        /// 鼠标离开图元事件
        /// </summary>
        event EventHandler<MFElementLeaveEventArgs> ElementLeaveEvent;

        /// <summary>
        /// 图元点击事件
        /// </summary>
        event EventHandler<MFElementClickEventArgs> ElementClickEvent;
        #endregion

        #region 控件事件

        event EventHandler<MapZoomChangedEventArgs> MapZoomChangedEvent;

        #endregion

        #region 键盘事件
        /// <summary>
        /// 键盘按键按下事件
        /// </summary>
        event EventHandler<MFKeyEventArgs> KeyDownEvent;

        /// <summary>
        /// 键盘按键弹起事件
        /// </summary>
        event EventHandler<MFKeyEventArgs> KeyUpEvent;

        /// <summary>
        /// 键盘按键事件
        /// </summary>
        event EventHandler<MFKeyPressEventArgs> KeyPressEvent;


        #endregion

        /// <summary>
        /// 地图初始化完成事件
        /// </summary>
        event EventHandler<EventArgs> InitFinishEvent;


        /// <summary>
        /// 地图控件
        /// </summary>
        object MapControl { get; }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapFile"></param>
        void LoadMap(string mapFile);

        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        MapLngLat FromLocalToLngLat(int x, int y);

        /// <summary>
        /// 地理坐标转屏幕坐标
        /// </summary>
        /// <param name="position">地理坐标</param>
        /// <returns>屏幕坐标</returns>
        Point FromLngLatToLocal(MapLngLat position);

        /// <summary>
        /// 地图快照
        /// </summary>
        /// <returns>图片</returns>
        Image Snapshot();

    }
}
