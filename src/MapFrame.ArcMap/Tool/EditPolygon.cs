/**************************************************************************
 * 类名：EditPolygon.cs
 * 描述：编辑点
 * 作者：lx
 * 日期：Aug 30,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Element;
using MapFrame.ArcMap.Model;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 编辑面
    /// </summary>
    class EditPolygon : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// arcgis的地图控件 
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 当前正在编辑的图形图元
        /// </summary>
        private PolygonElementClass polygonElement = null;
        /// <summary>
        /// 编辑用的图层
        /// </summary>
        private ILayer layer = null;
        /// <summary>
        /// 线的点集合
        /// </summary>
        private List<Point_ArcMap> pointList = null;
        /// <summary>
        /// Ctrl键是否按下
        /// </summary>
        private bool isControlDown = false;
        /// <summary>
        /// 当前编辑的点
        /// </summary>
        private EditMarker currentMarker = null;
        /// <summary>
        /// 全局点
        /// </summary>
        private EditMarker editPoint = null;
        /// <summary>
        /// 当前点的位置
        /// </summary>
        private IPoint currentPoint = null;
        /// <summary>
        /// 编辑点列表
        /// </summary>
        private List<EditMarker> markerList = null;
        /// <summary>
        /// 当前被编辑的图元
        /// </summary>
        private IMFPolygon element = null;
        /// <summary>
        /// 逻辑接口
        /// </summary>
        private IMapLogic mapLogic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">arcgis地图控件</param>
        /// <param name="_element">要编辑的面图元</param>
        public EditPolygon(AxMapControl _mapControl, IMFElement _element)
        {
            element = _element as IMFPolygon;
            mapControl = _mapControl;
            markerList = new List<Model.EditMarker>();
            polygonElement = _element as PolygonElementClass;
            pointList = new List<Element.Point_ArcMap>();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = new CompositeGraphicsLayerClass();
            layer.Name = "editpolygon_layer";
            mapControl.AddLayer(layer, 0);
            InitMarker();
            RegisterEvent();
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            foreach (var item in markerList)
                item.Dispose();
            markerList.Clear();
            if (mapControl != null)
            {
                mapControl.Map.DeleteLayer(layer);
                LogoutEvent();
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;
            }
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs args = new MessageEventArgs()
                {
                    Describe = "编辑多边形，返回多边形对象",
                    Data = element,
                    ToolType = Core.Model.ToolTypeEnum.Edit
                };
                CommondExecutedEvent(this, args);
            }
        }

        /// <summary>
        /// 添加点
        /// </summary>
        private void InitMarker()
        {
            IPointCollection polygonPoint = polygonElement.Geometry as IPointCollection;
            IPoint onePoint = polygonPoint.get_Point(0);
            polygonPoint.AddPoint(onePoint);
            polygonElement.Geometry = polygonPoint as IGeometry;
            int count = polygonPoint.PointCount;
            bool markerBeing = false;
            for (int i = 0; i < count - 1; i++)
            {
                IPoint p = polygonPoint.get_Point(i);
                editPoint = new Model.EditMarker(mapControl, layer);
                markerList.Add(editPoint);
                if (!markerBeing)
                {
                    editPoint.MarkerMouseDownEvent += new Model.EditMarker.MarkerMouseDownDelegate(editPoint_MarkerMouseDownEvent);
                    editPoint.MarkerMouseMoveEvent += new Model.EditMarker.MarkerMouseMoveDelegate(editPoint_MarkerMouseMoveEvent);
                }
                editPoint.InitMarker(i.ToString(), p);
                (layer as CompositeGraphicsLayerClass).AddElement(editPoint, 0);
                markerBeing = true;
            }
        }

        /// <summary>
        /// 结束编辑，删除坐标集合最后一个坐标点
        /// </summary>
        private void endEdit()
        {
            IPointCollection polygonPoint = polygonElement.Geometry as IPointCollection;
            polygonPoint.RemovePoints(polygonPoint.PointCount - 1, 1);
            polygonElement.Geometry = polygonPoint as IGeometry;
        }

        #region 事件
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvent()
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
            editPoint.MarkerMouseDownEvent -= editPoint_MarkerMouseDownEvent;
            editPoint.MarkerMouseMoveEvent -= editPoint_MarkerMouseMoveEvent;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
        }

        /// <summary>
        /// 双击结束编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            ReleaseCommond();
            endEdit();
            RegistCommondExcutedEvent();
        }

        /// <summary>
        /// Ctrl键弹起，禁止整体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            if (e.keyCode == 17)
                isControlDown = false;
        }

        /// <summary>
        /// Ctrl键按下整体移动，ESC键按下结束编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 17)
                isControlDown = true;
            if (e.keyCode == 27)//ESC
            {
                ReleaseCommond(); endEdit();
            }
        }

        /// <summary>
        /// 编辑点移动事件
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        void editPoint_MarkerMouseMoveEvent(IElement element, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint point;

            if (isControlDown)
            {
                IPointCollection pointCollection = polygonElement.Geometry as IPointCollection;
                double x = currentPoint.X - e.mapX;
                double y = currentPoint.Y - e.mapY;
                for (int i = 0; i < pointCollection.PointCount - 1; i++)
                {
                    point = pointCollection.get_Point(i);
                    point.X -= x; point.Y -= y;
                    pointCollection.UpdatePoint(i, point);
                }
                foreach (var item in markerList)
                {
                    point = item.Geometry as IPoint;
                    point.X -= x;
                    point.Y -= y;
                    item.MoveTo(point);
                }
                polygonElement.Geometry = pointCollection as IGeometry;
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, polygonElement, null);
            }
            else
            {
                IPointCollection pointCollection = polygonElement.Geometry as IPointCollection;

                IPoint newPoint = new PointClass();
                newPoint.PutCoords(e.mapX, e.mapY);
                currentMarker.MoveTo(newPoint);
                pointCollection.UpdatePoint(int.Parse(currentMarker.Name), newPoint);

                polygonElement.Geometry = pointCollection as IGeometry;
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, polygonElement, null);
            }
            currentPoint = new PointClass();
            currentPoint.PutCoords(e.mapX, e.mapY);
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
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            endEdit();
            mapControl = null;
            polygonElement = null;
            layer = null;
            pointList = null;
            currentMarker = null;
            editPoint = null;
            currentPoint = null;
            markerList = null;
        }

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
            return element;
        }
    }
}
