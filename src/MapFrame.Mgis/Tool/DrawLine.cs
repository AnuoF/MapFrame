using System;
using System.Collections.Generic;
using System.Linq;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Runtime.InteropServices;
using MapFrame.Core.Interface;
using MapFrame.Mgis.Common;

namespace MapFrame.Mgis.Tool
{
    /// <summary>
    /// MGis线图元
    /// </summary>
    class DrawLine : IMFTool, IMFDraw
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listPoints = null;
        /// <summary>
        /// 图层集合管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 临时名称
        /// </summary>
        private string tempName = string.Empty;
        /// <summary>
        /// 线图元
        /// </summary>
        private IMFLine lineElement = null;
        /// <summary>
        /// 是否按下Control键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawLine(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
            listPoints = new List<MapLngLat>();
        }

        #region 命令
        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_NO");
            layer = mapLogic.AddLayer("draw_mgis");
            RegistEvent();

        }
        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
            LogoutEvent();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.eventLButtonDown += mapControl_eventLButtonDown;
            mapControl.eventKeyDown += mapControl_eventKeyDown;
            mapControl.eventLButtonDbClick += mapControl_eventLButtonDbClick;
            mapControl.eventMouseMove += mapControl_eventMouseMove;
            mapControl.eventKeyUp += mapControl_eventKeyUp;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventKeyDown -= mapControl_eventKeyDown;
            mapControl.eventLButtonDbClick -= mapControl_eventLButtonDbClick;
            mapControl.eventMouseMove -= mapControl_eventMouseMove;
            mapControl.eventKeyUp -= mapControl_eventKeyUp;
        }

        /// <summary>
        /// 绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制线，返回线对象",
                    ToolType = ToolTypeEnum.Draw,
                    Data = lineElement
                };
                this.CommondExecutedEvent("draw_line", msg);//
            }
        }

        #endregion

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
        /// 鼠标左键双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDbClick(object sender, _DHOSOFTMapControlEvents_eventLButtonDbClickEvent e)
        {
            if (!isControl && listPoints.Count >= 2)
            {
                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                //if (listPoints.Count < 2) return;
                Kml kml = new Kml();
                KmlLineString line = new KmlLineString();
                line.PositionList = listPoints;
                line.Color = System.Drawing.Color.Red;
                line.Width = 3;
                kml.Placemark.Name = "mgis_line" + Utils.ElementIndex;
                kml.Placemark.Graph = line;
                IMFElement element = null;
                layer.AddElement(kml, out element);
                lineElement = element as IMFLine;
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
                listPoints.Clear();
                isFinish = true;
            }
        }


        /// <summary>
        /// 键盘按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (e.nChar == 27)
            {
                if (!isFinish)
                {
                    //layer.RemoveElement(lineElement);
                    if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                    listPoints.Clear();
                    isFinish = true;
                }
                else
                {
                    ReleaseCommond();
                }
            }
            if (e.nChar == 17)
            {
                isControl = true;
                mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
            }
        }

        /// <summary>
        /// 鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            if (!isControl)
            {
                //var point = new MapLngLat(e.dLong, e.dLat);
                listPoints.Add(new MapLngLat(e.dLong, e.dLat));
                isFinish = false;
            }
        }    

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventMouseMove(object sender, _DHOSOFTMapControlEvents_eventMouseMoveEvent e)
        {
            if (listPoints.Count != 0 && !isControl)
            {
                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                var point = new MapLngLat(e.dLong, e.dLat);
                listPoints.Add(point);
                int count = listPoints.Count();
                float[] vertex = new float[count * 2];
                IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
                for (int i = 0; i < count; i++)
                {
                    vertex[2 * i] = (float)listPoints[i].Lng;
                    vertex[2 * i + 1] = (float)listPoints[i].Lat;
                }
                Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
                tempName = mapControl.MgsDrawLine(10, (ulong)(ptrVert.ToInt64()), count);
                Marshal.FreeHGlobal(ptrVert);
                listPoints.Remove(point);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            listPoints.Clear();
            listPoints = null;
            mapControl = null;
            lineElement = null;
            isFinish = false;
            isControl = false;
            layer = null;
            mapLogic = null;
        }

        #region IMFDraw
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
        public Core.Interface.IMFElement GetDrawElement()
        {
            return lineElement;
        }
        #endregion
    }
}
