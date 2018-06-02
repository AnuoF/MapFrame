using GlobleSituation.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using GlobleSituation.Business;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Collections.Generic;
using System.Data;
using DevExpress.XtraEditors;
using System.Drawing;

namespace GlobleSituation.UI
{
    /// <summary>
    /// 预警区域管理窗口
    /// </summary>

    public partial class frmAreaManager : DevExpress.XtraEditors.XtraForm
    {
        #region 内部成员
        /// <summary>
        /// 预警区域存放文件
        /// </summary>
        private string listFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Data\AreaList.xml";

        private WarnManager wanrMgr = null;
        private IMapLogic mapLogic = null;
        private DataTable dt = null;
        private DataTable dtDetail = null;
        #endregion

        #region 构造
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public frmAreaManager(WarnManager _wanrMgr)
        {
            InitializeComponent();

            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            this.Icon = global::GlobleSituation.Properties.Resources.App;

            this.MouseWheel += FrmAreaManager_MouseWheel;
            wanrMgr = _wanrMgr;
            mapLogic = _wanrMgr.mapMapLogic;

            LoadAreaFromXml(listFile);

            InitDataTable();
        }

        private void InitDataTable()
        {
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("areaName", typeof(string)),
                new DataColumn("Type", typeof(string)),
                new DataColumn("IsWarningArea", typeof(string)),
                new DataColumn("IsImportant", typeof(string)),
                new DataColumn("IsVisible", typeof(string)),
                new DataColumn("Color", typeof(Color))
            });

            gcAreaMangaer.DataSource = dt;

            dtDetail = new DataTable();
            dtDetail.Columns.AddRange(new DataColumn[] {
                new DataColumn("Longitude", typeof(double)),
                new DataColumn("Latgitude", typeof(double)),
                new DataColumn("Altitude", typeof(double))
            });

