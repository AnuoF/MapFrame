/**************************************************************************
 * 类名：EditCircle.cs
 * 描述：编辑圆
 * 作者：LX
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Model;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Element;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 编辑圆
    /// </summary>
    class EditCircle : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 当前正在编辑的图元
        /// </summary>
        private CircleElementClass circleElement = null;
        /// <summary>
        /// 编辑用的图层
        /// </summary>
        private ILayer layer;
        /// <summary>
        /// Ctrl键是否按下
        /// </summary>
        private bool isControlDown;
        /// <summary>
        /// 当前编辑的点
        /// </summary>
        private EditMarker currentMarker;
        /// <summary>
        /// 当前点的位置
        /// </summary>
        private IPoint currentPoint;
        /// <summary>
        /// 全局点
        /// </summary>
        private EditMarker editPoint;
        /// <summary>
        /// 圆形图元
        /// </summary>
        private IMFCircle circleArcMap;
        /// <summary>
        /// 编辑点集合
        /// </summary>
        private List<EditMarker> markerList;
        /// <summary>
        /// 圆心编辑点,(ctrl)键按住的时候会用到
        /// </summary>
        private EditMarker centerMarker;
        /// <summary>
        /// 地图逻辑
        /// </summary>
        private IMapLogic mapLogic = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        /// <param name="element">图元</param>
        public EditCircle(AxMapControl _mapControl, IMFElement element)
        {
            mapControl = _mapControl;
            markerList = new List<EditMarker>();
            circleElement = element as CircleElementClass;
            circleArcMap = circleElement as Circle_ArcMap;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = new CompositeGraphicsLayerClass();
            layer.Name = "editcircle_layer";
            mapControl.AddLayer(layer, 0);
            InitMarker();
            RegisteEvent();
        }

        /// <summary>
        /// 取消命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                LogoutEvent();
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;
            }
        }

        /// <summary>
        /// 添加编辑点
        /// </summary>
        private void InitMarker()
        {
            var pointDot = circleArcMap.GetCenterDot();
            IPoint centerPoint = new PointClass() { X = pointDot.Lng, Y = pointDot.Lat };
            bool markerBeing = false;
            int i = 1;

            for (float ang = 0; ang <= 270; ang += 90)
            {
                var point = GetPointByDistanceAndAngle(circleArcMap.GetRadius(), centerPoint, ang);
                editPoint = new Model.EditMarker(mapControl, layer);
                markerList.Add(editPoint);
                if (!markerBeing)
                {
                    editPoint.MarkerMouseDownEvent += new Model.EditMarker.MarkerMouseDownDelegate(editPoint_MarkerMouseDownEvent);
                    editPoint.MarkerMouseMoveEvent += new Model.EditMarker.MarkerMouseMoveDelegate(editPoint_MarkerMouseMoveEvent);
                }
                editPoint.InitMarker(i.ToString(), point);
                (layer as CompositeGraphicsLayerClass).AddElement(editPoint, 0);
                markerBeing = true;
                i++;
            }
            editPoint = new Model.EditMarker(mapControl, layer);
            centerMarker = new Model.EditMarker(mapControl, layer);
            editPoint.InitMarker("s", centerPoint);
            centerMarker = editPoint;
            (layer as CompositeGraphicsLayerClass).AddElement(editPoint, 0);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, circleElement, null);
        }


        #region 事件

        private void RegisteEvent()
        {
            mapControl.OnKeyDown += new IMapControlEvents2_Ax_OnKeyDownEventHandler(mapControl_OnKeyDown);
            mapControl.OnKeyUp += new IMapControlEvents2_Ax_OnKeyUpEventHandler(mapControl_OnKeyUp);
            mapControl.OnDoubleClick += new IMapControlEvents2_Ax_OnDoubleClickEventHandler(mapControl_OnDoubleClick);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            editPoint.MarkerMouseDownEvent -= new Model.EditMarker.MarkerMouseDownDelegate(editPoint_MarkerMouseDownEvent);
            editPoint.MarkerMouseMoveEvent -= new Model.EditMarker.MarkerMouseMoveDelegate(editPoint_MarkerMouseMoveEvent);
            mapControl.OnKeyDown -= new IMapControlEvents2_Ax_OnKeyDownEventHandler(mapControl_OnKeyDown);
            mapControl.OnKeyUp -= new IMapControlEvents2_Ax_OnKeyUpEventHandler(mapControl_OnKeyUp);
            mapControl.OnDoubleClick -= new IMapControlEvents2_Ax_OnDoubleClickEventHandler(mapControl_OnDoubleClick);
        }

        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            foreach (var item in markerList)
                item.Dispose();
            centerMarker.Dispose();
            mapControl.Map.DeleteLayer(layer);
            ReleaseCommond();

            if (CommondExecutedEvent != null)
            {
                MapFrame.Core.Model.MessageEventArgs args = new Core.Model.MessageEventArgs()
                {
                    ToolType = Core.Model.ToolTypeEnum.Edit,
                    Data = circleElement,
                    Describe = "编辑完成",
                };
                CommondExecutedEvent(this, args);
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
                isControlDown = false;
        }

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 17)
                isControlDown = true;

            if (e.keyCode == 27)  //ESC
            {
                foreach (var item in markerList)
                    item.Dispose();
                centerMarker.Dispose();
                mapControl.Map.DeleteLayer(layer);
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 编辑点按下事件
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        void editPoint_MarkerMouseDownEvent(IElement element, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (element == null) return;
            currentPoint = new PointClass();
            currentPoint.PutCoords(e.mapX, e.mapY);
            currentMarker = element as Model.EditMarker;
        }

        /// <summary>
        /// 编辑点移动事件
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        void editPoint_MarkerMouseMoveEvent(IElement element, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint newPoint = new PointClass();
            var mapPoint = new MapFrame.Core.Model.MapLngLat() { Lng = e.mapX, Lat = e.mapY };
            newPoint.PutCoords(e.mapX, e.mapY);
            if (isControlDown)
            {
                centerMarker.MoveTo(newPoint);//圆心点也跟着移动
                circleArcMap.UpdatePosition(mapPoint);
                int i = 0;
                for (float ang = 0; ang <= 270; ang += 90)
                {
                    var point = GetPointByDistanceAndAngle(circleArcMap.GetRadius() * 100000, newPoint, ang);
                    markerList[i].MoveTo(point);
                    i++;
                }
            }
            else
            {
                if (currentMarker.Name == "s")
                {
                    currentMarker.MoveTo(newPoint);
                    circleArcMap.UpdatePosition(mapPoint);
                    int i = 0;
                    for (float ang = 0; ang <= 270; ang += 90)
                    {
                        var point = GetPointByDistanceAndAngle(circleArcMap.GetRadius(), newPoint, ang);
                        markerList[i].MoveTo(point);
                        i++;
                    }
                }
                else
                {
                    IPoint centerDot = new PointClass() { X = circleArcMap.GetCenterDot().Lng, Y = circleArcMap.GetCenterDot().Lat };
                    double radius = MapFrame.Core.Common.Utils.GetDistance(circleArcMap.GetCenterDot(), mapPoint);
                    circleArcMap.UpdatePosition(radius);
                    int i = 0;

                    for (float ang = 0; ang <= 270; ang += 90)
                    {
                        var point = GetPointByDistanceAndAngle(radius, centerDot, ang);
                        markerList[i].MoveTo(point);
                        i++;
                    }
                }
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, circleElement, null);
            }
            currentPoint = new PointClass();
            currentPoint.PutCoords(e.mapX, e.mapY);
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            circleElement = null;
            CommondExecutedEvent = null;
            layer = null;
            currentMarker = null;
            currentPoint = null;
            circleArcMap = null;
            markerList = null;
            centerMarker = null;
            mapControl = null;
            mapLogic = null;
        }

        #region IDraw
        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <returns></returns>
        public Core.Interface.IMFElement GetDrawElement()
        {
            return circleArcMap;
        }
        #endregion


        private IPoint GetPointByDistanceAndAngle(double distance, IPoint point, double angle)
        {
            double lng1 = point.X;
            double lat1 = point.Y;
            // 将距离转换成经度的计算公式 * Math.PI / 180
            double lon = lng1 + (distance * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat1 * Math.PI / 180));
            // 将距离转换成纬度的计算公式
            double lat = lat1 + (distance * Math.Cos(angle * Math.PI / 180)) / 111;

            IPoint newPoint = new PointClass();
            newPoint.X = lon;
            newPoint.Y = lat;
            return newPoint;
        }
    }
}
