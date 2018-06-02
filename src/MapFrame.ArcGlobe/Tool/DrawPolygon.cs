
using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Analyst3D;
using MapFrame.Core.Interface;
using System.Drawing;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Tool
{
    class DrawPolygon : IMFTool, IMFDraw
    {
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 面的点集合
        /// </summary>
        private List<MapLngLat> pointList = null;
        /// <summary>
        /// 当前生成的面
        /// </summary>
        private IMFElement polygonElement = null;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// kml
        /// </summary>
        private Kml kml = null;
        /// <summary>
        /// 多边形kml
        /// </summary>
        private KmlPolygon polygonKml = null;
        /// <summary>
        /// 添加图元是否成功
        /// </summary>
        private bool drawn = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public DrawPolygon(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        #region  IDraw

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取绘制后的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return polygonElement;
        }

        #endregion

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = mapLogic.AddLayer("draw_layer");
            pointList = new List<MapLngLat>();
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
        }

        /// <summary>
        /// 鼠标双击完成绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnDoubleClick(object sender, IGlobeControlEvents_OnDoubleClickEvent e)
        {
            RegistCommondExecutedEvent();
            kml = null;
            if (pointList != null)
                pointList.Clear();
        }

        /// <summary>
        /// 键盘监听事件Esc取消绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IGlobeControlEvents_OnKeyDownEvent e)
        {
            if (e.keyCode == (int)ConsoleKey.Escape)
            {
                ReleaseCommond();
                if (drawn) layer.RemoveElement(polygonElement);
            }
        }
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseDown(object sender, IGlobeControlEvents_OnMouseDownEvent e)
        {
            if (e.button == 1) //鼠标左键
            {
                MapLngLat lnglat = this.SceneToGeographyPoint(e.x, e.y);
                lnglat.Alt = lnglat.Alt * -1;
                pointList.Add(lnglat);
                if (pointList.Count == 2)
                {
                    kml = new Kml();
                    kml.Placemark.Name = "辅助线";
                    KmlLineString lineKml = new KmlLineString();
                    lineKml.Rasterize = true;
                    lineKml.Description = "辅助线";
                    lineKml.PositionList = pointList;
                    lineKml.Color = Color.FromArgb(70, Color.Orange);
                    kml.Placemark.Graph = lineKml;
                    layer.AddElement(kml);

                    kml.Placemark.Name = "绘制面" + Utils.Index;
                    polygonKml = new KmlPolygon();
                    polygonKml.Rasterize = true;
                    polygonKml.Description = "手动绘制的面";
                    polygonKml.FillColor = Color.FromArgb(70, Color.Orange);
                    kml.Placemark.Graph = polygonKml;
                }
                else if (pointList.Count == 3)                      //更新面
                {
                    layer.RemoveElement("辅助线");
                    polygonKml.PositionList = pointList;
                    kml.Placemark.Graph = polygonKml;
                    drawn = layer.AddElement(kml, out polygonElement);
                }
                else if (pointList.Count > 3)
                {
                    layer.RemoveElement(polygonElement);
                    polygonKml.PositionList = pointList;
                    kml.Placemark.Graph = polygonKml;
                    drawn = layer.AddElement(kml, out polygonElement);
                }
                layer.Refresh();
            }
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.Navigate = true;
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    ToolType = ToolTypeEnum.Draw,
                    Describe = "手动绘制多边形，数据返回为多边形对象",
                    Data = polygonElement
                };
                CommondExecutedEvent(this, msg);
            }
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
            MapLngLat lnglat = new MapLngLat(dLon, dLat, dAlt);
            return lnglat;
        }

        /// <summary>
        /// 地理坐标转化为屏幕坐标
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        /// <returns></returns>
        public Point GeographyToScenePoint(double lon, double lat, double alt = 0)
        {
            int x, y;
            IGlobeViewUtil globeViewUtil = mapControl.GlobeCamera as IGlobeViewUtil;
            IGlobeDisplay pGlobeDisplay = mapControl.GlobeDisplay;
            globeViewUtil.GeocentricToWindow(lon, lat, alt, out x, out y);
            Point point = new Point(x, y);
            return point;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            mapLogic = null;
            mapControl = null;
            layer = null;
            pointList = null;
            polygonElement = null;
            CommondExecutedEvent = null;
            kml = null;
            polygonKml = null;
            drawn = false;
        }
    }
}
