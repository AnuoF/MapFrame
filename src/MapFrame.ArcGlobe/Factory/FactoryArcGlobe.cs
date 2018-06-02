/**************************************************************************
 * 类名：FactoryArcGlobe.cs
 * 描述：ArcGlobe工厂类
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System;
using System.IO;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GlobeCore;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcGlobe.Common;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// ArcGlobe工厂类
    /// </summary>
    public class FactoryArcGlobe : IMapFactory, IMFMap
    {
        /// <summary>
        /// AxGlobe控件
        /// </summary>
        private AxGlobeControl axGlobeControl = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private LayerManager layerMgr = null;
        /// <summary>
        /// 点图元工厂
        /// </summary>
        private PointFactory pointFac = null;
        /// <summary>
        /// 线图元工厂
        /// </summary>
        private LineFactory lineFac = null;
        /// <summary>
        /// 面图元工厂
        /// </summary>
        private PolygonFactory polygonFac = null;
        /// <summary>
        /// 文字图元工厂
        /// </summary>
        private TextFactory textFac = null;
        /// <summary>
        /// 图标图元工厂
        /// </summary>
        private PictureFactory pictureFac = null;
        /// <summary>
        /// 3D模型图元工厂
        /// </summary>
        private Model3dFactory model3dFac = null;
        /// <summary>
        /// 圆图元工厂
        /// </summary>
        private CircleFactory circleFac = null;

        /// <summary>
        /// Refresh()方法是否可以被调用
        /// </summary>
        private bool isRefreshCall = true;
        /// <summary>
        /// 刷新间隔timer
        /// </summary>
        private System.Timers.Timer refreshTimer = null;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_globeObject">地球控件</param>
        public FactoryArcGlobe(object _globeObject)
        {
            if (_globeObject != null)
            {
                axGlobeControl = _globeObject as AxGlobeControl;
            }
            else
            {
                InitGlobeControl();
            }

            axGlobeControl.OnMouseDown += new IGlobeControlEvents_Ax_OnMouseDownEventHandler(axGlobeControl_OnMouseDown);
            axGlobeControl.OnMouseMove += new IGlobeControlEvents_Ax_OnMouseMoveEventHandler(axGlobeControl_OnMouseMove);
            axGlobeControl.OnMouseUp += new IGlobeControlEvents_Ax_OnMouseUpEventHandler(axGlobeControl_OnMouseUp);
            axGlobeControl.OnDoubleClick += new IGlobeControlEvents_Ax_OnDoubleClickEventHandler(axGlobeControl_OnDoubleClick);
            axGlobeControl.OnKeyDown += new IGlobeControlEvents_Ax_OnKeyDownEventHandler(axGlobeControl_OnKeyDown);
            axGlobeControl.OnKeyUp += new IGlobeControlEvents_Ax_OnKeyUpEventHandler(axGlobeControl_OnKeyUp);

            layerMgr = new LayerManager(axGlobeControl, this);
            pointFac = new PointFactory(axGlobeControl);
            lineFac = new LineFactory(axGlobeControl);
            polygonFac = new PolygonFactory(axGlobeControl);
            textFac = new TextFactory(axGlobeControl);
            pictureFac = new PictureFactory();
            model3dFac = new Model3dFactory(axGlobeControl);
            circleFac = new CircleFactory(axGlobeControl);

            refreshTimer = new System.Timers.Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Elapsed += refreshTimer_Elapsed;
        }

        #region Layer

        /// <summary>
        /// 获取地图控件
        /// </summary>
        /// <returns></returns>
        public object GetMapControl()
        {
            return axGlobeControl;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            // 需要判断是否可以被调用
            lock (lockObj)
            {
                if (isRefreshCall)
                {
                    isRefreshCall = false;
                    refreshTimer.Start();
                }
            }
        }

        /// <summary>
        /// 刷新指定图层
        /// </summary>
        /// <param name="layer">图层</param>
        public void Refresh(IMFLayer layer)
        {
            // 需要判断是否可以被调用
            lock (lockObj)
            {
                if (isRefreshCall)
                {
                    string name = layer.LayerName;
                    isRefreshCall = false;
                    refreshTimer.Start();
                }
            }
        }

        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                isRefreshCall = true;
                refreshTimer.Stop();
            }

            // 单位时间内，只执行一次，且是在单位时间结束时执行
            if (axGlobeControl.InvokeRequired)
            {
                axGlobeControl.Invoke((Action)delegate()
                {
                    axGlobeControl.GlobeDisplay.RefreshViewers();
                });
            }
            else
            {
                //(axGlobeControl.GlobeDisplay as IGlobeDisplayLayers2).RefreshLayer(refreshLayer);
                axGlobeControl.GlobeDisplay.RefreshViewers();
            }
        }

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="layerName">图层名</param>
        /// <returns></returns>
        private ILayer GetLayerByName(string layerName)
        {
            IEnumLayer pEnumLayer = (axGlobeControl.Globe as IScene).get_Layers(null, false);
            if (pEnumLayer == null)
                return null;

            ILayer pLayer = null;
            while ((pLayer = pEnumLayer.Next()) != null)
            {
                ILayer retLayer = GetLayerByName(pLayer, layerName);
                if (retLayer != null) return retLayer;
            }

            return pLayer;
        }

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="pLayer">图层</param>
        /// <param name="layerName">图层名</param>
        /// <returns></returns>
        private ILayer GetLayerByName(ILayer pLayer, string layerName)
        {
            if (pLayer.Name == layerName)
            {
                return pLayer;
            }
            else
            {
                if (pLayer is GroupLayer)
                {
                    ICompositeLayer pCompositeLayer = pLayer as ICompositeLayer;
                    for (int i = 0; i < pCompositeLayer.Count; i++)
                    {
                        ILayer tmpLayer = pCompositeLayer.get_Layer(i);
                        ILayer retLayer = GetLayerByName(tmpLayer, layerName);
                        if (retLayer != null) return retLayer;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool AddLayer(string layerName)
        {
            return layerMgr.AddLayer(layerName);
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool RemoveLayer(string layerName)
        {
            return layerMgr.RemoveLayer(layerName);
        }

        /// <summary>
        /// 移除所有图层
        /// </summary>
        public bool RemoveAllLayer()
        {
            return layerMgr.RemoveAllLayer();
        }

        /// <summary>
        /// 清除当前图层的图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ClearLayer(string layerName)
        {
            layerMgr.ClearLayer(layerName);
        }

        /// <summary>
        /// 清除所有图层的图元
        /// </summary>
        public void ClearLayer()
        {
            layerMgr.ClearLayer();
        }

        /// <summary>
        /// 显示隐藏图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible">true，显示；false，隐藏</param>
        public void SetLayerVisiable(string layerName, bool visible)
        {
            layerMgr.ShowLayer(layerName, visible);
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

            var layer = layerMgr.GetLayer(layerName);
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
            // 三维模型
            else if (type == typeof(KmlModel3d))
            {
                elementFactory = model3dFac;
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
            ILayer layer = layerMgr.GetLayer(layerName);
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

                case ElementTypeEnum.Model3D:
                    elementFactory = model3dFac;
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

        public event EventHandler<MFElementLeaveEventArgs> ElementLeaveEvent;

        public event EventHandler<Core.Model.MFElementClickEventArgs> ElementClickEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyDownEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyUpEvent;

        public event EventHandler<MFKeyPressEventArgs> KeyPressEvent;

        public event EventHandler<EventArgs> InitFinishEvent;


        public object MapControl
        {
            get
            {
                return axGlobeControl;
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
        /// 加载地图文件
        /// </summary>
        /// <param name="mapFile"></param>
        public void LoadMap(string mapFile)
        {
            if (!File.Exists(mapFile)) return;

            FileInfo fi = new FileInfo(mapFile);
            string extensionName = fi.Extension.ToLower();

            switch (extensionName)
            {
                case ".3dd":
                    Open3ddFile(mapFile);
                    break;
                case ".lyr":
                    OpenLyrFile(mapFile);
                    break;
                case ".shp":
                    OpenShpFile(mapFile);
                    break;

                default:
                    break;
            }
        }

        public MapLngLat FromLocalToLngLat(int x, int y)
        {
            return new MapLngLat();
        }

        public System.Drawing.Point FromLngLatToLocal(MapLngLat position)
        {
            return new System.Drawing.Point(); ;
        }

        public System.Drawing.Image Snapshot()
        {
            return null;
        }
        #endregion


        #region Private Function
        /// <summary>
        /// 初始化地图控件
        /// </summary>
        private void InitGlobeControl()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);      // 绑定ArcGIS

            axGlobeControl = new AxGlobeControl();
            axGlobeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            axGlobeControl.Location = new System.Drawing.Point(0, 0);
            axGlobeControl.Name = "axTOCControl1";
            //axGlobeControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            axGlobeControl.Size = new System.Drawing.Size(114, 425);
        }

        /// <summary>
        /// 加载地图文档
        /// </summary>
        /// <param name="fileName"></param>
        private void Open3ddFile(string fileName)
        {
            if (axGlobeControl.Check3dFile(fileName))
            {
                axGlobeControl.Load3dFile(fileName);
            }
        }

        /// <summary>
        /// 加载.lyr文件
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenLyrFile(string fileName)
        {
            try
            {
                LayerFile playerFile = new LayerFile();
                playerFile.New(fileName);
                ILayer pLayer = playerFile.Layer;
                axGlobeControl.Globe.AddLayerType(pLayer, esriGlobeLayerType.esriGlobeLayerTypeDraped, false);

                IGlobeDisplayLayers globeDisplayLayers = axGlobeControl.Globe.GlobeDisplay as IGlobeDisplayLayers;
                globeDisplayLayers.RefreshLayer(pLayer);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 加载.shp文件
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenShpFile(string fileName)
        {
            try
            {
                string path = fileName;
                string pFolder = System.IO.Path.GetDirectoryName(path);
                string pFileName = System.IO.Path.GetFileName(path);
                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
                IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pFolder, 0);
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                IFeatureClass featureClass = pFeatureWorkspace.OpenFeatureClass(pFileName);
                //创建layer并把上一步生成的featureClass赋值给featureLayer
                IFeatureLayer featureLayer = new FeatureLayerClass();
                featureLayer.FeatureClass = featureClass;
                featureLayer.Name = featureClass.AliasName;
                IGroupLayer groupLayer = new GroupLayerClass();
                groupLayer.Name = "add" + pFileName;
                groupLayer.Add(featureLayer);

                axGlobeControl.Globe.AddLayerType(groupLayer as ILayer, esriGlobeLayerType.esriGlobeLayerTypeElevation, true);
            }
            catch (Exception)
            {
            }
        }

        void axGlobeControl_OnKeyUp(object sender, IGlobeControlEvents_OnKeyUpEvent e)
        {
            if (Utils.bPublishEvent == false) return;

        }

        void axGlobeControl_OnKeyDown(object sender, IGlobeControlEvents_OnKeyDownEvent e)
        {
        }

        void axGlobeControl_OnDoubleClick(object sender, IGlobeControlEvents_OnDoubleClickEvent e)
        {

        }

        void axGlobeControl_OnMouseUp(object sender, IGlobeControlEvents_OnMouseUpEvent e)
        {
        }

        void axGlobeControl_OnMouseMove(object sender, IGlobeControlEvents_OnMouseMoveEvent e)
        {
        }

        void axGlobeControl_OnMouseDown(object sender, IGlobeControlEvents_OnMouseDownEvent e)
        {
        }

        #endregion


        public event EventHandler<MapZoomChangedEventArgs> MapZoomChangedEvent;


        public void SetRasterize(string layerName, bool bRasterize)
        {
            layerMgr.SetRasterize(layerName, bRasterize);
        }

        public void GetRasterize(string layerName)
        {
            layerMgr.GetRasterize(layerName);
        }

        public void SetTransparency(string layerName, short transparency)
        {
            layerMgr.SetTransparency(layerName, transparency);
        }


    }
}
