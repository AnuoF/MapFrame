/****************************************************
 * Author:cj
 * FileName:EditMarker
 * Represent:编辑点
 * DateTime:2016年8月25日
 * *************************************************/

using ESRI.ArcGIS.Carto;
using System.Drawing;
using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace MapFrame.ArcMap.Model
{

    /// <summary>
    /// 编辑点
    /// </summary>
    class EditMarker : MarkerElementClass, IDisposable
    {
        public delegate void MarkerMouseDownDelegate(IElement element, IMapControlEvents2_OnMouseDownEvent e);
        /// <summary>
        /// 编辑点的按下事件
        /// </summary>
        public event MarkerMouseDownDelegate MarkerMouseDownEvent;

        public delegate void MarkerMouseUpDelegate(IElement element, IMapControlEvents2_OnMouseUpEvent e);
        /// <summary>
        /// 编辑点的弹起事件
        /// </summary>
        public event MarkerMouseUpDelegate MarkerMouseUpEvent;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        public delegate void MarkerMouseMoveDelegate(IElement element, IMapControlEvents2_OnMouseMoveEvent e);
        /// <summary>
        /// 编辑点的移动事件
        /// </summary>
        public event MarkerMouseMoveDelegate MarkerMouseMoveEvent;
        /// <summary>
        /// 当前图层
        /// </summary>
        private ILayer layer;
        /// <summary>
        /// 地图控件
        /// </summary>
        private static AxMapControl mapControl;
        /// <summary>
        /// 编辑点被选中
        /// </summary>
        private volatile bool markerSelected;
        /// <summary>
        /// 当前被编辑的点
        /// </summary>
        private EditMarker editMarker;
        /// <summary>
        /// 编辑点被选中事件
        /// </summary>
        public bool MarkerSelected
        {
            get { return markerSelected; }
        }

        /// <summary>
        /// 编辑点构造函数
        /// </summary>
        /// <param name="lnglat"></param>
        public EditMarker(AxMapControl _mapControl, ILayer _layer)
        {
            layer = _layer;
            if (mapControl == null)
            {
                mapControl = _mapControl;
                InitEvent();
            }
            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
            Color c = Color.Green;
            IRgbColor color = new RgbColorClass()
            {
                Transparency = c.A,
                Red = c.R,
                Green = c.G,
                Blue = c.B
            };

            pMarkerSymbol.Color = color;
            pMarkerSymbol.Size = 7;
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            base.Symbol = pMarkerSymbol;
        }

        /// <summary>
        /// 初始化点
        /// </summary>
        /// <param name="name">点的名字</param>
        /// <param name="point">点的位置</param>
        public void InitMarker(string name, IPoint point)
        {
            base.Name = name;
            base.Geometry = point;
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        private void InitEvent()
        {
            mapControl.OnMouseDown += new IMapControlEvents2_Ax_OnMouseDownEventHandler(mapControl_OnMouseDown);
            mapControl.OnMouseMove += new IMapControlEvents2_Ax_OnMouseMoveEventHandler(mapControl_OnMouseMove_Cursor);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="point"></param>
        public void MoveTo(IPoint point)
        {
            this.Geometry = point;
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// 移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseMove_Cursor(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IGraphicsContainer gc = layer as IGraphicsContainer;
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = gc.LocateElements(point, 0);
            if (elementenum != null)
            {
                mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }
            else
            {
                mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            if (editMarker != null)
            {
                if (editMarker.markerSelected)
                {
                    point.PutCoords(e.mapX, e.mapY);
                    //editMarker.Geometry = point;//重新给位置
                    mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//刷新
                    if (MarkerMouseMoveEvent != null)
                        MarkerMouseMoveEvent(editMarker, e);
                }
            }
        }

        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            IGraphicsContainer gc = layer as IGraphicsContainer;
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = gc.LocateElements(point, 0);
            if (MarkerMouseDownEvent != null && elementenum != null)
            {
                editMarker = elementenum.Next() as EditMarker;
                editMarker.markerSelected = true;
                mapControl.OnMouseMove += new IMapControlEvents2_Ax_OnMouseMoveEventHandler(mapControl_OnMouseMove);
                mapControl.OnMouseUp += new IMapControlEvents2_Ax_OnMouseUpEventHandler(mapControl_OnMouseUp);
                MarkerMouseDownEvent(editMarker, e);
            }
        }

        /// <summary>
        /// 鼠标弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            IGraphicsContainer gc = layer as IGraphicsContainer;
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = gc.LocateElements(point, 0);
            if (elementenum != null)
            {
                editMarker = elementenum.Next() as EditMarker;
                editMarker.markerSelected = false;
            }

            if (MarkerMouseUpEvent != null)
            {
                MarkerMouseUpEvent(editMarker, null);
            }
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (mapControl != null)
            {
                mapControl.OnMouseDown -= mapControl_OnMouseDown;
                mapControl.OnMouseUp -= mapControl_OnMouseUp;
                mapControl.OnMouseMove -= mapControl_OnMouseMove;
                mapControl.OnMouseMove -= mapControl_OnMouseMove_Cursor;
                mapControl = null;
            }
            this.editMarker = null;
        }
    }
}
