/**************************************************************************
 * 类名：DrawCircle.cs
 * 描述：绘制圆
 * 作者：LX
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Common;
using System.Drawing;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 绘制圆
    /// </summary>
    class DrawCircle : IMFTool, IMFDraw
    {
        /// <summary>
        /// 绘制完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// arcgis的地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 鼠标是否新欢
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 是否完成绘制
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 图层集合管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 圆对象
        /// </summary>
        private IMFCircle circleElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private MapLngLat centerDot = null;
        /// <summary>
        /// 是否按住Control
        /// </summary>
        private bool isControl = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapControl"></param>
        public DrawCircle(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
        }

        #region 命令
        /// <summary>
        /// 执行命令
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
        /// 注册绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制圆",
                    Data = circleElement,
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
            mapControl.OnMouseUp += mapControl_OnMouseUp;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
        }

        #endregion

        #region 事件
        /// <summary>
        /// 键盘按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                if (!isFinish)
                {
                    layer.RemoveElement(circleElement);
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
            if (e.button == 1)
            {
                if (centerDot.Lng == e.mapX && centerDot.Lat == e.mapY)
                {
                    if (circleElement != null)
                        layer.RemoveElement(circleElement);
                }
                if (!isControl && !isFinish)
                {
                    isMouseDown = false;
                    isFinish = true;
                    RegistCommondExecutedEvent();
                    ReleaseCommond();//修改  陈静
                }
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
                double circleRadius = MapFrame.Core.Common.Utils.GetDistance(centerDot, new MapLngLat(e.mapX, e.mapY));
                circleElement.UpdatePosition(circleRadius);
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
                centerDot = new MapLngLat() { Lng = e.mapX, Lat = e.mapY };
                Kml kml = new Kml();
                kml.Placemark.Name = "arc_circle" + Utils.ElementIndex;
                KmlCircle circle = new KmlCircle();
                circle.Position = centerDot;
                circle.Radius = 0.1;
                circle.StrokeColor = Color.Yellow;
                circle.StrokeWidth = 2;
                circle.FillColor = System.Drawing.Color.Red;
                kml.Placemark.Graph = circle;
                IMFElement element = null;
                layer.AddElement(kml, out element);
                circleElement = element as IMFCircle;
                isMouseDown = true;
                isFinish = false;
            }
        }

        #endregion

        #region IMFDraw
        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 返回绘制的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return circleElement;
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
            centerDot = null;
            circleElement = null;
            isFinish = false;
            isMouseDown = false;
            isControl = false;
            layer = null;
        }
    }
}
