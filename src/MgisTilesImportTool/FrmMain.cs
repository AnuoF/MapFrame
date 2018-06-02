
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MgisTilesImportTool
{
    /// <summary>
    /// MGIS瓦片图导入工具
    /// </summary>
    public partial class FrmMain : Form
    {

        private Thread pickThd = null;
        private Thread importThd = null;
        private List<Tile> tileList = new List<Tile>();

        private string tilesPath;
        private string sqlitePath;
        private int DbId;
        private int id;
        private SQLiteHelper sqliteHelper = null;

        public FrmMain()
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
            pickThd = new Thread(new ThreadStart(PickTiles));
            pickThd.IsBackground = true;
            pickThd.Priority = ThreadPriority.AboveNormal;

            importThd = new Thread(new ThreadStart(ImportTiles));
            importThd.IsBackground = true;
            importThd.Priority = ThreadPriority.AboveNormal;
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
            string path = txtSqlitePath.Text.Trim();
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else
            {
                MessageBox.Show("目录不存在！");
            }
        }

        // 开始
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

            DateTime dt = DateTime.Now;
            string timeStr = dt.ToLongDateString() + " " + dt.ToLongTimeString();
            lbStartTime.Text = "开始时间：" + timeStr;

            sqliteHelper.CreateEmptyDb(sqlitePath);
            EnableCtrl(false, false);

            pickThd.Start();
            importThd.Start();
        }

        // 暂停
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (pickThd.IsAlive == false) return;

            if (btnPause.Text == "暂  停")
            {
                pickThd.Suspend();
                importThd.Suspend();

                btnPause.Text = "继  续";
            }
            else
            {
                pickThd.Resume();
                importThd.Resume();

                btnPause.Text = "暂  停";
            }
        }

        // 停止
        private void btnStop_Click(object sender, EventArgs e)
        {

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
                    int zoom = Convert.ToInt32(zoomStr) - 1;                   // MGIS是从1开始，而GMap是从0开始

                    ShowInfo(string.Format("开始提取 {0} 的数据...\r", floderName));
                    // 提取图片
                    string[] tiles = Directory.GetFiles(path, "*.jpg");
                    InitProgressBar(tiles.Length);
                    int i = 1;
                    ShowInfo(string.Format("提取 {0} 的数据完成，开始移交入库...\r", floderName));

                    foreach (string tileName in tiles)
                    {
                        FileInfo fi = new FileInfo(tileName);
                        string fileNme = fi.Name;
                        string[] name = fileNme.Split(new char[] { '-' });
                        int y = Convert.ToInt32(name[0]);
                        string[] arr = name[1].Split(new char[] { '.' });
                        int x = Convert.ToInt32(arr[0]);
                        byte[] tile = File.ReadAllBytes(tileName);

                        Tile t = new Tile(tile, DbId, x, y, zoom);

                        lock (tileList)
                            tileList.Add(t);

                        id++;

                        UpdateProgressBar(i);
                        i++;
                    }

                    ShowInfo(string.Format("{0} 移交入库完成。\r", floderName));
                }
            }

            Thread.Sleep(1000);
            MessageBox.Show("数据导入完成！");

            // 执行完成后的操作
            EnableCtrl(true, true);
        }

        public void ImportTiles()
        {
            while (true)
            {
                lock (tileList)
                {
                    if (tileList.Count > 0)
                    {
                        sqliteHelper.PutTileToCachePL(tileList);

                        string info = string.Format("插入数据 =>  {0} 条。\r", tileList.Count);

                        tileList.Clear();

                        ShowInfo(info);
                    }
                }

                Thread.Sleep(4000);
            }
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
                    btnOpen.Enabled = enable;

                    btnStart.Enabled = enable;
                    btnPause.Enabled = !enable;
                    btnStop.Enabled = !enable;
                }));
            }
            else
            {
                txtSqlitePath.Enabled = enable;
                txtTilePath.Enabled = enable;

                btnSearch.Enabled = enable;
                btnChose.Enabled = enable;
                btnOpen.Enabled = enable;

                btnStart.Enabled = enable;
                btnPause.Enabled = !enable;
                btnStop.Enabled = !enable;
            }
        }

        // 显示运行日志
        private void ShowInfo(string info)
        {
            string timeStr = string.Format("[{0}] {1}", DateTime.Now.ToLongTimeString(), info);
            if (rtbInfo.InvokeRequired)
            {
                rtbInfo.BeginInvoke(new Action(delegate
                {
                    if (rtbInfo.Lines.Length > 1000)
                        rtbInfo.Clear();
                    rtbInfo.AppendText(timeStr);
                }));
            }
            else
            {
                if (rtbInfo.Lines.Length > 1000)
                    rtbInfo.Clear();
                rtbInfo.AppendText(timeStr);
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否停止数据导入并关闭程序？如果数据导入尚未完成，可能会影响导入数据不完整。", "关闭", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Environment.Exit(1);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void InitProgressBar(int totalNum)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new Action(delegate
                {
                    progressBar1.Maximum = totalNum;
                    progressBar1.Minimum = 0;
                }));
            }
            else
            {
                progressBar1.Maximum = totalNum;
                progressBar1.Minimum = 0;
            }
        }

        private void UpdateProgressBar(int currentNum)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new Action(delegate
                {
                    progressBar1.Value = currentNum;
                }));
            }
            else
            {
                progressBar1.Value = currentNum;
            }
        }
    }
}
