/**************************************************************************
 * 类名：ToolBox.cs
 * 描述：工具箱
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using System;
using MapFrame.Core.Interface;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 工具箱
    /// </summary>
    public class ToolBox : IMFToolBox
    {
        /// <summary>
        /// 当前的工具命令
        /// </summary>
        private IMFTool currentTool = null;
        /// <summary>
        /// arcgis地图控件
        /// </summary>
        private AxMapControl mapControl;
        /// <summary>
        /// 地图逻辑控制接口
        /// </summary>
        private MapFrame.Core.Interface.IMapLogic mapLogic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapObject">地图对象</param>
        /// <param name="_mapLogic">地图逻辑类</param>
        public ToolBox(object mapObject, MapFrame.Core.Interface.IMapLogic _mapLogic)
        {
            mapLogic = _mapLogic;
            mapControl = mapObject as AxMapControl;
        }

        /// <summary>
        /// 工具命令完成事件
        /// </summary>
        public event EventHandler<MapFrame.Core.Model.MessageEventArgs> CommondExecutedEvent;

        /// <summary>
        /// 漫游
        /// </summary>
        public void Roam(bool isDrag = true)
        {
            ICommand command = new ControlsMapPanToolClass();
            command.OnCreate(mapControl.Object);
            if (command.Enabled)
            {
                mapControl.CurrentTool = command as ITool;
            }
        }

        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomIn()
        {
            ICommand tool = new ControlsMapZoomInToolClass();
            tool.OnCreate(mapControl.Object);
            if (tool.Enabled)
            {
                mapControl.CurrentTool = tool as ESRI.ArcGIS.SystemUI.ITool;
            }
        }

        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomOut()
        {
            ICommand tool = new ControlsMapZoomOutToolClass();
            tool.OnCreate(mapControl.Object);
            if (tool.Enabled)
            {
                mapControl.CurrentTool = tool as ESRI.ArcGIS.SystemUI.ITool;
            }
        }

        /// <summary>
        /// 全图显示
        /// </summary>
        public void FullView()
        {
            ICommand cmd = new ControlsMapFullExtentCommandClass();
            cmd.OnCreate(mapControl.Object);
            if (cmd.Enabled)
            {
                cmd.OnClick();
            }
        }

        /// <summary>
        /// 定位
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        /// <param name="zoomLevel"></param>
        public void ZoomToPosition(Core.Model.MapLngLat lngLat, int? zoomLevel = null)
        {
            ESRI.ArcGIS.Geometry.IPoint point = new PointClass();
            point.PutCoords(lngLat.Lng, lngLat.Lat);
            mapControl.CenterAt(point);
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="measureType">测量的类型</param>
        public void Measure(Core.Model.MeasureTypeEnum measureType)
        {
            ReleaseTool();
            switch (measureType)
            {
                case Core.Model.MeasureTypeEnum.Angle:
                    currentTool = new MeasureAngle(mapControl);
                    break;
                case Core.Model.MeasureTypeEnum.Area:
                    currentTool = new Measure(mapControl, mapLogic, "area");
                    break;
                case Core.Model.MeasureTypeEnum.Distance:
                    currentTool = new Measure(mapControl, mapLogic, "distance");
                    break;
            }
            currentTool.RunCommond();
            if (currentTool != null)
            {
                currentTool.CommondExecutedEvent += new EventHandler<Core.Model.MessageEventArgs>(currentTool_CommondExecutedEvent);
            }
        }

        /// <summary>
        /// 选择图元
        /// </summary>
        public void Select()
        {
            ReleaseTool();
            currentTool = new SelectElements(mapControl);
            if (currentTool != null)
            {
                currentTool.RunCommond();//执行命令
                currentTool.CommondExecutedEvent += new EventHandler<Core.Model.MessageEventArgs>(currentTool_CommondExecutedEvent);
            }
        }

        /// <summary>
        /// 释放当前工具
        /// </summary>
        public void ReleaseTool()
        {
            if (currentTool != null)
            {
                currentTool.CommondExecutedEvent -= currentTool_CommondExecutedEvent;
                currentTool.ReleaseCommond();
                currentTool.Dispose();
                currentTool = null;
            }
        }

        public void EditElement(MapFrame.Core.Interface.IMFElement element)
        {
            //释放之前的工具
            ReleaseTool();

            switch (element.ElementType)
            {
                case MapFrame.Core.Model.ElementTypeEnum.Point://编辑点
                    currentTool = new EditPoint(mapControl, element);
                    break;

                case MapFrame.Core.Model.ElementTypeEnum.Line://编辑线
                    currentTool = new EditLine(mapControl, element);
                    break;

                case MapFrame.Core.Model.ElementTypeEnum.Polygon://编辑多边形
                    currentTool = new EditPolygon(mapControl, element);
                    break;

                case MapFrame.Core.Model.ElementTypeEnum.Circle://编辑圆
                    currentTool = new EditCircle(mapControl, element);
                    break;

                case MapFrame.Core.Model.ElementTypeEnum.Text://编辑文字
                    currentTool = new EditText(mapControl, element);
                    break;

                case MapFrame.Core.Model.ElementTypeEnum.Other:
                    break;
            }

            if (currentTool != null)
            {
                currentTool.RunCommond();//执行命令
                (currentTool as MapFrame.Core.Interface.IMFDraw).MapLogic = mapLogic;
                currentTool.CommondExecutedEvent += new EventHandler<Core.Model.MessageEventArgs>(currentTool_CommondExecutedEvent);
            }
        }

        private void currentTool_CommondExecutedEvent(object sender, Core.Model.MessageEventArgs e)
        {
            if (CommondExecutedEvent != null)
            {
                CommondExecutedEvent(sender, e);
            }
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="elementName"></param>
        public void EditElement(string elementName)
        {
            MapFrame.Core.Interface.IMFElement element = mapLogic.GetElement(elementName);
            if (element == null) return;
            EditElement(element);
        }

        public void DrawGraphic(Core.Model.ElementTypeEnum type)
        {
            ReleaseTool();//释放之前的工具
            switch (type)
            {
                case Core.Model.ElementTypeEnum.Point://绘制点

                    break;
                case Core.Model.ElementTypeEnum.Line://绘制线
                    currentTool = new DrawLine(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Polygon://绘制面
                    currentTool = new DrawPolygon(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Rectangle://绘制矩形
                    currentTool = new DrawRectangle(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Circle://绘制圆
                    currentTool = new DrawCircle(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Text://绘制文字
                    currentTool = new DrawText(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Other://绘制其他

                    break;
            }

            if (currentTool != null)
            {
                (currentTool as MapFrame.Core.Interface.IMFDraw).MapLogic = mapLogic;
                currentTool.CommondExecutedEvent += new EventHandler<Core.Model.MessageEventArgs>(currentTool_CommondExecutedEvent);
                currentTool.RunCommond();//执行命令
            }
        }

        /// <summary>
        /// 获取当前正在使用的工具
        /// </summary>
        /// <returns></returns>
        public MapFrame.Core.Interface.IMFTool GetCurrentTool()
        {
            return currentTool;
        }

        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        /// <returns></returns>
        public Core.Model.MapLngLat SceneToGeographyPoint(int x, int y)
        {
            IPoint point = mapControl.ToMapPoint(x, y);
            Core.Model.MapLngLat lnglat = new Core.Model.MapLngLat(point.X, point.Y);
            return lnglat;
        }

        /// <summary>
        /// 地理坐标转化为屏幕坐标
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        /// <returns></returns>
        public System.Drawing.Point GeographyToScenePoint(double lon, double lat, double alt = 0)
        {
            int x = 0, y = 0;
            IPoint p = new PointClass();
            p.PutCoords(lon, lat);
            mapControl.FromMapPoint(p, ref x, ref y);
            System.Drawing.Point point = new System.Drawing.Point(x, y);
            return point;
        }
    }
}
