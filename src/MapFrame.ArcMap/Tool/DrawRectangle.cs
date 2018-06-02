/**************************************************************************
 * 类名：DrawRectangle.cs
 * 描述：绘制矩形
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using System.Drawing;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Common;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 绘制矩形
    /// </summary>
    class DrawRectangle : IMFTool, IMFDraw
    {
        /// <summary>
        /// arcgis的地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 图元
        /// </summary>
        private IMFPolygon polygonElement = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> pointList = null;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 是否按下Control键
        /// </summary>
        private bool isControl = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawRectangle(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
            pointList = new List<MapLngLat>();
        }

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取绘制后的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return polygonElement;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = mapLogic.AddLayer("draw_arcLayer");
            mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            RegistEvent();
        }

        /// <summary>
        /// 终止命令
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
        /// 注册绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制矩形",
                    Data = polygonElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
            mapControl.OnMouseUp += mapControl_OnMouseUp;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
        }

        #region  事件

        /// <summary>
        /// 按下Esc  取消绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27) //esc 27
            {
                if (!isFinish)
                {
                    layer.RemoveElement(polygonElement);
                    isMouseDown = false;
                    isFinish = true;
                }
                else
                {
                    ReleaseCommond();
                }
            }
            else if (e.keyCode == 17)
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
        /// 鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (e.button == 1 && !isControl && !isFinish)
            {
                if (pointList[0].Lng == pointList[1].Lng && pointList[0].Lat == pointList[1].Lat)
                {
                    if (polygonElement != null)
                        layer.RemoveElement(polygonElement);
                }
                else
                {
                    isMouseDown = false;
                    isFinish = true;
                    RegistCommondExecutedEvent();
                    pointList.Clear();
                }
                ReleaseCommond();//修改  陈静
            }
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (isMouseDown && !isControl)
            {
                MapLngLat lnglat1 = new MapLngLat(pointList[0].Lng, e.mapY);
                MapLngLat lnglat2 = new MapLngLat(e.mapX, e.mapY);
                MapLngLat lnglat3 = new MapLngLat(e.mapX, pointList[0].Lat);
                pointList[1] = lnglat1;
                pointList[2] = lnglat2;
                pointList[3] = lnglat3;
                polygonElement.UpdatePosition(pointList);
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1 && !isMouseDown && !isControl)
            {
                //绘制矩形  四个点的位置集中在一个点上
                Kml kml = new Kml();
                kml.Placemark.Name = "arc_rectangle" + Utils.ElementIndex;
                KmlPolygon kmlRectangle = new KmlPolygon();
                kmlRectangle.Description = "手动绘制的一个矩形";
                kmlRectangle.FillColor = Color.Yellow;
                kmlRectangle.OutLineColor = Color.Black;
                kmlRectangle.OutLineSize = 1;
                MapLngLat lnglat = new MapLngLat(e.mapX, e.mapY);
                pointList.Add(lnglat);
                pointList.Add(lnglat);
                pointList.Add(lnglat);
                pointList.Add(lnglat);
                kmlRectangle.PositionList = pointList;
                kml.Placemark.Graph = kmlRectangle;
                IMFElement element = null;
                layer.AddElement(kml, out element);
                polygonElement = element as IMFPolygon;
                isMouseDown = true;
                isFinish = false;
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
            layer = null;
            polygonElement = null;
            mapLogic = null;
            pointList = null;
            isMouseDown = false;
            isFinish = false;
            isControl = false;
        }
    }
}
