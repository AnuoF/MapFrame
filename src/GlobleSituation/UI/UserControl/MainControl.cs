

using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GlobleSituation.Business;
using GlobleSituation.Common;
using GlobleSituation.Model;

namespace GlobleSituation.UI
{
    public partial class MainControl : XtraUserControl
    {
        private DataContainerControl dataContainerCtrl = null;       // 数据列表容器控件
        private DisplayMgrControl displayCtrl = null;                // 显示控制控件
        public MapGlobeContainer GlobeMapContainer = null;           // 二维三维地图控件
        public LayersControl LayersCtrl;                             // 图层控制控件
        private TSDataRecv tsDataRecv = null;                        // 态势数据接收类
        private ReadBeamData beanDataRecv = null;                    // 波束数据接收类
        private HistoryContainer historyContainerCtrl = null;        // 历史态势容器

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainControl()
        {
            InitializeComponent();
            InitMainControl();
            InitEnvironment();

            // 初始化接收态势数据
            tsDataRecv = new TSDataRecv();
            tsDataRecv.InitDataRecv();

            // 波束数据
            beanDataRecv = new ReadBeamData();
            beanDataRecv.PushData += RecvBeamData;
        }

        /// <summary>
        /// 初始化主界面
        /// </summary>
        private void InitMainControl()
        {
            GlobeMapContainer = new MapGlobeContainer() { Dock = DockStyle.Fill };
            this.panelControl1.Controls.Add(GlobeMapContainer);

            LayersCtrl = new LayersControl(GlobeMapContainer.globeCtrl.globeBusiness, GlobeMapContainer.gmapCtrl.mapBusiness) { Dock = DockStyle.Fill };
            dpLayers.Controls.Add(LayersCtrl);

            displayCtrl = new DisplayMgrControl(GlobeMapContainer.globeCtrl.globeBusiness, GlobeMapContainer.gmapCtrl.mapBusiness) { Dock = DockStyle.Fill };
            displayCtrl.BindControl(GlobeMapContainer.globeCtrl.axGlobeControl1);
            this.dockPanel1.Controls.Add(displayCtrl);

            dataContainerCtrl = new DataContainerControl(GlobeMapContainer.globeCtrl.globeBusiness, GlobeMapContainer.gmapCtrl.mapBusiness) { Dock = DockStyle.Fill };
            this.dockPanel2.Controls.Add(dataContainerCtrl);

            this.historyContainerCtrl = new HistoryContainer(GlobeMapContainer.globeCtrl.globeBusiness, GlobeMapContainer.gmapCtrl.mapBusiness);
            historyContainerCtrl.Dock = DockStyle.Fill;
            this.controlContainer1.Controls.Add(this.historyContainerCtrl);

            //// 
            //// uc_HistoryDataCtrl1
            //// 
            //this.uc_HistoryDataCtrl1 = new uc_HistoryDataCtrl(GlobeMapContainer.globeCtrl.globeBusiness, GlobeMapContainer.gmapCtrl.mapBusiness);
            //this.controlContainer1.Controls.Add(this.uc_HistoryDataCtrl1);
            //this.uc_HistoryDataCtrl1.AutoSize = true;
            //this.uc_HistoryDataCtrl1.Dock = DockStyle.Fill;
            //this.uc_HistoryDataCtrl1.Location = new System.Drawing.Point(0, 0);
            //this.uc_HistoryDataCtrl1.Name = "uc_HistoryDataCtrl1";
            //this.uc_HistoryDataCtrl1.Size = new System.Drawing.Size(224, 430);
            //this.uc_HistoryDataCtrl1.TabIndex = 0;


        }

        /// <summary>
        /// 加载3d文档
        /// </summary>
        /// <param name="_3dFile"></param>
        public void Load3dFile(string _3dFile)
        {
            GlobeMapContainer.Load3dFile(_3dFile);
        }

        /// <summary>
        /// 从服务器加载地图
        /// </summary>
        public void LoadTitleFromServer()
        {
            GlobeMapContainer.LoadTitleFromServer();
        }

        /// <summary>
        /// 加载波束数据
        /// </summary>
        /// <param name="load"></param>
        public void LoadBeamData(bool load)
        {
            if (load)
            {
                //beanDataRecv.Start();
                tsDataRecv.StartBeam();
            }
            else
                //beanDataRecv.Stop();
                tsDataRecv.StopBeam();
        }

        /// <summary>
        /// 接入态势数据
        /// </summary>
        /// <param name="load"></param>
        public void LoadTSData(bool load)
        {
            if (load)
            {
                tsDataRecv.StartTs();
            }
            else
            {
                tsDataRecv.StopTs();
            }
        }

        // 显示控制
        public void ShowPanel(string name)
        {
            if (name == "目标显控" || name == "地图显控" || name == "区域显控")
            {
                dockPanel1.Show();
                displayCtrl.ShowPanel(name);
            }
            else if (name == "历史查询")
            {
                dockPanel3.Show();
            }
            else if (name == "数据列表")
            {
                dockPanel2.Show();
            }
            else
            {
                dpLayers.Show();
            }
        }

        // 接收波束数据
        private void RecvBeamData(BeamData beamData)
        {
            EventPublisher.PublishBeamDataComeEvent(this, beamData);
        }

        private void InitEnvironment()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Config\\Bookmark.xml";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(MainControl), ex.Message);
            }
        }



    }
}
