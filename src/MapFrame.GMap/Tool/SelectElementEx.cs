/**************************************************************************
 * 类名：SelectElementEx.cs
 * 描述：目标框选
 * 作者：Allen
 * 日期：Aug 12,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 目标框选
    /// </summary>
    class SelectElementEx : IMFTool, IMFSelect
    {
        /// <summary>
        /// 选择完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 地图逻辑
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 被选择图元集合
        /// </summary>
        private List<IMFElement> elementList = null;
        /// <summary>
        /// 是否是多选
        /// </summary>
        private bool bIsMultiSelect = false;
        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool bIsLeftButtonDown = false;
        /// <summary>
        /// 当前鼠标点击的第一个点位置
        /// </summary>
        private MapLngLat currentPoint = null;
        /// <summary>
        /// 矩形的点集合
        /// </summary>
        private List<MapLngLat> pointList = null;
        /// <summary>
        /// 绘制的矩形
        /// </summary>
        private IMFPolygon polygonElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        /// <param name="_mapLogic">地图逻辑控制类</param>
        public SelectElementEx(GMapControl _mapControl, IMapLogic _mapLogic)
        {
            gmapControl = _mapControl;
            mapLogic = _mapLogic;
            elementList = new List<IMFElement>();

            pointList = new List<MapLngLat>();
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            layer = mapLogic.AddLayer("select_draw_layer");
            gmapControl.SetCursor(Cursors.Cross);
            Utils.bPublishEvent = false;
            gmapControl.OnMarkerClick += gmapControl_OnMarkerClick;
            gmapControl.KeyDown += gmapControl_KeyDown;
            gmapControl.KeyUp += gmapControl_KeyUp;

            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.MouseUp += gmapControl_MouseUp;
            gmapControl.DoubleClick += gmapControl_DoubleClick;
        }


        /// <summary>
        /// 释放工具命令
        /// </summary>
        public void ReleaseCommond()
        {
            mapLogic.RemoveLayer(layer);
            if (elementList != null)
            {
                foreach (var item in elementList)
                {
                    item.HightLight(false);
                }
            }
            if (gmapControl != null)
            {
                gmapControl.CanDragMap = true;
                gmapControl.SetCursor(Cursors.Default);
                gmapControl.OnMarkerClick -= gmapControl_OnMarkerClick;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                gmapControl.KeyUp -= gmapControl_KeyUp;

                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.MouseUp -= gmapControl_MouseUp;
                gmapControl.DoubleClick -= gmapControl_DoubleClick;
            }

            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 获取选择的图元
        /// </summary>
        /// <returns></returns>
        public List<IMFElement> GetSelectElements()
        {
            return elementList;
        }

        // 鼠标按下事件，
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bIsLeftButtonDown = true;
                if (bIsMultiSelect == false) return;

                PointLatLng point = gmapControl.FromLocalToLatLng(e.X, e.Y);
                currentPoint = new MapLngLat(point.Lng, point.Lat);
                pointList[0] = currentPoint;
                Kml polygonKml = new Kml();
                polygonKml.Placemark.Name = "select_rect";

                List<MapLngLat> pList = new List<MapLngLat>();
                pList.Add(new MapLngLat(point.Lng, point.Lat));
                pList.Add(new MapLngLat(point.Lng, point.Lat));
                pList.Add(new MapLngLat(point.Lng, point.Lat));
                pList.Add(new MapLngLat(point.Lng, point.Lat));

                polygonKml.Placemark.Graph = new KmlPolygon()
                {
                    Description = "框框",
                    FillColor = Color.FromArgb(20, Color.Blue),
                    OutLineColor = Color.Blue,
                    PositionList = pList
                };
                IMFElement element = null;
                bool f = layer.AddElement(polygonKml, out element);
                polygonElement = element as IMFPolygon;
            }
        }

        /// <summary>
        /// 双击完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            ReleaseCommond();
        }

        // 鼠标移动事件，主要完成矩形绘制
        private void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (bIsLeftButtonDown == false || bIsMultiSelect == false) return;   // 如果鼠标左键和Ctrl没有按下，则不执行操作
            if (currentPoint == null) return;

            PointLatLng point = gmapControl.FromLocalToLatLng(e.X, e.Y);
            //右上角
            MapLngLat p1 = new MapLngLat(point.Lng, currentPoint.Lat);
            //右下角
            MapLngLat p2 = new MapLngLat(point.Lng, point.Lat);
            //左下角
            MapLngLat p3 = new MapLngLat(currentPoint.Lng, point.Lat);

            pointList[1] = p1;
            pointList[2] = p2;
            pointList[3] = p3;
            //修改其他三个点的位置

            if (polygonElement != null)
                polygonElement.UpdatePosition(pointList);
        }

        // 鼠标起来事件
        private void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            bIsLeftButtonDown = false;

            if (polygonElement == null) return;

            GMapPolygon rectPolygon = polygonElement as GMapPolygon;
            List<GPoint> gpointList = rectPolygon.LocalPoints;
            if (gpointList.Count == 0) return;

            GPoint p0 = gpointList[0];
            GPoint p1 = gpointList[1];
            GPoint p2 = gpointList[2];
            GPoint p3 = gpointList[3];

            Point leftUpPoint = new Point((int)p0.X, (int)p0.Y);
            int width = 0;
            int height = 0;
            width = Math.Abs((int)p1.X - (int)p0.X);
            height = Math.Abs((int)p2.Y - (int)p1.Y);
            Rectangle rect = new Rectangle(leftUpPoint, new Size(width, height));

            // 框选需要清除上一次的图元
            foreach (IMFElement ele in elementList)
            {
                ele.HightLight(false);
            }
            elementList.Clear();

            foreach (IMFLayer maplayer in mapLogic.GetLayers())
            {
                foreach (IMFElement element in maplayer.GetElementList())
                {
                    if (element.IsHightLight)
                        continue;

                    GMapMarker marker = element as GMapMarker;
                    if (marker == null) continue;

                    Point tmpPoint = marker.LocalPosition;
                    if (rect.Contains(tmpPoint))
                    {
                        element.HightLight(true);
                        elementList.Add(element);
                    }
                }
            }

            layer.RemoveElement("select_rect");
            RegistCommondExcuteEvent();

        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcuteEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "选择工具，返回选中的图元",
                    Data = elementList,
                    ToolType = ToolTypeEnum.Select
                };
                CommondExecutedEvent.Invoke(this, msg);
            }
        }

        // 键盘按下事件
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 17)
            {
                bIsMultiSelect = true;
                gmapControl.CanDragMap = false;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ReleaseCommond();
            }
        }

        // 键盘起来事件
        private void gmapControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 17)
            {
                bIsMultiSelect = false;
                gmapControl.CanDragMap = true;
            }
        }

        // Marker点击事件
        private void gmapControl_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            IMFElement element = item as IMFElement;
            if (element == null) return;

            // 如果已经高亮，那判断是否是框选的高亮，如果是，则取消高亮，从而防止更改外部的高亮操作
            if (element.IsHightLight)
            {
                if (elementList.Contains(element))
                {
                    element.HightLight(false);
                    elementList.Remove(element);
                }
                return;
            }

            if (bIsMultiSelect)   // 多选
            {
                if (!elementList.Contains(element))
                {
                    elementList.Add(element);
                    element.HightLight(true);
                }
            }
            else                  // 单选          
            {
                foreach (IMFElement ele in elementList)
                {
                    ele.HightLight(false);
                }
                elementList.Clear();

                element.HightLight(true);
                elementList.Add(element);
            }

            RegistCommondExcuteEvent();
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
            elementList = null;
            bIsMultiSelect = false;
            bIsLeftButtonDown = false;
            currentPoint = null;
            pointList = null;
            polygonElement = null;
        }
    }
}
