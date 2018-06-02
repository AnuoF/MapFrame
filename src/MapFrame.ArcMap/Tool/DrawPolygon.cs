/**************************************************************************
 * 类名：DrawPolygon.cs
 * 描述：绘制多边形
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Common;
using System.Drawing;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 绘制多边形
    /// </summary>
    class DrawPolygon : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// arcgis的地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        ///图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 多边形图元
        /// </summary>
        private IMFPolygon polygonElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// 是否按下Control键
        /// </summary>
        private bool isControl = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawPolygon(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
            listMapPoints = new List<MapLngLat>();
        }

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return polygonElement;
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = mapLogic.AddLayer("draw_arcLayer");
            RegistEvent();
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                LogoutEvent();
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
        }

        /// <summary>
        /// 注册绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制多边形",
                    Data = polygonElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        #region  事件

        /// <summary>
        /// 画多边形按下esc取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                if (!isFinish)
                {
                    layer.RemoveElement(polygonElement);
                    listMapPoints.Clear();
                    isFinish = true;
                }
                else
                {
                    ReleaseCommond();
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
        /// 空格键弹起事件
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
        /// 鼠标双击完成绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                if (listMapPoints.Count == 1)
                {
                    if (polygonElement != null)
                    {
                        layer.RemoveElement(polygonElement);
                    }
                }
                else
                {
                    isFinish = true;
                    listMapPoints.Clear();//坐标集合清空
                    RegistCommondExecutedEvent();
                }

                ReleaseCommond();//修改  陈静
            }
        }

        /// <summary>
        /// 鼠标移动实时生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (listMapPoints.Count != 0 && !isControl)
            {
                MapLngLat moveLngLat = new MapLngLat() { Lng = e.mapX, Lat = e.mapY };
                listMapPoints.Add(moveLngLat);
                polygonElement.UpdatePosition(listMapPoints);
                listMapPoints.Remove(moveLngLat);
            }
        }

        /// <summary>
        /// 鼠标按下开始绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                MapLngLat lngLat = new MapLngLat(e.mapX, e.mapY);
                if (listMapPoints.Count == 0)
                {
                    listMapPoints.Add(lngLat);
                    Kml kml = new Kml();
                    kml.Placemark.Name = "arc_Polygon" + Utils.ElementIndex;
                    Color outlineColor = Color.Blue;
                    Color fillColor = Color.Black;
                    kml.Placemark.Graph = new KmlPolygon() { FillColor = fillColor, OutLineColor = outlineColor, OutLineSize = 1, PositionList = listMapPoints };
                    IMFElement element = null;
                    layer.AddElement(kml, out element);
                    polygonElement = element as IMFPolygon;
                    isFinish = false;//为不完成状态
                }
                else if (listMapPoints.Find(p => p.Lng == e.mapX && p.Lat == e.mapY) == null)
                {
                    listMapPoints.Add(lngLat);
                }
            }
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            mapControl = null;
            mapLogic = null;
            listMapPoints = null;
            isControl = false;
            isFinish = false;
            layer = null;
        }

    }
}