using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using GlobleSituation.Model;
using System.Text;
using GlobleSituation.Business;
using DevExpress.XtraEditors;
using GlobleSituation.Common;
using MapFrame.Core.Model;

namespace GlobleSituation.UI
{
    public partial class DeviceDataList : UserControl
    {
        private DataTable dt = null;
        private ArcGlobeBusiness globeBusiness = null;
        private GMapControlBusiness mapBusiness = null;

        public DeviceDataList(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _mapBusiness)
        {
            InitializeComponent();

            globeBusiness = _globeBusiness;
            mapBusiness = _mapBusiness;
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);

            InitDataTable();
            InitDeviceData();
        }

        // 显示行号
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        /// <summary>
        /// 初始化数据表
        /// </summary>
        private void InitDataTable()
        {
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("DeviceName", typeof(string)),
                new DataColumn("DeviceNumber", typeof(string)),
                new DataColumn("RangeRadius", typeof(double)),
                new DataColumn("Lng", typeof(double)),
                new DataColumn("Lat", typeof(double)),
                new DataColumn("Alt", typeof(double)),
                new DataColumn("Belang", typeof(string)),
                new DataColumn("Remark", typeof(string)),
                new DataColumn("ColShow",typeof(string))
            });

            gridControl1.DataSource = dt;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitDeviceData()
        {
            Random r = new Random();

            List<DeviceData> devices = new List<DeviceData>();

            for (int i = 0; i < 10; i++)
            {
                DeviceData device = new DeviceData();
                device.DeviceName = "信号基站";
                device.DeviceNumber = "基站" + i;
                device.RangeRadius = r.Next(100, 200);
                device.Lng = r.Next(-100, 130);
                device.Lat = r.Next(-30, 30);
                device.Alt = 0;
                device.Belang = "12j";
                device.Remark = "备注" + i;
                devices.Add(device);
            }

            foreach (DeviceData d in devices)
            {
                DataRow row = dt.NewRow();
                row["DeviceName"] = d.DeviceName;
                row["DeviceNumber"] = d.DeviceNumber;
                row["RangeRadius"] = d.RangeRadius;
                row["Lng"] = d.Lng;
                row["Lat"] = d.Lat;
                row["Alt"] = d.Alt;
                row["Belang"] = d.Belang;
                row["Remark"] = d.Remark;
                row["ColShow"] = "显示";

                dt.Rows.Add(row);
            }
        }

        // 显示详细信息
        private void gridView1_MouseMove(object sender, MouseEventArgs e)
        {
            var hitInfo = gridView1.CalcHitInfo(e.X, e.Y);    //计算鼠标位置是否在行上

            if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendFormat("设备名称：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "DeviceName"));
                strBuilder.AppendFormat("设备编号：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "DeviceNumber"));
                strBuilder.AppendFormat("服务半径：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "RangeRadius"));
                strBuilder.AppendFormat("经度：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Lng"));
                strBuilder.AppendFormat("纬度：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Lat"));
                strBuilder.AppendFormat("高度：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Alt"));
                strBuilder.AppendFormat("所属单位：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Belang"));
                strBuilder.AppendFormat("备注：{0}\r\n", gridView1.GetRowCellValue(hitInfo.RowHandle, "Remark"));

                toolTipDetail.ShowHint(strBuilder.ToString(), Control.MousePosition);
            }
            else
            {
                toolTipDetail.HideHint();
            }
        }

        // 绘制设备服务
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                globeBusiness.DrawDeviceRanage(dt);
                mapBusiness.DrawDeviceRanage(dt);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(DeviceDataList), ex.Message);
            }
        }

        // 清除设备
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                globeBusiness.ClearDevice();
                mapBusiness.ClearDevice();
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(DeviceDataList), ex.Message);
            }
        }

        // 显示、隐藏设备
        private void repositoryItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBoxEdit cmb = sender as ComboBoxEdit;
                if (cmb == null) return;
                DataRow row = gridView1.GetFocusedDataRow();
                if (row == null) return;

                string name = row["DeviceNumber"].ToString();
                bool visible = cmb.SelectedIndex == 0 ? true : false;
                globeBusiness.SetDeviceVisible(name, visible);
                mapBusiness.SetDeviceVisible(name, visible);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(DeviceDataList), ex.Message);
            }
        }

        // 显示隐藏图层
        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            if (cb == null) return;

            globeBusiness.SetDeviceRangeLayerVisible(cb.Checked);
            mapBusiness.SetDeviceRangeLayerVisible(cb.Checked);
        }

        private void 跳转到二维地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = gridView1.GetFocusedDataRow();
                if (row == null) return;

                double lng = Convert.ToDouble(row["Lng"]);
                double lat = Convert.ToDouble(row["Lat"]);
                double alt = Convert.ToDouble(row["Alt"]);

                MapLngLat lngLat = new MapLngLat(lng, lat, alt);
                mapBusiness.mapLogic.GetToolBox().ZoomToPosition(lngLat);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(DeviceDataList), ex.Message);
            }
        }

        private void 跳转到三维地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = gridView1.GetFocusedDataRow();
                if (row == null) return;

                double lng = Convert.ToDouble(row["Lng"]);
                double lat = Convert.ToDouble(row["Lat"]);
                double alt = Convert.ToDouble(row["Alt"]);

                MapLngLat lngLat = new MapLngLat(lng, lat, alt);
                globeBusiness.mapLogic.GetToolBox().ZoomToPosition(lngLat);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(DeviceDataList), ex.Message);
            }
        }



    }
}
