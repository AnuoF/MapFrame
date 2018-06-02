/**************************************************************************
 * 类名：DrawCircle.cs
 * 描述：绘制圆形图元
 * 作者：Allen
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 绘制圆形图元
    /// </summary>
    class DrawCircle : IMFTool, IMFDraw
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
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private MapLngLat centerPoint = null;
        /// <summary>
        /// 移动坐标
        /// </summary>
        private MapLngLat movePoint = null;
        /// <summary>
        /// 圆图元
        /// </summary>
        private IMFCircle circleElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 是否绘制完成
        /// </summary>
        private bool drawn = false;
        /// <summary>
        /// 地球半径，单位千米
        /// </summary>
        private const double Earth = 6378.137;
        /// <summary>
        /// 半径
        /// </summary>
        private double radius;

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
        public DrawCircle(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            centerPoint = new MapLngLat();
            movePoint = new MapLngLat();
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            gmapControl.CanDragMap = false;
            layer = mapLogic.AddLayer("draw_layer");
            gmapControl.SetCursor(Cursors.Cross);
            Utils.bPublishEvent = false;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.KeyDown += gmapControl_KeyDown;
            gmapControl.KeyUp += gmapControl_KeyUp;
        }

        /// <summary>
        /// 释放工具命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                gmapControl.CanDragMap = true;
                gmapControl.SetCursor(Cursors.Default);
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                gmapControl.KeyUp -= gmapControl_KeyUp;
                gmapControl.MouseUp -= gmapControl_MouseUp;
                Utils.bPublishEvent = true;
            }
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommonExecuteEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制圆，返回圆对象",
                    Data = circleElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
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
            mapLogic = null;
            centerPoint = null;
            movePoint = null;
            circleElement = null;
            layer = null;
        }

        /// <summary>
        /// 获取所画的图元
        /// </summary>
        /// <returns>图元</returns>
        public IMFElement GetDrawElement()
        {
            return circleElement;
        }

        // 键盘按下事件
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (drawn && circleElement != null)
                {
                    layer.RemoveElement(circleElement);
                }
                else
                    ReleaseCommond();
            }
            else if (e.KeyCode == (Keys.LButton | Keys.ShiftKey))
            {
                gmapControl.CanDragMap = true;
            }
        }

        /// <summary>
        /// 添加按下后弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == (Keys.LButton | Keys.ShiftKey))
            {
                gmapControl.CanDragMap = false;
            }
        }

        // 鼠标左键按下
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !gmapControl.CanDragMap)//空格按下后漫游
            {
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
                centerPoint.Lng = lngLat.Lng;
                centerPoint.Lat = lngLat.Lat;
                Kml kml = new Kml();
                kml.Placemark.Name = "draw_circle" + Utils.ElementIndex;

                KmlCircle circleKml = new KmlCircle();
                circleKml.FillColor = Color.FromArgb(50, Color.Blue);
                circleKml.Position = new MapLngLat(lngLat.Lng, lngLat.Lat);
                circleKml.RandomPosition = circleKml.Position;
                circleKml.Radius = 0;
                circleKml.StrokeColor = Color.Gray;
                circleKml.StrokeWidth = 2;
                kml.Placemark.Graph = circleKml;
                IMFElement element = null;
                drawn = layer.AddElement(kml, out element);
                circleElement = element as IMFCircle;

                gmapControl.MouseMove += gmapControl_MouseMove;
                gmapControl.MouseUp += gmapControl_MouseUp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseUp -= gmapControl_MouseUp;
            drawn = false;

            if (centerPoint.Lng == gmapControl.FromLocalToLatLng(e.X, e.Y).Lng && centerPoint.Lat == gmapControl.FromLocalToLatLng(e.X, e.Y).Lat)
            {
                if (circleElement != null)
                    layer.RemoveElement(circleElement);
            }
            else
            {
                RegistCommonExecuteEvent();
                ReleaseCommond();//修改  陈静
            }
        }

        // 移动鼠标实时绘制圆
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawn) return;
            movePoint.Lng = gmapControl.FromLocalToLatLng(e.X, e.Y).Lng;
            movePoint.Lat = gmapControl.FromLocalToLatLng(e.X, e.Y).Lat;
            this.radius = MapFrame.Core.Common.Utils.GetDistance(centerPoint, movePoint) * 1000;
            circleElement.UpdatePosition(this.radius);
        }

        /// <summary>
        /// 根据两点坐标求半径
        /// </summary>
        /// <param name="aroundPoint">圆弧上的一点</param>
        /// <returns>半径，单位米</returns>
        private double GetDistance(PointLatLng aroundPoint)
        {
            PointLatLng lnglat = new PointLatLng(centerPoint.Lat, centerPoint.Lng);
            double distance = gmapControl.MapProvider.Projection.GetDistance(lnglat, aroundPoint);

            return distance * 1000;
        }

        /// <summary>
        /// 弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private double rad(double d)
        {
            return d * Math.PI / 180;
        }



    }
}
