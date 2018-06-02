/**************************************************************************
 * 类名：DrawText.cs
 * 描述：绘制文字
 * 作者：LX
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Common;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Windows;
using System.Drawing;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 绘制文字
    /// </summary>
    class DrawText : IMFTool, IMFDraw
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 文字控件
        /// </summary>
        private TextInput textCtrl = null;
        /// <summary>
        /// 坐标点
        /// </summary>
        private MapLngLat downPoint = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 文字图元
        /// </summary>
        private IMFText textElement = null;
        /// <summary>
        /// 命令完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 是否按下Control键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 是否按下
        /// </summary>
        private bool isMouseDown = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawText(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
            layer = mapLogic.AddLayer("draw_arcLayer");
        }

        /// <summary>
        /// 释放工具命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;

                mapControl.OnMouseDown -= mapControl_OnMouseDown;
                textCtrl = null;
                mapControl.OnKeyDown -= mapControl_OnKeyDown;
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
                    Describe = "手动绘制线，返回线对象",
                    Data = textElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1 && !isControl && !isMouseDown)
            {
                textCtrl = new TextInput();
                textCtrl.Location = new Point(e.x, e.y);
                mapControl.CreateControl();//强制创建控件
                mapControl.Controls.Add(textCtrl);
                textCtrl.InputFinished += InputFinish;
                downPoint = new MapLngLat() { Lng = e.mapX, Lat = e.mapY };
                isMouseDown = true;
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
                ReleaseCommond();
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
        /// 文字输入完成
        /// </summary>
        /// <param name="context">输入的文本内容</param>
        /// <param name="font">文字字体</param>
        /// <param name="color">字体颜色</param>
        /// <param name="esc">是否取消编辑</param>
        private void InputFinish(string context, Font font, Color color, bool esc)
        {
            if (!esc)
            {
                if (!string.IsNullOrEmpty(context))
                {
                    Kml kml = new Kml();
                    kml.Placemark.Name = "arc_text" + Utils.ElementIndex;
                    kml.Placemark.Graph = new KmlText() { Position = downPoint, Size = font.Size, Content = context, Font = font.Name, Color = color, FontStyle = font.Style };
                    IMFElement element = null;
                    layer.AddElement(kml, out element);
                    textElement = element as IMFText;
                }
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
            }
            else
            {
                isMouseDown = false;
            }
        }

        /// <summary>
        /// 图层集合管理对象
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取所绘制的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return textElement;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            mapControl = null;
            textCtrl = null;
            downPoint = null;
            mapLogic = null;
            textElement = null;
            isControl = false;
        }

    }
}