            gcPoints.DataSource = dtDetail;
        }

        #endregion

        #region 预警区域管理
        /// <summary>
        /// 删除所选预警区域
        /// </summary>
        private void DeleteSelectedArea()
        {
            try
            {
                if (gvPointList.FocusedRowHandle >= 0)
                {
                    DataRow row = dt.Rows[gvPointList.FocusedRowHandle];
                    if (row == null) return;

                    string name = row["areaName"].ToString();
                    wanrMgr.DeleteArea(name);

                    dt.Rows.RemoveAt(gvPointList.FocusedRowHandle);

                    if (dt.Rows.Count == 0)
                    {
                        // if there no data in gridviewControl,clear the point in gvPointList.
                        dtDetail.Clear();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 删除所选预警区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除预警区域DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedArea();
        }

        /// <summary>
        /// 从Xml文件加载预警区域
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public bool LoadAreaFromXml(string xmlFile)
        {
            return false;
        }

        /// <summary>
        /// 将预警区域保存到Xml文件
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        [Description("将预警区域保存到Xml文件")]
        public bool SaveAreaToXml(string xmlFile)
        {
            return false;
        }

        /// <summary>
        /// 增加预警区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddArea_Click(object sender, EventArgs e)
        {
            var tool = mapLogic.GetToolBox();
            if (tool == null) return;

            ElementTypeEnum type = cmbAreaType.Text == "多边形" ? ElementTypeEnum.Polygon : ElementTypeEnum.Rectangle;
            tool.CommondExecutedEvent += Tool_CommondExecutedEvent;
            tool.DrawGraphic(type);
            this.Visible = false;
        }

        // 绘图完成事件
        private void Tool_CommondExecutedEvent(object sender, MapFrame.Core.Model.MessageEventArgs e)
        {
            this.Visible = true;

            var tool = mapLogic.GetToolBox();
            if (tool == null) return;

            tool.CommondExecutedEvent -= Tool_CommondExecutedEvent;
            tool.ReleaseTool();

            IMFPolygon polygon = e.Data as IMFPolygon;
            if (polygon == null) return;

            List<MapLngLat> points = polygon.GetLngLat();
            if (points == null || points.Count <= 0) return;

            txtAreaName.Text = polygon.ElementName;
            var layer = mapLogic.GetLayer(polygon.BelongLayer.LayerName);
            layer.RemoveElement(polygon);

            bool isWarn = cmbIsWarningArea.SelectedIndex == 0 ? true : false;
            bool isImportant = cmbImportantArea.SelectedIndex == 0 ? true : false;

            wanrMgr.DrawArea(txtAreaName.Text, points, isWarn, isImportant);

            ShowInfo(txtAreaName.Text);
        }
        #endregion

        #region 界面处理

        /// <summary>
        /// 编辑时不显示错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAreaName_EditValueChanged(object sender, EventArgs e)
        {
            dxErrorProviderMap.ClearErrors();
        }

        /// <summary>
        /// 关闭改隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAreaManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        /// <summary>
        /// 显示端点行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvPointList_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        /// <summary>
        /// 显示区域行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvAreaList_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        /// <summary>
        /// 焦点行改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvAreaList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)delegate() { RefreshSelectedAreaInfo(e); });
                }
                else
                {
                    RefreshSelectedAreaInfo(e);
                }
            }
        }

        /// <summary>
        /// 刷新所选预警区域信息
        /// </summary>
        /// <param name="e"></param>
        private void RefreshSelectedAreaInfo(DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                if (gvPointList.FocusedRowHandle < 0) return;

                DataRow row = gvAreaList.GetFocusedDataRow();
                if (row == null) return;

                string name = row["areaName"].ToString();

                ShowDetail(name);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 选择删除行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvAreaList_KeyDown(object sender, KeyEventArgs e)
        {
            if (gvAreaList.FocusedRowHandle >= 0 && e.KeyCode == Keys.Delete)
            {
                DeleteSelectedArea();
            }
        }

        // 保存端点修改
        private void btnSaveChange_Click(object sender, EventArgs e)
        {
            try
            {
                DataView t = gvAreaList.DataSource as DataView;
                if (t == null || t.Table == null || t.Table.Rows.Count <= 0) return;

                var dtTmp = gvPointList.DataSource as DataView;
                if (dtTmp == null || dtTmp.Table == null || dtTmp.Table.Rows.Count <= 0) return;

                string name = gcPoints.Tag.ToString();
                List<MapLngLat> points = new List<MapLngLat>();
                foreach (DataRow row in dtTmp.Table.Rows)
                {
                    double lng = Convert.ToDouble(row["Longitude"]);
                    double lat = Convert.ToDouble(row["Latgitude"]);
                    points.Add(new MapLngLat(lng, lat));
                }

                wanrMgr.ReDrawArea(name, points);

                Color c = Color.Blue;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["areaName"].ToString() == name)
                    {
                        c = (Color)row["Color"];
                        break;
                    }
                }

                wanrMgr.SetAreaColor(name, c);
            }
            catch (Exception)
            {
            }
        }

        // 关闭
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        #endregion


        private void FrmAreaManager_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                double opacity = this.Opacity;
                opacity += 0.1D;
                if (opacity > 1D)
                {
                    this.Opacity = 1D;
                }
                else
                    this.Opacity = opacity;
            }
            else
            {
                double opacity = this.Opacity;
                opacity -= 0.1D;
                if (opacity < 0.3D)
                {
                    this.Opacity = 0.3D;
                }
                else
                    this.Opacity = opacity;
            }
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bWarn"></param>
        private void ShowInfo(string name)
        {
            WarnArea area = wanrMgr.GetAreaByName(name);
            if (area == null) return;

            DataRow row = dt.NewRow();
            row["areaName"] = name;
            row["Type"] = "多边形";
            row["IsWarningArea"] = area.IsWarn == true ? "是" : "否";
            row["IsImportant"] = area.IsImportant == true ? "是" : "否";
            row["IsVisible"] = area.IsVisible == true ? "是" : "否";
            row["Color"] = Color.Blue;

            dt.Rows.Add(row);

            gvAreaList.FocusedRowHandle = dt.Rows.Count - 1;

            if (gvAreaList.FocusedRowHandle > -1)
            {
                dtDetail.Rows.Clear();

                foreach (MapLngLat point in area.Points)
                {
                    DataRow r = dtDetail.NewRow();
                    r["Longitude"] = point.Lng;
                    r["Latgitude"] = point.Lat;
                    r["Altitude"] = wanrMgr.Atilute;
                    dtDetail.Rows.Add(r);
                }
            }

            gcPoints.Tag = name;
        }

        /// <summary>
        /// 显示详细信息
        /// </summary>
        /// <param name="name"></param>
        private void ShowDetail(string name)
        {
            WarnArea area = wanrMgr.GetAreaByName(name);
            if (area == null) return;

            if (gvAreaList.FocusedRowHandle > -1)
            {
                dtDetail.Rows.Clear();

                foreach (MapLngLat point in area.Points)
                {
                    DataRow r = dtDetail.NewRow();
                    r["Longitude"] = point.Lng;
                    r["Latgitude"] = point.Lat;
                    r["Altitude"] = wanrMgr.Atilute;
                    dtDetail.Rows.Add(r);
                }
            }

            gcPoints.Tag = name;
            gvPointList.RefreshData();
        }

        // 预警
        private void cmbAreaIsWarningChanged_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBoxEdit cmb = sender as ComboBoxEdit;
                if (cmb == null) return;
                if (gvPointList.FocusedRowHandle >= 0)
                {
                    DataRow row = dt.Rows[gvPointList.FocusedRowHandle];
                    if (row == null) return;

                    string name = row["areaName"].ToString();
                    bool isWarn = cmb.SelectedIndex == 0 ? true : false;
                    wanrMgr.SetAreaWarn(name, isWarn);
                }
            }
            catch (Exception)
            {
            }
        }

        // 显示隐藏
        private void cmbAreaVisibleChanged_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBoxEdit cmb = sender as ComboBoxEdit;
                if (cmb == null) return;
                DataRow row = gvAreaList.GetFocusedDataRow();
                if (row == null) return;

                string name = row["areaName"].ToString();
                bool visible = cmb.SelectedIndex == 0 ? true : false;
                wanrMgr.SetAreaVisible(name, visible);
            }
            catch (Exception)
            {
            }
        }

        // 颜色
        private void repositoryItemColorEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ColorEdit edit = sender as ColorEdit;
                if (edit == null) return;

                DataRow row = gvAreaList.GetFocusedDataRow();
                if (row == null) return;

                string name = row["areaName"].ToString();
                Color color = edit.Color;

                wanrMgr.SetAreaColor(name, color);
            }
            catch (Exception)
            {
            }
        }

        // 重点区域
        private void repositoryItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBoxEdit cmb = sender as ComboBoxEdit;
                if (cmb == null) return;

                if (gvPointList.FocusedRowHandle >= 0)
                {
                    DataRow row = dt.Rows[gvPointList.FocusedRowHandle];
                    if (row == null) return;

                    string name = row["areaName"].ToString();
                    bool isImportant = cmb.SelectedIndex == 0 ? true : false;
                    wanrMgr.SetAreaImportant(name, isImportant);
                }
            }
            catch (Exception)
            {
            }
        }

        private void 查看统计分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (gvPointList.FocusedRowHandle >= 0)
                {
                    DataRow row = gvAreaList.GetFocusedDataRow();
                    if (row == null) return;

                    string name = row["areaName"].ToString();

                    WarnArea area = wanrMgr.GetAreaByName(name);
                    if (area == null) return;

                    frmWarnStat f = new frmWarnStat(area);
                    {
                        f.TopMost = true;
                        f.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmAreaManager), ex.Message);
            }
        }

    }
}