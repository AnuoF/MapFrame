/**************************************************************************
 * 类名：FactoryGMap.cs
 * 描述：GMap工厂类
 * 作者：Allen
 * 日期：July 11,2016
 * 
 * ************************************************************************/

using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// GMap工厂类
    /// </summary>
    public class FactoryGMap : IMapFactory, IMFMap
    {
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private GMapControl mapControl = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private LayerManger lyMgr = null;
        /// <summary>
        /// 点工厂
        /// </summary>
        private PointFactory pointFac = null;
        /// <summary>
        /// 图片工厂
        /// </summary>
        private PictureFactory pictureFac = null;
        /// <summary>
        /// 线工厂
        /// </summary>
        private LineFactory lineFac = null;
        /// <summary>
        /// 面工厂
        /// </summary>
        private PolygonFactory polygonFac = null;
        /// <summary>
        /// 文字工厂
        /// </summary>
        private TextFactory textFac = null;
        /// <summary>
        /// 圆图元
        /// </summary>
        private CircleFactory circleFac = null;
        /// <summary>
        /// 刷新间隔timer
        /// </summary>
        private System.Timers.Timer refreshTimer = null;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 上次刷新记录的事件
        /// </summary>
        private DateTime lastTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapObject">地图控件对象</param>
        public FactoryGMap(object _mapObject)
        {
            lastTime = DateTime.Now;
            if (_mapObject != null)
            {
                mapControl = _mapObject as GMapControl;
            }
            else
            {
                InitMapControl();    // 初始化地图控件

                if (InitFinishEvent != null)
                {
                    InitFinishEvent.Invoke(this, EventArgs.Empty);
                }
            }

            mapControl.MouseClick += new MouseEventHandler(mapControl_MouseClick);
            mapControl.MouseMove += new MouseEventHandler(mapControl_MouseMove);
            mapControl.MouseDown += new MouseEventHandler(mapControl_MouseDown);
            mapControl.MouseUp += new MouseEventHandler(mapControl_MouseUp);
            mapControl.MouseDoubleClick += new MouseEventHandler(mapControl_MouseDoubleClick);
            mapControl.OnMarkerClick += new MarkerClick(mapControl_OnMarkerClick);
            mapControl.OnPolygonClick += new PolygonClick(mapControl_OnPolygonClick);
            mapControl.OnRouteClick += new RouteClick(mapControl_OnRouteClick);
            mapControl.OnMarkerEnter += new MarkerEnter(mapControl_OnMarkerEnter);
            mapControl.OnMarkerLeave += new MarkerLeave(mapControl_OnMarkerLeave);
            mapControl.OnRouteEnter += new RouteEnter(mapControl_OnRouteEnter);
            mapControl.OnRouteLeave += new RouteLeave(mapControl_OnRouteLeave);
            mapControl.OnPolygonEnter += new PolygonEnter(mapControl_OnPolygonEnter);
            mapControl.OnPolygonLeave += new PolygonLeave(mapControl_OnPolygonLeave);
            mapControl.KeyDown += new KeyEventHandler(mapControl_KeyDown);
            mapControl.KeyPress += new KeyPressEventHandler(mapControl_KeyPress);
            mapControl.KeyUp += new KeyEventHandler(mapControl_KeyUp);
            mapControl.OnMapZoomChanged += new MapZoomChanged(mapControl_OnMapZoomChanged);

            lyMgr = new LayerManger(mapControl, this);
            pointFac = new PointFactory();
            pictureFac = new PictureFactory();
            lineFac = new LineFactory();
            polygonFac = new PolygonFactory();
            textFac = new TextFactory();
            circleFac = new CircleFactory();

            refreshTimer = new System.Timers.Timer();
            refreshTimer.Interval = 500;
            refreshTimer.Elapsed += refreshTimer_Elapsed;

        }

     

        #region MapControl

        /// <summary>
        /// 获取地图控件
        /// </summary>
        /// <returns></returns>
        public object GetMapControl()
        {
            return mapControl;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            RefreshEx();
        }

        /// <summary>
        /// 刷新指定图层
        /// </summary>
        /// <param name="layer">图层</param>
        public void Refresh(IMFLayer layer)
        {
            RefreshEx();
        }

        private void RefreshEx()
        {
            TimeSpan span = DateTime.Now - lastTime;
            if (span.TotalMilliseconds < refreshTimer.Interval)
            {
                refreshTimer.Start();
            }
            else
            {
                // 单位时间内，只执行一次，且是在单位时间结束时执行
                if (mapControl.InvokeRequired)
                {
                    mapControl.Invoke(new Action(delegate
                    {
                        mapControl.Refresh();
                    }));
                }
                else
                {
                    mapControl.Refresh();
                }
            }

            lastTime = DateTime.Now;
        }

        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke(new Action(delegate
                {
                    mapControl.Refresh();
                }));
            }
            else
            {
                mapControl.Refresh();
            }
            if ((DateTime.Now - lastTime).TotalMilliseconds > refreshTimer.Interval)
            {
                refreshTimer.Stop();
            }

        }
        #endregion

        #region Layer
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool AddLayer(string layerName)
        {
            return lyMgr.AddLayer(layerName);
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool RemoveLayer(string layerName)
        {
            return lyMgr.RemoverLayer(layerName);
        }

        /// <summary>
        /// 移除所有图层
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllLayer()
        {
            return lyMgr.RemoveAllLayer();
        }

        /// <summary>
        /// 清除当前图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ClearLayer(string layerName)
        {
            lyMgr.ClearLayer(layerName);
        }

        /// <summary>
        /// 清除所有图层
        /// </summary>
        public void ClearLayer()
        {
            lyMgr.ClearLayer();
        }

        /// <summary>
        /// 显示、隐藏图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="visible"></param>
        public void SetLayerVisiable(string layerName, bool visible)
        {
            lyMgr.ShowLayer(layerName, visible);
        }
        #endregion

        #region Element
        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="kml">Kml对象</param>
        /// <returns></returns>
        public IMFElement AddElement(string layerName, Kml kml)
        {
            if (kml == null) return null;
            if (kml.Placemark == null) return null;
            if (kml.Placemark.Graph == null) return null;
            if (string.IsNullOrEmpty(kml.Placemark.Name)) return null;

            var layer = lyMgr.GetLayer(layerName);
            if (layer == null) return null;

            IElementFactory elementFactory = null;
            Type type = kml.Placemark.Graph.GetType();

            // 点
            if (type == typeof(KmlPoint))
            {
                elementFactory = pointFac;
            }
            // 线
            else if (type == typeof(KmlLineString))
            {
                elementFactory = lineFac;
            }
            // 面
            else if (type == typeof(KmlPolygon))
            {
                elementFactory = polygonFac;
            }
            // 文字
            else if (type == typeof(KmlText))
            {
                elementFactory = textFac;
            }
            // 圆
            else if (type == typeof(KmlCircle))
            {
                elementFactory = circleFac;
            }
            else if (type == typeof(KmlPicture))
            {
                elementFactory = pictureFac;
            }

            if (elementFactory == null) return null;

            IMFElement element = elementFactory.CreateElement(kml, layer);
            if (element != null)
            {
                element.ElementName = kml.Placemark.Name;
            }

            return element;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="element">图元</param>
        /// <returns></returns>
        public bool RemoveElement(string layerName, IMFElement element)
        {
            if (element == null) return true;
            var layer = lyMgr.GetLayer(layerName);
            if (layer == null) return true;
            IElementFactory elementFactory = null;

            switch (element.ElementType)
            {
                case ElementTypeEnum.Point:
                    elementFactory = pointFac;
                    break;

                case ElementTypeEnum.Picture:
                    elementFactory = pictureFac;
                    break;

                case ElementTypeEnum.Line:
                    elementFactory = lineFac;
                    break;

                case ElementTypeEnum.Polygon:
                    elementFactory = polygonFac;
                    break;

                case ElementTypeEnum.Text:
                    elementFactory = textFac;
                    break;

                case ElementTypeEnum.Circle:
                    elementFactory = circleFac;
                    break;
            }

            if (elementFactory == null) return false;

            bool ret = elementFactory.RemoveElement(element, layer);
            element.Dispose();
            return ret;
        }


        #endregion

        #region IMFMap

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseClickEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseMoveEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseDownEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseUpEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseDbClickEvent;

        public event EventHandler<Core.Model.MFElementEnterEventArgs> ElementEnterEvent;

        public event EventHandler<Core.Model.MFElementLeaveEventArgs> ElementLeaveEvent;

        public event EventHandler<Core.Model.MFElementClickEventArgs> ElementClickEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyDownEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyUpEvent;

        public event EventHandler<MFKeyPressEventArgs> KeyPressEvent;

        public event EventHandler<EventArgs> InitFinishEvent;

        public event EventHandler<MapZoomChangedEventArgs> MapZoomChangedEvent;


        /// <summary>
        /// 地图控件
        /// </summary>
        public object MapControl
        {
            get
            {
                return mapControl;
            }
        }

        /// <summary>
        /// 获取地图接口对象
        /// </summary>
        /// <returns></returns>
        public IMFMap GetIMFMap()
        {
            return this;
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapFile"></param>
        public void LoadMap(string mapFile)
        {
            mapControl.CacheLocation = mapFile;
        }

        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapLngLat FromLocalToLngLat(int x, int y)
        {
            if (mapControl != null)
            {
                var p = mapControl.FromLocalToLatLng(x, y);
                return new MapLngLat(p.Lng, p.Lat);
            }
            return null;
        }

        /// <summary>
        /// 地理坐标转屏幕坐标
        /// </summary>
        /// <param name="position">地理坐标</param>
        /// <returns>屏幕坐标</returns>
        public System.Drawing.Point FromLngLatToLocal(MapLngLat position)
        {
            var p = mapControl.FromLatLngToLocal(new PointLatLng(position.Lat, position.Lng));
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }

        /// <summary>
        /// 地图快照
        /// </summary>
        /// <returns>图片</returns>
        public System.Drawing.Image Snapshot()
        {
            return mapControl.ToImage();
        }

        #endregion

        #region Private Function

        private void mapControl_OnMapZoomChanged()
        {
            MapZoomChangedEventArgs e = new MapZoomChangedEventArgs();
            e.Zoom = mapControl.Zoom;

            if (MapZoomChangedEvent != null)
            {
                MapZoomChangedEvent.Invoke(this, e);
            }
        }

        // 地图控件键盘弹起事件
        private void mapControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            MFKeyEventArgs args = new MFKeyEventArgs();
            args.KeyEventArgs = e;
            if (KeyUpEvent != null)
            {
                KeyUpEvent.Invoke(this, args);
            }
        }

        // 地图控件键盘按键事件
        private void mapControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            MFKeyPressEventArgs args = new MFKeyPressEventArgs();
            args.KeyPressEventArgs = e;
            if (KeyPressEvent != null)
            {
                KeyPressEvent.Invoke(this, args);
            }
        }

        // 地图控件按键按下事件
        private void mapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            MFKeyEventArgs args = new MFKeyEventArgs();
            args.KeyEventArgs = e;
            if (KeyUpEvent != null)
            {
                KeyUpEvent.Invoke(this, args);
            }
        }

        // 地图控件鼠标双击事件
        private void mapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            var p = FromLocalToLngLat(e.X, e.Y);
            MFMouseEventArgs args = new MFMouseEventArgs(p, e.Button, e.X, e.Y);
            if (MouseDbClickEvent != null)
            {
                MouseDbClickEvent.Invoke(this, args);
            }
        }

        // 鼠标弹起事件
        private void mapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            var p = FromLocalToLngLat(e.X, e.Y);
            MFMouseEventArgs args = new MFMouseEventArgs(p, e.Button, e.X, e.Y);
            if (MouseUpEvent != null)
            {
                MouseUpEvent.Invoke(this, args);
            }
        }

        // 鼠标按下事件
        private void mapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            var p = FromLocalToLngLat(e.X, e.Y);
            MFMouseEventArgs args = new MFMouseEventArgs(p, e.Button, e.X, e.Y);
            if (MouseDownEvent != null)
            {
                MouseDownEvent.Invoke(this, args);
            }
        }

        // 鼠标点击地图控件事件
        private void mapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            var p = FromLocalToLngLat(e.X, e.Y);
            MFMouseEventArgs args = new MFMouseEventArgs(p, e.Button, e.X, e.Y);
            if (MouseClickEvent != null)
            {
                MouseClickEvent.Invoke(this, args);
            }
        }

        // 鼠标移动事件
        private void mapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;

            var p = FromLocalToLngLat(e.X, e.Y);
            MFMouseEventArgs args = new MFMouseEventArgs(p, e.Button, e.X, e.Y);
            if (MouseMoveEvent != null)
            {
                MouseMoveEvent.Invoke(this, args);
            }
        }

        // 鼠标点击线事件
        private void mapControl_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;      // 在使用工具时不需要向外发布事件
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;

            MFElementClickEventArgs args = new MFElementClickEventArgs();
            args.Element = element;
            args.MouseEventArgs = e;
            if (ElementClickEvent != null)
            {
                ElementClickEvent.Invoke(this, args);
            }
        }

        // 鼠标点击多边形事件
        private void mapControl_OnPolygonClick(GMapPolygon item, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;

            MFElementClickEventArgs args = new MFElementClickEventArgs();
            args.Element = element;
            args.MouseEventArgs = e;
            if (ElementClickEvent != null)
            {
                ElementClickEvent.Invoke(this, args);
            }
        }

        // 鼠标点击点事件
        private void mapControl_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;

            MFElementClickEventArgs args = new MFElementClickEventArgs();
            args.Element = element;
            args.MouseEventArgs = e;
            if (ElementClickEvent != null)
            {
                ElementClickEvent.Invoke(this, args);
            }
        }

        // 鼠标进入点事件
        private void mapControl_OnMarkerEnter(GMapMarker item)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;
            MFElementEnterEventArgs args = new MFElementEnterEventArgs();
            args.Element = element;
            if (ElementEnterEvent != null)
            {
                ElementEnterEvent.Invoke(this, args);
            }
        }

        // 鼠标离开点事件
        private void mapControl_OnMarkerLeave(GMapMarker item)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;
            MFElementLeaveEventArgs args = new MFElementLeaveEventArgs();
            args.Element = element;
            if (ElementEnterEvent != null)
            {
                ElementLeaveEvent.Invoke(this, args);
            }
        }

        // 鼠标进入多边形事件
        void mapControl_OnPolygonEnter(GMapPolygon item)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;
            MFElementEnterEventArgs args = new MFElementEnterEventArgs();
            args.Element = element;
            if (ElementEnterEvent != null)
            {
                ElementEnterEvent.Invoke(this, args);
            }
        }

        // 鼠标离开多边形事件
        void mapControl_OnPolygonLeave(GMapPolygon item)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;
            MFElementLeaveEventArgs args = new MFElementLeaveEventArgs();
            args.Element = element;
            if (ElementEnterEvent != null)
            {
                ElementLeaveEvent.Invoke(this, args);
            }
        }

        // 鼠标进入线事件
        void mapControl_OnRouteEnter(GMapRoute item)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;
            MFElementEnterEventArgs args = new MFElementEnterEventArgs();
            args.Element = element;
            if (ElementEnterEvent != null)
            {
                ElementEnterEvent.Invoke(this, args);
            }
        }

        // 鼠标离开线事件
        void mapControl_OnRouteLeave(GMapRoute item)
        {
            if (MapFrame.GMap.Common.Utils.bPublishEvent == false) return;
            if (item.Tag == null) return;

            IMFElement element = item.Tag as IMFElement;
            MFElementLeaveEventArgs args = new MFElementLeaveEventArgs();
            args.Element = element;
            if (ElementEnterEvent != null)
            {
                ElementLeaveEvent.Invoke(this, args);
            }
        }

        /// <summary>
        /// 初始化地图控件
        /// </summary>
        private void InitMapControl()
        {
            try
            {
                mapControl = new GMapControl();

                XmlDocument doc = new XmlDocument();
                string xmlPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "MapFrame.Config.xml");
                doc.Load(xmlPath);

                int minZoom = 0;
                int maxZoom = 18;
                string cacheLocation = "";
                GMapProvider mapProvider;
                int zoom = 0;
                bool showCenter = false;
                PointLatLng position;
                MouseButtons dragButton;
                bool markersEnabled = true;
                bool ignoreMarkerOnMouseWheel = true;
                bool mouseWheelZoomEnabled = true;
                bool routesEnabled = true;
                bool showTileGridLines = false;
                bool polygonsEnabled = true;
                AccessMode mode = AccessMode.ServerAndCache;

                XmlNode node = null;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/MinZoom");
                minZoom = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("MapFrame/Config/GMap/MaxZoom");
                maxZoom = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("MapFrame/Config/GMap/CacheLocation");
                //cacheLocation = node.InnerXml;       // 绝对路径
                cacheLocation = Path.Combine(Application.StartupPath, node.InnerXml);       // 相对路径
                node = doc.SelectSingleNode("MapFrame/Config/GMap/MapProvider");
                mapProvider = GetGMapProvider(node.InnerXml);
                node = doc.SelectSingleNode("MapFrame/Config/GMap/Zoom");
                zoom = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("MapFrame/Config/GMap/ShowCenter");
                showCenter = node.InnerXml == "false" ? false : true;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/Position");
                string[] arr = node.InnerXml.Split(new char[] { ',' });
                double lng = Convert.ToDouble(arr[0]);
                double lat = Convert.ToDouble(arr[1]);
                position = new PointLatLng(lat, lng);
                node = doc.SelectSingleNode("MapFrame/Config/GMap/DragButton");
                dragButton = node.InnerXml == "Left" ? MouseButtons.Left : MouseButtons.Right;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/MarkersEnabled");
                markersEnabled = node.InnerXml == "true" ? true : false;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/IgnoreMarkerOnMouseWheel");
                ignoreMarkerOnMouseWheel = node.InnerXml == "true" ? true : false;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/MouseWheelZoomEnabled");
                mouseWheelZoomEnabled = node.InnerXml == "true" ? true : false;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/RoutesEnabled");
                routesEnabled = node.InnerXml == "true" ? true : false;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/ShowTileGridLines");
                showTileGridLines = node.InnerXml == "false" ? false : true;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/PolygonsEnabled");
                polygonsEnabled = node.InnerXml == "true" ? true : false;
                node = doc.SelectSingleNode("MapFrame/Config/GMap/AccessMode");
                mode = GetAccessMode(node.InnerXml);

                mapControl.CacheLocation = cacheLocation;                         //缓存位置
                mapControl.MapProvider = mapProvider;                             //google china 地图
                mapControl.MinZoom = minZoom;                                     //最小比例
                mapControl.MaxZoom = maxZoom;                                     //最大比例
                mapControl.Zoom = zoom;                                           //当前比例
                mapControl.ShowCenter = showCenter;                               //不显示中心十字点
                mapControl.DragButton = dragButton;                               //左键拖拽地图
                mapControl.Position = position;                                   //地图中心位置：成都
                mapControl.MarkersEnabled = true;
                mapControl.MarkersEnabled = markersEnabled;
                mapControl.IgnoreMarkerOnMouseWheel = ignoreMarkerOnMouseWheel;
                mapControl.MouseWheelZoomEnabled = mouseWheelZoomEnabled;
                mapControl.RoutesEnabled = routesEnabled;
                mapControl.ShowTileGridLines = showTileGridLines;
                mapControl.PolygonsEnabled = polygonsEnabled;
                mapControl.Manager.Mode = mode;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 转换地图获取方式
        /// </summary>
        /// <param name="modeStr"></param>
        /// <returns></returns>
        private AccessMode GetAccessMode(string modeStr)
        {
            AccessMode mode;

            switch (modeStr)
            {
                case "CacheOnly":
                    mode = AccessMode.CacheOnly;
                    break;
                case "ServerAndCache":
                    mode = AccessMode.ServerAndCache;
                    break;
                case "ServerOnly":
                    mode = AccessMode.ServerOnly;
                    break;
                default:
                    mode = AccessMode.ServerAndCache;
                    break;
            }
            return mode;
        }

        /// <summary>
        /// 转换为地图提供者对象
        /// </summary>
        /// <param name="mapType"></param>
        /// <returns></returns>
        private GMapProvider GetGMapProvider(string mapType)
        {
            GMapProvider gmapProvider = null;

            switch (mapType)
            {
                case "AMap":
                    gmapProvider = GMapProviders.AMap;
                    break;
                case "ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_Map":
                    gmapProvider = GMapProviders.ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_Map;
                    break;
                case "ArcGIS_Imagery_World_2D_Map":
                    gmapProvider = GMapProviders.ArcGIS_Imagery_World_2D_Map;
                    break;
                case "ArcGIS_ShadedRelief_World_2D_Map":
                    gmapProvider = GMapProviders.ArcGIS_ShadedRelief_World_2D_Map;
                    break;
                case "ArcGIS_StreetMap_World_2D_Map":
                    gmapProvider = GMapProviders.ArcGIS_StreetMap_World_2D_Map;
                    break;
                case "ArcGIS_Topo_US_2D_Map":
                    gmapProvider = GMapProviders.ArcGIS_Topo_US_2D_Map;
                    break;
                case "ArcGIS_World_Physical_Map":
                    gmapProvider = GMapProviders.ArcGIS_World_Physical_Map;
                    break;
                case "ArcGIS_World_Shaded_Relief_Map":
                    gmapProvider = GMapProviders.ArcGIS_World_Shaded_Relief_Map;
                    break;
                case "ArcGIS_World_Street_Map":
                    gmapProvider = GMapProviders.ArcGIS_World_Street_Map;
                    break;
                case "ArcGIS_World_Terrain_Base_Map":
                    gmapProvider = GMapProviders.ArcGIS_World_Terrain_Base_Map;
                    break;
                case "ArcGIS_World_Topo_Map":
                    gmapProvider = GMapProviders.ArcGIS_World_Topo_Map;
                    break;
                case "BingHybridMap;":
                    gmapProvider = GMapProviders.BingHybridMap;
                    break;
                case "BingMap":
                    gmapProvider = GMapProviders.BingMap;
                    break;
                case "BingSatelliteMap":
                    gmapProvider = GMapProviders.BingSatelliteMap;
                    break;
                case "CloudMadeMap":
                    gmapProvider = GMapProviders.CloudMadeMap;
                    break;
                case "CzechGeographicMap":
                    gmapProvider = GMapProviders.CzechGeographicMap;
                    break;
                case "CzechHistoryMap":
                    gmapProvider = GMapProviders.CzechHistoryMap;
                    break;
                case "CzechHistoryOldMap":
                    gmapProvider = GMapProviders.CzechHistoryOldMap;
                    break;
                case "CzechHybridMap":
                    gmapProvider = GMapProviders.CzechHybridMap;
                    break;
                case "CzechHybridOldMap":
                    gmapProvider = GMapProviders.CzechHybridOldMap;
                    break;
                case "CzechMap":
                    gmapProvider = GMapProviders.CzechMap;
                    break;
                case "CzechOldMap":
                    gmapProvider = GMapProviders.CzechOldMap;
                    break;
                case "CzechSatelliteMap":
                    gmapProvider = GMapProviders.CzechSatelliteMap;
                    break;
                case "CzechSatelliteOldMap":
                    gmapProvider = GMapProviders.CzechSatelliteOldMap;
                    break;
                case "CzechTuristMap":
                    gmapProvider = GMapProviders.CzechTuristMap;
                    break;
                case "CzechTuristOldMap":
                    gmapProvider = GMapProviders.CzechTuristOldMap;
                    break;
                case "CzechTuristWinterMap":
                    gmapProvider = GMapProviders.CzechTuristWinterMap;
                    break;
                case "EmptyProvider":
                    gmapProvider = GMapProviders.EmptyProvider;
                    break;
                case "GoogleChinaHybridMap":
                    gmapProvider = GMapProviders.GoogleChinaHybridMap;
                    break;
                case "GoogleChinaMap":
                    gmapProvider = GMapProviders.GoogleChinaMap;
                    break;
                case "GoogleChinaSatelliteMap":
                    gmapProvider = GMapProviders.GoogleChinaSatelliteMap;
                    break;
                case "GoogleChinaTerrainMap":
                    gmapProvider = GMapProviders.GoogleChinaTerrainMap;
                    break;
                case "GoogleHybridMap":
                    gmapProvider = GMapProviders.GoogleHybridMap;
                    break;
                case "GoogleKoreaHybridMap":
                    gmapProvider = GMapProviders.GoogleKoreaHybridMap;
                    break;
                case "GoogleKoreaMap":
                    gmapProvider = GMapProviders.GoogleKoreaMap;
                    break;
                case "GoogleKoreaSatelliteMap":
                    gmapProvider = GMapProviders.GoogleKoreaSatelliteMap;
                    break;
                case "GoogleMap":
                    gmapProvider = GMapProviders.GoogleMap;
                    break;
                case "GoogleSatelliteMap":
                    gmapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case "GoogleTerrainMap":
                    gmapProvider = GMapProviders.GoogleTerrainMap;
                    break;
                case "LatviaMap":
                    gmapProvider = GMapProviders.LatviaMap;
                    break;
                case "Lithuania3dMap":
                    gmapProvider = GMapProviders.Lithuania3dMap;
                    break;
                case "LithuaniaHybridMap":
                    gmapProvider = GMapProviders.LithuaniaHybridMap;
                    break;
                case "LithuaniaHybridOldMap":
                    gmapProvider = GMapProviders.LithuaniaHybridOldMap;
                    break;
                case "LithuaniaMap":
                    gmapProvider = GMapProviders.LithuaniaMap;
                    break;
                case "LithuaniaOrtoFotoMap":
                    gmapProvider = GMapProviders.LithuaniaOrtoFotoMap;
                    break;
                case "LithuaniaOrtoFotoOldMap":
                    gmapProvider = GMapProviders.LithuaniaOrtoFotoOldMap;
                    break;
                case "LithuaniaReliefMap":
                    gmapProvider = GMapProviders.LithuaniaReliefMap;
                    break;
                case "LithuaniaTOP50Map":
                    gmapProvider = GMapProviders.LithuaniaTOP50Map;
                    break;
                case "MapBenderWMSdemoMap":
                    gmapProvider = GMapProviders.MapBenderWMSdemoMap;
                    break;
                case "Mgis":
                    gmapProvider = GMapProviders.Mgis;
                    break;
                case "NearHybridMap":
                    gmapProvider = GMapProviders.NearHybridMap;
                    break;
                case "NearMap":
                    gmapProvider = GMapProviders.NearMap;
                    break;
                case "OpenCycleLandscapeMap":
                    gmapProvider = GMapProviders.OpenCycleLandscapeMap;
                    break;
                case "OpenCycleMap":
                    gmapProvider = GMapProviders.OpenCycleMap;
                    break;
                case "OpenCycleTransportMap":
                    gmapProvider = GMapProviders.OpenCycleTransportMap;
                    break;
                case "OpenSeaMapHybrid":
                    gmapProvider = GMapProviders.OpenSeaMapHybrid;
                    break;
                case "OpenStreet4UMap":
                    gmapProvider = GMapProviders.OpenStreet4UMap;
                    break;
                case "OpenStreetMap":
                    gmapProvider = GMapProviders.OpenStreetMap;
                    break;
                case "OpenStreetMapQuest":
                    gmapProvider = GMapProviders.OpenStreetMapQuest;
                    break;
                case "OpenStreetMapQuestHybrid":
                    gmapProvider = GMapProviders.OpenStreetMapQuestHybrid;
                    break;
                case "OpenStreetMapQuestSattelite":
                    gmapProvider = GMapProviders.OpenStreetMapQuestSattelite;
                    break;
                case "OviHybridMap":
                    gmapProvider = GMapProviders.OviHybridMap;
                    break;
                case "OviMap":
                    gmapProvider = GMapProviders.OviMap;
                    break;
                case "OviSatelliteMap":
                    gmapProvider = GMapProviders.OviSatelliteMap;
                    break;
                case "OviTerrainMap":
                    gmapProvider = GMapProviders.OviTerrainMap;
                    break;
                case "SpainMap":
                    gmapProvider = GMapProviders.SpainMap;
                    break;
                case "SwedenMap":
                    gmapProvider = GMapProviders.SwedenMap;
                    break;
                case "TurkeyMap":
                    gmapProvider = GMapProviders.TurkeyMap;
                    break;
                case "WikiMapiaMap":
                    gmapProvider = GMapProviders.WikiMapiaMap;
                    break;
                case "YahooHybridMap":
                    gmapProvider = GMapProviders.YahooHybridMap;
                    break;
                case "YahooMap":
                    gmapProvider = GMapProviders.YahooMap;
                    break;
                case "YahooSatelliteMap":
                    gmapProvider = GMapProviders.YahooSatelliteMap;
                    break;
                case "YandexHybridMap":
                    gmapProvider = GMapProviders.YandexHybridMap;
                    break;
                case "YandexMap":
                    gmapProvider = GMapProviders.YandexMap;
                    break;
                case "YandexSatelliteMap":
                    gmapProvider = GMapProviders.YandexSatelliteMap;
                    break;
                default:
                    gmapProvider = GMapProviders.AMap;
                    break;
            }

            return gmapProvider;
        }
        #endregion






        public void SetRasterize(string layerName, bool bRasterize)
        {
            throw new NotImplementedException();
        }

        public void GetRasterize(string layerName)
        {
            throw new NotImplementedException();
        }


        public void SetTransparency(string layerName, short transparency)
        {
            throw new NotImplementedException();
        }


    }
}
