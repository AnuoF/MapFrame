
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.SystemUI;
using GlobleSituation.Business;
using GlobleSituation.Common;
using GlobleSituation.Model;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Xml;

namespace GlobleSituation.UI
{
    public partial class AxGlobeControlEx : XtraUserControl
    {
        public MapFrame.Core.Interface.IMapLogic mapLogic = null;              // 地图框架逻辑处理接口
        public ArcGlobeBusiness globeBusiness = null;                          // 三维地图业务处理类

        private IMFMap mfMap = null;                                           // 地图接口
        private TrackLineManager trackMgr = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="trackMgr"></param>
        public AxGlobeControlEx(TrackLineManager _trackMgr)
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            trackMgr = _trackMgr;
            this.Load += new EventHandler(AxGlobeControlEx_Load);

            InitializeComponent();
        }

        private void AxGlobeControlEx_Load(object sender, EventArgs e)
        {
            IGlobeDisplayEvents_Event m_GlobeDisplayEvents = axGlobeControl1.Globe.GlobeDisplay as IGlobeDisplayEvents_Event;
            this.MouseWheel += new MouseEventHandler(GlobleControl_MouseWheel);
            //将三维视图重绘事件委托给m_GlobeDisplayEvents_AfterDraw方法
            m_GlobeDisplayEvents.AfterDraw += new IGlobeDisplayEvents_AfterDrawEventHandler(GlobeControl_DisplayEvents_AfterDraw);
            axMapControl1.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(axMapControl1_OnMouseDown);
            axMapControl1.Extent = axMapControl1.FullExtent;

            // 初始化地图框架
            MapFrame.Logic.InitMapFrame mapFrame = new MapFrame.Logic.InitMapFrame(MapEngineType.ArcGlobe, axGlobeControl1);
            mapLogic = mapFrame.GetMapLogic();
            mfMap = mapLogic.GetIMFMap();

            Load3dFile();   // 加载3d文档
            LoadMxdFile();

            // 自定义显示鹰眼工具
            ShowEagleEyeCmd showEagleEyeCmd = new ShowEagleEyeCmd(this);
            axToolbarControl1.AddItem(showEagleEyeCmd, -1, 22, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            globeBusiness = new ArcGlobeBusiness(mapLogic, trackMgr);
            eagleEyePanel.Visible = false;

            InitBookmark();     // 初始化书签
        }

        // 处理态势数据
        public void DealTSData(RealData data)
        {
            globeBusiness.DealRealData(data);
        }

        // 处理预警数据
        public void DealWarnData(RealData data, bool isWarn, List<string> warnNames)
        {
            globeBusiness.DealWarnData(data, isWarn, warnNames);
        }

        /// <summary>
        /// 显示鹰眼  hide  visible
        /// </summary>
        public void ShowEagleEye(string visiable = "else")
        {
            if (visiable == "hide")
                eagleEyePanel.Visible = false;
            else if (visiable == "visible")
                eagleEyePanel.Visible = true;
            else if (visiable == "else")
                eagleEyePanel.Visible = !eagleEyePanel.Visible;
        }

        /// <summary>
        /// 显示工具条
        /// </summary>
        /// <param name="show"></param>
        public void ShowToolBar(bool show)
        {
            //axToolbarControl1.Visible = show;
        }

        // 加载地图
        public void Load3dFile(string _3dFile = "")
        {
            mfMap.LoadMap(_3dFile);
        }

        // 从服务器加载地图
        public void LoadTitleFromServer()
        {
            try
            {
                LoadLyrFile();

                globeBusiness.AddLayerInit();

                List<TileServer> imgServers = GetServerPath();
                if (imgServers == null || imgServers.Count <= 0) return;

                foreach (var server in imgServers)
                {
                    IImageServerLayer layer = new ImageServerLayerClass();
                    layer.SpatialReference = new SpatialReferenceEnvironmentClass().CreateProjectedCoordinateSystem(3857);
                    //layer.SpatialReference = new SpatialReferenceEnvironmentClass().CreateProjectedCoordinateSystem((int)esriSRProjCS3Type.esriSRProjCS_WGS1984WebMercatorMajorAuxSphere);

                    layer.Initialize(server.Url);
                    IRaster raster = layer.Raster;
                    IRasterLayer pRasterLayer = new RasterLayerClass();
                    pRasterLayer.Name = server.Name;
                    pRasterLayer.CreateFromRaster(raster);

                    axGlobeControl1.Globe.AddLayerType(pRasterLayer, esriGlobeLayerType.esriGlobeLayerTypeDraped);
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(AxGlobeControlEx), ex.Message);
            }
        }

        /// <summary>
        /// 获取ArcGIS地图服务地址
        /// </summary>
        /// <returns></returns>
        private List<TileServer> GetServerPath()
        {
            try
            {
                string xmlConfig = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/ArcGIS_Server");
                if (node == null) return null;

                XmlNodeList nodes = node.ChildNodes;
                if (nodes == null || nodes.Count <= 0) return null;

                List<TileServer> servers = new List<TileServer>();

                foreach (XmlNode n in nodes)
                {
                    TileServer server = new TileServer();
                    server.Name = n.Attributes["name"].InnerXml;
                    server.Url = n.InnerXml;
                    servers.Add(server);
                }

                return servers;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(AxGlobeControlEx), ex.Message);
                return null;
            }
        }

        /// <summary>
        ///  鼠标滚轮事件，实现地图放大缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobleControl_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Drawing.Point sceLoc = axGlobeControl1.PointToScreen(axGlobeControl1.Location);
            System.Drawing.Point pt = this.PointToScreen(e.Location);

            if (pt.X < sceLoc.X || pt.X > sceLoc.X + axGlobeControl1.Width || pt.Y < sceLoc.Y || pt.Y > sceLoc.Y + axGlobeControl1.Height)
            {
                return;
            }

