using System;
using System.Collections.Generic;
using AxHOSOFTMapControlLib;
using System.Runtime.InteropServices;
using MapFrame.Core.Model;
using MapFrame.Core.Interface;
using MapFrame.Mgis.Common;

namespace MapFrame.Mgis.Tool
{
    /// <summary>
    /// Mgis矩形图元
    /// </summary>
    class DrawRectangle : IMFTool, IMFDraw
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapFrame.Core.Model.MapLngLat> listPoints = null;
        /// <summary>
        /// 图层集合管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 多边形图元
        /// </summary>
        private IMFPolygon recElement = null;
        /// <summary>
        /// 临时名称
        /// </summary>
        private string tempName = string.Empty;
        /// <summary>
        /// 是否完成绘制
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 是否按了Control键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 绘制完成事件
        /// </summary>
        public event EventHandler<Core.Model.MessageEventArgs> CommondExecutedEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawRectangle(AxHOSOFTMapControl _mapControl)
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
            RegistEvent();
            layer = mapLogic.AddLayer("draw_mgis");
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_NO");
        }

        /// <summary>
        /// 释放命令
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
            LogoutEvent();
        }
        /// <summary>
        /// 注册绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "绘制圆",
                    ToolType = ToolTypeEnum.Draw,
                    Data = recElement
                };
                CommondExecutedEvent("draw_rec", msg);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.eventLButtonDown += mapControl_eventLButtonDown;
            mapControl.eventLButtonUp += mapControl_eventLButtonUp;
            mapControl.eventKeyDown += mapControl_eventKeyDown;
            mapControl.eventKeyUp +=mapControl_eventKeyUp;
            mapControl.eventMouseMove += mapControl_eventMouseMove;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventLButtonUp -= mapControl_eventLButtonUp;
            mapControl.eventKeyDown -= mapControl_eventKeyDown;
            mapControl.eventMouseMove -= mapControl_eventMouseMove;
            mapControl.eventKeyUp -= mapControl_eventKeyUp;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (e.nChar == 27)
            {
                if (!isFinish)
                {
                    if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                    isFinish = true;
                }
                else
                    ReleaseCommond();
            }
            else if (e.nChar == 17)
            {
                isControl = true;
                mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
            }
        }

        /// <summary>
        /// 键盘弹起事件
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
        /// 鼠标弹起结束绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonUp(object sender, _DHOSOFTMapControlEvents_eventLButtonUpEvent e)
        {
            if (!isControl)
            {
                MapLngLat p1 = new MapLngLat(e.dLong, listPoints[0].Lat);
                MapLngLat p2 = new MapLngLat(e.dLong, e.dLat);
                MapLngLat p3 = new MapLngLat(listPoints[0].Lng, e.dLat);
                listPoints.Add(p1);
                listPoints.Add(p2);
                listPoints.Add(p3);

                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                Kml kml = new Kml();
                KmlPolygon rectangle = new KmlPolygon();
                kml.Placemark.Name = "mgis_rec" + Utils.ElementIndex;
                rectangle.PositionList = listPoints;
                rectangle.FillColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
                rectangle.OutLineColor = System.Drawing.Color.Red;
                rectangle.OutLineSize = 3;
                kml.Placemark.Graph = rectangle;
                IMFElement element = null;
                layer.AddElement(kml, out element);
                recElement = element as IMFPolygon;
                
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
                isFinish = true;
                listPoints.Clear();
            }
            
        }

        /// <summary>
        /// 鼠标点击开始绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            if (!isControl && listPoints.Count == 0)
            {
                listPoints.Add(new MapLngLat(e.dLong, e.dLat));
                isFinish = false;
            }
        }

        /// <summary>
        /// 鼠标移动，实时绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventMouseMove(object sender, _DHOSOFTMapControlEvents_eventMouseMoveEvent e)
        {
            if (listPoints.Count != 0 && !isControl)
            {
                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                float[] vertex = new float[4];
                IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * 4);
                vertex[0] = (float)listPoints[0].Lng;
                vertex[1] = (float)listPoints[0].Lat;

                vertex[2] = (float)e.dLong;
                vertex[3] = (float)e.dLat;
                Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
                tempName = mapControl.MgsDrawLine(15, (ulong)(ptrVert.ToInt64()), 2);
                Marshal.FreeHGlobal(ptrVert);
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
        /// 获取绘制的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return recElement;
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            listPoints.Clear();
            listPoints = null;
            tempName = string.Empty;
            mapControl = null;
            mapLogic = null;
            layer = null;
            isFinish = false;
            isControl = false;
            recElement = null;
        }
    }
}
