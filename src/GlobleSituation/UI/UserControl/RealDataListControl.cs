
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GlobleSituation.Model;
using GlobleSituation.Common;
using MapFrame.Core.Interface;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    public partial class RealDataListControl : DevExpress.XtraEditors.XtraUserControl
    {
        /// <summary>
        /// 目标上限
        /// </summary>
        private const int TargetLimit = 5000;
        /// <summary>
        /// 实时数据列表
        /// </summary>
        private List<RealData> dataLst = new List<RealData>();
        /// <summary>
        /// 数据库写入线程
        /// </summary>
        private System.Threading.Thread dbWriteThread = null;
        /// <summary>
        /// 实时数据缓冲
        /// </summary>
        private List<RealData> dataBufferListA = new List<RealData>();
        /// <summary>
        /// 实时数据缓冲
        /// </summary>
        private List<RealData> dataBufferListB = new List<RealData>();
        /// <summary>
        /// 缓冲区A使用标志
        /// </summary>
        private bool useBufferA = false;
        /// <summary>
        /// 地图框架
        /// </summary>
        private ArcGlobeBusiness globeBusiness = null;
        private GMapControlBusiness mapBusiness = null;
        /// <summary>
        /// 态势图层名称
        /// </summary>
        private string layerName = "天空态势图层";


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_globeBusiness"></param>
        public RealDataListControl(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _mapBusiness)
        {
            InitializeComponent();

            globeBusiness = _globeBusiness;
            mapBusiness = _mapBusiness;
            dbWriteThread = new System.Threading.Thread(WriteDB);
            dbWriteThread.Start();
        }

        public void Initial()
        {
            EventPublisher.TSDataEvent += Event_RcvRealDataEvent;
        }

        /// <summary>
        /// 写入数据库
        /// </summary>
        private void WriteDB()
        {
            while (true)
            {
                if (useBufferA)
                {
                    WriteListIntoDB(dataBufferListA, false);
                }
                else
                {
                    WriteListIntoDB(dataBufferListB, true);
                }

                System.Threading.Thread.Sleep(5000);
            }
        }

        private void WriteListIntoDB(List<RealData> dataBufferList, bool useA)
        {
            if (dataBufferList.Count > 0)
            {
                lock (dataBufferList)
                {
                    //SQLiteHelper.Data2Db(dataBufferList);
                    if (Utils.Mode == ShowMode.REAL_TIME)
                        EventPublisher.PublishSendInsertDataToStoreEvent(this, new SendInsertDataToStoreEventArgs() { DataList = dataBufferList });
                    dataBufferList.Clear();
                }
            }
            useBufferA = useA;
        }

        void Event_RcvRealDataEvent(object sender, TSDataEventArgs e)
        {
            if (e.Data == null) return;
            AddData(e.Data);

            if (useBufferA)
            {
                dataBufferListB.Add(e.Data);
            }
            else
            {
                dataBufferListA.Add(e.Data);
            }
        }

        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvRealData_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="realData"></param>
        public void AddData(RealData realData)
        {
            if (gcRealData.InvokeRequired)
            {
                gcRealData.Invoke((Action)delegate() { AddDataDelegate(realData); });
            }
            else
            {
                AddDataDelegate(realData);
            }
        }

        /// <summary>
        /// 增加数据委托
        /// </summary>
        /// <param name="realData"></param>
        private void AddDataDelegate(RealData realData)
        {
            if (dataLst.Count >= TargetLimit)
            {
                dataLst.Clear();
            }

            int oldDataIndex = dataLst.FindIndex(i => i.TargetNum == realData.TargetNum);

            if (oldDataIndex < 0)
            {
                dataLst.Add(realData);

                if (ToolStripMenuItemRefresh.Checked)
                {
                    gvRealData.FocusedRowHandle = dataLst.Count - 1;
                }
            }
            else
            {
                dataLst[oldDataIndex] = realData;
            }
            gcRealData.DataSource = dataLst;
            gcRealData.RefreshDataSource();
        }

        /// <summary>
        /// 删除目标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    try
                    {
                        dataLst.RemoveAt(gvRealData.FocusedRowHandle);

                        //TODO:删除地图数据
                        RealData data = gvRealData.GetFocusedRow() as RealData;
                        if (data == null) return;

                        string elementName = data.TargetNum.ToString();
                        string lineName = elementName + "line";

                        var layer = globeBusiness.mapLogic.GetLayer(layerName);
                        if (layer == null) return;

                        layer.RemoveElement(elementName);
                        layer.RemoveElement(lineName);

                        var layerMap = mapBusiness.mapLogic.GetLayer(layerName);
                        if (layerMap != null)
                        {
                            layerMap.RemoveElement(elementName);
                            layerMap.RemoveElement(lineName);
                        }

                        gcRealData.RefreshDataSource();
                    }
                    catch (Exception ex)
                    {
                        Log4Allen.WriteLog(typeof(RealDataListControl), ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 清空目标列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemClear_Click(object sender, EventArgs e)
        {
            if (DevExpress.XtraEditors.XtraMessageBox.Show("是否清空列表中所有实时数据？", "清空提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            try
            {
                if (dataLst != null)
                {
                    dataLst.Clear();
                    gcRealData.RefreshDataSource();
                    //TODO:清空

                    globeBusiness.ClearRealData();
                    mapBusiness.ClearRealData();
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(RealDataListControl), ex.Message);
            }
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 显示详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 隐藏详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvRealData_MouseLeave(object sender, EventArgs e)
        {
            toolTipDetail.HideHint();
        }

        private void gvRealData_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == "时间" && e.CellValue != null)
            {
                e.DisplayText = DateTime.FromFileTime((long)e.CellValue).ToString("yyyy-HH-MM HH:mm:ss");
            }
        }

        #region 目标操作


        private void 跳转ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RealData focusedData = gvRealData.GetFocusedRow() as RealData;

            if (focusedData != null)
            {
                try
                {
                    globeBusiness.mapLogic.GetToolBox().ZoomToPosition(new MapFrame.Core.Model.MapLngLat(focusedData.Longitude, focusedData.Latitude, focusedData.Altitude));
                }
                catch (Exception ex)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("跳转失败：" + ex.Message);
                }
            }
        }

        private void 闪烁ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RealData data = gvRealData.GetFocusedRow() as RealData;
            if (data == null) return;

            try
            {
                var element = globeBusiness.mapLogic.GetElement(data.TargetNum.ToString());
                if (element == null) return;
                I3DModel model = element as I3DModel;
                if (model == null) return;

                bool flash = !model.IsFlash;
                model.Flash(flash, 500);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(RealDataListControl), ex.Message);
            }
        }

        private void 设置颜色ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RealData data = gvRealData.GetFocusedRow() as RealData;
            if (data == null) return;

            try
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        var color = dlg.Color;

                        var element = globeBusiness.mapLogic.GetElement(data.TargetNum.ToString());
                        if (element == null) return;
                        I3DModel model = element as I3DModel;
                        if (model == null) return;

                        model.SetColor(color.ToArgb());
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(RealDataListControl), ex.Message);
            }
        }
        #endregion

    }
}