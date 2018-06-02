using System;


using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;


namespace MgisTilesImportTool
{
    public partial class uc_Import : UserControl
    {
        string path;
        int zoom;
        int DbId;
        SQLiteHelper sqliteHelper;
        string floderName;
        List<Tile> tileList = new List<Tile>();
        int count = 0;

        public uc_Import(string _floderName, string _path, int _zoom, int _dbId, SQLiteHelper _sqliteHelper)
        {
            InitializeComponent();

            floderName = _floderName;
            path = _path;
            zoom = _zoom;
            DbId = _dbId;
            sqliteHelper = _sqliteHelper;
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                ShowInfo("初始化...\r");
                ShowInfo(string.Format("开始提取 {0} 的数据...\r", floderName));

                string[] tiles = Directory.GetFiles(path, "*.jpg");
                count = tiles.Length;
                InitProgressBar(count);
                int i = 1;
                ShowInfo(string.Format("{0} 提取数据完成，开始移交入库...\r", floderName));

                ThreadPool.QueueUserWorkItem(o =>
                {
                    ImportTiles();
                });

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
                    {
                        tileList.Add(t);
                    }

                    UpdateProgressBar(i);
                    i++;
                }

                ShowInfo(string.Format("{0} 移交入库完成。\r", floderName));
            });
        }

        private void ImportTiles()
        {
            int importCount = 0;

            while (true)
            {
                lock (tileList)
                {
                    if (tileList.Count > 0)
                    {
                        sqliteHelper.PutTileToCachePL(tileList);

                        string info = string.Format("插入数据 {0} 条。\r", tileList.Count);
                        ShowInfo(info);
                        importCount += tileList.Count;

                        tileList.Clear();
                    }
                }

                Thread.Sleep(5000);
            }
        }

        // 显示运行日志
        public void ShowInfo(string info)
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
