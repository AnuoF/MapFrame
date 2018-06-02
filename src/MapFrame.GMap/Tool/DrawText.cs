/**************************************************************************
 * 类名：ToolBox.cs
 * 描述：工具箱
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.GMap.Windows;
using System.Drawing;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;
using GMap.NET;
using System.Windows.Forms;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 添加文字工具
    /// </summary>
    class DrawText : IMFTool, IMFDraw
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 文本输入控件
        /// </summary>
        private TextInput textCtrl = null;
        /// <summary>
        /// 文字的地理位置
        /// </summary>
        private PointLatLng txtPoint;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 文字图元
        /// </summary>
        IMFText textElement = null;
        /// <summary>
        /// 命令完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        public DrawText(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            textCtrl = new TextInput();
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            gmapControl.SetCursor(Cursors.Cross);
            // 订阅事件
            Utils.bPublishEvent = false;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.KeyDown += gmapControl_KeyDown;

            textCtrl.InputFinished += InputFinish;

            // 创建图层
            layer = mapLogic.AddLayer("draw_layer");
        }

        /// <summary>
        /// ESC取消工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 释放工具命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (textCtrl != null)
            {
                gmapControl.Controls.Remove(textCtrl);
                textCtrl.Dispose();
                textCtrl = null;
            }
            if (gmapControl != null)
            {
                gmapControl.SetCursor(Cursors.Default);
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.KeyDown -= gmapControl_KeyDown;
            }

            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExecuteEvent()
        {
            // 通知外部，完成绘制
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动添加文字，返回文字对象",
                    Data = textElement,
                    ToolType = ToolTypeEnum.Draw
                };
                CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            gmapControl = null;
            mapLogic = null;
            textCtrl = null;
            layer = null;
            textElement = null;
            CommondExecutedEvent = null;
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

        // 鼠标点击事件
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (!gmapControl.Controls.Contains(textCtrl))
            {
                gmapControl.Controls.Add(textCtrl);
            }
            textCtrl.Location = new Point(e.X, e.Y);
            textCtrl.Show();
            textCtrl.SetTextFocucs();
            txtPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
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
                    Kml textKml = new Kml();
                    textKml.Placemark.Name = "draw_text" + Utils.ElementIndex;
                    textKml.Placemark.Graph = new KmlText() { Position = new MapLngLat(txtPoint.Lng, txtPoint.Lat), Size = font.Size, Content = context, Font = font.Name, Color = color, FontStyle = font.Style };
                    IMFElement element = null;
                    layer.AddElement(textKml, out element);
                    textElement = element as IMFText;
                    RegistCommondExecuteEvent();
                    ReleaseCommond();//修改  陈静
                }
            }
        }

    }
}
