/**************************************************************************
 * 类名：EditLine.cs
 * 描述：编辑线类
 * 作者：CJ
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
using MapFrame.Core.Model;
using MapFrame.ArcMap.Element;
using MapFrame.ArcMap.Model;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 编辑线类
    /// </summary>
    class EditLine : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 当前正在编辑的线
        /// </summary>
        private LineElementClass lineElement;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl;
        /// <summary>
        /// 编辑用的图层
        /// </summary>
        private ILayer layer;
        /// <summary>
        /// 线的点集合
        /// </summary>
        private List<Point_ArcMap> pointList;
        /// <summary>
        /// Ctrl键是否按下
        /// </summary>
        private bool isControlDown;
        /// <summary>
        /// 当前编辑的点
        /// </summary>
        private Model.EditMarker currentMarker;
        /// <summary>
        /// 全局点
        /// </summary>
        private Model.EditMarker editPoint;
        /// <summary>
        /// 当前点的位置
        /// </summary>
        private IPoint currentPoint;
        /// <summary>
        /// 编辑点列表
        /// </summary>
        private List<EditMarker> markerList;
        /// <summary>
        /// 当前被编辑的图元
        /// </summary>
        private IMFLine element;
        /// <summary>
        /// 地图逻辑
        /// </summary>
        private IMapLogic mapLogic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">arcgis地图控件</param>
        /// <param name="_element">要编辑的线图元</param>
        public EditLine(AxMapControl _mapControl, IMFElement _element)
        {
            element = _element as IMFLine;
            markerList = new List<EditMarker>();
            lineElement = _element as LineElementClass;
            mapControl = _mapControl;
            pointList = new List<Point_ArcMap>();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = new CompositeGraphicsLayerClass();
            layer.Name = "edit_layer";
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
            {
                item.Dispose();
            }
            markerList.Clear();
            mapControl.Map.DeleteLayer(layer);
            LogoutEvent();
            ICommand tool = new ControlsMapPanToolClass();
            tool.OnCreate(mapControl.Object);
            mapControl.CurrentTool = tool as ESRI.ArcGIS.SystemUI.ITool;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitMarker()
        {
            IPointCollection linePoint = lineElement.Geometry as IPointCollection;
            int cout = linePoint.PointCount;
            bool markerBeing = false;
            for (int i = 0; i < cout; i++)
            {
                IPoint p = linePoint.get_Point(i);
                editPoint = new MapFrame.ArcMap.Model.EditMarker(mapControl, layer);
                markerList.Add(editPoint);
                if (!markerBeing)
                {
                    editPoint.MarkerMouseDownEvent += editPoint_MarkerMouseDownEvent;
                    editPoint.MarkerMouseUpEvent += editPoint_MarkerMouseUpEvent;
                    editPoint.MarkerMouseMoveEvent += editPoint_MarkerMouseMoveEvent;
                }
                editPoint.InitMarker(i.ToString(), p);
                (layer as CompositeGraphicsLayerClass).AddElement(editPoint, 0);
                markerBeing = true;
            }
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            editPoint.MarkerMouseDownEvent -= editPoint_MarkerMouseDownEvent;
            editPoint.MarkerMouseMoveEvent -= editPoint_MarkerMouseMoveEvent;
            editPoint.MarkerMouseUpEvent -= editPoint_MarkerMouseUpEvent;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvent()
        {
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
        }

        #region  事件

        /// <summary>
        /// 编辑点弹起事件
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private void editPoint_MarkerMouseUpEvent(IElement element, IMapControlEvents2_OnMouseUpEvent e)
        {
            editPoint.MarkerMouseMoveEvent -= editPoint_MarkerMouseMoveEvent;
        }

        /// <summary>
        /// 编辑点移动事件
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private void editPoint_MarkerMouseMoveEvent(IElement element, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint point;
            if (isControlDown)
            {
                IPointCollection pointCollection = lineElement.Geometry as IPointCollection;
                double x = currentPoint.X - e.mapX;
                double y = currentPoint.Y - e.mapY;
                for (int i = 0; i < pointCollection.PointCount; i++)
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
                lineElement.Geometry = pointCollection as IGeometry;
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            else
            {
                IPointCollection pointCollection = lineElement.Geometry as IPointCollection;
                point = pointCollection.get_Point(int.Parse(currentMarker.Name));
                IPoint newPoint = new PointClass();
                newPoint.PutCoords(e.mapX, e.mapY);
                currentMarker.MoveTo(newPoint);
                pointCollection.UpdatePoint(int.Parse(currentMarker.Name), newPoint);
                lineElement.Geometry = pointCollection as IGeometry;
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            currentPoint = new PointClass();
            currentPoint.PutCoords(e.mapX, e.mapY);
        }

        /// <summary>
        /// 双击编辑完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            ReleaseCommond();

            if (CommondExecutedEvent != null)
            {
                MapFrame.Core.Model.MessageEventArgs args = new Core.Model.MessageEventArgs()
                {
                    ToolType = Core.Model.ToolTypeEnum.Edit,
                    Data = lineElement,
                    Describe = "编辑完成"
                };
                CommondExecutedEvent(this, args);
            }
        }

        /// <summary>
        /// 键盘按键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            if (e.keyCode == 17)//ctrl键
            {
                isControlDown = false;
            }
            if (e.keyCode == 27) //Esc退出编辑
            {
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 键盘按键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 17)
            {
                isControlDown = true;
            }
        }

        /// <summary>
        /// 编辑点按下事件
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private void editPoint_MarkerMouseDownEvent(IElement element, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (element == null) return;
            currentPoint = new PointClass();
            currentPoint.PutCoords(e.mapX, e.mapY);
            currentMarker = element as Model.EditMarker;
        }

        #endregion

        /// <summary>
        /// 释放该类
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            currentPoint = null;
            markerList = null;
            editPoint = null;
            pointList = null;
            layer = null;
            mapControl = null;
            lineElement = null;
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
