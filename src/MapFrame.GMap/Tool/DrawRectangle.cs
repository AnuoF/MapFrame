/**************************************************************************
 * 类名：DrawRectangle.cs
 * 描述：绘制矩形
 * 作者：陈静
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using GMap.NET;
using System.Windows.Forms;
using MapFrame.GMap.Common;
using System.Drawing;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 绘制矩形
    /// </summary>
    class DrawRectangle : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 矩形图元对象
        /// </summary>
        private IMFPolygon rectangElement = null;
        /// <summary>
        /// 矩形的点集合
        /// </summary>
        private List<MapLngLat> pointList = null;
        /// <summary>
        /// 当前鼠标点击的第一个点位置
        /// </summary>
        private MapLngLat currentPoint = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 是否按住ctrl按键，绘制正方形
        /// </summary>
        private bool bIsCtrlDown;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 添加图元是否成功
        /// </summary>
        private bool drawn = false;
        /// <summary>
        /// 记录第一次点击的时间
        /// </summary>
        private DateTime fistTimer;

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl"></param>
        public DrawRectangle(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            pointList = new List<MapLngLat>();
            //初始时给矩形四个点先赋值，后面移动的时候再更新位置
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return rectangElement;
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        public void RunCommond()
        {
            gmapControl.CanDragMap = false;
            layer = mapLogic.AddLayer("draw_layer");

            gmapControl.SetCursor(Cursors.Cross);
            Utils.bPublishEvent = false;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.DoubleClick += gmapControl_DoubleClick;
            gmapControl.KeyDown += gmapControl_KeyDown;
            gmapControl.KeyUp += gmapControl_KeyUp;

        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                gmapControl.SetCursor(Cursors.Default);
                gmapControl.CanDragMap = true;
                gmapControl.MouseDoubleClick -= gmapControl_DoubleClick;
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.MouseUp -= gmapControl_MouseUp;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                gmapControl.KeyUp -= gmapControl_KeyUp;
                Utils.bPublishEvent = true;
            }
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommonExcuteEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制矩形，返回矩形对象",
                    Data = rectangElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 画多边形时按下esc取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            bIsCtrlDown = e.Control;
            if (e.KeyCode == Keys.Escape)
            {
                if (rectangElement != null && drawn)
                    layer.RemoveElement(rectangElement);
                else
                    ReleaseCommond();
            }
        }

        private void gmapControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                drawn = false;
            else if (e.KeyCode == (Keys.ShiftKey | Keys.LButton))
            {
                bIsCtrlDown = !e.Control;
                bIsCtrlDown = false;
            }
        }

        /// <summary>
        /// 鼠标双击事件  绘制矩形完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            ReleaseCommond();
        }

        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            TimeSpan span = DateTime.Now - fistTimer;
            if (span.TotalMilliseconds < 170) return;
            fistTimer = DateTime.Now;
            if (e.Clicks == 2 || e.Button != MouseButtons.Left) return;
            PointLatLng point = gmapControl.FromLocalToLatLng(e.X, e.Y);
            currentPoint = new MapLngLat(point.Lng, point.Lat);
            pointList[0] = currentPoint;
            Kml polygonKml = new Kml();
            polygonKml.Placemark.Name = "draw_rectangle" + Utils.ElementIndex;

            List<MapLngLat> pList = new List<MapLngLat>();
            pList.Add(new MapLngLat(point.Lng, point.Lat));
            pList.Add(new MapLngLat(point.Lng, point.Lat));
            pList.Add(new MapLngLat(point.Lng, point.Lat));
            pList.Add(new MapLngLat(point.Lng, point.Lat));

            polygonKml.Placemark.Graph = new KmlPolygon()
            {
                Description = "手动绘制矩形",
                PositionList = pList,
                FillColor = Color.FromArgb(155, Color.AliceBlue),
                OutLineColor = Color.FromArgb(155, Color.MidnightBlue)
            };
            IMFElement element = null;
            drawn = layer.AddElement(polygonKml, out element);
            rectangElement = element as IMFPolygon;

            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.MouseUp += gmapControl_MouseUp;
        }

        /// <summary>
        /// 鼠标移动事件  拉大矩形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawn) return;
            PointLatLng point = gmapControl.FromLocalToLatLng(e.X, e.Y);
            //右上角
            MapLngLat p1 = new MapLngLat(0, 0);
            //右下角
            MapLngLat p2 = new MapLngLat(0, 0);
            //左下角
            MapLngLat p3 = new MapLngLat(0, 0);

            if (bIsCtrlDown) //画正方形
            {
                double x = point.Lng - currentPoint.Lng;
                double y = point.Lat - currentPoint.Lat;
                if (x > y)
                {
                    pointList[1].Lng = currentPoint.Lng + y;
                    pointList[1].Lat = currentPoint.Lat;
                    pointList[2].Lng = currentPoint.Lng + y;
                    pointList[2].Lat = point.Lat;
                    pointList[3].Lng = currentPoint.Lng;
                    pointList[3].Lat = currentPoint.Lat + y;
                }
                else
                {
                    pointList[1].Lng = currentPoint.Lng + x;
                    pointList[1].Lat = currentPoint.Lat;
                    pointList[2].Lng = point.Lng;
                    pointList[2].Lat = currentPoint.Lat + x;
                    pointList[3].Lng = currentPoint.Lng;
                    pointList[3].Lat = pointList[1].Lat;
                }
            }
            else      //画矩形 
            {
                pointList[1].Lng = point.Lng;
                pointList[1].Lat = currentPoint.Lat;
                pointList[2].Lng = point.Lng;
                pointList[2].Lat = point.Lat;
                pointList[3].Lng = currentPoint.Lng;
                pointList[3].Lat = point.Lat;
            }

            //修改其他三个点的位置
            rectangElement.UpdatePosition(pointList);
        }

        /// <summary>
        /// 鼠标松开事件  完成一次绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseUp(object sender, MouseEventArgs e)
        {
            gmapControl.MouseUp -= gmapControl_MouseUp;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            drawn = false;

            PointLatLng point = gmapControl.FromLocalToLatLng(e.X, e.Y);
            if (currentPoint.Lng == point.Lng && currentPoint.Lat == point.Lat)
                layer.RemoveElement(rectangElement);
            else
            {
                RegistCommonExcuteEvent();
                ReleaseCommond();//修改  陈静
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
            pointList = null;
            currentPoint = null;
            mapLogic = null;
            layer = null;
        }
    }
}
