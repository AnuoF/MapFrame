

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MgisTilesImportTool
{
    public partial class FrmNewMain : Form
    {
        private SQLiteHelper sqliteHelper = null;
        private int DbId;
        private string tilesPath;
        private string sqlitePath;
        private List<Tile> tileList = new List<Tile>();


        public FrmNewMain()
        {
            InitializeComponent();

            txtTilePath.Text = @"D:\Allen\Maps";
            txtSqlitePath.Text = @"e:\";
            EnableCtrl(true, false);

            using (var HashProvider = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                Guid Id = new Guid("EF3DD303-3F74-4938-BF40-232D0595EE77");
                DbId = Math.Abs(BitConverter.ToInt32(HashProvider.ComputeHash(Id.ToByteArray()), 0));
            }

            sqliteHelper = new SQLiteHelper();
        }

        // 选择瓦片图目录
        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "选择MGIS瓦片图所在的目录";
                dlg.ShowNewFolderButton = false;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtTilePath.Text = dlg.SelectedPath;
                }
            }
        }

        // 选择数据库目录
        private void btnChose_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtSqlitePath.Text = dlg.SelectedPath;
                }
            }
        }

        // 打开数据库所在目录
        private void btnOpen_Click(object sender, EventArgs e)
        {
            string path = txtTilePath.Text.Trim();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else
            {
                MessageBox.Show("目录不存在！");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            tilesPath = txtTilePath.Text.Trim();
            if (string.IsNullOrEmpty(tilesPath))
            {
                MessageBox.Show("请选择瓦片图所在目录！");
                txtTilePath.SelectAll();
                txtTilePath.Focus();
                return;
            }

            if (!Directory.Exists(tilesPath))
            {
                MessageBox.Show("目录不存在！");
                txtTilePath.SelectAll();
                txtTilePath.Focus();
                return;
            }

            sqlitePath = txtSqlitePath.Text.Trim();
            if (string.IsNullOrEmpty(sqlitePath))
            {
                MessageBox.Show("请选择数据库生成的在目录！");
                txtSqlitePath.SelectAll();
                txtSqlitePath.Focus();
                return;
            }

            if (!Directory.Exists(sqlitePath))
            {
                MessageBox.Show("目录不存在！");
                txtSqlitePath.SelectAll();
                txtSqlitePath.Focus();
                return;
            }

            sqliteHelper.CreateEmptyDb(sqlitePath);
            EnableCtrl(false, false);
            PickTiles();
        }

        // 使控件可用或不可用
        private void EnableCtrl(bool enable, bool invike)
        {
            if (invike)
            {
                this.Invoke(new Action(delegate
                {
                    txtSqlitePath.Enabled = enable;
                    txtTilePath.Enabled = enable;

                    btnSearch.Enabled = enable;
                    btnChose.Enabled = enable;
                    //btnOpen.Enabled = enable;
                    btnStart.Enabled = enable;
                }));
            }
            else
            {
                txtSqlitePath.Enabled = enable;
                txtTilePath.Enabled = enable;

                btnSearch.Enabled = enable;
                btnChose.Enabled = enable;
                //btnOpen.Enabled = enable;
                btnStart.Enabled = enable;
            }
        }

        /// <summary>
        /// 导入瓦片图
        /// </summary>
        private void PickTiles()
        {
            string[] tilePathArr = Directory.GetDirectories(tilesPath);
            if (tilePathArr.Length > 0)
            {
                foreach (string path in tilePathArr)
                {
                    // 提取缩放级别  zoom
                    string[] floders = path.Split(new char[] { '\\' });
                    string floderName = floders[floders.Length - 1];
                    string zoomStr = floderName.Substring(1, 2);
                    int zoom = Convert.ToInt32(zoomStr) - 1;

                    uc_Import importCtrl = new uc_Import(floderName, path, zoom, DbId, sqliteHelper) { Dock = DockStyle.Fill };

                    TabPage page = new System.Windows.Forms.TabPage();
                    page.Name = "tabPage" + zoomStr;
                    page.TabIndex = zoom;
                    page.Text = "Zoom：" + zoomStr;
                    page.Controls.Add(importCtrl);
                    tabControl1.TabPages.Add(page);

                    importCtrl.Start();
                }
            }
        }

        private void FrmNewMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否停止数据导入？如果数据导入尚未完成，可能会影响导入数据不完整。", "停止导入", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }


    }
}
