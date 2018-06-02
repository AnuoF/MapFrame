/**************************************************************************
 * 类名：EditPolygonAdjustSize.cs
 * 描述：调整面图元大小
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

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 调整面图元大小
    /// </summary>
    class EditPolygonAdjustSize : ITool, IDisposable
    {
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
        /// 鼠标左键是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 是否选中面图元
        /// </summary>
        private bool isSelectPolygon = false;
        /// <summary>
        /// 鼠标第一次按下时的点
        /// </summary>
        private PointLatLng prevPoint;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        /// <param name="_element">图元</param>
        public EditPolygonAdjustSize(GMapControl _gmapControl, IElement _element)
        {
            this.gmapControl = _gmapControl;
            this.polygon = _element as GMapPolygon;
            editMarkerList = new List<EditMarker>();
        }

        /// <summary>
        /// 执行命令：画端点，在移动端点的时候改变多边形的形状
        /// </summary>
        public void RunCommond()
        {
            if (polygon == null) return;

            // 画点、注册事件
            AddMarker();

            gmapControl.OnPolygonEnter += gmapControl_OnPolygonEnter;
            gmapControl.OnPolygonLeave += gmapControl_OnPolygonLeave;
            gmapControl.OnMarkerEnter += gmapControl_OnMarkerEnter;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.MouseUp += gmapControl_MouseUp;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.DoubleClick += gmapControl_DoubleClick;
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            RemoveMarker();

            gmapControl.OnPolygonEnter -= gmapControl_OnPolygonEnter;
            gmapControl.OnPolygonLeave -= gmapControl_OnPolygonLeave;
            gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseUp -= gmapControl_MouseUp;
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.DoubleClick -= gmapControl_DoubleClick;
        }

        void gmapControl_OnMarkerEnter(GMapMarker item)
        {
            currentPoint = item as EditMarker;
            if (currentPoint == null) return;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
        }

        // 鼠标按下事件
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isMouseDown = true;
                prevPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
            }
        }

        // 鼠标移动事件，如果是选中状态下，则拖动图元
        private void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown && currentPoint != null)   // 调整大小
            {
                var lnglat = gmapControl.FromLocalToLatLng(e.X, e.Y);

                int index = polygon.Points.FindIndex(o => o == currentPoint.Position);
                if (index != -1)
                {
                    polygon.Points[index] = lnglat;
                    currentPoint.Position = lnglat;
                    gmapControl.UpdatePolygonLocalPosition(polygon);
                }
            }
            else if (isMouseDown && isSelectPolygon)   // 整体移动
            {
                PointLatLng currPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
                CaluPointUpdatePositon(currPoint);   // 移动面图元

                RemoveMarker();
                AddMarker();
            }
        }

        // 鼠标松开事件
        private void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;
            currentPoint = null;
        }

        // 鼠标双击，结束此次编辑
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            ReleaseCommond();
        }

        // 鼠标进入面图元事件
        void gmapControl_OnPolygonEnter(GMapPolygon item)
        {
            isSelectPolygon = true;
            gmapControl.CanDragMap = false;
        }

        // 鼠标离开图元事件
        void gmapControl_OnPolygonLeave(GMapPolygon item)
        {
            isSelectPolygon = false;
            gmapControl.CanDragMap = true;
        }

        /// <summary>
        /// 添加端点Marker
        /// </summary>
        private void AddMarker()
        {
            editMarkerList.Clear();

            for (int i = 0; i < polygon.Points.Count;i++ )
            {
                EditMarker marker = new EditMarker(polygon.Points[i]);
                marker.Tag = "编辑点" + i;
                gmapControl.Overlays[0].Markers.Add(marker);
                editMarkerList.Add(marker);
            }
        }

        /// <summary>
        /// 移除端点Marker
        /// </summary>
        private void RemoveMarker()
        {
            if (editMarkerList.Count == 0) return;

            foreach (EditMarker marker in editMarkerList)
            {
                gmapControl.Overlays[0].Markers.Remove(marker);
            }
        }

        /// <summary>
        /// 计算各个定点位置并更新图元位置
        /// </summary>
        /// <param name="currPoint">当前点的位置</param>
        private void CaluPointUpdatePositon(PointLatLng currPoint)
        {
            double distance = gmapControl.MapProvider.Projection.GetDistance(prevPoint, currPoint);    // 计算距离
            double bear = gmapControl.MapProvider.Projection.GetBearing(prevPoint, currPoint);   // 计算方位角

            List<PointLatLng> prevList = polygon.Points;
            List<PointLatLng> currList = new List<PointLatLng>();

            for (int i = 0; i < prevList.Count; i++)
            {
                PointLatLng newPoint = GetPointByDistanceAndAngle(distance, prevList[i], bear);

                if (newPoint.Lng > 180 || newPoint.Lng < -180) return;
                if (newPoint.Lat > 90 || newPoint.Lat < -90) return;

                currList.Add(newPoint);
            }

            polygon.Points.Clear();
            polygon.Points.AddRange(currList);
            gmapControl.UpdatePolygonLocalPosition(polygon);
        }

        /// <summary>
        /// 根据距离和方位角获取点
        /// </summary>
        /// <param name="distance">距离</param>
        /// <param name="point">点</param>
        /// <param name="angle">方位角</param>
        /// <returns>点</returns>
        private PointLatLng GetPointByDistanceAndAngle(double distance, PointLatLng point, double angle)
        {
            double lng1 = point.Lng;
            double lat1 = point.Lat;
            // 将距离转换成经度的计算公式
            double lon = lng1 + (distance * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat1 * Math.PI / 180));
            // 将距离转换成纬度的计算公式
            double lat = lat1 + (distance * Math.Cos(angle * Math.PI / 180)) / 111;

            PointLatLng newPoint = new PointLatLng();
            newPoint.Lng = lon;
            newPoint.Lat = lat;

            return newPoint;
        }
    }
}
