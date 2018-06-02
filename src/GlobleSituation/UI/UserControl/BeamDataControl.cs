using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GlobleSituation.Common;
using GlobleSituation.Model;

namespace GlobleSituation.UI
{
    public partial class BeamDataControl : XtraUserControl
    {
        /// <summary>
        /// 波束数据表
        /// </summary>
        private DataTable dt = null;
        /// <summary>
        /// 实时显示的数据条数
        /// </summary>
        private int dataCount = 100;
        /// <summary>
        /// 数据是否滚动显示
        /// </summary>
        private bool bAutoShow = false;


        public BeamDataControl()
        {
            InitializeComponent();

            InitDataTable();
            EventPublisher.BeamDataComeEvent += EventPublisher_BeamDataComeEvent;
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
        }

        // 初始化数据表
        private void InitDataTable()
        {
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("StateId", typeof(int)),
                new DataColumn("BeamId", typeof(int)),
                new DataColumn("Lng", typeof(double)),
                new DataColumn("Lat", typeof(double)),
                new DataColumn("Alt", typeof(double)),
                new DataColumn("BType", typeof(string))
            });

            gridControl1.DataSource = dt;
        }

        // 接收卫星波束数据
        private void EventPublisher_BeamDataComeEvent(object sender, BeamData e)
        {
            if (e == null) return;

            if (gridControl1.InvokeRequired)
            {
                gridControl1.Invoke(new Action(delegate
                {
                    if (dt.Rows.Count > dataCount)
                    {
                        dt.Rows.RemoveAt(0);
                    }

                    DataRow row = dt.NewRow();
                    row["StateId"] = e.SatelliteId;
                    row["BeamId"] = e.BeamId;
                    row["Lng"] = e.Point.Lng;
                    row["Lat"] = e.Point.Lat;
                    row["Alt"] = e.Point.Alt;
                    row["BType"] = e.PointType == 0 ? "卫星" : "波束";
                    dt.Rows.Add(row);

                    if (bAutoShow == true)
                    {
                        gridView1.FocusedRowHandle = dt.Rows.Count - 1;
                    }
                }));
            }
            else
            {
                if (dt.Rows.Count > dataCount)
                {
                    dt.Rows.RemoveAt(0);
                }

                DataRow row = dt.NewRow();
                row["StateId"] = e.SatelliteId;
                row["BeamId"] = e.BeamId;
                row["Lng"] = e.Point.Lng;
                row["Lat"] = e.Point.Lat;
                row["Alt"] = e.Point.Alt;
                row["BType"] = e.PointType == 0 ? "卫星" : "波束";
                dt.Rows.Add(row);

                if (bAutoShow == true)
                {
                    gridView1.FocusedRowHandle = dt.Rows.Count - 1;
                }
            }
        }

        // 数据滚动
        private void 数据滚动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bAutoShow = !bAutoShow;
        }

        // 显示行号
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        // 显示详细信息
        private void gridView1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var hitInfo = gridView1.CalcHitInfo(e.X, e.Y);    //计算鼠标位置是否在行上
            if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendFormat("卫星编号：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "StateId"));
                strBuilder.AppendFormat("波束编号：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "BeamId"));
                strBuilder.AppendFormat("经度：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Lng"));
                strBuilder.AppendFormat("纬度：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Lat"));
                strBuilder.AppendFormat("高度：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Alt"));
                strBuilder.AppendFormat("数据类型：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "BType"));

                toolTipDetail.ShowHint(strBuilder.ToString(), Control.MousePosition);
            }
            else
            {
                toolTipDetail.HideHint();
            }
        }
    }
}
