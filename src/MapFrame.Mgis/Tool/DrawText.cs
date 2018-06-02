using System;
using MapFrame.Core.Interface;
using AxHOSOFTMapControlLib;
using MapFrame.Mgis.Windows;
using MapFrame.Core.Model;
using MapFrame.Mgis.Common;
using System.Drawing;

namespace MapFrame.Mgis.Tool
{
    /// <summary>
    /// MGis文字图元
    /// </summary>
    class DrawText : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 输入文字控件
        /// </summary>
        private TextInput textCtrl = null;
        /// <summary>
        /// 图层集合管理
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
        /// 临时名称
        /// </summary>
        private string tempName = string.Empty;
        /// <summary>
        /// 文字坐标
        /// </summary>
        private MapLngLat downPoint;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        private bool isControl = false;
        private bool isMouseDown = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawText(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            layer = mapLogic.AddLayer("draw_mgis");
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_NO");
            RegistEvent();
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            LogoutEvent();

            if (textCtrl != null)
            {
                textCtrl.Dispose();
            }
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
        }

        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动输入文字，返回文字对象",
                    Data = textElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent("绘制文字", msg);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.eventLButtonDown += mapControl_eventLButtonDown;
            mapControl.eventKeyDown += mapControl_eventKeyDown;
            mapControl.eventKeyUp += mapControl_eventKeyUp;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventKeyDown -= mapControl_eventKeyDown;
            mapControl.eventKeyUp -= mapControl_eventKeyUp; 
        }

        /// <summary>
        /// 开始绘制文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            //try
            //{
            //    textCtrl.Location = new Point(e.x, e.y);
            //    mapControl.CreateControl();
            //    mapControl.Controls.Add(textCtrl);
            //    downPoint = new MapLngLat(e.dLong, e.dLat);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            if (!isControl && !isMouseDown)
            {
                textCtrl = new TextInput();
                textCtrl.InputFinished += InputFinish;
                textCtrl.Location = new Point(e.x, e.y);
                //mapControl.CreateControl();
                mapControl.Controls.Add(textCtrl);
                //textCtrl.Show();
                downPoint = new MapLngLat(e.dLong, e.dLat);
                isMouseDown = true;
            }

            textCtrl.SetTextFocucs();
        }

        /// <summary>
        /// 键盘弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventKeyUp(object sender, _DHOSOFTMapControlEvents_eventKeyUpEvent e)
        {
            if (e.nChar == 17)
            {
                isControl = false;
                mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_NO");
            }
        }

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke(new Action(delegate
                {
                    if (e.nChar == 27)
                    {
                        ReleaseCommond();
                    }
                    else if (e.nChar == 17)
                    {
                        isControl = true;
                        mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
                    }
                }));
            }
            else
            {
                if (e.nChar == 27)
                {
                    ReleaseCommond();
                }
                else if (e.nChar == 17)
                {
                    isControl = true;
                    mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
                }
            }
            //if (e.nChar == 27)
            //{
            //    ReleaseCommond();
            //}
            //else if (e.nChar == 17)
            //{
            //    isControl = true;
            //    mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
            //}
        }
       

        /// <summary>
        /// 完成事件
        /// </summary>
        /// <param name="context">文字内容</param>
        /// <param name="color">颜色</param>
        /// <param name="esc">是否绘制</param>
        private void InputFinish(string context, Color color, bool esc)
        {
            if (esc)
            {
                if (!string.IsNullOrEmpty(context))
                {
                    Kml kml = new Kml();
                    KmlText text = new KmlText();
                    text.Content = context;
                    text.Position = downPoint;
                    text.Size = (float)0.5;
                    text.Color = color;
                    kml.Placemark.Name = "mgis_Text" + Utils.ElementIndex;
                    kml.Placemark.Graph = text;
                    IMFElement element = null;
                    layer.AddElement(kml, out element);
                    textElement = element as IMFText;
                }
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
            }
            isMouseDown = false;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (textCtrl != null)
                textCtrl.Dispose();
            downPoint = null;
            isControl = true;
            textElement = null;
            mapLogic = null;
            layer = null;
            mapControl = null;
        }

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取绘制的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return textElement;
        }
    }
}
