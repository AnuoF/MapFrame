using GlobleSituation.Common;
using GlobleSituation.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using DevExpress.XtraEditors;

namespace GlobleSituation.UI
{
    public partial class frmRealData : DevExpress.XtraEditors.XtraForm
    {
        public delegate void RcvRealDataDelegate(RealData data);
        public event RcvRealDataDelegate RcvRealDataEventHandler;
        public frmRealData()
        {
            InitializeComponent();
            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;

            LoadParamFromConfig();
        }

        // 加载陪参数
        private void LoadParamFromConfig()
        {
            try
            {
                string ip = "";
                string port = "";

                string xmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/TCPServer");

                ip = node.Attributes["Ip"].InnerXml;
                port = node.Attributes["Port"].InnerXml;

                txtIp.Text = ip;
                txtPort.Text = port;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmSet), ex.Message);
            }
        }

        // 确定
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = txtIp.Text.Trim();
                string port = txtPort.Text.Trim();

                string xmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/TCPServer");
                node.Attributes["Ip"].InnerXml = ip;
                node.Attributes["Port"].InnerXml = port;

                doc.Save(xmlConfig);

                XtraMessageBox.Show("参数保存成功，重启后生效。");
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmSet), ex.Message);
            }

            this.DialogResult = DialogResult.OK;
        }

        // 取消
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
