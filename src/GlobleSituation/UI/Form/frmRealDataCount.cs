using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using DevExpress.XtraCharts;
using GlobleSituation.Common;
using GlobleSituation.Model;

namespace GlobleSituation.UI
{
    public partial class frmRealDataCount : DevExpress.XtraEditors.XtraForm
    {
        // 数据表
        private DataTable table = null;
        /// <summary>
        /// 目标统计
        /// </summary>
        private Dictionary<byte, List<RealData>> countDic = new Dictionary<byte, List<RealData>>();


        public frmRealDataCount()
        {
            InitializeComponent();
            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;

            InitDataTable();
            BuilderDevChart();

            EventPublisher.TSDataEvent += new System.EventHandler<Model.TSDataEventArgs>(EventPublisher_TSDataEvent);
        }

        /// <summary>
        /// 初始化数据源
        /// </summary>
        private void InitDataTable()
        {
            table = new DataTable("Table1");
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Value", typeof(Int32));
            table.Rows.Add(new object[] { "空中目标", 0 });
            table.Rows.Add(new object[] { "海上目标", 0 });
            table.Rows.Add(new object[] { "陆地目标", 0 });
            table.Rows.Add(new object[] { "不明目标", 0 });
        }

        private void frmRealDataCount_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            EventPublisher.TSDataEvent -= new System.EventHandler<Model.TSDataEventArgs>(EventPublisher_TSDataEvent);
        }

        // 接收态势数据，进行统计
        private void EventPublisher_TSDataEvent(object sender, Model.TSDataEventArgs e)
        {
            byte type = e.Data.TargetType;

            if (!countDic.ContainsKey(type))
            {
                List<RealData> names = new List<RealData>();
                names.Add(e.Data);
                countDic.Add(type, names);

                UpdateUI();
            }
            else
            {

                var d = countDic[type].Find(obj => obj.TargetNum == e.Data.TargetNum);   // 判断是否已经添加
                if (d == null)
                {
                    countDic[type].Add(e.Data);

                    UpdateUI();
                }
            }
        }

        /// <summary>
        /// 更新界面
        /// </summary>
        private void UpdateUI()
        {
            int skyCount = 0;       // 空中目标
            int seaCount = 0;       // 海上目标
            int landCount = 0;      // 陆地目标
            int unkonwCount = 0;    // 不明目标

            foreach (var type in countDic.Keys)
            {
                switch (type)
                {
                    case 0:   // 空中目标
                        skyCount = countDic[type].Count;
                        break;
                    case 1:   // 陆地目标
                        landCount = countDic[type].Count;
                        break;
                    case 2:   // 海上目标
                        seaCount = countDic[type].Count;
                        break;
                    case 3:   // 未知目标
                        unkonwCount = countDic[type].Count;
                        break;
                    default:
                        break;
                }
            }

            string info = string.Format("当前共接入数据：\n空中目标 {0} 条\n海上目标 {1} 条\n陆地目标 {2} 条\n不明目标 {3} 条", skyCount, seaCount, landCount, unkonwCount);

            if (labelControl1.InvokeRequired)
            {
                labelControl1.Invoke(new Action(delegate
                {
                    labelControl1.Text = info;
                }));
            }
            else
                labelControl1.Text = info;

            UpdateChart(skyCount, seaCount, landCount, unkonwCount);
        }

        /// <summary>
        /// 更新饼图
        /// </summary>
        private void UpdateChart(int skyCount, int seaCount, int landCount, int unkonwCount)
        {
            table.Rows[0]["Value"] = skyCount;
            table.Rows[1]["Value"] = seaCount;
            table.Rows[2]["Value"] = landCount;
            table.Rows[3]["Value"] = unkonwCount;

            if (chartControl1.InvokeRequired)
            {
                chartControl1.Invoke(new Action(delegate
                {
                    chartControl1.RefreshData();
                }));
            }
            else
                chartControl1.RefreshData();
        }

        private void BuilderDevChart()
        {
            Series _pieSeries = new Series("测试", ViewType.Pie);
            _pieSeries.ValueDataMembers[0] = "Value";
            _pieSeries.ArgumentDataMember = "Name";
            _pieSeries.DataSource = table;
            chartControl1.Series.Add(_pieSeries);

            //----------------------------------------
            _pieSeries.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            SetPiePercentage(_pieSeries);
        }

        /// <summary>
        /// 饼状Series设置成百分比显示
        /// </summary>
        /// <param name="series">Series</param>
        public void SetPiePercentage(Series series)
        {
            if (series.View is PieSeriesView)
            {
                ((PiePointOptions)series.PointOptions).PercentOptions.ValueAsPercent = true;
                ((PiePointOptions)series.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
                ((PiePointOptions)series.PointOptions).ValueNumericOptions.Precision = 0;
            }
        }

        // 关闭
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
