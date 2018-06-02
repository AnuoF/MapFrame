using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace GlobleSituation.UI
{
    public partial class BookmarkAddControl : XtraUserControl
    {
        string name = "";

        public BookmarkAddControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbOk_Click(object sender, System.EventArgs e)
        {
            if(string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                XtraMessageBox.Show("书签名称不能为空！");
                txtName.Focus();
                return;
            }

            name = txtName.Text.Trim();
            this.ParentForm.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 获取书签名称
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// 获取书签备注
        /// </summary>
        /// <returns></returns>
        public string GetRemark()
        {
            return txtRemark.Text.Trim();
        }
    }
}
