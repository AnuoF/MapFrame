using System;
using System.Data;
using System.Windows.Forms;
using GlobleSituation.Common;
using System.Collections.Generic;
using GlobleSituation.Model;
using System.Threading;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    /// <summary>
    /// 历史数据播放控件
    /// </summary>
    public partial class HistoryDataPlayControl : UserControl
    {

        private double perNum = 1000;
        private Thread readyDataThd = null;
        private string condition;
        private int tatalCount = 0;
        private ArcGlobeBusiness globeBusiness;
        private GMapControlBusiness gmapBusiness;
        /// <summary>
        /// 查询次数，以此作为是否查询完毕
        /// </summary>
        private int count = 0;

        public HistoryDataPlayControl(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _gmapBusiness)
        {
            InitializeComponent();

            globeBusiness = _globeBusiness;
            gmapBusiness = _gmapBusiness;

            btnStartOrSuppend.Enabled = false;
            btnStop.Enabled = false;

            EventPublisher.ChangedToRealTimeEvent += new EventHandler<EventArgs>(EventPublisher_ChangedToRealTimeEvent);
        }

        /// <summary>
        /// 初始化查询结果
        /// </summary>
        /// <param name="_condition">查询条件</param>
        /// <param name="_totalCount">总条数</param>
        public void InitSearch(string _condition, int _totalCount)
        {
            condition = _condition;
            tatalCount = _totalCount;
            if (string.IsNullOrEmpty(_condition) || _totalCount <= 0) return;

            InitData();    // 准备数据                     
        }

        /// <summary>
        /// 查询，初始化
        /// </summary>
        /// <param name="condition"></param>
        private void InitData()
        {
            dtQueue.Clear();
            int totalPage = (int)Math.Ceiling(tatalCount / perNum);

            List<string> sqlList = new List<string>();
            if (totalPage <= 1)
            {
                string sql = string.Format("select * from RealData {0} order by PositionDate limit 0,{1}", condition, perNum);
                sqlList.Add(sql);
            }
            else
            {
                for (int i = 0; i < totalPage; i++)
                {
                    string sql = string.Format("select * from RealData {0} order by PositionDate limit {1},{2}", condition, i * perNum, perNum);
                    sqlList.Add(sql);
                }
            }

            btnStartOrSuppend.Enabled = false;
            btnStop.Enabled = false;

            EventPublisher.RecvSearchDataEvent += new EventHandler<RecvSearchDataEventArgs>(EventPublisher_RecvSearchDataEvent);

            foreach (string sql in sqlList)
            {
                count++;
                EventPublisher.PublishSendSearchDataToStoreEvent(this, new SendSearchDataToStoreEventArgs() { SqlStr = sql }); ;
            }
        }

        private void EventPublisher_RecvSearchDataEvent(object sender, RecvSearchDataEventArgs e)
        {
            count--;
            DataTable dt = Utils.DeserializeDataTableFromBytes(e.Data, false);
            if (dt != null)
            {
                lock (dtQueue)
                    dtQueue.Enqueue(dt);
            }

            if (count == 0)   // 准备数据完成
            {
                EventPublisher.RecvSearchDataEvent -= new EventHandler<RecvSearchDataEventArgs>(EventPublisher_RecvSearchDataEvent);

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(delegate
                    {
                        btnStartOrSuppend.Enabled = true;
                    }));
                }
                else
                    btnStartOrSuppend.Enabled = true;

                if (readyDataThd == null)
                {
                    readyDataThd = new Thread(new ThreadStart(Play));
                    readyDataThd.IsBackground = true;
                }
                else
                {
                    try
                    {
                        readyDataThd.Abort();
                        readyDataThd = null;
                    }
                    catch (Exception)
                    {
                        readyDataThd = null;
                    }

                    readyDataThd = new Thread(new ThreadStart(Play));
                    readyDataThd.IsBackground = true;
                }
            }
        }

        private Queue<DataTable> dtQueue = new Queue<DataTable>();

        // 播放
        private void Play()
        {
            int index = 0;     // 进度条的值
            if (dtQueue.Count <= 0) return;

            int total = 0;
            foreach (DataTable tb in dtQueue.ToArray())
            {
                total += tb.Rows.Count;
            }
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(delegate
                {
                    progressBar1.Maximum = tatalCount;
                }));
            }
            else
                progressBar1.Maximum = tatalCount;

            while (dtQueue.Count > 0)
            {
                //DataSet ds = SQLiteHelper.ExecuteDataSet(sql);
                //if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0) continue;

                RealData prevRealData = null;

                //DataTable dt = ds.Tables[0];

                DataTable dt = dtQueue.Dequeue();
                if (dt == null && dt.Rows.Count < 0) continue;

                foreach (DataRow row in dt.Rows)
                {
                    RealData data = RealData.ToRealData(row);
                    if (data == null) continue;

                    EventPublisher.PublishTSDataEvent(this, new TSDataEventArgs() { Data = data });

                    if (prevRealData != null)
                    {
                        TimeSpan ts = DateTime.FromFileTime(data.PositionDate) - DateTime.FromFileTime(prevRealData.PositionDate);
                        Thread.Sleep(ts);
                    }

                    prevRealData = data;

                    try
                    {
                        // 更新进度条
                        if (progressBar1.InvokeRequired)
                        {
                            progressBar1.Invoke(new Action(delegate
                            {
                                if (index > progressBar1.Maximum)
                                    index = progressBar1.Maximum;
                                progressBar1.Value = index;
                            }));
                        }
                        else
                        {
                            if (index > progressBar1.Maximum)
                                index = progressBar1.Maximum;
                            progressBar1.Value = index;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    index++;
                }

                // 推送当前页的最后一包数据
                EventPublisher.PublishTSDataEvent(this, new TSDataEventArgs() { Data = prevRealData });
            }

            // 播放完成
            if (btnStop.InvokeRequired)
            {
                btnStop.Invoke(new Action(delegate
                {
                    btnStop.Enabled = false;
                }));
            }
            else
                btnStop.Enabled = false;

            readyDataThd = null;
        }

        /// <summary>
        /// 开始暂停
        /// </summary>
        /// <param name="play"></param>
        private void StartStop(bool play)
        {
            if (readyDataThd == null) return;

            if (play)
            {
                if (readyDataThd.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                    readyDataThd.Start();
            }
            else
            {
                try
                {
                    readyDataThd.Abort();
                    readyDataThd = null;
                }
                catch (Exception)
                {
                    readyDataThd = null;
                }
            }
        }

        // 开始
        private void btnStartOrSuppend_Click(object sender, EventArgs e)
        {
            btnStartOrSuppend.Enabled = false;
            btnStop.Enabled = true;

            EventPublisher.PublishChangedToHistoryEvent(this, null);

            StartStop(true);
        }

        // 停止
        private void btnStop_Click(object sender, EventArgs e)
        {
            StartStop(false);

            btnStartOrSuppend.Enabled = false;
            btnStop.Enabled = false;

            progressBar1.Value = progressBar1.Maximum;
        }

        // 切换到实时数据
        private void EventPublisher_ChangedToRealTimeEvent(object sender, EventArgs e)
        {
            StartStop(false);

            globeBusiness.ClearRealData();
            gmapBusiness.ClearRealData();

            progressBar1.Value = progressBar1.Maximum;
            Utils.Mode = ShowMode.REAL_TIME;
        }


    }
}
