using GlobleSituation.Common;
using System.Drawing;
using System;
using System.Xml;

namespace GlobleSituation.UI
{
    public partial class frmWarnSet : DevExpress.XtraEditors.XtraForm
    {
        public frmWarnSet()
        {
            InitializeComponent();

            LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;

            LoadSetInfo();
        }

        // 保存
        private void sbOk_Click(object sender, System.EventArgs e)
        {
            SaveSetInfo();
            this.Close();
        }

        // 取消
        private void sbCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 加载信息
        /// </summary>
        private void LoadSetInfo()
        {
            cbRecvData.Checked = Utils.bStartWarn;
            cbSound.Checked = Utils.bStartSound;
            cbTip.Checked = Utils.bStartTip;
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        private void SaveSetInfo()
        {
            Utils.bStartWarn = cbRecvData.Checked;
            Utils.bStartSound = cbSound.Checked;
            Utils.bStartTip = cbTip.Checked;

            return;  // 暂时不保存到本地配置文件

            string xmlConfig = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(xmlConfig);
                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/StartWarn");
                node.InnerXml = Utils.bStartWarn == true ? "true" : "false";
                node = doc.SelectSingleNode("Globe/Config/StartSound");
                node.InnerXml = Utils.bStartSound == true ? "true" : "false";
                node = doc.SelectSingleNode("Globe/Config/StartTip");
                node.InnerXml = Utils.bStartTip == true ? "true" : "false";

                doc.Save(xmlConfig);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmWarnSet), ex.Message);
                throw;
            }
        }





    }
}
