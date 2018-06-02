using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using System.IO;
using GlobleSituation.Common;
using System.Xml;

namespace GlobleSituation.UI
{
    public partial class frmSet : XtraForm
    {
        public frmSet()
        {
            InitializeComponent();

            LoadParamFromConfig();
        }

        // 加载陪参数
        private void LoadParamFromConfig()
        {
            try
            {
                string pointCount = "";
                string planeScale = "";
                string sallteScale = "";

                string xmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/TrackPointLimit");
                pointCount = node.InnerXml;
                node = doc.SelectSingleNode("Globe/Config/PlaneModelScale");
                planeScale = node.InnerXml;
                node = doc.SelectSingleNode("Globe/Config/SallteModelScale");
                sallteScale = node.InnerXml;

                txtPointCount.Text = pointCount;
                txtSallteModelScaleEx.Text = sallteScale;
                txtPlaneModelScale.Text = planeScale;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmSet), ex.Message);
            }
        }

        // 确定
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string pointCount = txtPointCount.Text.Trim();
                string planeScale = txtPlaneModelScale.Text.Trim();
                string sallteScale = txtSallteModelScaleEx.Text.Trim(); ;

                string xmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/TrackPointLimit");
                node.InnerXml = pointCount;
                node = doc.SelectSingleNode("Globe/Config/PlaneModelScale");
                node.InnerXml = planeScale;
                node = doc.SelectSingleNode("Globe/Config/SallteModelScale");
                node.InnerXml = sallteScale;
                doc.Save(xmlConfig);

                XtraMessageBox.Show("参数保存成功，重启后生效。");

                Utils.TrackPointNum = Convert.ToInt32(pointCount);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmSet), ex.Message);
            }

            this.DialogResult = DialogResult.OK;
        }

        // 取消
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }



    }
}
