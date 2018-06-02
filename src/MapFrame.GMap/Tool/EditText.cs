/**************************************************************************
 * 类名：EditPoint.cs
 * 描述：编辑点图元
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using GMap.NET;
using MapFrame.GMap.Windows;
using System.Drawing;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 编辑文字图元
    /// </summary>
    class EditText : IMFTool
    {
        /// <summary>
        /// 判断是否是在文本图元上
        /// </summary>
        private bool bTextOn = false;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 需要编辑的图元对象
        /// </summary>
        private GMapMarker marker = null;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 当前编辑的图元
        /// </summary>
        private IMFText element = null;
        /// <summary>
        /// 文字编辑控件
        /// </summary>
        private TextInput textCtrl = null;
        /// <summary>
        /// 修改之前的文字内容
        /// </summary>
        private string beforeContext = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        /// <param name="_element">图元</param>
        public EditText(GMapControl _gmapControl, IMFElement _element)
        {
            gmapControl = _gmapControl;
            marker = _element as GMapMarker;
            element = _element as IMFText;
            textCtrl = new TextInput();
            textCtrl.InputFinished += InputFinish;
        }

        #region ITool

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            if (marker == null) return;
            element.HightLight(true);

            Utils.bPublishEvent = false;
            gmapControl.OnMarkerEnter += new MarkerEnter(gmapControl_OnMarkerEnter);
            gmapControl.DoubleClick += new EventHandler(gmapControl_DoubleClick);
            gmapControl.KeyDown += new System.Windows.Forms.KeyEventHandler(gmapControl_KeyDown);
        }

        /// <summary>
        /// 按下esc取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gmapControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                ReleaseCommond();
                RegistCommondExcutedEvent();
            }
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "编辑文字，编辑完成返回文字对象",
                    Data = element,
                    ToolType = ToolTypeEnum.Edit
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (element != null)
            {
                element.HightLight(false);
            }
            if (gmapControl != null)
            {
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseUp -= gmapControl_MouseUp;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.DoubleClick -= gmapControl_DoubleClick;
                gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
                gmapControl.OnMarkerEnter -= gmapControl_OnMarkerEnter;
                gmapControl.KeyDown -= gmapControl_KeyDown;
            }
            if (textCtrl != null) textCtrl.Dispose();

            Utils.bPublishEvent = true;
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
        }

        //鼠标进入目标事件
        private void gmapControl_OnMarkerEnter(GMapMarker item)
        {
            bTextOn = true;

            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.OnMarkerLeave += new MarkerLeave(gmapControl_OnMarkerLeave);
        }

        //鼠标离开目标事件
        private void gmapControl_OnMarkerLeave(GMapMarker item)
        {
            bTextOn = false;

            gmapControl.OnMarkerLeave -= gmapControl_OnMarkerLeave;
            gmapControl.MouseDown -= gmapControl_MouseDown;
        }

        // 鼠标按下事件
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = true;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.MouseUp += gmapControl_MouseUp;
        }

        //鼠标松开事件
        private void gmapControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;

            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseUp -= gmapControl_MouseUp;
        }

        // 鼠标移动事件，如果是选中状态下，则拖动图元
        private void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown == false) return;

            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            marker.Position = lngLat;
        }

        //鼠标双击，打开文字编辑控件
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            if (bTextOn == false) return;

            Font font = element.GetFont();
            Color color = element.GetColor();

            GPoint p = gmapControl.FromLatLngToLocal(marker.Position);
            Point viewPoint = new Point((int)p.X, (int)p.Y);
            beforeContext = element.GetContext();

            textCtrl.Location = viewPoint;
            textCtrl.SetText(beforeContext);
            textCtrl.SetColor(color);
            textCtrl.SetFont(font);
            gmapControl.Controls.Add(textCtrl);
        }

        /// <summary>
        /// 输入修改完成
        /// </summary>
        /// <param name="context">文本内容</param> 
        /// <param name="font">文本格式</param>
        /// <param name="color">文本颜色</param>
        /// <param name="esc">是否取消编辑</param>
        private void InputFinish(string context, Font font, Color color, bool esc)
        {
            if (!esc)
            {
                //若文字为空，或者没有修改，则文字内容不变
                if (string.IsNullOrEmpty(context) || context.Equals(beforeContext))
                {
                    element.SetContext(beforeContext);
                }
                else
                {
                    element.SetContext(context);
                    beforeContext = context;
                }
                element.SetFont(font.Name, font.Size, font.Style);
                element.SetColor(color);
            }

            ReleaseCommond();

            //通知外部完成绘制
            RegistCommondExcutedEvent();
        }

    }
}
