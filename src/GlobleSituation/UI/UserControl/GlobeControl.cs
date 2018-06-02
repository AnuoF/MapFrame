
using DevExpress.XtraEditors;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GlobeCore;
using GlobleSituation.Common;
using System;
using System.Windows.Forms;
using ESRI.ArcGIS.SystemUI;

namespace GlobleSituation.UI
{
    public partial class GlobeControl : XtraUserControl
    {
        public GlobeControl()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);

            InitializeComponent();

            IGlobeDisplayEvents_Event m_GlobeDisplayEvents = axGlobeControl1.Globe.GlobeDisplay as IGlobeDisplayEvents_Event;
            this.MouseWheel += new MouseEventHandler(GlobleControl_MouseWheel); ;
            //将三维视图重绘事件委托给m_GlobeDisplayEvents_AfterDraw方法
            m_GlobeDisplayEvents.AfterDraw += new IGlobeDisplayEvents_AfterDrawEventHandler(GlobeControl_DisplayEvents_AfterDraw);
            axMapControl1.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(axMapControl1_OnMouseDown);
            axMapControl1.Extent = axMapControl1.FullExtent;

            Load3dFile();

            ShowEagleEyeCmd showEagleEyeCmd = new ShowEagleEyeCmd(this);
            axToolbarControl1.AddItem(showEagleEyeCmd, -1, 22, false, -1, esriCommandStyles.esriCommandStyleIconOnly);
        }

        /// <summary>
        /// 显示鹰眼
        /// </summary>
        public void ShowEagleEye()
        {
            eagleEyePanel.Visible = !eagleEyePanel.Visible;
        }

        // 加载地图
        public void Load3dFile(string _3dFile = "")
        {
            string mapFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps\\world\\World Map.mxd");
            if (axMapControl1.CheckMxFile(mapFile))
            {
                axMapControl1.LoadMxFile(mapFile);
            }

            if (string.IsNullOrEmpty(_3dFile))
                _3dFile = @"D:\Untitled.3dd";         //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps\\worldWorld Map.mxd");
            if (axGlobeControl1.Check3dFile(_3dFile))
            {
                axGlobeControl1.Load3dFile(_3dFile);
                EventPublisher.PublishReload3dDocumentEvent(this, null);
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


    }
}
