using System.Drawing;
using GlobleSituation.Common;
using System.Data;
using System;
using DevExpress.XtraCharts;
using MapFrame.Core.Interface;
using System.Collections.Generic;
using System.Text;

namespace GlobleSituation.UI
{
    public partial class frmAreaChart : DevExpress.XtraEditors.XtraForm
    {
        private DataTable table = null;
        private IMFToolBox toolBox = null;

        public frmAreaChart(IMFToolBox _toolBox)
        {
            InitializeComponent();
            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;

            toolBox = _toolBox;

            SetChartVisible(false);
            btnExpand.Text = ">>";

            Init();
        }

        // 初始化
        private void Init()
        {
            table = new DataTable("Table1");
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("Value", typeof(Int32));
            table.Rows.Add(new object[] { "空中目标", 0 });
            table.Rows.Add(new object[] { "海上目标", 0 });
            table.Rows.Add(new object[] { "陆地目标", 0 });
            table.Rows.Add(new object[] { "不明目标", 0 });

            // 饼状
            Series _pieSeries = new Series("区域目标统计", ViewType.Pie);
            _pieSeries.ValueDataMembers[0] = "Value";
            _pieSeries.ArgumentDataMember = "Name";
            _pieSeries.DataSource = table;
            chartControl1.Series.Add(_pieSeries);

            _pieSeries.LegendPointOptions.PointView = PointView.ArgumentAndValues;
            if (_pieSeries.View is PieSeriesView)
            {
                ((PiePointOptions)_pieSeries.PointOptions).PercentOptions.ValueAsPercent = true;
                ((PiePointOptions)_pieSeries.PointOptions).ValueNumericOptions.Format = NumericFormat.Percent;
                ((PiePointOptions)_pieSeries.PointOptions).ValueNumericOptions.Precision = 0;
            }

            // 柱状
            Series _pieSeries2 = new Series("区域目标统计", ViewType.Bar);
            _pieSeries2.ValueDataMembers[0] = "Value";
            _pieSeries2.ArgumentDataMember = "Name";
            _pieSeries2.DataSource = table;
            chartControl2.Series.Add(_pieSeries2);

            _pieSeries2.LegendPointOptions.PointView = PointView.ArgumentAndValues;
        }

        // 展开隐藏按钮
        private void btnExpand_Click(object sender, System.EventArgs e)
        {
            bool bShow = false;

            if (btnExpand.Text == ">>")
            {
                bShow = true;
            }
            else
            {
                bShow = false;
            }

            SetChartVisible(bShow);
        }

        // 框选区域
        private void btnSelectArea_Click(object sender, EventArgs e)
        {
            if (toolBox == null) return;
            this.Visible = false;

            toolBox.CommondExecutedEvent += new EventHandler<MapFrame.Core.Model.MessageEventArgs>(toolBox_CommondExecutedEvent);
            toolBox.Select();
        }

        /// <summary>
        /// 显示隐藏图表
        /// </summary>
        /// <param name="bShow"></param>
        private void SetChartVisible(bool bShow)
        {
            if (bShow)
            {
                btnExpand.Text = "<<";
                groupControl1.Visible = true;
                this.Height = 429;
            }
            else
            {
                btnExpand.Text = ">>";
                groupControl1.Visible = false;
                this.Height = 120;
            }
        }

        // 框选完成
        private void toolBox_CommondExecutedEvent(object sender, MapFrame.Core.Model.MessageEventArgs e)
        {
            if (this.IsDisposed) return;
            toolBox.CommondExecutedEvent -= new EventHandler<MapFrame.Core.Model.MessageEventArgs>(toolBox_CommondExecutedEvent);

            this.Visible = true;
            SetChartVisible(true);

            if (e.ToolType != MapFrame.Core.Model.ToolTypeEnum.Select) return;
            if (e.Data == null) return;

            List<IMFElement> elements = e.Data as List<IMFElement>;
            if (elements == null || elements.Count <= 0)
            {
                string msg = "您所框选的区域没有动目标，可尝试重新框选。";
                UpdateUI(msg);
                return;
            }

            int totalNumber = elements.Count;
            int skyCount = 0;       // 空中目标
            int seaCount = 0;       // 海上目标
            int landCount = 0;      // 陆地目标
            int unkonwCount = 0;    // 不明目标

            foreach (IMFElement ele in elements)
            {
                switch (ele.BelongLayer.LayerName)
                {
                    case "天空态势图层":
                        skyCount++;
                        break;
                    case "陆地态势图层":
                        seaCount++;
                        break;
                    case "海洋态势图层":
                        landCount++;
                        break;
                    case "未知目标图层":
                        unkonwCount++;
                        break;
                    default:
                        break;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("当前区域共有目标：{0} 个\n", totalNumber));
            sb.AppendLine(string.Format("空中目标 {0} 条\n", skyCount));
            sb.AppendLine(string.Format("海上目标 {0} 条\n", seaCount));
            sb.AppendLine(string.Format("陆地目标 {0} 条\n", landCount));
            sb.AppendLine(string.Format("不明目标 {0} 条", unkonwCount));

            UpdateUI(sb);
            UpdateChart(skyCount, seaCount, landCount, unkonwCount);
        }

        /// <summary>
        /// 更新界面
        /// </summary>
        private void UpdateUI(StringBuilder sb)
        {
            if (txtInfo.InvokeRequired)
            {
                txtInfo.Invoke(new Action(delegate
                {
                    txtInfo.Text = sb.ToString();
                }));
            }
            else
                txtInfo.Text = sb.ToString();
        }

        /// <summary>
        /// 更新界面
        /// </summary>
        private void UpdateUI(string info)
        {
            if (txtInfo.InvokeRequired)
            {
                txtInfo.Invoke(new Action(delegate
                {
                    txtInfo.Text = info;
                }));
            }
            else
                txtInfo.Text = info;
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
                    chartControl2.RefreshData();
                }));
            }
            else
            {
                chartControl1.RefreshData();
                chartControl2.RefreshData();
            }
        }

        // 关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
