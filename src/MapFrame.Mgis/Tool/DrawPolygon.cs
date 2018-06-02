using System;
using System.Collections.Generic;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Runtime.InteropServices;
using MapFrame.Core.Interface;
using MapFrame.Mgis.Common;
using System.Drawing;

namespace MapFrame.Mgis.Tool
{
    /// <summary>
    /// Mgis绘制多边形图元
    /// </summary>
    class DrawPolygon : IMFTool, IMFDraw
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
        /// 图元名称
        /// </summary>
        private IMFPolygon polygonElement = null;
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
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 是否按着Control键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawPolygon(AxHOSOFTMapControl _mapControl)
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
            layer = mapLogic.AddLayer("draw_mgis");
            RegistEvent();
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_NO");
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
                    Data = polygonElement
                };
                CommondExecutedEvent("draw_polygon", msg);
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
            mapControl.eventMouseMove += mapControl_eventMouseMove;
            mapControl.eventLButtonDbClick += mapControl_eventLButtonDbClick;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.eventLButtonDown -= mapControl_eventLButtonDown;
            mapControl.eventKeyDown -= mapControl_eventKeyDown;
            mapControl.eventMouseMove -= mapControl_eventMouseMove;
            mapControl.eventKeyUp -= mapControl_eventKeyUp;
            mapControl.eventLButtonDbClick -= mapControl_eventLButtonDbClick;
        }

        #endregion

        #region 事件
        /// <summary>
        /// 键盘按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (e.nChar == 27)//不需要是否绘制完成的判断
            {
                if (!isFinish)
                {
                    if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                    listPoints.Clear();
                    isFinish = true;
                }
                else
                {
                    ReleaseCommond();
                }
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
        /// 鼠标左键双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDbClick(object sender, _DHOSOFTMapControlEvents_eventLButtonDbClickEvent e)
        {
            if (!isControl && listPoints.Count > 2)
            {
                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);

                Kml kml = new Kml();
                KmlPolygon polygon = new KmlPolygon();
                polygon.PositionList = listPoints;
                polygon.FillColor = Color.FromArgb(0, Color.White);
                polygon.OutLineColor = Color.Red;
                polygon.OutLineSize = 3;
                kml.Placemark.Name = "mgis_polygon" + Utils.ElementIndex;
                kml.Placemark.Graph = polygon;
                IMFElement element = null;
                layer.AddElement(kml, out element);
                polygonElement = element as IMFPolygon;
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
                isFinish = true;
                listPoints.Clear();

            }
        }

        /// <summary>
        /// 鼠标左键单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            if (!isControl)
            {
                listPoints.Add(new MapLngLat(e.dLong, e.dLat));
                isFinish = false;
            }
        }

        /// <summary>
        /// 鼠标移动实时绘制
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
                int count = listPoints.Count;
                float[] vertex = new float[count * 2];
                IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
                for (int i = 0; i < count; i++)
                {
                    vertex[2 * i] = (float)listPoints[i].Lng;
                    vertex[2 * i + 1] = (float)listPoints[i].Lat;
                }
                Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
                tempName = mapControl.MgsDrawLine(11, (ulong)(ptrVert.ToInt64()), count);
                Marshal.FreeHGlobal(ptrVert);
                listPoints.Remove(point);
            }
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
            polygonElement = null;
            mapLogic = null;
            mapControl = null;
            isFinish = false;
            isControl = false;
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
        public IMFElement GetDrawElement()
        {
            return polygonElement ;
        }
        #endregion
    }
}
