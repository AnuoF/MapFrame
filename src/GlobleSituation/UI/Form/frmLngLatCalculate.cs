    using System;
using System.Drawing;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geometry;
using GlobleSituation.Common;

namespace GlobleSituation.UI
{
    public partial class frmLngLatCalculate : DevExpress.XtraEditors.XtraForm
    {
        private string countLong = string.Empty;
        private string countLat = string.Empty;
        private ESRI.ArcGIS.Controls.AxGlobeControl globe = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uc_Ctrl"></param>
        public frmLngLatCalculate(MainControl uc_Ctrl)
        {
            InitializeComponent();
            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;
            globe = uc_Ctrl.GlobeMapContainer.globeCtrl.axGlobeControl1;
        }

        private void CheckNull()
        {
            if (string.IsNullOrWhiteSpace(text_InputAngle.Text))
            {
                dxErrorProvider1.SetError(text_InputAngle, "角度格式不符合标准。\r\n角度（-360～360)");
            }
            if (string.IsNullOrWhiteSpace(text_InputDistance.Text))
            {
                dxErrorProvider1.SetError(text_InputDistance, "请输入正整数与小数。\r\n范围（0～40076)");
            }
            if (string.IsNullOrWhiteSpace(text_InputLat.Text))
            {
                dxErrorProvider1.SetError(text_InputLat, "纬度格式不符合标准。\r\n纬度（-90～90）");
            }
            if (string.IsNullOrWhiteSpace(text_InputLon.Text))
            {
                dxErrorProvider1.SetError(text_InputLon, "经度格式不符合标准。\r\n经度（-180～180）");
            }
        }

        /// <summary>
        /// 地球半径，单位千米
        /// </summary>
        private const double Earth = 6378.137;
        /// <summary>
        /// 计算经纬度
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private string FormatPositionString(int type, float distance, double longitude, double latitude, double angle)
        {
            double lng = longitude;
            double lat = latitude;
            var strPosition = string.Empty;
            double c = (distance / Earth) * (180 / Math.PI);
            double a = Math.Acos(Math.Cos(90 - angle) * Math.Cos(c) + Math.Sin(90 - lat) * Math.Sin(c) * Math.Cos(angle));
            double d = Math.Asin((Math.Sin(c) * Math.Sin(angle)) / Math.Sin(a));
            switch (type)
            {
                case 0:
                    {
                        //double newLon = lng + (distance * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat * Math.PI / 180));
                        //strPosition = newLon.ToString();
                        strPosition = (lng + d).ToString();
                        break;
                    }
                case 1:
                    {
                        //double newLat = lat + (distance * Math.Cos(angle * Math.PI / 180)) / 111;
                        //strPosition = newLat.ToString();
                        strPosition = (90 - a).ToString();
                        break;
                    }
                default:
                    break;
            }

            return strPosition;
        }

        #region 验证
        /// <summary>
        /// 验证纬度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void text_InputLat_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(text_InputLat.Text))
            {
                //var str = CommonHelper.ToDBC(text_InputLat.Text);
                string str = text_InputLat.Text;
                if (Regex.IsMatch(str, @"^(-?((90)|((([0-8]\d)|(\d{1}))(\.\d+)?)))$"))
                {
                    dxErrorProvider1.SetError(text_InputLat, null);
                }
                else
                {
                    dxErrorProvider1.SetError(text_InputLat, "纬度格式不符合标准。\r\n纬度（-90～90）");
                }
            }
            else
            {
                dxErrorProvider1.SetError(text_InputLat, null);
            }
            countLong = string.Empty;
            countLat = string.Empty;
            text_OutputLat.Text = "";
            text_OutputLon.Text = " ";
            BtnPosition.Enabled = false;
        }

