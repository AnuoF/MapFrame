using System;
using MapFrame.Core.Interface;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;

namespace MapFrame.Mgis.Tool
{
    class EditPoint : IMFTool
    {
        /// <summary>
        /// 命令完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 当前被编辑的点
        /// </summary>
        private ulong moveObj = 0;
        /// <summary>
        /// 被编辑的图元
        /// </summary>
        private IMFElement element = null;
        /// <summary>
        /// 地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 
        /// </summary>
        private bool keyDown = false;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图对象</param>
        /// <param name="_element">要编辑的图元</param>
        public EditPoint(AxHOSOFTMapControl _mapControl, IMFElement _element)
        {
            this.element = _element;
            this.moveObj = Convert.ToUInt64(_element.ElementPtr);
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_NO");
            mapControl.loadMouseCur(@"D:\workspace\Src\MapFrame.Mgis\Resources\edit.png");
            mapControl.setMgsEditFlag(0);
            mapControl.eventLButtonDbClick += mapControl_eventLButtonDbClick;
            mapControl.eventKeyDown += mapControl_eventKeyDown;
            mapControl.eventSelMoveObject += mapControl_eventSelMoveObject;
            mapControl.eventSelectedMoveObjectChange += mapControl_eventSelectedMoveObjectChange;
            mapControl.eventNotSelMoveObject += mapControl_eventNotSelMoveObject;
        }

        /// <summary>
        /// 按下Esc取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (e.nChar == (uint)ConsoleKey.Escape)
            {
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 鼠标双击事件取消编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_eventLButtonDbClick(object sender, _DHOSOFTMapControlEvents_eventLButtonDbClickEvent e)
        {
            ReleaseCommond();
        }

        /// <summary>
        /// 没选中目标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventNotSelMoveObject(object sender, _DHOSOFTMapControlEvents_eventNotSelMoveObjectEvent e)
        {
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;//注销鼠标左键按下事件
        }

        //选中目标事件
        void mapControl_eventSelMoveObject(object sender, _DHOSOFTMapControlEvents_eventSelMoveObjectEvent e)
        {
            mapControl.eventLButtonDown += mapControl_eventLButtonDown;//注册鼠标左键按下事件
        }

        void mapControl_eventSelectedMoveObjectChange(object sender, _DHOSOFTMapControlEvents_eventSelectedMoveObjectChangeEvent e)
        {

        }

        /// <summary>
        /// 鼠标左键点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            keyDown = true;
            mapControl.eventMouseMove += mapControl_eventMouseMove;
            mapControl.eventLButtonUp += mapControl_eventLButtonUp;
        }

        /// <summary>
        /// 鼠标左键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_eventLButtonUp(object sender, _DHOSOFTMapControlEvents_eventLButtonUpEvent e)
        {
            keyDown = false;
            mapControl.eventMouseMove -= mapControl_eventMouseMove;
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventLButtonUp -= mapControl_eventLButtonUp;
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_eventMouseMove(object sender, _DHOSOFTMapControlEvents_eventMouseMoveEvent e)
        {
            if (keyDown)
            {
                mapControl.setMoveObjectPositon(this.moveObj, e.dLong, e.dLat, 0);
                mapControl.update();
            }
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.eventMouseMove -= mapControl_eventMouseMove;
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventLButtonUp -= mapControl_eventLButtonUp;
            mapControl.eventLButtonDbClick -= mapControl_eventLButtonDbClick;
            mapControl.eventKeyDown -= mapControl_eventKeyDown;
            mapControl.eventSelMoveObject -= mapControl_eventSelMoveObject;
            mapControl.eventSelectedMoveObjectChange -= mapControl_eventSelectedMoveObjectChange;
            mapControl.eventNotSelMoveObject -= mapControl_eventNotSelMoveObject;
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "框选图元，返回被框选图元",
                    Data = element,
                    ToolType = ToolTypeEnum.Select
                };
                this.CommondExecutedEvent(mapControl, msg);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            moveObj = 0;
            element = null;
            mapControl = null;
            keyDown = false;
        }
    }
}
