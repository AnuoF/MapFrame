using GlobleSituation.Common;
using GlobleSituation.Business;
using MapFrame.Core.Model;
using DevExpress.XtraCharts;
using System;

namespace GlobleSituation.UI
{
    public partial class frmWarnStat : DevExpress.XtraEditors.XtraForm
    {
        public frmWarnStat(WarnArea _area)
        {
            InitializeComponent();

            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            this.Icon = global::GlobleSituation.Properties.Resources.App;

            InitUI(_area);

            InitChartData();
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <param name="_area"></param>
        private void InitUI(WarnArea area)
        {
            txtName.Text = area.Name;
            txtImportant.Text = area.IsImportant ? "重要区域" : "普通区域";
            txtVisible.Text = area.IsVisible ? "在地图上可见" : "在地图上不可见";
            txtWarn.Text = area.IsWarn ? "预警区域" : "非预警区域";

            string pointInfo = "";
            foreach (MapLngLat p in area.Points)
            {
                pointInfo += string.Format("{0},{1} ", p.Lng, p.Lat);
            }

            txtPoint.Text = pointInfo;
        }

        // 关闭
        private void simpleButton1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitChartData()
        {
            Series series1 = new Series("Series1", ViewType.Line);

            series1.Points.Add(new SeriesPoint(DateTime.Now, 2));
            series1.Points.Add(new SeriesPoint(DateTime.Now.AddHours(0.5), 12));
            series1.Points.Add(new SeriesPoint(DateTime.Now.AddHours(1), 14));
            series1.Points.Add(new SeriesPoint(DateTime.Now.AddHours(1.5), 17));

            chartControl1.Series.Add(series1);

            series1.ArgumentScaleType = ScaleType.DateTime;

            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisX.Title.Alignment = System.Drawing.StringAlignment.Center;
            diagram.AxisX.Title.Text = "时间轴";
            diagram.AxisX.Title.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisX.Title.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);

            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Alignment = System.Drawing.StringAlignment.Center;
            diagram.AxisY.Title.Text = "值";
            diagram.AxisY.Title.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Font = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);


            ((XYDiagram)chartControl1.Diagram).EnableAxisXScrolling = true;

            chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            chartControl1.Titles.Add(new ChartTitle());
            chartControl1.Titles[0].Text = "区域变化折线图";

        }
    }
}
