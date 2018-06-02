/**************************************************************************
 * 类名：EditPolygon.cs
 * 描述：编辑面图元
 * 作者：Allen
 * 日期：July 19,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.GMap.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 编辑面图元
    /// </summary>
    public class EditPolygon : IMFTool
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// GMap地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 待编辑的面图元
        /// </summary>
        private GMapPolygon polygon = null;
        /// <summary>
        /// 端点集合
        /// </summary>
        private List<EditMarker> editMarkerList = null;
        /// <summary>
        /// 当前编辑的点
        /// </summary>
        private EditMarker currentPoint = null;
        /// <summary>
        /// 编辑时用的图层
        /// </summary>
        private GMapOverlay overlay = null;
        /// <summary>
        /// 当前编辑的图元
        /// </summary>
        private IMFElement element = null;
        /// <summary>
        /// 鼠标第一次按下时的点
        /// </summary>
        private PointLatLng prevPoint;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        /// <param name="_element">图元</param>
        public EditPolygon(GMapControl _gmapControl, IMFElement _element)
        {
            this.gmapControl = _gmapControl;
            this.element = _element;
            this.polygon = _element as GMapPolygon;
            editMarkerList = new List<EditMarker>();
        }

        /// <summary>
        /// 执行命令：画端点，在移动端点的时候改变多边形的形状
        /// </summary>
        public void RunCommond()
        {
            if (polygon == null) return;
            overlay = new GMapOverlay("draw_layer");
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                EditMarker marker = new EditMarker(polygon.Points[i]);
                marker.Tag = "编辑点" + i;
                overlay.Markers.Add(marker);
                gmapControl.UpdateMarkerLocalPosition(marker);
            }

            gmapControl.Overlays.Add(overlay);
            Utils.bPublishEvent = false;
            gmapControl.OnPolygonEnter += gmapControl_OnPolygonEnter;
            gmapControl.OnPolygonLeave += gmapControl_OnPolygonLeave;
            gmapControl.OnMarkerEnter += gmapControl_OnMarkerEnter;
            gmapControl.DoubleClick += gmapControl_DoubleClick;
            gmapControl.KeyDown += new KeyEventHandler(gmapControl_KeyDown);
            gmapControl.MouseDown += gmapControl_MouseDown;
        }

        /// <summary>
        /// 按下esc取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ReleaseCommond();
                RegistCommonExcutedEvent();
            }
        }

        private void RegistCommonExcutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "编辑面，返回面图元",
                    Data = element,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent.Invoke(element, msg);
            }
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl == null) return;
            if (gmapControl.Overlays.Contains(overlay))
            {
                overlay.Clear();
                gmapControl.Overlays.Remove(overlay);
            }

            gmapControl.OnPolygonEnter -= gmapControl_OnPolygonEnter;
            gmapControl.OnPolygonLeave -= gmapControl_OnPolygonLeave;
            gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
            gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseUp -= gmapControl_MouseUp;
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.DoubleClick -= gmapControl_DoubleClick;
            gmapControl.MouseMove -= gmapControl_MouseMovePoint;
            gmapControl.MouseUp -= gmapControl_MouseUpPoint;
            gmapControl.MouseDown -= gmapControl_MouseDownPoint;
            gmapControl.KeyDown -= gmapControl_KeyDown;
            Utils.bPublishEvent = true;
        }

        #region  点编辑

        /// <summary>
        /// 编辑点的进入事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnMarkerEnter(GMapMarker item)
        {
            gmapControl.CanDragMap = false;
            if (item.Overlay.Id == "draw_layer")
            {
                currentPoint = item as EditMarker;
                gmapControl.MouseDown += gmapControl_MouseDownPoint;
                gmapControl.OnMarkerLeave += gmapControl_OnMarkerLeave;
            }
        }

        /// <summary>
        /// 点离开事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnMarkerLeave(GMapMarker item)
        {
            gmapControl.CanDragMap = true;
            gmapControl.MouseDown -= gmapControl_MouseDownPoint;
            gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
        }

        // 点的鼠标按下事件
        private void gmapControl_MouseDownPoint(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            gmapControl.MouseMove += gmapControl_MouseMovePoint;
            gmapControl.MouseUp += gmapControl_MouseUpPoint;
        }

        // 点的鼠标移动事件，如果是选中状态下，则拖动图元
        private void gmapControl_MouseMovePoint(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //面移动
            var lnglat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            int index = polygon.Points.FindIndex(o => o == currentPoint.Position);
            if (index != -1)
            {
                polygon.Points[index] = lnglat;
            }
            //点移动
            currentPoint.Position = lnglat;
            gmapControl.UpdatePolygonLocalPosition(polygon);
        }

        // 点的鼠标松开事件
        private void gmapControl_MouseUpPoint(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            gmapControl.MouseDown -= gmapControl_MouseDownPoint;
            gmapControl.MouseMove -= gmapControl_MouseMovePoint;
            gmapControl.MouseUp -= gmapControl_MouseUpPoint;
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            gmapControl = null;
            polygon = null;
            editMarkerList = null;
            currentPoint = null;
            overlay = null;
            element = null;
        }

        #region  面移动

        bool isP;

        // 鼠标进入面图元事件
        void gmapControl_OnPolygonEnter(GMapPolygon item)
        {
            isP = true;
            gmapControl.Cursor = Cursors.SizeAll;
            gmapControl.CanDragMap = false;
        }

        // 鼠标离开图元事件
        void gmapControl_OnPolygonLeave(GMapPolygon item)
        {
            isP = false;
            gmapControl.Cursor = Cursors.Default;
            gmapControl.CanDragMap = true;
        }

        // 鼠标按下事件
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && isP)
            {
                gmapControl.MouseUp += gmapControl_MouseUp;
                gmapControl.MouseMove += gmapControl_MouseMove;
                prevPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
            }
        }

        // 鼠标移动事件，如果是选中状态下，则拖动图元
        private void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (polygon == null) return;
            if (polygon.Points.Count != overlay.Markers.Count)
            {
                ReleaseCommond();
                return;
            }
            var lnglat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                double clng = 0;
                double clat = 0;

                clng = lnglat.Lng - prevPoint.Lng;
                clat = lnglat.Lat - prevPoint.Lat;
                PointLatLng newP1 = new PointLatLng(polygon.Points[i].Lat + clat, polygon.Points[i].Lng + clng);
                polygon.Points[i] = newP1;
                overlay.Markers[i].Position = newP1;
            }
            gmapControl.UpdatePolygonLocalPosition(polygon);
            prevPoint = lnglat;
        }

        // 鼠标松开事件
        private void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            gmapControl.MouseUp -= gmapControl_MouseUp;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            currentPoint = null;
        }

        // 鼠标双击，结束此次编辑
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            ReleaseCommond();
            RegistCommonExcutedEvent();
        }

        #endregion

    }
}
