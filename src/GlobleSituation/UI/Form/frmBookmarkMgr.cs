using GlobleSituation.Common;
using GlobleSituation.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace GlobleSituation.UI
{
    public partial class frmBookmarkMgr : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 数据表
        /// </summary>
        private DataTable dt = null;

        public frmBookmarkMgr()
        {
            InitializeComponent();
            InitDataTable();
            LoadBookmarkFromXml();
        }

        /// <summary>
        /// 初始化表
        /// </summary>
        private void InitDataTable()
        {
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn("Show", typeof(bool)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Remark", typeof(string)),
                new DataColumn("Longitude", typeof(double)),
                new DataColumn("Latitude", typeof(double)),
                new DataColumn("Altitude", typeof(double)),
                new DataColumn("Azimuth", typeof(double)),
                new DataColumn("Inclination", typeof(double)),
                new DataColumn("ViewingDistance", typeof(double)),
                new DataColumn("ViewFieldAngle", typeof(double)),
                new DataColumn("RollAngle", typeof(double))
        });
        }

        /// <summary>
        /// 从配置文件加载书签数据
        /// </summary>
        private void LoadBookmarkFromXml()
        {
            dt.Rows.Clear();
            string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "Config\\Bookmark.xml";
            if (!File.Exists(xmlFile)) return;

            BookmarkList bookmarkList = XmlHelper.XmlDeserializeFromFile<BookmarkList>(xmlFile, Encoding.UTF8);
            if (bookmarkList == null || bookmarkList.BookmarkArr == null || bookmarkList.BookmarkArr.Count <= 0) return;

            foreach (Bookmark bm in bookmarkList.BookmarkArr)
            {
                bool bShow = bm.Show == "true" ? true : false;
                string name = bm.Name;
                string remark = bm.Remark;
                double longitude = bm.Longitude;
                double latitude = bm.Latitude;
                double altitude = bm.Altitude;
                double azimuth = bm.Azimuth;
                double inclination = bm.Inclination;
                double viewingDistance = bm.ViewingDistance;
                double viewFieldAngle = bm.ViewFieldAngle;
                double rollAngle = bm.RollAngle;

                DataRow row = dt.NewRow();
                row["Show"] = bShow;
                row["Name"] = name;
                row["Remark"] = remark;
                row["Longitude"] = longitude;
                row["Latitude"] = latitude;
                row["Altitude"] = altitude;
                row["Azimuth"] = azimuth;
                row["Inclination"] = inclination;
                row["ViewingDistance"] = viewingDistance;
                row["ViewFieldAngle"] = viewFieldAngle;
                row["RollAngle"] = rollAngle;

                dt.Rows.Add(row);
            }

            gridControl1.DataSource = dt;

            #region 读XML的方式

            return;
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);

            XmlNodeList nodeList = doc.SelectSingleNode("./Bookmark").ChildNodes;
            if (nodeList == null || nodeList.Count <= 0) return;

            foreach (XmlNode node in nodeList)
            {
                bool bShow = node.Attributes["Show"].Value == "true" ? true : false;
                string name = node.Attributes["Name"].Value;
                string remark = node.Attributes["Remark"].Value;
                string longitude = node.Attributes["Longitude"].Value;
                string latitude = node.Attributes["Latitude"].Value;
                string altitude = node.Attributes["Altitude"].Value;
                string target = node.Attributes["Target"].Value;
                string azimuth = node.Attributes["Azimuth"].Value;
                string inclination = node.Attributes["Inclination"].Value;
                string viewingDistance = node.Attributes["ViewingDistance"].Value;
                string viewFieldAngle = node.Attributes["ViewFieldAngle"].Value;
                string rollAngle = node.Attributes["RollAngle"].Value;

                DataRow row = dt.NewRow();
                row["Show"] = bShow;
                row["Name"] = name;
                row["Remark"] = remark;
                row["Longitude"] = longitude;
                row["Latitude"] = latitude;
                row["Altitude"] = altitude;
                row["Target"] = target;
                row["Azimuth"] = azimuth;
                row["Inclination"] = inclination;
                row["ViewingDistance"] = viewingDistance;
                row["ViewFieldAngle"] = viewFieldAngle;
                row["RollAngle"] = rollAngle;

                dt.Rows.Add(row);
            }

            gridControl1.DataSource = dt;
            #endregion
        }

        // 确定
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "Config\\Bookmark.xml";

                DataTable dtTmp = gridControl1.DataSource as DataTable;
                if (dtTmp == null || dtTmp.Rows.Count <= 0)
                {
                    if (File.Exists(xmlFile))
                        File.Delete(xmlFile);

                    this.ParentForm.Close();
                    EventPublisher.PublishShowBookmarkEvent(this, new ShowBookmarkEventArgs() { NameList = null, Append = false });
                    return;
                }

                BookmarkList bookmarkList = new BookmarkList();
                List<string> nameList = new List<string>();

                foreach (DataRow row in dtTmp.Rows)
                {
                    Bookmark bm = new Bookmark();
                    bm.Show = Convert.ToBoolean(row["Show"]) == true ? "true" : "false";
                    bm.Name = row["Name"].ToString();
                    bm.Remark = row["Remark"].ToString();
                    bm.Longitude = Convert.ToDouble(row["Longitude"]);
                    bm.Latitude = Convert.ToDouble(row["Latitude"]);
                    bm.Altitude = Convert.ToDouble(row["Altitude"]);
                    bm.Azimuth = Convert.ToDouble(row["Azimuth"]);
                    bm.Inclination = Convert.ToDouble(row["Inclination"]);
                    bm.ViewingDistance = Convert.ToDouble(row["ViewingDistance"]);
                    bm.ViewFieldAngle = Convert.ToDouble(row["ViewFieldAngle"]);
                    bm.RollAngle = Convert.ToDouble(row["RollAngle"]);

                    if (string.IsNullOrEmpty(bm.Name))
                    {
                        MessageBox.Show("书签名称不能为空！");
                        return;
                    }

                    if (bm.Show == "true")
                    {
                        nameList.Add(bm.Name);
                    }

                    bookmarkList.BookmarkArr.Add(bm);
                }

                // 显示书签
                EventPublisher.PublishShowBookmarkEvent(this, new ShowBookmarkEventArgs() { NameList = nameList, Append = false });
                // 保存到xml文件
                XmlHelper.XmlSerializeToFile(bookmarkList, xmlFile, Encoding.UTF8);

                this.Close();
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmBookmarkMgr), ex.Message);
            }
        }

        // 取消
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 右键删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridView1.DeleteSelectedRows();
        }
    }
}
