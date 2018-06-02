
/**************************************************************************
 * 类名：ToolBox.cs
 * 描述：工具箱
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/
using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geometry;
using MapFrame.Core.Model;
using MapFrame.Core.Interface;

namespace MapFrame.ArcGlobe.Tool
{
    /// <summary>
    /// 工具箱
    /// </summary>
    public class ToolBox : IMFToolBox
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 当前的工具命令
        /// </summary>
        private IMFTool currentTool = null;
        /// <summary>
        /// 逻辑接口
        /// </summary>
        private IMapLogic mapLogic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapObject">地图对象</param>
        /// <param name="_mapLogic">逻辑对象</param>
        public ToolBox(object mapObject, IMapLogic _mapLogic)
        {
            mapControl = mapObject as AxGlobeControl; ;
            mapLogic = _mapLogic;
        }

        /// <summary>
        /// 漫游
        /// </summary>
        public void Roam(bool isDrag = true)
        {
            mapControl.Navigate = true;
        }

        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomIn()
        {
            ICommand command = new ControlsGlobeFixedZoomInCommandClass();
            command.OnCreate(mapControl.Object);
            command.OnClick();
            mapControl.CurrentTool = command as ITool;

        }

        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomOut()
        {
            ICommand command = new ControlsGlobeFixedZoomOutCommandClass();
            command.OnCreate(mapControl.Object);
            command.OnClick();
            mapControl.CurrentTool = command as ITool;
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
        /// <param name="lngLat"></param>
        /// <param name="zoomLevel"></param>
        public void ZoomToPosition(Core.Model.MapLngLat lngLat, int? zoomLevel = null)
        {
            ISceneViewer m_ActiveView = mapControl.Globe.GlobeDisplay.ActiveViewer;
            IEnvelope enve = new EnvelopeClass();

            enve.PutCoords(lngLat.Lng, lngLat.Lat, lngLat.Lng, lngLat.Lat);
            enve.ZMin = lngLat.Alt*10;
            enve.ZMax = lngLat.Alt*10;
            mapControl.GlobeCamera.SetToZoomToExtents(enve, mapControl.Globe, m_ActiveView);
            m_ActiveView.Redraw(false);
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="measureType"></param>
        public void Measure(Core.Model.MeasureTypeEnum measureType)
        {
            ICommand command = null;
            switch (measureType)
            {
                case Core.Model.MeasureTypeEnum.Angle:
                    break;
                case Core.Model.MeasureTypeEnum.Area:
                    command = new ControlsGlobeMeasureToolClass();
                    command.OnCreate(mapControl.Object);
                    //command.OnClick();
                    mapControl.CurrentTool = command as ITool;

                    break;
                case Core.Model.MeasureTypeEnum.Distance:
                    command = new ControlsGlobeMeasureToolClass();
                    command.OnCreate(mapControl.Object);
                    //command.OnClick();
                    mapControl.CurrentTool = command as ITool;

                    break;
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        public void Select()
        {

        }

        /// <summary>
        /// 重置工具
        /// </summary>
        public void ReleaseTool()
        {
            if (currentTool != null)
            {
                currentTool.ReleaseCommond();
                currentTool.CommondExecutedEvent -= currentTool_CommondExecutedEvent;
                currentTool.Dispose();
                currentTool = null;
            }
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="element">图元</param>
        public void EditElement(Core.Interface.IMFElement element)
        {
            switch (element.ElementType) 
            {
                case Core.Model.ElementTypeEnum.Circle://圆
                    break;
                case Core.Model.ElementTypeEnum.Line://线
                    break;
                case Core.Model.ElementTypeEnum.Model3D://模型
                    break;
                case Core.Model.ElementTypeEnum.Other://其他
                    break;
                case Core.Model.ElementTypeEnum.Picture://图片
                    break;
                case Core.Model.ElementTypeEnum.Point://点
                    break;
                case Core.Model.ElementTypeEnum.Polygon://面
                    currentTool = new DrawPolygon(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Rectangle://正方形
                    break;
                case Core.Model.ElementTypeEnum.Text://文字
                    break;
            }
            if (currentTool != null)
            {
                currentTool.RunCommond();
                currentTool.CommondExecutedEvent += currentTool_CommondExecutedEvent;
            }
        }

        /// <summary>
        /// 工具命令执行完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentTool_CommondExecutedEvent(object sender, MessageEventArgs e)
        {
            this.CommondExecutedEvent(sender, e);
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        public void EditElement(string elementName)
        {
            MapFrame.Core.Interface.IMFElement element = mapLogic.GetElement(elementName);
            EditElement(element);
        }

        /// <summary>
        /// 画图形
        /// </summary>
        /// <param name="type">图形类型</param>
        public void DrawGraphic(ElementTypeEnum type)
        {
            ReleaseTool();

            switch (type)
            {
                case Core.Model.ElementTypeEnum.Model3D:
                    break;
                case Core.Model.ElementTypeEnum.Point:
                    currentTool = new DrawPoint(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Line:
                    currentTool = new DrawLine(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Polygon:
                    currentTool = new DrawPolygon(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Circle:
                    currentTool = new DrawCircle(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Picture:
                    break;
                case Core.Model.ElementTypeEnum.Rectangle:
                    currentTool = new DrawRectangle(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Text:
                    currentTool = new DrawText(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Other:
                    break;
            }
            if (currentTool != null) 
            {
                (currentTool as IMFDraw).MapLogic = mapLogic;
                currentTool.CommondExecutedEvent += currentTool_CommondExecutedEvent;
                currentTool.RunCommond();
            }
        }

        /// <summary>
        /// 获取当前工具
        /// </summary>
        /// <returns></returns>
        public IMFTool GetCurrentTool()
        {
            return currentTool;
        }

        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        /// <returns></returns>
        public MapLngLat SceneToGeographyPoint(int x, int y)
        {
            double dLat, dLon, dAlt;
            IGlobeDisplay pGlobeDisplay = mapControl.GlobeDisplay;
            ISceneViewer pViewer = mapControl.GlobeDisplay.ActiveViewer;
            IGlobeViewUtil pGlobeViewUtil = mapControl.GlobeCamera as IGlobeViewUtil;

            pGlobeViewUtil.WindowToGeographic(pGlobeDisplay, pViewer, x, y, true, out dLon, out dLat, out dAlt);
            Core.Model.MapLngLat lnglat = new Core.Model.MapLngLat(dLon, dLat, dAlt);
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
            int x, y;
            IGlobeViewUtil globeViewUtil = mapControl.GlobeCamera as IGlobeViewUtil;
            IGlobeDisplay pGlobeDisplay = mapControl.GlobeDisplay;
            globeViewUtil.GeocentricToWindow(lon, lat, alt, out x, out y);
            System.Drawing.Point point = new System.Drawing.Point(x, y);
            return point;
        }
    }
}
