using System.Windows.Forms;
using DevExpress.XtraEditors;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    public partial class DataContainerControl : XtraUserControl
    {
        private BeamDataControl beamDataCtrl = null;
        private RealDataListControl realDataListControl1;
        private DeviceDataList deviceDataCtrl = null;

        public DataContainerControl(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _mapBusiness)
        {
            InitializeComponent();

            this.realDataListControl1 = new RealDataListControl(_globeBusiness, _mapBusiness);
            this.xtraTabPage1.Controls.Add(this.realDataListControl1);
            // 
            // uc_RealDataList1
            // 
            this.realDataListControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.realDataListControl1.Location = new System.Drawing.Point(0, 0);
            this.realDataListControl1.Name = "uc_RealDataList1";
            this.realDataListControl1.Size = new System.Drawing.Size(490, 332);
            this.realDataListControl1.TabIndex = 0;

            this.realDataListControl1.Initial();

            beamDataCtrl = new BeamDataControl() { Dock = DockStyle.Fill };
            this.xtraTabPage2.Controls.Add(beamDataCtrl);

            deviceDataCtrl = new DeviceDataList(_globeBusiness, _mapBusiness) { Dock = DockStyle.Fill };
            this.xtraTabPage3.Controls.Add(deviceDataCtrl);
        }

    }
}
