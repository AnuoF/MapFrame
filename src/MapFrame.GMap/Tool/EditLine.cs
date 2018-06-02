/*************************************************************
 * 类名：EditLine
 * 描述:编辑线图元
 * 创建人：陈静
 * 时间：2016年7月19日15:48:48
 * 修改时间：2016年7月19日15:49:02
 * **********************************************************/

using MapFrame.Core.Interface;
using System;
using System.Collections.Generic;
using GMap.NET.WindowsForms;
using GMap.NET;
using MapFrame.Core.Model;
using MapFrame.GMap.Element;
using System.Windows.Forms;
using MapFrame.GMap.Model;
using MapFrame.Core.Interface;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 编辑线图元
    /// </summary>
    class EditLine : IToolEditLine
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
        /// 需要编辑的线对象
        /// </summary>
        private GMapRoute route = null;
        /// <summary>
        /// 记录鼠标点击时的点位置
        /// </summary>
        private PointLatLng pointLatLng;
        /// <summary>
        /// 当前编辑的图元
        /// </summary>
        private IMFElement element = null;
        /// <summary>
        /// 编辑时用的图层
        /// </summary>
        private GMapOverlay overlay = null;
        /// <summary>
        /// 线的点集合
        /// </summary>
        private List<Point_GMap> pointList = null;
        /// <summary>
        /// 当前操作的点
        /// </summary>
        private EditMarker CurrentPoint = null;
        /// <summary>
        /// 上一次移动的位置
        /// </summary>
        private PointLatLng lastMovePoint;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 是否是编辑测向线
        /// </summary>
        private bool bIsEditDirectionLine = false;
        /// <summary>
        /// 编辑点
        /// </summary>
        private PointLatLng editPointLatLng;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        /// <param name="_element">图元</param>
        public EditLine(GMapControl _gmapControl, IMFElement _element)
        {
            gmapControl = _gmapControl;
            route = _element as GMapRoute;
            element = _element;
            pointList = new List<Point_GMap>();
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        public void RunCommond()
        {
            overlay = new GMapOverlay("edit_layer");
            for (int i = 0; i < route.Points.Count; i++)
            {
                EditMarker marker = new EditMarker(route.Points[i]);
                overlay.Markers.Add(marker);
                gmapControl.UpdateMarkerLocalPosition(marker);
            }
            gmapControl.Overlays.Add(overlay);
            Utils.bPublishEvent = false;
            gmapControl.OnRouteEnter += new RouteEnter(gmapControl_OnRouteEnter);
            gmapControl.DoubleClick += gmapControl_DoubleClick;
            gmapControl.OnMarkerEnter += new MarkerEnter(gmapControl_OnMarkerEnter);
            gmapControl.KeyDown += new KeyEventHandler(gmapControl_KeyDown);
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl == null) return;
            if (gmapControl.Overlays.Contains(overlay))
            {
                overlay.Clear();
                gmapControl.Overlays.Remove(overlay);
            }
            gmapControl.CanDragMap = true;
            gmapControl.OnRouteLeave -= gmapControl_OnRouteLeave;
            gmapControl.OnRouteEnter -= gmapControl_OnRouteEnter;
            gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseUp -= gmapControl_MouseUp;
            gmapControl.MouseDown -= gmapControl_MouseDownPoint;
            gmapControl.DoubleClick -= gmapControl_DoubleClick;
            gmapControl.MouseUp -= gmapControl_MouseUpPoint;
            gmapControl.MouseDown -= gmapControl_MouseDownPoint;
            gmapControl.MouseMove -= gmapControl_MouseMovePoint;
            gmapControl.KeyDown -= gmapControl_KeyDown;
            bIsEditDirectionLine = false;
            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 设置编辑点
        /// </summary>
        /// <param name="editPoint">编辑点</param>
        public void SetEditPoint(MapLngLat editPoint)
        {
            bIsEditDirectionLine = true;
            editPointLatLng = new PointLatLng(editPoint.Lat, editPoint.Lng);

            overlay.Markers.Clear();
            for (int i = 0; i < route.Points.Count; i++)
            {
                // 只添加当前编辑点
                if (editPointLatLng == route.Points[i])
                {
                    EditMarker marker = new EditMarker(route.Points[i]);
                    overlay.Markers.Add(marker);
                    gmapControl.UpdateMarkerLocalPosition(marker);
                    break;
                }
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            gmapControl = null;
            route = null;
            element = null;
            overlay = null;
            pointList = null;
            CurrentPoint = null;
            isMouseDown = false;
            bIsEditDirectionLine = false;
        }

        #region  编辑线

        /// <summary>
        /// 点进入事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnMarkerEnter(GMapMarker item)
        {
            gmapControl.CanDragMap = false;
            if (item.Overlay.Id == overlay.Id)
            {
                CurrentPoint = item as EditMarker;
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
        private void gmapControl_MouseDownPoint(object sender, MouseEventArgs e)
        {
            pointLatLng = gmapControl.FromLocalToLatLng(e.X, e.Y);
            gmapControl.MouseMove += gmapControl_MouseMovePoint;
            gmapControl.MouseUp += gmapControl_MouseUpPoint;
        }

        // 点的鼠标移动事件，如果是选中状态下，则拖动图元
        private void gmapControl_MouseMovePoint(object sender, MouseEventArgs e)
        {
            //线移动
            var lnglat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            int index = route.Points.FindIndex(o => o == CurrentPoint.Position);
            if (index != -1)
            {
                route.Points[index] = lnglat;
            }
            //点移动
            CurrentPoint.Position = lnglat;
            gmapControl.UpdateRouteLocalPosition(route);
        }

        // 点的鼠标松开事件
        private void gmapControl_MouseUpPoint(object sender, MouseEventArgs e)
        {
            gmapControl.MouseDown -= gmapControl_MouseDownPoint;
            gmapControl.MouseMove -= gmapControl_MouseMovePoint;
            gmapControl.MouseUp -= gmapControl_MouseUpPoint;
        }

        #endregion

        #region  整体移动线

        /// <summary>
        /// 线的离开事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnRouteLeave(GMapRoute item)
        {
            if (!isMouseDown)
            {
                gmapControl.Cursor = Cursors.Default;
                gmapControl.CanDragMap = true;
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseUp -= gmapControl_MouseUp;
            }
            gmapControl.OnRouteLeave -= gmapControl_OnRouteLeave;
        }

        /// <summary>
        /// 线的进入事件
        /// </summary>
        /// <param name="item"></param>
        private void gmapControl_OnRouteEnter(GMapRoute item)
        {
            // 如果是编辑测向线，则不能整体移动线条
            if (bIsEditDirectionLine == true) return;

            route = item;
            gmapControl.Cursor = Cursors.SizeAll;
            gmapControl.CanDragMap = false;
            gmapControl.OnRouteLeave += gmapControl_OnRouteLeave;
            if (!isMouseDown)
            {
                gmapControl.MouseDown += gmapControl_MouseDown;
                gmapControl.MouseUp += gmapControl_MouseUp;
            }
        }

        // 鼠标按下事件
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = true;
            pointLatLng = gmapControl.FromLocalToLatLng(e.X, e.Y);
            lastMovePoint = pointLatLng;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.MouseUp += gmapControl_MouseUp;
        }

        // 鼠标松开事件
        private void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.MouseUp -= gmapControl_MouseUp;
        }

        // 鼠标移动事件，如果是选中状态下，则拖动图元
        private void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (route != null)
            {
                if (route.Points.Count != overlay.Markers.Count)
                {
                    ReleaseCommond();
                    return;
                }
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
                for (int i = 0; i < route.Points.Count; i++)
                {
                    double clng = 0;
                    double clat = 0;

                    clng = lngLat.Lng - lastMovePoint.Lng;
                    clat = lngLat.Lat - lastMovePoint.Lat;

                    PointLatLng newP1 = new PointLatLng(route.Points[i].Lat + clat, route.Points[i].Lng + clng);
                    route.Points[i] = newP1;
                    overlay.Markers[i].Position = newP1;
                }
                lastMovePoint = lngLat;
                gmapControl.UpdateRouteLocalPosition(route);
            }
        }

        #endregion

        /// <summary>
        /// 按下esc取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RegistCommonExcutedEvent();
                ReleaseCommond();
            }
        }

        //双击完成
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            RegistCommonExcutedEvent();
            ReleaseCommond();
        }

        private void RegistCommonExcutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "编辑线，返回线图元",
                    Data = element,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent(this, msg);
            }
        }

    }
}
