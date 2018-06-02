/**************************************************************************
 * 类名：EditCircle.cs
 * 描述：圆图元编辑
 * 作者：LX
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.GMap.Model;
using GMap.NET;
using System.Collections.Generic;
using System.Windows.Forms;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 圆图元编辑
    /// </summary>
    public class EditCircle : IMFTool
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
        private GMapMarker circleMarker = null;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isLeftButtonDown = false;
        /// <summary>
        /// 当前编辑的点
        /// </summary>
        private EditMarker currentEditPoint = null;
        /// <summary>
        /// 编辑图层
        /// </summary>
        private GMapOverlay overlay = null;
        /// <summary>
        /// 圆图元
        /// </summary>
        private IMFCircle circleElement = null;
        /// <summary>
        /// 编辑点
        /// </summary>
        private List<PointLatLng> listPoints = null;//存储坐标点
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private PointLatLng centerPoint;
        /// <summary>
        /// 半径
        /// </summary>
        private double radius = 0;
        /// <summary>
        /// 当前编辑的点
        /// </summary>
        private int currIndex = -1;
        /// <summary>
        /// 当前编辑类型，0 - 圆；1 - 编辑点
        /// </summary>
        private int editType = -1;
        /// <summary>
        /// OnMarkerEnter事件是否注册
        /// </summary>
        private bool bOnMarkerEnter = false;
        /// <summary>
        /// bOnMarkerLeave事件是否注册
        /// </summary>
        private bool bOnMarkerLeave = false;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        /// <param name="_element">图元</param>
        public EditCircle(GMapControl _gmapControl, IMFElement _element)
        {
            gmapControl = _gmapControl;
            circleMarker = _element as GMapMarker;
            this.circleElement = _element as IMFCircle;
            circleElement = _element as IMFCircle;
            listPoints = new List<PointLatLng>();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            if (circleMarker == null) return;

            // 添加编辑图层
            overlay = new GMapOverlay("edit_layer");
            gmapControl.Overlays.Add(overlay);

            // 获取圆心和半径
            centerPoint = circleMarker.Position;
            radius = circleElement.GetRadius();

            // 添加编辑图元
            AddEditMarker();

            Utils.bPublishEvent = false;
            // 订阅事件
            gmapControl.OnMarkerEnter += gmapControl_OnMarkerEnter;
            gmapControl.DoubleClick += gmapControl_DoubleClick;
            gmapControl.KeyDown += gmapControl_KeyDown;

            gmapControl.OnMarkerLeave += gmapControl_OnMarkerLeave;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.MouseUp += gmapControl_MouseUp;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.OnMarkerClick += gmapControl_OnMarkerClick;

            bOnMarkerEnter = true;
            bOnMarkerLeave = true;
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                gmapControl.OnMarkerClick -= gmapControl_OnMarkerClick;
                gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
                gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
                gmapControl.DoubleClick -= gmapControl_DoubleClick;
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.MouseUp -= gmapControl_MouseUp;
                gmapControl.KeyDown -= gmapControl_KeyDown;

                gmapControl.CanDragMap = true;

                // 移除编辑点
                RemoveEditMarker();
                // 删除图层
                gmapControl.Overlays.Remove(overlay);
            }

            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            gmapControl = null;
            circleMarker = null;
            isLeftButtonDown = false;
            currentEditPoint = null;
            overlay = null;
            circleElement = null;
            listPoints = null;//存储坐标点
        }

        #region 圆编辑

        // 鼠标进入Marker事件
        private void gmapControl_OnMarkerEnter(GMapMarker item)
        {
            gmapControl.CanDragMap = false;

            editType = circleMarker == item ? 0 : 1;
            currentEditPoint = item as EditMarker;

        }

        // 鼠标离开Marker事件
        private void gmapControl_OnMarkerLeave(GMapMarker item)
        {
            gmapControl.CanDragMap = true;
            editType = -1;
        }

        // 鼠标点击Marker事件
        private void gmapControl_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            editType = circleMarker == item ? 0 : 1;
        }

        // 鼠标按下事件
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isLeftButtonDown = true;

            if (currentEditPoint != null)
            {
                currIndex = listPoints.FindIndex(o => o == currentEditPoint.Position);
            }
        }

        // 鼠标起来事件
        private void gmapControl_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftButtonDown = false;

            if (bOnMarkerEnter == false)
            {
                bOnMarkerEnter = true;
                gmapControl.OnMarkerEnter += gmapControl_OnMarkerEnter;
            }
            if (bOnMarkerLeave == false)
            {
                bOnMarkerLeave = true;
                gmapControl.OnMarkerLeave += gmapControl_OnMarkerLeave;
            }
        }

        // 鼠标移动事件
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftButtonDown == false) return;

            if (bOnMarkerEnter == true)
            {
                bOnMarkerEnter = false;
                gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
            }
            if (bOnMarkerLeave == true)
            {
                bOnMarkerLeave = false;
                gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
            }

            if (editType == 0)
            {
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
                circleMarker.Position = lngLat;
                gmapControl.Cursor = Cursors.SizeAll;
                // 实时更新圆心坐标
                centerPoint = circleMarker.Position;
            }
            else if (editType == 1)
            {
                if (currentEditPoint == null || currIndex == -1) return;

                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);

                if (currIndex == 0 || currIndex == 2)
                {
                    lngLat.Lng = currentEditPoint.Position.Lng;
                }
                else if (currIndex == 1 || currIndex == 3)
                {
                    lngLat.Lat = currentEditPoint.Position.Lat;
                }

                currentEditPoint.Position = lngLat;
                double distance = gmapControl.MapProvider.Projection.GetDistance(centerPoint, lngLat) * 1000;
                circleMarker.Position = centerPoint;
                circleElement.UpdatePosition(distance);
                gmapControl.UpdateMarkerLocalPosition(circleMarker);

                // 实时更新半径
                radius = distance;
            }

            RemoveEditMarker();
            AddEditMarker();
        }

        // 鼠标双击，结束此次编辑
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
                    Describe = "编辑圆，返回圆对象",
                    Data = circleElement,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent(this, msg);
            }
        }

        // 按键事件
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                RegistCommonExcutedEvent();
                ReleaseCommond();
            }
        }
        #endregion

        /// <summary>
        /// 添加编辑点
        /// </summary>
        private void AddEditMarker()
        {
            listPoints.Clear();
            for (float ang = 0; ang <= 270; ang += 90)
            {
                var point = GetPointByDistanceAndAngle((float)radius, centerPoint, ang);
                listPoints.Add(point);//添加到集合

                EditMarker editMarker = new EditMarker(point);
                overlay.Markers.Add(editMarker);
                gmapControl.UpdateMarkerLocalPosition(editMarker);
            }
        }

        /// <summary>
        /// 移除编辑点
        /// </summary>
        private void RemoveEditMarker()
        {
            //移除编辑点，删除图层
            if (gmapControl.Overlays.Contains(overlay))
            {
                overlay.Clear();
            }
        }

        /// <summary>
        /// 求坐标点
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private PointLatLng GetPointByDistanceAndAngle(float distance, PointLatLng point, double angle)
        {
            double lng1 = point.Lng;
            double lat1 = point.Lat;
            // 将距离转换成经度的计算公式 * Math.PI / 180
            double lon = lng1 + (distance / 1000 * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat1 * Math.PI / 180));
            // 将距离转换成纬度的计算公式
            double lat = lat1 + (distance / 1000 * Math.Cos(angle * Math.PI / 180)) / 111;

            PointLatLng newPoint = new PointLatLng();
            newPoint.Lng = lon;
            newPoint.Lat = lat;
            return newPoint;
        }

        /// <summary>
        /// 计算宽
        /// </summary>
        /// <returns></returns>
        private int CalcuWidth()
        {
            var point1 = gmapControl.FromLatLngToLocal(listPoints[1]);
            var point2 = gmapControl.FromLatLngToLocal(listPoints[3]);

            return Math.Abs((int)point1.X - (int)point2.X);
        }

        /// <summary>
        /// 计算高
        /// </summary>
        /// <returns></returns>
        private int CalcuHeight()
        {
            var point1 = gmapControl.FromLatLngToLocal(listPoints[0]);
            var point2 = gmapControl.FromLatLngToLocal(listPoints[2]);

            return Math.Abs((int)point1.Y - (int)point2.Y);
        }


    }
}