            double scale = 0.2;
            if (e.Delta > 0) scale = -scale;
            IGlobeCamera globeCamera = axGlobeControl1.GlobeCamera;
            ICamera camera = globeCamera as ICamera;
            IGlobeDisplay globeDisplay = axGlobeControl1.GlobeDisplay;
            if (globeCamera.OrientationMode == esriGlobeCameraOrientationMode.esriGlobeCameraOrientationGlobal)
            {
                double xo, yo, zo;
                globeCamera.GetObserverLatLonAlt(out xo, out yo, out zo);
                zo = zo * (1 + scale);
                globeCamera.SetObserverLatLonAlt(xo, yo, zo);
            }
            else
            {
                camera.ViewingDistance += camera.ViewingDistance * scale;
            }
            axGlobeControl1.GlobeDisplay.RefreshViewers();
        }

        #region 二三维联动
        //获得三维视图的显示范围，并在二维地图上显示
        private void GlobeControl_DisplayEvents_AfterDraw(ISceneViewer pViewer)
        {
            IEnvelope m_MapExtent = new EnvelopeClass();
            IGlobeViewUtil m_GlobeViewUtil = axGlobeControl1.GlobeCamera as IGlobeViewUtil;
            m_GlobeViewUtil.QueryVisibleGeographicExtent(m_MapExtent);
            IGraphicsContainer pGra = axMapControl1.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            pGra.DeleteAllElements();

            IRectangleElement rec = new RectangleElementClass();
            IElement ele = rec as IElement;
            ele.Geometry = m_MapExtent;

            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;

            ILineSymbol line = new SimpleLineSymbolClass();
            line.Color = pColor;
            line.Width = 2;

            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;

            IFillSymbol fill = new SimpleFillSymbolClass();
            fill.Outline = line;
            fill.Color = pColor;

            IFillShapeElement pFillElement = ele as IFillShapeElement;
            pFillElement.Symbol = fill;

            pGra.AddElement((IElement)pFillElement, 0);
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        //在二维地图上画矩形控制三维视图显示范围
        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if (axMapControl1.Map.LayerCount != 0)
            {
                if (e.button == 1)
                { return; }
                else if (e.button == 2)
                {
                    IEnvelope env = axMapControl1.TrackRectangle();
                    IZAware ZAware = env as IZAware;
                    ZAware.ZAware = true;
                    axGlobeControl1.GlobeCamera.SetToZoomToExtents(env, axGlobeControl1.Globe, axGlobeControl1.GlobeDisplay.ActiveViewer);
                    axGlobeControl1.GlobeDisplay.RefreshViewers();
                }
            }
        }
        #endregion

        /// <summary>
        /// 加载鹰眼小地图数据
        /// </summary>
        private void LoadMxdFile()
        {
            string arcMapFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps\\world\\World Map.mxd");
            if (axMapControl1.CheckMxFile(arcMapFile))
            {
                axMapControl1.LoadMxFile(arcMapFile);
            }
        }

        /// <summary>
        /// 程序启动时加载一个矢量图层，解决加载切片服务时，地球两级有黑洞的现象
        /// </summary>
        private void LoadLyrFile()
        {
            string north = AppDomain.CurrentDomain.BaseDirectory + "Maps\\north.lyr";
            if (File.Exists(north))
            {
                try
                {
                    mfMap.LoadMap(north);
                }
                catch (Exception ex)
                {
                    Log4Allen.WriteLog(typeof(AxGlobeControlEx), ex.Message);
                }
            }

            string south = AppDomain.CurrentDomain.BaseDirectory + "Maps\\south.lyr";
            if (File.Exists(south))
            {
                try
                {
                    mfMap.LoadMap(south);
                }
                catch (Exception ex)
                {
                    Log4Allen.WriteLog(typeof(AxGlobeControlEx), ex.Message);
                }
            }
        }

        // 初始化书签
        private void InitBookmark()
        {
            return;
            string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "Config\\Bookmark.xml";
            if (!File.Exists(xmlFile)) return;

            BookmarkList bookmarkList = XmlHelper.XmlDeserializeFromFile<BookmarkList>(xmlFile, Encoding.UTF8);
            if (bookmarkList == null || bookmarkList.BookmarkArr == null || bookmarkList.BookmarkArr.Count <= 0) return;

            List<string> names = new List<string>();
            foreach (Bookmark mark in bookmarkList.BookmarkArr)
            {
                // 创建书签
                ISceneBookmarks pBookmarks = axGlobeControl1.Globe.GlobeDisplay.Scene as ISceneBookmarks;
                IPoint point = new PointClass();
                point.X = mark.Longitude;
                point.Y = mark.Latitude;
                point.Z = mark.Altitude;

                ICamera camera = new CameraClass();
                camera.ProjectionType = esri3DProjectionType.esriPerspectiveProjection;
                camera.Inclination = mark.Inclination;
                camera.Azimuth = mark.Azimuth;
                camera.RollAngle = mark.RollAngle;
                camera.ViewFieldAngle = mark.ViewFieldAngle;
                camera.ViewingDistance = mark.ViewingDistance;
                camera.Observer = point;

                IBookmark3D pBookmark3D = new Bookmark3DClass();
                pBookmark3D.Name = mark.Name;
                pBookmark3D.Capture(camera);
                pBookmarks.AddBookmark(pBookmark3D);

                // 发布事件，通知主窗体添加按钮
                EventPublisher.PublishShowBookmarkEvent(this, new ShowBookmarkEventArgs() { NameList = new System.Collections.Generic.List<string> { mark.Name }, Append = true });
            }
        }

    }

    class TileServer
    {
        public string Name;

        public string Url;
    }
}
