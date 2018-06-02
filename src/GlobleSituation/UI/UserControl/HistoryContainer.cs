using System.Windows.Forms;
using DevExpress.XtraEditors;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    public partial class HistoryContainer : XtraUserControl
    {

        private uc_HistoryDataCtrl searchCtrl = null;

        public HistoryContainer(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _gmapBusiness)
        {
            this.dataPlayControl1 = new GlobleSituation.UI.HistoryDataPlayControl(_globeBusiness, _gmapBusiness);

            InitializeComponent();

            searchCtrl = new uc_HistoryDataCtrl(_globeBusiness, _gmapBusiness) { Dock = DockStyle.Fill };
            this.panelControl1.Controls.Add(searchCtrl);

            searchCtrl.SearchData += dataPlayControl1.InitSearch;
        }
    }
}
