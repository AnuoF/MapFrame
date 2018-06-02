/**************************************************************************
 * 类名：DrawLine.cs
 * 描述：绘制折线
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/


using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Controls;
using System.Drawing;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Common;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 绘制折线
    /// </summary>
    class DrawLine : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// ArcMap地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 是否完成绘制
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 线图元对象
        /// </summary>
        private IMFLine lineElement = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// 鼠标按下的坐标点(求长度的时候用到)
        /// </summary>
        private MapLngLat downPoint = null;
        /// <summary>
        /// 是否Control键
        /// </summary>
        private bool isControl = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapControl">arcmap地图控件</param>
        public DrawLine(AxMapControl _mapControl)
        {
            this.mapControl = _mapControl;
            listMapPoints = new List<MapLngLat>();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            mapControl.CurrentTool = null;
            //添加图层
            layer = mapLogic.AddLayer("draw_arcLayer");
            RegistEvent();
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;
                LogoutEvent();
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
                    Describe = "手动绘制线",
                    Data = lineElement,
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
            mapControl.OnMouseDown += axMapControl_OnMouseDown;
            mapControl.OnKeyDown += axMapControl_OnKeyDown;
            mapControl.OnKeyUp += axMapControl_OnKeyUp;
            mapControl.OnDoubleClick += axMapControl_OnDoubleClick;
            mapControl.OnMouseMove += axMapControl_OnMouseMove;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnDoubleClick -= axMapControl_OnDoubleClick;
            mapControl.OnKeyDown -= axMapControl_OnKeyDown;
            mapControl.OnMouseDown -= axMapControl_OnMouseDown;
            mapControl.OnKeyUp -= axMapControl_OnKeyUp;
            mapControl.OnMouseMove -= axMapControl_OnMouseMove;
        }

        #region  事件

        /// <summary>
        /// 鼠标移动事件  实时绘制线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (listMapPoints.Count != 0 && !isControl)
            {
                if (!isControl)//若没有按下空格
                {
                    MapLngLat moveLnglat = new MapLngLat(e.mapX, e.mapY);
                    listMapPoints.Add(moveLnglat);
                    lineElement.UpdatePosition(listMapPoints);
                    listMapPoints.Remove(moveLnglat);
                }
            }
        }

        /// <summary>
        /// 鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            if (e.keyCode == 17)
            {
                isControl = false;//键盘弹起
                mapControl.CurrentTool = null;//将地图的工具设为空
            }
        }

        /// <summary>
        /// 双击完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                if (listMapPoints.Count == 1)//避免相同的点重复点击
                {
                    if (lineElement != null)
                    {
                        layer.RemoveElement(lineElement);
                    }
                }
                else
                {
                    isFinish = true;//绘制完成
                    listMapPoints.Clear();//清空

                    RegistCommondExecutedEvent();//绘制完成事件
                }

                ReleaseCommond();
            }
        }

        /// <summary>
        /// 按下Esc停止绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                if (!isFinish)
                {
                    layer.RemoveElement(lineElement);
                    listMapPoints.Clear();
                    isFinish = true;
                }
                else
                {
                    ReleaseCommond();
                }
            }
            if (e.keyCode == 17)
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
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                downPoint = new MapLngLat(e.mapX, e.mapY);
                if (listMapPoints.Count == 0)
                {
                    listMapPoints.Add(downPoint);

                    Kml kmlLine = new Kml();
                    kmlLine.Placemark.Name = "arc_line" + Utils.ElementIndex;

                    KmlLineString line = new KmlLineString();
                    line.PositionList = listMapPoints;
                    line.Color = Color.Gray;
                    line.Width = 2;
                    kmlLine.Placemark.Graph = line;
                    IMFElement element = null;
                    layer.AddElement(kmlLine, out element);
                    lineElement = element as IMFLine;//绘制完成后得到该图元
                    isFinish = false;
                }
                //若重复点击同一个点则不添加
                else if (listMapPoints.Find(p => p.Lng == downPoint.Lng && p.Lat == downPoint.Lat) == null)
                {
                    listMapPoints.Add(downPoint);
                }
            }
        }

        #endregion

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
            return lineElement;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();

            CommondExecutedEvent = null;
            mapLogic = null;
            mapControl = null;
            lineElement = null;
            listMapPoints = null;
            isControl = false;
            isFinish = false;
        }
    }
}
