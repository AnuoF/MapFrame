using System;
using System.Collections.Generic;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Runtime.InteropServices;
using MapFrame.Core.Interface;
using MapFrame.Mgis.Common;

namespace MapFrame.Mgis.Tool
{
    class DrawCircle : IMFTool, MapFrame.Core.Interface.IMFDraw
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        //private List<MapFrame.Core.Model.MapLngLat> listPoints = null;
        /// <summary>
        /// 图层集合管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 圆形图元
        /// </summary>
        private IMFCircle circleElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 绘制完成事件
        /// </summary>
        public event EventHandler<Core.Model.MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 是否按住Shift键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 是否按下鼠标
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private MapLngLat centerPoint = null;
        /// <summary>
        /// 临时图元
        /// </summary>
        private string tempName = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawCircle(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

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
            LogoutEvent();
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.eventLButtonDown += mapControl_eventLButtonDown;
            mapControl.eventLButtonUp += mapControl_eventLButtonUp;
            mapControl.eventKeyDown += mapControl_eventKeyDown;
            mapControl.eventKeyUp += mapControl_eventKeyUp;
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
                    Data = circleElement
                };
                CommondExecutedEvent("draw_circle", msg);
            }
        }

        
        #region 事件

        /// <summary>
        /// 鼠标按下，准备绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            //if (!isMouseDown && !isShift)
            //{
            //    centerPoint = new MapLngLat(e.dLong, e.dLat);
            //    Kml kml = new Kml();
            //    KmlCircle circle = new KmlCircle();
            //    kml.Placemark.Name = "mgis_circle" + Utils.ElementIndex;
            //    circle.Position = centerPoint;
            //    circle.RandomPosition = centerPoint;
            //    circle.StrokeColor = System.Drawing.Color.Red;
            //    circle.FillColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
            //    circle.StrokeWidth = 3;
            //    kml.Placemark.Graph = circle;
            //    IMFElement element = null;
            //    layer.AddElement(kml, out element);
            //    circleElement = element as IMFCircle;
            //    isFinish = false;
            //    isMouseDown = true;
            //}
            if (!isMouseDown && !isControl)
            {
                centerPoint = new MapLngLat(e.dLong, e.dLat);
                isMouseDown = true;
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
            //if (isMouseDown && !isShift)
            //{
            //    double circleRadius = MapFrame.Core.Common.Utils.GetDistance(centerPoint, new MapLngLat(e.dLong, e.dLat));
            //    circleElement.UpdatePosition(circleRadius);
            //}
            if (isMouseDown && !isControl)
            {
                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                float[] vertex = new float[4];
                IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * 4);
                vertex[0] = (float)centerPoint.Lng;
                vertex[1] = (float)centerPoint.Lat;

                vertex[2] = (float)e.dLong;
                vertex[3] = (float)e.dLat;
                Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
                tempName = mapControl.MgsDrawLine(16, (ulong)(ptrVert.ToInt64()), 2);
                Marshal.FreeHGlobal(ptrVert);
            }
        }

        /// <summary>
        /// 左键弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_eventLButtonUp(object sender, _DHOSOFTMapControlEvents_eventLButtonUpEvent e)
        {
            //if (!isShift)
            //{
            //    isFinish = true;
            //    isMouseDown = false;
            //    RegistCommondExecutedEvent();
            //}

            if (!isControl)
            {
                if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                Kml kml = new Kml();
                KmlCircle circle = new KmlCircle();
                kml.Placemark.Name = "mgis_circle" + Utils.ElementIndex;
                circle.Position = centerPoint;
                circle.RandomPosition = new MapLngLat(e.dLong, e.dLat);
                circle.StrokeColor = System.Drawing.Color.Red;
                circle.FillColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
                circle.StrokeWidth = 3;
                kml.Placemark.Graph = circle;
                IMFElement element = null;
                layer.AddElement(kml, out element);
                circleElement = element as IMFCircle;
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
                isFinish = true;
                isMouseDown = false;
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
                    if (!string.IsNullOrEmpty(tempName)) mapControl.MgsDelObject(tempName);
                    isMouseDown = false;
                    isFinish = true;
                }
                else { ReleaseCommond(); }
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
        #endregion

        

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            mapControl = null;
            mapLogic = null;
            isFinish = false;
            isControl = false;
            circleElement = null;
            centerPoint = null;
            layer = null;
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
        /// 返回绘制的图元
        /// </summary>
        /// <returns></returns>
        public Core.Interface.IMFElement GetDrawElement()
        {
            return circleElement;
        }
        #endregion

    }
}