        /// <summary>
        /// 验证经度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void text_InputLon_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(text_InputLon.Text))
            {
                var str = text_InputLon.Text;
                if (Regex.IsMatch(str, @"^(-?((180)|(((1[0-7]\d)|(\d{1,2}))(\.\d+)?)))$"))
                {
                    dxErrorProvider1.SetError(text_InputLon, null);
                }
                else
                {
                    dxErrorProvider1.SetError(text_InputLon, "经度格式不符合标准。\r\n经度（-180～180）");
                }
            }
            else
            {
                dxErrorProvider1.SetError(text_InputLon, null);
            }
            countLong = string.Empty;
            countLat = string.Empty;
            text_OutputLat.Text = "";
            text_OutputLon.Text = " ";
            BtnPosition.Enabled = false;
        }

        /// <summary>
        /// 验证角度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void text_InputAngle_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(text_InputAngle.Text))
            {
                var str = text_InputAngle.Text;
                if (Regex.IsMatch(str, @"^(-?((360)|(((1[0-9][0-9])|(2[0-9][0-9])|(3[0-5]\d)|(\d{1,2}))(\.\d+)?)))$"))
                {
                    dxErrorProvider1.SetError(text_InputAngle, null);
                }
                else
                {
                    dxErrorProvider1.SetError(text_InputAngle, "角度格式不符合标准。\r\n角度（-360～360)");
                }
            }
            else
            {
                dxErrorProvider1.SetError(text_InputAngle, null);
            }
            countLong = string.Empty;
            countLat = string.Empty;
            text_OutputLat.Text = "";
            text_OutputLon.Text = "";
            BtnPosition.Enabled = false;
        }

        /// <summary>
        /// 验证距离
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void text_InputDistance_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(text_InputDistance.Text))
            {
                var str = text_InputDistance.Text;
                if (Regex.IsMatch(str, @"^\d+\.?\d*$"))
                {
                    var d = Convert.ToDouble(text_InputDistance.Text);
                    if (d > 40076)
                    {
                        dxErrorProvider1.SetError(text_InputDistance, "请输入正整数与小数。。\r\n范围（0～40076)");
                    }
                    else
                    {
                        dxErrorProvider1.SetError(text_InputDistance, null);
                    }

                }
                else
                {

                    dxErrorProvider1.SetError(text_InputDistance, "请输入正整数与小数。\r\n范围（0～40076)");
                }
            }
            else
            {
                dxErrorProvider1.SetError(text_InputDistance, null);
            }
            countLong = string.Empty;
            countLat = string.Empty;
            text_OutputLat.Text = " ";
            text_OutputLon.Text = " ";
            BtnPosition.Enabled = false;
        }
        #endregion

        #region 按钮事件

        /// <summary>
        /// 计算经纬度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            CheckNull();

            if (!dxErrorProvider1.HasErrors)
            {
                try
                {
                    var log = float.Parse(text_InputLon.Text); //角度 
                    var lat = float.Parse(text_InputLat.Text); //角度
                    var angle = float.Parse(text_InputAngle.Text); //角度
                    var distance = float.Parse(text_InputDistance.Text); //弧长
                    while (angle < 0)
                    {
                        angle += 360;
                    }
                    while (angle >= 360)
                    {
                        angle -= 360;
                    }

                    text_OutputLon.Text = FormatPositionString(0, distance, log, lat, angle);
                    text_OutputLat.Text = FormatPositionString(1, distance, log, lat, angle);

                    countLong = text_OutputLon.Text;
                    countLat = text_OutputLat.Text;
                    BtnPosition.Enabled = true;

                }
                catch (Exception)
                {
                }
            }
        }
        /// <summary>
        /// 定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPosition_Click(object sender, EventArgs e)
        {
            try
            {
                ISceneViewer m_ActiveView = globe.GlobeDisplay.ActiveViewer;
                IEnvelope enve = new EnvelopeClass();
                double newLng = Convert.ToDouble(countLong);
                double newLat = Convert.ToDouble(countLat);
                enve.PutCoords(newLng, newLat, newLng + 2, newLat + 2);
                enve.ZMin = 0;
                enve.ZMax = 0;
                globe.GlobeCamera.SetToZoomToExtents(enve, globe.Globe, m_ActiveView);
                m_ActiveView.Redraw(false);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(frmLngLatCalculate), ex.Message);
            }
        }

        // 关闭
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

       
    }
}
