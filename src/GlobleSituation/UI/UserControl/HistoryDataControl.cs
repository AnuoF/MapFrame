using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GlobleSituation.Model;
using GlobleSituation.Common;
using GlobleSituation.Business;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace GlobleSituation.UI
{
    public partial class uc_HistoryDataCtrl : XtraUserControl
    {

        /// <summary>
        /// 实时数据列表
        /// </summary>
        private List<RealData> dataLst = new List<RealData>();
        /// <summary>
        /// arcglobe业务
        /// </summary>
        private ArcGlobeBusiness globeBusiness = null;
        /// <summary>
        /// gmap业务
        /// </summary>
        private GMapControlBusiness gmapBusiness = null;

        public delegate void SearchDataDelegate(string sql, int totalCount);
        public SearchDataDelegate SearchData;

        public uc_HistoryDataCtrl(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _gmapBusiness)
        {
            InitializeComponent();

            dateEnd.DateTime = DateTime.Now; //设置默认时间
            dateStart.DateTime = dateEnd.DateTime.AddDays(-7);
            globeBusiness = _globeBusiness;
            gmapBusiness = _gmapBusiness;
        }

        private void gvRealData_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "时间")
            {
                try
                {
                    e.DisplayText = DateTime.FromFileTime((long)e.CellValue).ToString("yyyy-HH-MM HH:mm:ss");
                }
                catch
                {
                    e.DisplayText = "未知时间";
                }
            }
        }

        private void gvRealData_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

        }

        private void ToolStripMenuItemJump_Click(object sender, EventArgs e)
        {
            try
            {
                RealData focusedData = gvRealData.GetFocusedRow() as RealData;
                if (focusedData != null)
                {
                    MapLngLat lnglat = new MapLngLat(focusedData.Longitude, focusedData.Latitude, focusedData.Altitude);
                    globeBusiness.mapLogic.GetToolBox().ZoomToPosition(lnglat);
                    gmapBusiness.mapLogic.GetToolBox().ZoomToPosition(lnglat);
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("跳转失败：" + ex.Message);
            }
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            if (gcRealData.InvokeRequired)
            {
                gcRealData.Invoke((Action)delegate() { DeleteFocusedItem(); });
            }
            else
            {
                DeleteFocusedItem();
            }
        }

        /// <summary>
        /// 删除所选目标
        /// </summary>
        private void DeleteFocusedItem()
        {
            if (DevExpress.XtraEditors.XtraMessageBox.Show("是否从列表删除所选实时数据？", "删除提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (gvRealData.FocusedRowHandle < 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("您还未选中任何数据！", "删除提示", MessageBoxButtons.OK);
                }
                else
                {
                    dataLst.RemoveAt(gvRealData.FocusedRowHandle);

                    //TODO:删除数据库数据
                }
            }
        }

        private void ToolStripMenuItemClear_Click(object sender, EventArgs e)
        {
            if (DevExpress.XtraEditors.XtraMessageBox.Show("是否清空列表中所有历史数据？", "清空提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (gcRealData.InvokeRequired)
                {
                    gcRealData.Invoke((Action)delegate() { ClearListDelegate(); });
                }
                else
                {
                    ClearListDelegate();
                }
            }
        }

        /// <summary>
        /// 删除所有目标
        /// </summary>
        private void ClearListDelegate()
        {
            if (dataLst != null)
            {
                dataLst.Clear();
                gcRealData.RefreshDataSource();
                //TODO:清空
            }
        }

        private void ToolStripMenuItemExport_Click(object sender, EventArgs e)
        {
            if (saveRealDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (saveRealDialog.FilterIndex)
                    {
                        case 1: //CSV
                            gvRealData.ExportToCsv(saveRealDialog.FileName);
                            break;
                        case 2://PDF
                            gvRealData.ExportToPdf(saveRealDialog.FileName);
                            break;
                        case 3://HTML
                            gvRealData.ExportToHtml(saveRealDialog.FileName);
                            break;
                        case 4://RTF
                            gvRealData.ExportToRtf(saveRealDialog.FileName);
                            break;
                        case 5://EXCEL2003
                            gvRealData.ExportToXls(saveRealDialog.FileName);
                            break;
                        case 6://EXCEL2007
                            gvRealData.ExportToXlsx(saveRealDialog.FileName);
                            break;
                        default:
                            DevExpress.XtraEditors.XtraMessageBox.Show("请选择需要导出的文件格式！");
                            break;
                    }

                    DevExpress.XtraEditors.XtraMessageBox.Show("导出列表成功！");
                }
                catch (Exception ex)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出列表失败：" + ex.Message);
                }
            }
        }

        // 条件
        string condition = "";

        private void btnSearch_Click(object sender, EventArgs e)
        {
            totalPage = 0;
            currentPage = 1;
            lblPageInfo.Text = "第0页，共0页";

            if (dateStart.DateTime > dateEnd.DateTime)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("查询结束时间不能小于开始时间");
                return;
            }

            condition = string.Format(" where PositionDate between {0} and {1}", dateStart.DateTime.ToFileTime(), dateEnd.DateTime.ToFileTime());

            string sql = string.Format("select count(*) from RealData where PositionDate between {0} and {1}", dateStart.DateTime.ToFileTime(), dateEnd.DateTime.ToFileTime());

            //DataSet totalData = SQLiteHelper.ExecuteDataSet(sql);
            EventPublisher.RecvSearchDataEvent += new EventHandler<RecvSearchDataEventArgs>(EventPublisher_RecvSearchDataEvent);
            EventPublisher.PublishSendSearchDataToStoreEvent(this, new SendSearchDataToStoreEventArgs() { SqlStr = sql });

            //int dataCount = 0;

            //if (totalData != null && totalData.Tables.Count == 1 && totalData.Tables[0].Rows.Count == 1)
            //{
            //    dataCount = int.Parse(totalData.Tables[0].Rows[0][0].ToString());
            //    totalPage = (int)Math.Ceiling(dataCount / 100d);
            //}

            //// 不管查询是否成功，都调用委托
            //SearchData(condition, dataCount);

            //if (totalPage <= 1)
            //{
            //    SetBtnEnable(false, false);
            //}
            //else
            //{
            //    SetBtnEnable(false, true);
            //}

            //if (dataCount > 0)
            //{
            //    GetData(dateStart.DateTime.ToFileTime(), dateEnd.DateTime.ToFileTime(), 0);
            //}
            //else
            //{
            //    DevExpress.XtraEditors.XtraMessageBox.Show("未能查询到符合条件的数据");
            //    return;
            //}
        }

        void EventPublisher_RecvSearchDataEvent(object sender, RecvSearchDataEventArgs e)
        {
            EventPublisher.RecvSearchDataEvent -= new EventHandler<RecvSearchDataEventArgs>(EventPublisher_RecvSearchDataEvent);

            DataTable dt = Utils.DeserializeDataTableFromBytes(e.Data, false);

            int dataCount = 0;

            if (dt != null && dt.Rows.Count == 1)
            {
                dataCount = int.Parse(dt.Rows[0][0].ToString());
                totalPage = (int)Math.Ceiling(dataCount / 100d);
            }

            // 不管查询是否成功，都调用委托
            SearchData(condition, dataCount);

            if (totalPage <= 1)
            {
                SetBtnEnable(false, false);
            }
            else
            {
                SetBtnEnable(false, true);
            }

            if (dataCount > 0)
            {
                GetData(dateStart.DateTime.ToFileTime(), dateEnd.DateTime.ToFileTime(), 0);
            }
            else
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("未能查询到符合条件的数据");
                return;
            }
        }

        private List<RealData> FromDataTable(DataTable dt)
        {
            List<RealData> dataLst = new List<RealData>();

            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    RealData realData = new RealData();

                    realData.ActionRange = double.Parse(item["ActionRange"].ToString());
                    realData.Country = short.Parse(item["Country"].ToString());
                    realData.EquipModelNumber = item["EquipModelNumber"].ToString();
                    realData.Altitude = double.Parse(item["Altitude"].ToString());
                    realData.InformationSource = byte.Parse(item["InformationSource"].ToString());
                    realData.Latitude = double.Parse(item["Latitude"].ToString());
                    realData.Longitude = double.Parse(item["Longitude"].ToString());

                    string dateStr = item["PositionDate"].ToString().ToUpper();
                    int eIndex = dateStr.IndexOf("E");
                    if (eIndex > 0)
                    {
                        dateStr = dateStr.Replace("E", "");
                        string[] dateNumArray = dateStr.Split('+');
                        realData.PositionDate = (long)(double.Parse(dateNumArray[0]) * Math.Pow(10, int.Parse(dateNumArray[1])));
                    }
                    else
                    {
                        realData.PositionDate = long.Parse(item["PositionDate"].ToString());
                    }

                    realData.ScanRange = double.Parse(item["ScanRange"].ToString());
                    realData.TargetNum = long.Parse(item["TargetNum"].ToString());
                    realData.TargetProperty = byte.Parse(item["TargetProperty"].ToString());
                    realData.TargetType = byte.Parse(item["TargetType"].ToString());

                    dataLst.Add(realData);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return dataLst;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="beginIndex">Index of the begin.</param>
        private void GetData(long startTime, long endTime, int beginIndex)
        {
            if (gcRealData.InvokeRequired)
            {
                gcRealData.Invoke(new Action(delegate
                    {
                        gcRealData.DataSource = null;
                        gcRealData.RefreshDataSource();
                    }));
            }
            else
            {
                gcRealData.DataSource = null;
                gcRealData.RefreshDataSource();
            }

            //查询历史数据
            //控制播放进度---吐数据速度
            //对吐出的数据进行处理
            ShowModeChangedEventArgs showModeChangeEventArgs = new ShowModeChangedEventArgs() { Mode = ShowMode.HISTORY };
            TSDataEventArgs _TSDataEventArgs = new TSDataEventArgs() { Data = null };

            string sql = string.Format("select * from RealData where PositionDate between {0} and {1} order by PositionDate limit {2},100",
                startTime, endTime, beginIndex);

            EventPublisher.PublishSendSearchDataToStoreEvent(this, new SendSearchDataToStoreEventArgs() { SqlStr = sql });
            EventPublisher.RecvSearchDataEvent += new EventHandler<RecvSearchDataEventArgs>(EventPublisher_RecvSearchDataEvent11);
        }

        void EventPublisher_RecvSearchDataEvent11(object sender, RecvSearchDataEventArgs e)
        {
            EventPublisher.RecvSearchDataEvent -= new EventHandler<RecvSearchDataEventArgs>(EventPublisher_RecvSearchDataEvent11);
            DataTable dt = Utils.DeserializeDataTableFromBytes(e.Data, false);

            if (dt != null && dt.Rows.Count > 0)
            {
                dataLst = FromDataTable(dt);

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(delegate
                    {
                        gcRealData.DataSource = dataLst;
                        gcRealData.RefreshDataSource();
                        lblPageInfo.Text = string.Format("第{0}页，共{1}页", currentPage, totalPage);
                    }));
                }
                else
                {
                    gcRealData.DataSource = dataLst;
                    gcRealData.RefreshDataSource();
                    lblPageInfo.Text = string.Format("第{0}页，共{1}页", currentPage, totalPage);
                }

                foreach (var item in dataLst)
                {
                    //TODO
                    //m_arcGisGlobeCtrl.RcvTSData(item, true);
                }
            }
        }

        private void gvRealData_MouseLeave(object sender, EventArgs e)
        {
            toolTipDetail.HideHint();
        }

        private void gvRealData_MouseMove(object sender, MouseEventArgs e)
        {
            var hitInfo = gvRealData.CalcHitInfo(e.X, e.Y);    //计算鼠标位置是否在行上
            if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            {
                RealData realData = gvRealData.GetRow(hitInfo.RowHandle) as RealData;  //获取鼠标所在行
                if (realData != null)
                {
                    toolTipDetail.ShowHint(realData.ToString(), Control.MousePosition);
                }
            }
            else
            {
                toolTipDetail.HideHint();
            }
        }

        /// <summary>
        /// 当前页
        /// </summary>
        private int currentPage = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        private int totalPage = 0;

        private void SetBtnEnable(bool pre, bool next)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate
                {
                    btnPre.Enabled = pre;
                    btnNext.Enabled = next;
                }));
            }
            else
            {
                btnPre.Enabled = pre;
                btnNext.Enabled = next;
            }
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            if (totalPage == 1)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("当前条件只有一页数据！");
                return;
            }

            if (currentPage <= 1)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("已是第一页！");
                return;
            }

            currentPage -= 1;
            GetData(dateStart.DateTime.ToFileTime(), dateEnd.DateTime.ToFileTime(), (currentPage - 1) * 100);
            SetBtnEnable(currentPage > 1, currentPage < totalPage);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (totalPage == 1)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("当前条件只有一页数据！");
                return;
            }

            if (currentPage >= totalPage)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("已是最后页！");
                return;
            }

            currentPage += 1;
            GetData(dateStart.DateTime.ToFileTime(), dateEnd.DateTime.ToFileTime(), (currentPage - 1) * 100);
            SetBtnEnable(currentPage > 1, currentPage < totalPage);
        }

        private void gvRealData_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }

        }
    }
}