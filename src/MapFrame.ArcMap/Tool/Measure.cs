/**************************************************************************
 * 类名：MFLayer.cs
 * 描述：图层类
 * 作者：LX
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using MapFrame.Core.Interface;
using ESRI.ArcGIS.Controls;
using MapFrame.ArcMap.Windows;
using MapFrame.Core.Model;
using ESRI.ArcGIS.SystemUI;
using System.Drawing;
using ESRI.ArcGIS.Geometry;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 测量距离、面积
    /// </summary>
    public class Measure : IMFTool
    {
        /// <summary>
        /// 完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 测量显示工具
        /// </summary>
        private MeasureTool measureTool = null;
        /// <summary>
        /// 委托
        /// </summary>
        /// <param name="result"></param>
        public delegate void ResultEvent(string result);
        /// <summary>
        /// 事件
        /// </summary>
        public event ResultEvent ResultEventArgs;
        /// <summary>
        /// 总长度、片段长度
        /// </summary>
        private double toltalLength = 0, segmentLength = 0;
        /// <summary>
        /// 记录上次鼠标点击的坐标
        /// </summary>
        private MapLngLat downPoint = null;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 是否按Control键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 地图逻辑
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// 测量直线图元
        /// </summary>
        private IMFLine measureLine = null;
        /// <summary>
        /// 测量面积图元
        /// </summary>
        private IMFPolygon measurePolygon = null;
        /// <summary>
        /// 面积
        /// </summary>
        private double measureArea = 0;
        /// <summary>
        /// 测量类型
        /// </summary>
        private string measureType = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        /// <param name="_mapLogic">地图逻辑</param>
        public Measure(AxMapControl _mapControl, IMapLogic _mapLogic, string type)
        {
            mapControl = _mapControl;
            mapLogic = _mapLogic;
            measureType = type;
            listMapPoints = new List<MapLngLat>();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            measureTool = new MeasureTool(this);
            mapControl.CreateControl();//强制创建控件
            measureTool.Location = new System.Drawing.Point(0, 0);
            mapControl.Controls.Add(measureTool);
            layer = mapLogic.AddLayer("measure_layer");
            RegistEvent();
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            ICommand tool = new ControlsMapPanToolClass();
            tool.OnCreate(mapControl.Object);
            mapControl.CurrentTool = tool as ITool;
            LogoutEvent();
            measureTool.Dispose();
        }
        /// <summary>
        /// 注册地图事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
        }

        /// <summary>
        /// 注册绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent(double result, string decribe)
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = decribe,
                    Data = result,
                    ToolType = ToolTypeEnum.Measure
                };
                this.CommondExecutedEvent(decribe, msg);
            }
        }

        #region 事件
        /// <summary>
        /// 双击完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                isFinish = true;//绘制完成
                listMapPoints.Clear();//清空
                switch (measureType)
                {
                    case "distance":
                        if (measureLine != null) layer.RemoveElement(measureLine);
                        RegistCommondExecutedEvent(toltalLength, "测量距离");//绘制完成事件
                        toltalLength = 0; segmentLength = 0;
                        break;
                    case "area":
                        if (measurePolygon != null) layer.RemoveElement(measurePolygon);
                        RegistCommondExecutedEvent(measureArea, "测量面积");//绘制完成事件
                        break;
                }
                ReleaseCommond();//修改  陈静
            }
        }

        /// <summary>
        /// 键盘弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            if (e.keyCode == 17)
            {
                isControl = false;//键盘弹起
                mapControl.CurrentTool = null;//将地图的工具设为空
            }
        }

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                if (measureLine != null) layer.RemoveElement(measureLine); toltalLength = 0; segmentLength = 0;
                if (measurePolygon != null) layer.RemoveElement(measurePolygon);
                if (isFinish) ReleaseCommond();
                else
                {
                    ResultEventArgs(" ");
                    isFinish = true;
                }
            }
            if (e.keyCode == 17)//空格
            {
                isControl = true;
                ICommand command = new ControlsMapPanToolClass();
                command.OnCreate(mapControl.Object);
                if (command.Enabled)
                {
                    mapControl.CurrentTool = command as ITool;
                }
            }
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (listMapPoints.Count != 0)
            {
                MapLngLat moveLnglat = new MapLngLat(e.mapX, e.mapY);
                if (!isControl)//若没有按下空格
                {
                    listMapPoints.Add(moveLnglat);

                    switch (measureType)
                    {
                        case "distance":
                            measureLine.UpdatePosition(listMapPoints);
                            segmentLength = MapFrame.Core.Common.Utils.GetDistance(downPoint, moveLnglat);
                            toltalLength += segmentLength;
                            ResultEventArgs(string.Format("当前线段长度为:{0} 千米 \n总线段长度为:{1}千米", segmentLength, toltalLength));
                            toltalLength -= segmentLength;
                            break;
                        case "area":

                            measurePolygon.UpdatePosition(listMapPoints);
                            IPolygon polygon = new PolygonClass();
                            IGeometry geometry = null;
                            ITopologicalOperator topo = null;
                            IPointCollection pointCollec = new PolygonClass();
                            for (int i = 0; i < listMapPoints.Count; i++)
                            {
                                pointCollec.AddPoint(new PointClass() { X = listMapPoints[i].Lng, Y = listMapPoints[i].Lat });
                            }
                            polygon = pointCollec as IPolygon;
                            if (polygon != null)
                            {
                                polygon.Close();
                                geometry = polygon as IGeometry;
                                topo = geometry as ITopologicalOperator;
                                topo.Simplify();
                                geometry.Project(mapControl.Map.SpatialReference);
                                IArea area = geometry as IArea;

                                if (area != null)
                                {
                                    measureArea = area.Area;
                                    ResultEventArgs(string.Format("面积为:{0} 万平方千米", measureArea));
                                    polygon = null;
                                }
                            }
                            break;
                    }
                    listMapPoints.Remove(moveLnglat);
                }
            }
        }

        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                downPoint = new MapLngLat(e.mapX, e.mapY);
                if (listMapPoints.Count == 0)
                {
                    ResultEventArgs(" ");
                    listMapPoints.Add(downPoint);
                    switch (measureType)
                    {
                        case "distance":
                            Kml kmlLine = new Kml();
                            kmlLine.Placemark.Name = "mea_line";

                            KmlLineString line = new KmlLineString();
                            line.PositionList = listMapPoints;
                            line.Color = Color.Gray;
                            line.Width = 2;
                            kmlLine.Placemark.Graph = line;
                            IMFElement element = null;
                            layer.AddElement(kmlLine, out element);
                            measureLine = element as IMFLine;//绘制完成后得到该图元
                            isFinish = false;
                            break;
                        case "area":
                            Kml kml = new Kml();
                            kml.Placemark.Name = "mea_arcPolygon";
                            Color outlineColor = Color.Blue;
                            Color fillColor = Color.Black;
                            kml.Placemark.Graph = new KmlPolygon() { FillColor = fillColor, OutLineColor = outlineColor, OutLineSize = 1, PositionList = listMapPoints };
                            IMFElement element1 = null;
                            layer.AddElement(kml, out element1);
                            measurePolygon = element1 as IMFPolygon;
                            isFinish = false;
                            break;
                    }
                }
                else
                {
                    listMapPoints.Add(downPoint);
                }

                if (segmentLength != 0)
                {
                    //长度
                    toltalLength += segmentLength;
                }
            }
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            mapControl = null;
            mapLogic = null;
            if (measureTool != null) measureTool.Dispose();
            downPoint = null;
            listMapPoints = null;
            toltalLength = 0;
            segmentLength = 0;
            measureArea = 0;
            measureLine = null;
            measurePolygon = null;
            isFinish = false;
            isControl = false;
            measureType = string.Empty;
            CommondExecutedEvent = null;
        }
    }
}
