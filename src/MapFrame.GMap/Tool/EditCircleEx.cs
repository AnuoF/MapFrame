using System;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using GMap.NET.WindowsForms;
using System.Windows.Forms;
using MapFrame.GMap.Model;
using GMap.NET;
using MapFrame.Core.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 编辑圆类
    /// </summary>
    class EditCircleEx : IMFTool
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;

        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl mapControl = null;
        /// <summary>
        /// 圆图元
        /// </summary>
        private IMFCircle circleElement = null;
        /// <summary>
        /// 编辑图层
        /// </summary>
        private GMapOverlay overlay = null;
        /// <summary>
        /// 
        /// </summary>
        private EditMarker randomMarker = null;
        /// <summary>
        /// 编辑类型 0 放大缩小  1 拖动
        /// </summary>
        private volatile int editType = -1;
        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool leftDown = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl"></param>
        /// <param name="_element"></param>
        public EditCircleEx(GMapControl _gmapControl, IMFElement _element)
        {
            this.mapControl = _gmapControl;
            this.circleElement = _element as IMFCircle;
            overlay = new GMapOverlay("draw_layer");
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CanDragMap = false;
            mapControl.Overlays.Add(overlay);
            mapControl.OnPolygonEnter += mapControl_OnPolygonEnter;
            mapControl.OnPolygonLeave += mapControl_OnPolygonLeave;
            mapControl.OnMarkerEnter += mapControl_OnMarkerEnter;
            mapControl.OnMarkerLeave += mapControl_OnMarkerLeave;
            mapControl.DoubleClick += mapControl_DoubleClick;
            mapControl.KeyDown += mapControl_KeyDown;
            mapControl.MouseDown += mapControl_MouseDown;
            randomMarker = new EditMarker(Get270RandomPoint());
            overlay.Markers.Add(randomMarker);
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcutedEvent() 
        {
            if (CommondExecutedEvent != null) 
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "编辑圆，返回圆对象",
                    Data = circleElement,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent.Invoke(this, msg);
            }
        }

        /// <summary>
        /// 获取270度点的位置
        /// </summary>
        /// <returns></returns>
        private PointLatLng Get270RandomPoint()
        {
            MapLngLat centerMapPoint = circleElement.GetCenterDot();
            double seg = Math.PI * 270 / 180;
            double a = centerMapPoint.Lng + circleElement.GetRadius() * Math.Cos(seg) / 100000;
            double b = centerMapPoint.Lat + circleElement.GetRadius() * Math.Sin(seg) / 100000;
            PointLatLng randomLatLng = new PointLatLng(b, a);
            return randomLatLng;
        }

        //Esc取消编辑
        private void mapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RegistCommondExcutedEvent();
                ReleaseCommond();
            }
        }

        //双击完成编辑
        private void mapControl_DoubleClick(object sender, EventArgs e)
        {
            RegistCommondExcutedEvent();
            ReleaseCommond();
        }

        //鼠标进入marker事件
        private void mapControl_OnMarkerEnter(GMapMarker item)
        {
            EditMarker marker = item as EditMarker;
            if (!randomMarker.Equals(marker)) return;
            editType = 0;
        }

        //鼠标离开marker事件
        private void mapControl_OnMarkerLeave(GMapMarker item)
        {
            if (!leftDown)
            {
                editType = -1;
            }
        }

        //鼠标进入面事件
        private void mapControl_OnPolygonEnter(GMapPolygon item)
        {
            if (item.Name != this.circleElement.ElementName) return;
            mapControl.SetCursor(Cursors.SizeAll);
            editType = 1;
        }

        //鼠标离开面事件
        private void mapControl_OnPolygonLeave(GMapPolygon item)
        {
            if (item.Name != this.circleElement.ElementName) return;
            mapControl.SetCursor(Cursors.Default);
            if (!leftDown)
            {
                editType = -1;
            }
        }

        //鼠标按下事件
        private void mapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && editType == -1) return;
            leftDown = true;
            mapControl.MouseMove += mapControl_MouseMove;
            mapControl.MouseUp += mapControl_MouseUp;
            mapControl.OnPolygonEnter -= mapControl_OnPolygonEnter;
            mapControl.OnMarkerEnter -= mapControl_OnMarkerEnter;
        }

        //鼠标弹起事件
        private void mapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            mapControl.MouseUp -= mapControl_MouseUp;
            mapControl.MouseMove -= mapControl_MouseMove;
            mapControl.OnPolygonEnter += mapControl_OnPolygonEnter;
            mapControl.OnMarkerEnter += mapControl_OnMarkerEnter;
            editType = -1;
            randomMarker.Position = Get270RandomPoint();
            randomMarker.IsVisible = true;
            leftDown = false;
        }

        //鼠标移动事件
        private void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng pointLatLng = mapControl.FromLocalToLatLng(e.X, e.Y);
            MapLngLat lnglat = new MapLngLat(pointLatLng.Lng, pointLatLng.Lat);
            if (e.Button != MouseButtons.Left) return;
            switch (editType)
            {
                case 0://修改大小
                    randomMarker.IsVisible = true;
                    randomMarker.Position = pointLatLng;
                    double radius = Utils.GetDistance(circleElement.GetCenterDot(), lnglat) * 1000;
                    circleElement.UpdatePosition(radius);
                    break;
                case 1://更新圆心
                    randomMarker.IsVisible = false;
                    circleElement.UpdatePosition(lnglat);
                    break;
            }
        }


        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                mapControl.CanDragMap = true;
                mapControl.OnPolygonEnter -= mapControl_OnPolygonEnter;
                mapControl.OnPolygonLeave -= mapControl_OnPolygonLeave;
                mapControl.OnMarkerEnter -= mapControl_OnMarkerEnter;
                mapControl.OnMarkerLeave -= mapControl_OnMarkerLeave;
                mapControl.MouseDown -= mapControl_MouseDown;
                mapControl.MouseMove -= mapControl_MouseMove;
                mapControl.MouseUp -= mapControl_MouseUp;
                mapControl.DoubleClick -= mapControl_DoubleClick;
                mapControl.KeyDown -= mapControl_KeyDown;
                mapControl.Overlays.Remove(overlay);
                mapControl.Refresh();
                mapControl = null;
            }
        }
        
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            mapControl = null;
            CommondExecutedEvent = null;
            circleElement = null;
            overlay = null;
        }
    }
}
