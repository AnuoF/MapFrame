using DevExpress.XtraEditors;
using ESRI.ArcGIS.Controls;
using System;
using System.Windows.Forms;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    public partial class DisplayMgrControl : XtraUserControl
    {
        /// <summary>
        /// Globe业务类
        /// </summary>
        private ArcGlobeBusiness globeBusiness = null;
        /// <summary>
        /// GMap业务类
        /// </summary>
        private GMapControlBusiness mapBusiness = null;
        /// <summary>
        /// Globe控件
        /// </summary>
        public ESRI.ArcGIS.Controls.AxGlobeControl GlobeControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_globeBusiness"></param>
        public DisplayMgrControl(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _mapBusiness)
        {
            InitializeComponent();

            globeBusiness = _globeBusiness;
            mapBusiness = _mapBusiness;
            InitUI();
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitUI()
        {
            panelObject.Dock = DockStyle.Fill;
            panelArea.Visible = false;
            panelMap.Visible = false;
        }

        /// <summary>
        /// 绑定Globe控件
        /// </summary>
        /// <param name="globe"></param>
        public void BindControl(ESRI.ArcGIS.Controls.AxGlobeControl globe)
        {
            if (globe != null)
            {
                GlobeControl = globe;
                axTOCControl1.SetBuddyControl(globe);
            }
        }

        /// <summary>
        /// 显示Panel
        /// </summary>
        /// <param name="name"></param>
        public void ShowPanel(string name)
        {
            switch (name)
            {
                case "目标显控":
                    ShowObjectPanel();
                    break;
                case "区域显控":
                    ShowAreaPanel();
                    break;
                case "地图显控":
                    ShowMapPanel();
                    break;
            }
        }

        // 显示数据
        private void sbObject_Click(object sender, EventArgs e)
        {
            ShowObjectPanel();
        }

        // 显示区域
        private void sbArea_Click(object sender, EventArgs e)
        {
            ShowAreaPanel();
        }

        // 显示地图图层
        private void sbMap_Click(object sender, EventArgs e)
        {
            ShowMapPanel();
        }

        private void ShowObjectPanel()
        {
            panelObject.Dock = DockStyle.Fill;
            panelObject.Visible = true;
            panelArea.Visible = false;
            panelMap.Visible = false;

            tpObject.Dock = DockStyle.Top;
            tpMap.Dock = DockStyle.Bottom;
            tpArea.Dock = DockStyle.Bottom;
        }

        private void ShowAreaPanel()
        {
            panelArea.Dock = DockStyle.Fill;
            panelArea.Visible = true;
            panelObject.Visible = false;
            panelMap.Visible = false;

            tpObject.Dock = DockStyle.Top;
            tpArea.Dock = DockStyle.Top;
            tpMap.Dock = DockStyle.Bottom;
        }

        private void ShowMapPanel()
        {
            panelMap.Dock = DockStyle.Fill;
            panelMap.Visible = true;
            panelObject.Visible = false;
            panelArea.Visible = false;

            tpObject.Dock = DockStyle.Top;
            tpArea.Dock = DockStyle.Bottom;
            tpMap.Dock = DockStyle.Top;
        }

        private void cbSatelliteBeam_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            if (cb == null) return;

            string layerName = cb.Tag.ToString();

            if (layerName == "卫星波束图层")
            {
                globeBusiness.mapLogic.SetLayerVisible("卫星图层", cb.Checked);
                globeBusiness.mapLogic.SetLayerVisible("波束图层", cb.Checked);
                globeBusiness.mapLogic.SetLayerVisible("覆盖图层", cb.Checked);
                mapBusiness.mapLogic.SetLayerVisible("波束图层", cb.Checked);
            }
            else
            {
                globeBusiness.mapLogic.SetLayerVisible(layerName, cb.Checked);
                mapBusiness.mapLogic.SetLayerVisible(layerName, cb.Checked);
            }
        }
    }
}
