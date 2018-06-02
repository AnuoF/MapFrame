/**************************************************************************
 * 类名：EditPolygonWholeMove.cs
 * 描述：整体拖动面图元
 * 作者：Allen
 * 日期：July 19,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 整体拖动面图元
    /// </summary>
    class EditPolygonWholeMove : ITool, IDisposable
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
        /// 鼠标是否按下
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
        public EditPolygonWholeMove(GMapControl _gmapControl, IElement _element)
        {
            this.gmapControl = _gmapControl;
            this.polygon = _element as GMapPolygon;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            if (polygon == null) return;

            gmapControl.OnPolygonEnter += gmapControl_OnPolygonEnter;
            gmapControl.OnPolygonLeave += gmapControl_OnPolygonLeave;
            gmapControl.MouseUp += gmapControl_MouseUp;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.MouseDoubleClick += gmapControl_MouseDoubleClick;
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            gmapControl.OnPolygonEnter -= gmapControl_OnPolygonEnter;
            gmapControl.OnPolygonLeave -= gmapControl_OnPolygonLeave;
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.MouseDoubleClick -= gmapControl_MouseDoubleClick;
            gmapControl.MouseUp -= gmapControl_MouseUp;
            gmapControl.MouseMove -= gmapControl_MouseMove;

            isMouseDown = false;
            isSelectPolygon = false;
            gmapControl.CanDragMap = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
        }

        // 鼠标按下事件
        void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isMouseDown = true;
                prevPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
            }
        }

        // 鼠标移动事件
        void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown == false || isSelectPolygon == false) return;

            PointLatLng currPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
            CaluPointUpdatePositon(currPoint);   // 移动面图元
        }

        // 鼠标松开事件
        void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;
        }

        // 鼠标双击事件，结束此次编辑
        void gmapControl_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ReleaseCommond();
        }

        // 鼠标进入面图元事件
        void gmapControl_OnPolygonEnter(GMapPolygon item)
        {
            isSelectPolygon = true;
            gmapControl.CanDragMap = false;
            gmapControl.Cursor = Cursors.SizeAll;
        }

        // 鼠标离开图元事件
        void gmapControl_OnPolygonLeave(GMapPolygon item)
        {
            isSelectPolygon = false;
            gmapControl.CanDragMap = true;
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
