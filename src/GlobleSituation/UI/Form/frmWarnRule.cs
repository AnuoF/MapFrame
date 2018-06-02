using GlobleSituation.Common;
using System.Drawing;

namespace GlobleSituation.UI
{
    public partial class frmWarnRule : DevExpress.XtraEditors.XtraForm
    {
        public frmWarnRule()
        {
            InitializeComponent();


            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvRule_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (e.Info.IsRowIndicator)
            {
                if (e.RowHandle >= 0)
                {
                    e.Info.DisplayText = (e.RowHandle + 1).ToString();
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "G" + e.RowHandle.ToString();
                }
            }
        }
    }
}
