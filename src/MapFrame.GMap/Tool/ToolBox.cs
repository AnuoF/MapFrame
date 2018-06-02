/**************************************************************************
 * 类名：ToolBox.cs
 * 描述：工具箱
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Threading;
using System;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 工具箱
    /// </summary>
    public class ToolBox : IMFToolBox
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 地图逻辑控制接口
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 当前的工具命令
        /// </summary>
        private IMFTool currentTool = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapObject">地图对象</param>
        /// <param name="_mapLogic">地图逻辑类</param>
        public ToolBox(object mapObject, IMapLogic _mapLogic)
        {
            mapLogic = _mapLogic;
            gmapControl = mapObject as GMapControl;
        }

        /// <summary>
        /// 漫游
        /// </summary>
        public void Roam(bool isDrag = true)
        {
            ReleaseTool();
            gmapControl.CanDragMap = isDrag;
        }

        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomIn()
        {
            gmapControl.Zoom++;
        }

        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomOut()
        {
            gmapControl.Zoom--;
        }

        /// <summary>
        /// 全图显示
        /// </summary>
        public void FullView()
        {
            gmapControl.Zoom = gmapControl.MinZoom;
            gmapControl.Position = new PointLatLng(0, 0);
        }

        /// <summary>
        /// 定位至某点
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        /// <param name="zoomLevel">缩放级别，默认不缩放</param>
        public void ZoomToPosition(MapLngLat lngLat, int? zoomLevel = null)
        {
            PointLatLng latlng = new global::GMap.NET.PointLatLng(lngLat.Lat, lngLat.Lng);

            if (zoomLevel != null && zoomLevel > 0)
            {
                gmapControl.Zoom = (double)zoomLevel;

                // 启动动画效果
                ThreadPool.QueueUserWorkItem(obj =>
                {
                    using (ZoomToPosition zoomTo = new ZoomToPosition(gmapControl))
                    {
                        zoomTo.ZoomTo(latlng);
                    }
                });
            }

            if (gmapControl.InvokeRequired)
            {
                gmapControl.Invoke(new Action(delegate
                {
                    gmapControl.Position = latlng;
                }));
            }
            else
                gmapControl.Position = latlng;
        }

        /// <summary>
        /// 切换工具时释放上一次的工具命令
        /// </summary>
        public void ReleaseTool()
        {
            if (currentTool != null)
            {
                currentTool.ReleaseCommond();
                currentTool.CommondExecutedEvent -= CommondExecutedEvent;
                currentTool.Dispose();
                currentTool = null;
            }
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="measureType">测量类型，有距离、面积和方位角</param>
        public void Measure(MeasureTypeEnum measureType)
        {
            ReleaseTool();   // 执行命令前，先释放上一次的工具命令

            switch (measureType)
            {
                case MeasureTypeEnum.Distance:
                    currentTool = new MeasureDistance(gmapControl);
                    break;

                case MeasureTypeEnum.Area:
                    currentTool = new MeasureArea(gmapControl);
                    break;

                case MeasureTypeEnum.Angle:
                    currentTool = new MeasureAngle(gmapControl);
                    break;
                default:
                    break;
            }

            if (currentTool != null)
            {
                currentTool.CommondExecutedEvent += CommondExecutedEvent;
                currentTool.RunCommond();
            }
        }

        /// <summary>
        /// 框选目标
        /// </summary>
        public void Select()
        {
            ReleaseTool();
            currentTool = new SelectElementEx(gmapControl, mapLogic);
            currentTool.CommondExecutedEvent += CommondExecutedEvent;
            currentTool.RunCommond();
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        public void EditElement(string elementName)
        {
            IMFElement element = mapLogic.GetElement(elementName);
            if (element != null)
                EditElement(element);
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="element">图元</param>
        public void EditElement(IMFElement element)
        {
            //释放之前的工具
            ReleaseTool();

            switch (element.ElementType)
            {
                case ElementTypeEnum.Point://编辑点
                    currentTool = new EditPoint(gmapControl, element);
                    break;

                case ElementTypeEnum.Line://编辑线
                    currentTool = new EditLine(gmapControl, element);
                    break;

                case ElementTypeEnum.Polygon://编辑多边形
                    currentTool = new EditPolygon(gmapControl, element);
                    break;

                case ElementTypeEnum.Circle://编辑圆
                    currentTool = new EditCircleEx(gmapControl, element);
                    break;

                case ElementTypeEnum.Text://编辑文字
                    currentTool = new EditText(gmapControl, element);
                    break;

                case ElementTypeEnum.Other:
                    break;
            }

            if (currentTool != null)
            {
                currentTool.CommondExecutedEvent += this.CommondExecutedEvent;
                currentTool.RunCommond();//执行命令
            }
        }

        /// <summary>
        /// 绘制图元
        /// </summary>
        /// <param name="type">图元类型</param>
        public void DrawGraphic(ElementTypeEnum type)
        {
            ReleaseTool();//释放之前的工具

            switch (type)
            {
                case ElementTypeEnum.Line://画线
                    currentTool = new DrawLine(gmapControl);
                    break;

                case ElementTypeEnum.Polygon://画多边形
                    currentTool = new DrawPolygon(gmapControl);
                    break;

                case ElementTypeEnum.Rectangle://画矩形
                    currentTool = new DrawRectangle(gmapControl);
                    break;

                case ElementTypeEnum.Circle://画圆
                    currentTool = new DrawCircle(gmapControl);
                    break;

                case ElementTypeEnum.Text://文字
                    currentTool = new DrawText(gmapControl);
                    break;

                case ElementTypeEnum.Other://其他
                    break;
            }

            if (currentTool != null)
            {
                (currentTool as IMFDraw).MapLogic = mapLogic;
                currentTool.CommondExecutedEvent += CommondExecutedEvent;
                currentTool.RunCommond();//执行命令
            }
        }

        /// <summary>
        /// 获取当前工具接口对象
        /// </summary>
        /// <returns>ITool</returns>
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
            PointLatLng latlng = gmapControl.FromLocalToLatLng(x, y);
            Core.Model.MapLngLat lnglat = new MapLngLat(latlng.Lng, latlng.Lat);
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
            PointLatLng latlng = new PointLatLng(lat, lon);
            GPoint p = gmapControl.FromLatLngToLocal(latlng);
            System.Drawing.Point point = new System.Drawing.Point((int)p.X, (int)p.Y);
            return point;
        }

    }
}
