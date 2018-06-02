using GlobleSituation.Common;
using System.Drawing;

namespace GlobleSituation.UI
{
    public partial class frmAbout : DevExpress.XtraEditors.XtraForm
    {

        public frmAbout()
        {
            InitializeComponent();
            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;
        }
    }
}