using System;
using System.Collections.Generic;
using MapFrame.Core.Interface;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Drawing;
using System.Xml;

namespace MapFrame.Mgis.Tool
{
    class SelectElement : IMFTool, IMFSelect
    {
        /// <summary>
        /// 被选中的目标
        /// </summary>
        private List<IMFElement> selectElements = null;
        /// <summary>
        /// 图元锁对象
        /// </summary>
        private object elementObj = new object();
        /// <summary>
        /// 单选的目标
        /// </summary>
        private ulong moveObj = 0;
        /// <summary>
        /// 地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 框选的起始位置
        /// </summary>
        private Point startPoint;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        /// <param name="_mapLogic">图层管理</param>
        public SelectElement(AxHOSOFTMapControl _mapControl, IMapLogic _mapLogic)
        {
            this.mapLogic = _mapLogic;
            this.mapControl = _mapControl;
            selectElements = new List<IMFElement>();
        }

        /// <summary>
        /// 获取选中的图元
        /// </summary>
        /// <returns></returns>
        public List<IMFElement> GetSelectElements()
        {
            return selectElements;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.setMgsEditFlag(1);
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVEOBJECT_SELECT");
            mapControl.eventSelMoveObject += mapControl_eventSelMoveObject;
            mapControl.eventLButtonDown += mapControl_eventLButtonDown;
            mapControl.eventKeyDown += mapControl_eventKeyDown;
            mapControl.drawRect(Color.Red.R, Color.Red.G, Color.Red.B, Color.Red.A, 1);
        }

        //
        private void mapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (e.nChar == (uint)ConsoleKey.Escape)
            {
                ReleaseCommond();
            }
        }


        //弹起
        private void mapControl_eventLButtonUp(object sender, _DHOSOFTMapControlEvents_eventLButtonUpEvent e)
        {
            mapControl.eventLButtonUp -= mapControl_eventLButtonUp;
            if (e.x == startPoint.X && e.y == startPoint.Y) return;
            string objXml = "";
            if (startPoint.X < e.x)
            {
                if (startPoint.Y < e.y)
                {
                    //左右下
                    objXml = mapControl.pickupByRect(startPoint.X, startPoint.Y, e.x, e.y);
                }
                else
                {
                    //左右上
                    objXml = mapControl.pickupByRect(startPoint.X, e.y, e.x, startPoint.Y);
                }
            }
            else
            {
                if (startPoint.Y < e.y)
                {
                    //右左下
                    objXml = mapControl.pickupByRect(e.x, startPoint.Y, startPoint.X, e.y);
                }
                else
                {
                    //右左上
                    objXml = mapControl.pickupByRect(e.x, e.y, startPoint.X, startPoint.Y);
                }
            }

            if (string.IsNullOrEmpty(objXml)) return;
            this.ClearSelectElements();//先清理列表

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(objXml);
            XmlNode nodes = xmlDoc.SelectSingleNode("root");
            foreach (XmlLinkedNode note in nodes.ChildNodes)
            {
                IMFElement element = this.mapLogic.GetElement(note.Attributes[0].Value + "");
                element.HightLight(true);
                this.AddElementtoList(element);
            }
            RegistCommondExecutedEvent();
        }

        /// <summary>
        /// 向列表中添加选中的图元
        /// </summary>
        /// <param name="element"></param>
        private void AddElementtoList(IMFElement element)
        {
            lock (elementObj)
            {
            tip:
                if (selectElements != null)
                {
                    if (!selectElements.Contains(element))
                    {
                        element.HightLight(true);
                        selectElements.Add(element);
                    }
                }
                else
                {
                    selectElements = new List<IMFElement>();
                    goto tip;
                }
            }
        }

        /// <summary>
        /// 清理选中的图元列表
        /// </summary>
        private void ClearSelectElements()
        {
            lock (elementObj)
            {
                if (selectElements != null)
                {
                    foreach (IMFElement element in selectElements)
                    {
                        element.HightLight(false);
                    }
                    selectElements.Clear();
                }
            }
        }

        //按下
        private void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            startPoint = new Point(e.x, e.y);
            mapControl.eventLButtonUp += mapControl_eventLButtonUp;
        }

        /// <summary>
        /// 选中目标事件(单选)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_eventSelMoveObject(object sender, _DHOSOFTMapControlEvents_eventSelMoveObjectEvent e)
        {
            if (e.objId != 0)
            {
                moveObj = e.objId;
                this.ClearSelectElements();
                IMFElement element = this.mapLogic.GetElement(moveObj + "");
                this.AddElementtoList(element);
                RegistCommondExecutedEvent();
            }
        }

        /// <summary>
        /// 结束命令
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.eventSelMoveObject -= mapControl_eventSelMoveObject;
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventKeyDown -= mapControl_eventKeyDown;
            mapControl.eventLButtonUp -= mapControl_eventLButtonUp;
            mapControl.setMgsEditFlag(0);
            mapControl.pickupByRect(0, 0, 0, 0);
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
            this.ClearSelectElements();
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
                    Data = selectElements,
                    ToolType = ToolTypeEnum.Select
                };
                this.CommondExecutedEvent(mapControl, msg);
            }
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            selectElements.Clear();
            selectElements = null;
            elementObj = new object();
            moveObj = 0;
            mapControl = null;
            CommondExecutedEvent = null;
            mapLogic = null;
        }
    }
}

