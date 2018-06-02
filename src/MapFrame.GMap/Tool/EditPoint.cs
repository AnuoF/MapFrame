/**************************************************************************
 * 类名：EditPoint.cs
 * 描述：编辑点图元
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using System;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 编辑点图元
    /// </summary>
    class EditPoint : IMFTool
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 需要编辑的图元对象
        /// </summary>
        private GMapMarker marker = null;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 当前编辑的图元
        /// </summary>
        private IMFElement element = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        /// <param name="_element">图元</param>
        public EditPoint(GMapControl _gmapControl, IMFElement _element)
        {
            gmapControl = _gmapControl;
            marker = _element as GMapMarker;
            element = _element;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            if (marker == null) return;
            element.HightLight(true);
            Utils.bPublishEvent = false;
            gmapControl.OnMarkerEnter += gmapControl_OnMarkerEnter;
            gmapControl.DoubleClick += gmapControl_DoubleClick;
            gmapControl.KeyDown += gmapControl_KeyDown;
        }

        /// <summary>
        /// 鼠标进入目标事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnMarkerEnter(GMapMarker item)
        {
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.OnMarkerLeave += gmapControl_OnMarkerLeave;
        }

        /// <summary>
        /// 鼠标离开目标事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnMarkerLeave(GMapMarker item)
        {
            gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
            gmapControl.MouseDown -= gmapControl_MouseDown;
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (element != null)
            {
                element.HightLight(false);
            }
            if (gmapControl != null)
            {
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseUp -= gmapControl_MouseUp;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.DoubleClick -= gmapControl_DoubleClick;
                gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
                gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                Utils.bPublishEvent = true;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            gmapControl = null;
            marker = null;
            isMouseDown = false;
            element = null;
        }

        // 鼠标移动事件，如果是选中状态下，则拖动图元
        void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown == false) return;

            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            marker.Position = lngLat;
        }

        // 鼠标按下事件
        void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (element.IsHightLight)
            {
                isMouseDown = true;
                gmapControl.MouseMove += gmapControl_MouseMove;
                gmapControl.MouseUp += gmapControl_MouseUp;
            }
        }

        // 鼠标松开事件
        void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseUp -= gmapControl_MouseUp;
        }

        // 鼠标双击，结束此次编辑
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            RegistCommondExcutedEvent();
            ReleaseCommond();
        }

        private void RegistCommondExcutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "编辑点，返回点图元",
                    Data = element,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent.Invoke(element, msg);
            }
        }

        // 鼠标按下事件  上 下 左 右
        void gmapControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            PointLatLng position = marker.Position;
            double step = 0.01;

            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                marker.Position = new PointLatLng(position.Lat + step, position.Lng);
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                marker.Position = new PointLatLng(position.Lat - step, position.Lng);
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Left)
            {
                marker.Position = new PointLatLng(position.Lat, position.Lng - step);
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Right)
            {
                marker.Position = new PointLatLng(position.Lat, position.Lng + step);
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                RegistCommondExcutedEvent();
                ReleaseCommond();
            }
        }

    }
}
