

using System.Windows.Forms;
using System.Xml;
using System;
using GlobleSituation.Common;
using GlobleSituation.Model;
using GlobleSituation.Business;
using MapFrame.Core.Model;
using System.Collections.Generic;

namespace GlobleSituation.UI
{
    public partial class MapGlobeContainer : UserControl
    {
        /// <summary>
        /// Globe
        /// </summary>
        public AxGlobeControlEx globeCtrl = null;
        ///// <summary>
        ///// Map
        ///// </summary>
        //private AxMapControlEx mapCtrl = null;

        /// <summary>
        /// GMap地图控件
        /// </summary>
        public GMapControlEx gmapCtrl = null;
        /// <summary>
        /// 预警管理
        /// </summary>
        public WarnManager warnMgr = null;                   // 预警管理

        private delegate bool WarnHandler(RealData dataIn, out RealData dataOut, out bool isWarn, out List<string> warnNames);



        public MapGlobeContainer()
        {
            InitializeComponent();


            // 航迹管理
            TrackLineManager trackMgr = new TrackLineManager();

            globeCtrl = new AxGlobeControlEx(trackMgr) { Dock = DockStyle.Fill };
            this.dockPanel1.Controls.Add(globeCtrl);
            //mapCtrl = new AxMapControlEx() { Dock = DockStyle.Fill };
            //this.dockPanel2.Controls.Add(mapCtrl);

            gmapCtrl = new GMapControlEx(trackMgr, globeCtrl.globeBusiness) { Dock = DockStyle.Fill };
            this.dockPanel2.Controls.Add(gmapCtrl);

            dockPanel1.Options.ShowCloseButton = false;
            dockPanel2.Options.ShowCloseButton = false;

            LoadSet();    // 加载配置

            // 预警
            warnMgr = new WarnManager(globeCtrl.mapLogic, gmapCtrl.mapLogic);

            EventPublisher.JumpToGlobeViewEvent += EventPublisher_JumpToGlobeViewEvent;
            EventPublisher.TSDataEvent += new EventHandler<TSDataEventArgs>(EventPublisher_TSDataEvent);               // 态势数据
        }

        // 接收处理态势数据
        private void EventPublisher_TSDataEvent(object sender, TSDataEventArgs e)
        {
            if (e.Data == null) return;

            globeCtrl.DealTSData(e.Data);
            gmapCtrl.DealRealData(e.Data);

            if (Utils.bStartWarn == false) return;

            // 异步回调（预警）
            MapLngLat point = new MapLngLat(e.Data.Longitude, e.Data.Latitude, e.Data.Altitude);
            List<string> warnNames;
            bool isWarn;
            RealData data;
            WarnHandler handler = new WarnHandler(warnMgr.Warn);
            IAsyncResult result = handler.BeginInvoke(e.Data, out data, out isWarn, out warnNames, new AsyncCallback(WarnResult), "AsycState:OK");
        }

        // 处理预警结果
        private void WarnResult(IAsyncResult result)
        {
            List<string> warnNames;
            bool isWarn;
            RealData data;
            WarnHandler handler = (WarnHandler)((System.Runtime.Remoting.Messaging.AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(out data, out isWarn, out warnNames, result);

            globeCtrl.DealWarnData(data, isWarn, warnNames);
            gmapCtrl.DealWarnData(data, isWarn, warnNames);
        }

        // 跳转到三维视图
        private void EventPublisher_JumpToGlobeViewEvent(object sender, JumpToGlobeViewEventArgs e)
        {
            //dockPanel2.Focus();
            globeCtrl.mapLogic.GetToolBox().ZoomToPosition(e.Position);
        }

        /// <summary>
        /// 加载3d文档
        /// </summary>
        /// <param name="_3dfile"></param>
        public void Load3dFile(string _3dfile)
        {
            globeCtrl.Load3dFile(_3dfile);
        }

        /// <summary>
        /// 从服务器加载地图
        /// </summary>
        public void LoadTitleFromServer()
        {
            globeCtrl.LoadTitleFromServer();
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        private void LoadSet()
        {
            string xmlConfig = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(xmlConfig);
                XmlNode nodes;
                nodes = doc.SelectSingleNode("Globe/Config/StartWarn");
                Utils.bStartWarn = nodes.InnerXml == "true" ? true : false;
                nodes = doc.SelectSingleNode("Globe/Config/StartSound");
                Utils.bStartSound = nodes.InnerXml == "true" ? true : false;
                nodes = doc.SelectSingleNode("Globe/Config/StartTip");
                Utils.bStartTip = nodes.InnerXml == "true" ? true : false;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(MapGlobeContainer), ex.Message);
            }
        }
    }
}
