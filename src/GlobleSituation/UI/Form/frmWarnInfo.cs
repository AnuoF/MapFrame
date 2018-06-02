
using GlobleSituation.Common;
using System.Drawing;
using System.Data;
using System;
using MapFrame.Core.Interface;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    public partial class frmWarnInfo : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 波束数据表
        /// </summary>
        private DataTable dt = null;
        /// <summary>
        /// 显示条数
        /// </summary>
        private int count = 100;
        /// <summary>
        /// 三维地图业务类
        /// </summary>
        private ArcGlobeBusiness globeBusiness = null;
        /// <summary>
        /// 二维地图业务类
        /// </summary>
        public GMapControlBusiness mapBusiness = null;


        public frmWarnInfo(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _mapBusiness)
        {
            InitializeComponent();

            LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;

            globeBusiness = _globeBusiness;
            mapBusiness = _mapBusiness;
            EventPublisher.WarnDataEvent += new EventHandler<Model.TSDataEventArgs>(EventPublisher_WarnDataEvent);
            InitDataTable();
        }

        // 显示行号
        private void gvWarnInfo_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (e.Info.IsRowIndicator)
            {
                if (e.RowHandle >= 0)
                {
                    e.Info.DisplayText = (e.RowHandle + 1).ToString();
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "G" + e.RowHandle.ToString();
                }
            }
        }

        // 初始化数据表
        private void InitDataTable()
        {
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("KzCol", typeof(string)),
                new DataColumn("TargetNumCol", typeof(string)),
                new DataColumn("RuleNameCol", typeof(string)),
                new DataColumn("WarnStartCol", typeof(string)),
                new DataColumn("TargetType", typeof(string))
            });

            gridControl1.DataSource = dt;
        }

        // 隐藏窗体
        private void frmWarnInfo_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        // 接收预警数据
        void EventPublisher_WarnDataEvent(object sender, Model.TSDataEventArgs e)
        {
            ShowWarnInfo(e);
        }

        // 显示预警信息
        private void ShowWarnInfo(Model.TSDataEventArgs e)
        {
            if (e == null) return;

            if (gridControl1.InvokeRequired)
            {
                gridControl1.Invoke(new Action(delegate
                {
                    DataRow row = dt.NewRow();
                    row["KzCol"] = "告警";
                    row["TargetNumCol"] = e.Data.TargetNum.ToString();
                    row["RuleNameCol"] = e.AreaName;
                    row["WarnStartCol"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    row["TargetType"] = e.Data.TargetType;
                    dt.Rows.Add(row);

                    if (dt.Rows.Count > count)
                        dt.Rows.RemoveAt(0);
                }));
            }
            else
            {
                DataRow row = dt.NewRow();
                row["KzCol"] = "告警";
                row["TargetNumCol"] = e.Data.TargetNum.ToString();
                row["RuleNameCol"] = e.AreaName;
                row["WarnStartCol"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                row["TargetType"] = e.Data.TargetType;
                dt.Rows.Add(row);

                if (dt.Rows.Count > count)
                    dt.Rows.RemoveAt(0);
            }

            gridControl1.RefreshDataSource();
        }

        // 跳转到三维地球
        private void 暂停全部目标预警效果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = gvWarnInfo.GetFocusedDataRow();
                if (row == null) return;

                string name = row["TargetNumCol"].ToString();
                byte type = Convert.ToByte(row["TargetType"]);
                globeBusiness.JumpToPlane(type, name);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmWarnInfo), ex.Message);
            }
        }

        // 跳转到二维地图
        private void 跳转到二维地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = gvWarnInfo.GetFocusedDataRow();
                if (row == null) return;

                string name = row["TargetNumCol"].ToString();
                byte type = Convert.ToByte(row["TargetType"]);

                mapBusiness.JumpToPlane(type,name);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmWarnInfo), ex.Message);
            }
        }

        // 情况列表
        private void 清空预警列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dt.Clear();
            gridControl1.RefreshDataSource();
        }

        private void sbHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }




    }
}
