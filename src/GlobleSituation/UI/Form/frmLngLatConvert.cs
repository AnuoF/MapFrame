using GlobleSituation.Common;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GlobleSituation.UI
{
    public partial class frmLngLatConvert : DevExpress.XtraEditors.XtraForm
    {
        public frmLngLatConvert()
        {
            InitializeComponent();

            this.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            Icon icon = global::GlobleSituation.Properties.Resources.App;
            this.Icon = icon;
        }

        #region 按钮事件
        private void sbClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        #endregion

        #region 度转换

        ///// <summary>
        ///// 转换度
        ///// </summary>
        //private void ConversionDu()
        //{
        //    if(radLngLat.SelectedIndex==0)
        //    {
        //        FormatLongitudeToCelsiusHanzi(double.Parse(txtInput.Text.Trim()));
        //    }
        //    else
        //    {
        //        FormatLatitudeToCelsiusHanzi(double.Parse(txtInput.Text.Trim()));
        //    }
        //}

        ///// <summary>
        /////     转换为°
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SignConversionDu()
        //{
        //    if (radLngLat.SelectedIndex == 0)
        //    {
        //        FormatLongitudeToCelsius(double.Parse(txtInput.Text.Trim()));
        //    }
        //    else
        //    {
        //        FormatLatitudeToCelsius(double.Parse(txtInput.Text.Trim()));
        //    }
        //}

        /// <summary>
        ///     转换经度为°格式
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string FormatLongitudeToCelsius(double longitude)
        {
            var jd = longitude;
            var tipJD = string.Empty;

            if (jd >= 0) //东经E
            {
                tipJD = "E";
            }
            else
            {
                tipJD = "W";
                jd = 0 - jd;
            }

            int Celsius_JD, Minute_JD;
            string strCelsius_JD, strMinute_JD;
            var strDMSJD = "";
            Celsius_JD = (int)Math.Floor(jd);
            if (Celsius_JD < 10) //只有一位，补两个0
            {
                strCelsius_JD = "00" + Celsius_JD;
            }
            else if (Celsius_JD < 100) //只有两位，补一个0
            {
                strCelsius_JD = "0" + Celsius_JD;
            }
            else
            {
                strCelsius_JD = Celsius_JD.ToString();
            }
            if ((jd - Celsius_JD) == 0)
            {
                strCelsius_JD += "°";
                strDMSJD = string.Format("{0}{1}", strCelsius_JD, tipJD);
            }
            else
            {
                Minute_JD = (int)Math.Floor((jd - Celsius_JD) * 60);
                if (Minute_JD < 10) //只有一位，补一个0
                {
                    strMinute_JD = "0" + Minute_JD;
                    strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD + ".", strMinute_JD + "°", tipJD);
                }
                else
                {
                    if (Minute_JD >= 60)
                    {
                        Celsius_JD++;
                        if (Celsius_JD < 10) //只有一位，补两个0
                        {
                            strCelsius_JD = "00" + Celsius_JD;
                        }
                        else if (Celsius_JD < 100) //只有两位，补一个0
                        {
                            strCelsius_JD = "0" + Celsius_JD;
                        }
                        else
                        {
                            strCelsius_JD = Celsius_JD.ToString();
                        }

                        strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD + ".", (Minute_JD - 60) + "°", tipJD);
                    }
                    else
                    {
                        strMinute_JD = Minute_JD.ToString();
                        strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD + ".", strMinute_JD + "°", tipJD);
                    }
                }
            }

            return strDMSJD;
        }

        /// <summary>
        ///     转换纬度为°格式
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public string FormatLatitudeToCelsius(double latitude)
        {
            var wd = latitude;
            var tipWD = string.Empty;

            if (wd >= 0) //北纬N
            {
                tipWD = "N";
            }
            else
            {
                tipWD = "S"; //南纬S
                wd = 0 - wd;
            }
            int Celsius_WD, Minute_WD;
            string strCelsius_WD, strMinute_WD;
            var strDMSWD = "";
            Celsius_WD = (int)Math.Floor(wd);
            if (Celsius_WD < 10) //只有一位，补一个0
            {
                strCelsius_WD = "0" + Celsius_WD;
            }
            else
            {
                strCelsius_WD = Celsius_WD.ToString();
            }
            if ((wd - Celsius_WD) == 0)
            {
                strCelsius_WD += "°";
                strDMSWD = string.Format("{0}{1}", strCelsius_WD, tipWD);
            }
            else
            {
                Minute_WD = (int)Math.Floor((wd - Celsius_WD) * 60);
                if (Minute_WD < 10) //只有一位，补一个0
                {
                    strMinute_WD = "0" + Minute_WD;
                    strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD + ".", strMinute_WD + "°", tipWD);
                }
                else
                {
                    if (Minute_WD >= 60)
                    {
                        Celsius_WD++;
                        if (Celsius_WD < 10) //只有一位，补一个0
                        {
                            strCelsius_WD = "0" + Celsius_WD;
                        }
                        else
                        {
                            strCelsius_WD = Celsius_WD.ToString();
                        }

                        strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD + ".", (Minute_WD - 60) + "°", tipWD);
                    }
                    else
                    {
                        strMinute_WD = Minute_WD.ToString();
                        strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD + ".", strMinute_WD + "°", tipWD);
                    }
                }
            }

            return strDMSWD;
        }

        /// <summary>
        ///     转换经度为度格式
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string FormatLongitudeToCelsiusHanzi(double longitude)
        {
            var jd = longitude;
            var tipJD = string.Empty;

            if (jd >= 0) //东经E
            {
                tipJD = "E";
            }
            else
            {
                tipJD = "W";
                jd = 0 - jd;
            }

            int Celsius_JD, Minute_JD;
            string strCelsius_JD, strMinute_JD;
            var strDMSJD = "";
            Celsius_JD = (int)Math.Floor(jd);
            if (Celsius_JD < 10) //只有一位，补两个0
            {
                strCelsius_JD = "00" + Celsius_JD;
            }
            else if (Celsius_JD < 100) //只有两位，补一个0
            {
                strCelsius_JD = "0" + Celsius_JD;
            }
            else
            {
                strCelsius_JD = Celsius_JD.ToString();
            }
            if ((jd - Celsius_JD) == 0)
            {
                strCelsius_JD += "度";
                strDMSJD = string.Format("{0}{1}", strCelsius_JD, tipJD);
            }
            else
            {
                Minute_JD = (int)Math.Floor((jd - Celsius_JD) * 60);
                if (Minute_JD < 10) //只有一位，补一个0
                {
                    strMinute_JD = "0" + Minute_JD;
                    strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD + ".", strMinute_JD + "度", tipJD);
                }
                else
                {
                    if (Minute_JD >= 60)
                    {
                        Celsius_JD++;
                        if (Celsius_JD < 10) //只有一位，补两个0
                        {
                            strCelsius_JD = "00" + Celsius_JD;
                        }
                        else if (Celsius_JD < 100) //只有两位，补一个0
                        {
                            strCelsius_JD = "0" + Celsius_JD;
                        }
                        else
                        {
                            strCelsius_JD = Celsius_JD.ToString();
                        }

                        strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD + ".", (Minute_JD - 60) + "度", tipJD);
                    }
                    else
                    {
                        strMinute_JD = Minute_JD.ToString();
                        strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD + ".", strMinute_JD + "度", tipJD);
                    }
                }
            }

            return strDMSJD;
        }

        /// <summary>
        ///     转换纬度为度格式
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public string FormatLatitudeToCelsiusHanzi(double latitude)
        {
            var wd = latitude;
            var tipWD = string.Empty;

            if (wd >= 0) //北纬N
            {
                tipWD = "N";
            }
            else
            {
                tipWD = "S"; //南纬S
                wd = 0 - wd;
            }
            int Celsius_WD, Minute_WD;
            string strCelsius_WD, strMinute_WD;
            var strDMSWD = "";
            Celsius_WD = (int)Math.Floor(wd);
            if (Celsius_WD < 10) //只有一位，补一个0
            {
                strCelsius_WD = "0" + Celsius_WD;
            }
            else
            {
                strCelsius_WD = Celsius_WD.ToString();
            }
            if ((wd - Celsius_WD) == 0)
            {
                strCelsius_WD += "度";
                strDMSWD = string.Format("{0}{1}", strCelsius_WD, tipWD);
            }
            else
            {
                Minute_WD = (int)Math.Floor((wd - Celsius_WD) * 60);
                if (Minute_WD < 10) //只有一位，补一个0
                {
                    strMinute_WD = "0" + Minute_WD;
                    strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD + ".", strMinute_WD + "度", tipWD);
                }
                else
                {
                    if (Minute_WD >= 60)
                    {
                        Celsius_WD++;
                        if (Celsius_WD < 10) //只有一位，补一个0
                        {
                            strCelsius_WD = "0" + Celsius_WD;
                        }
                        else
                        {
                            strCelsius_WD = Celsius_WD.ToString();
                        }

                        strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD + ".", (Minute_WD - 60) + "度", tipWD);
                    }
                    else
                    {
                        strMinute_WD = Minute_WD.ToString();
                        strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD + ".", strMinute_WD + "度", tipWD);
                    }
                }
            }

            return strDMSWD;
        }

        #endregion

        #region 度分转换

        ///// <summary>
        /////     转换为度分
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ConversionDuFen()
        //{
        //    if (radLngLat.SelectedIndex == 0)
        //    {
        //        FormatLongitudeToCelsiusMinuteHanzi(double.Parse(txtInput.Text.Trim()));
        //    }
        //    else
        //    {
        //        FormatLatitudeToCelsiusMinuteHanzi(double.Parse(txtInput.Text.Trim()));
        //    }
        //}

        ///// <summary>
        /////     转换为°′
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SignConversionDuFen()
        //{
        //    if (radLngLat.SelectedIndex == 0)
        //    {
        //        FormatLongitudeToCelsiusMinute(double.Parse(txtInput.Text.Trim()));
        //    }
        //    else
        //    {
        //        FormatLatitudeToCelsiusMinute(double.Parse(txtInput.Text.Trim()));
        //    }
        //}

        /// <summary>
        ///     转换经度为°′格式
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string FormatLongitudeToCelsiusMinute(double longitude)
        {
            var jd = longitude;
            var tipJD = string.Empty;

            if (jd >= 0) //东经E
            {
                tipJD = "E";
            }
            else
            {
                tipJD = "W";
                jd = 0 - jd;
            }

            int Celsius_JD, Minute_JD;
            string strCelsius_JD, strMinute_JD;

            Celsius_JD = (int)Math.Floor(jd);
            if (Celsius_JD < 10) //只有一位，补两个0
            {
                strCelsius_JD = "00" + Celsius_JD;
            }
            else if (Celsius_JD < 100) //只有两位，补一个0
            {
                strCelsius_JD = "0" + Celsius_JD;
            }
            else
            {
                strCelsius_JD = Celsius_JD.ToString();
            }
            strCelsius_JD += "°";

            Minute_JD = (int)Math.Floor((jd - Celsius_JD) * 60);
            if (Minute_JD < 10) //只有一位，补一个0
            {
                strMinute_JD = "0" + Minute_JD;
            }
            else
            {
                strMinute_JD = Minute_JD.ToString();
            }
            strMinute_JD += "′";


            var strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD, strMinute_JD, tipJD);
            return strDMSJD;
        }

        /// <summary>
        ///     转换纬度为°′格式
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public string FormatLatitudeToCelsiusMinute(double latitude)
        {
            var wd = latitude;
            var tipWD = string.Empty;

            if (wd >= 0) //北纬N
            {
                tipWD = "N";
            }
            else
            {
                tipWD = "S"; //南纬S
                wd = 0 - wd;
            }

            int Celsius_WD, Minute_WD;
            string strCelsius_WD, strMinute_WD;

            Celsius_WD = (int)Math.Floor(wd);
            if (Celsius_WD < 10) //只有一位，补一个0
            {
                strCelsius_WD = "0" + Celsius_WD;
            }
            else
            {
                strCelsius_WD = Celsius_WD.ToString();
            }
            strCelsius_WD += "°";

            Minute_WD = (int)Math.Floor((wd - Celsius_WD) * 60);
            if (Minute_WD < 10) //只有一位，补一个0
            {
                strMinute_WD = "0" + Minute_WD;
            }
            else
            {
                strMinute_WD = Minute_WD.ToString();
            }
            strMinute_WD += "′";


            var strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD, strMinute_WD, tipWD);

            return strDMSWD;
        }

        /// <summary>
        ///     转换经度为度分格式
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string FormatLongitudeToCelsiusMinuteHanzi(double longitude)
        {
            var jd = longitude;
            var tipJD = string.Empty;

            if (jd >= 0) //东经E
            {
                tipJD = "E";
            }
            else
            {
                tipJD = "W";
                jd = 0 - jd;
            }

            int Celsius_JD, Minute_JD;
            string strCelsius_JD, strMinute_JD;

            Celsius_JD = (int)Math.Floor(jd);
            if (Celsius_JD < 10) //只有一位，补两个0
            {
                strCelsius_JD = "00" + Celsius_JD;
            }
            else if (Celsius_JD < 100) //只有两位，补一个0
            {
                strCelsius_JD = "0" + Celsius_JD;
            }
            else
            {
                strCelsius_JD = Celsius_JD.ToString();
            }
            strCelsius_JD += "度";

            Minute_JD = (int)Math.Floor((jd - Celsius_JD) * 60);
            if (Minute_JD < 10) //只有一位，补一个0
            {
                strMinute_JD = "0" + Minute_JD;
            }
            else
            {
                strMinute_JD = Minute_JD.ToString();
            }
            strMinute_JD += "分";

            var strDMSJD = string.Format("{0}{1}{2}", strCelsius_JD, strMinute_JD, tipJD);
            return strDMSJD;
        }

        /// <summary>
        ///     转换纬度为度分格式
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public string FormatLatitudeToCelsiusMinuteHanzi(double latitude)
        {
            var wd = latitude;
            var tipWD = string.Empty;

            if (wd >= 0) //北纬N
            {
                tipWD = "N";
            }
            else
            {
                tipWD = "S"; //南纬S
                wd = 0 - wd;
            }

            int Celsius_WD, Minute_WD;
            string strCelsius_WD, strMinute_WD;

            Celsius_WD = (int)Math.Floor(wd);
            if (Celsius_WD < 10) //只有一位，补一个0
            {
                strCelsius_WD = "0" + Celsius_WD;
            }
            else
            {
                strCelsius_WD = Celsius_WD.ToString();
            }
            strCelsius_WD += "度";

            Minute_WD = (int)Math.Floor((wd - Celsius_WD) * 60);
            if (Minute_WD < 10) //只有一位，补一个0
            {
                strMinute_WD = "0" + Minute_WD;
            }
            else
            {
                strMinute_WD = Minute_WD.ToString();
            }
            strMinute_WD += "分";


            var strDMSWD = string.Format("{0}{1}{2}", strCelsius_WD, strMinute_WD, tipWD);

            return strDMSWD;
        }

        #endregion

        #region 度分秒转换

        ///// <summary>
        /////     转换为度分秒
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ConversionDuFenMiao()
        //{
        //    if (radLngLat.SelectedIndex == 0)
        //    {
        //        FormatLongitudeToCelsiusMinuteSecondHanzi(double.Parse(txtInput.Text.Trim()));
        //    }
        //    else
        //    {
        //        FormatLatitudeToCelsiusMinuteSecondHanzi(double.Parse(txtInput.Text.Trim()));
        //    }
        //}

        ///// <summary>
        /////     转换为°′″
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SignConversionDuFenMiao()
        //{
        //    if (radLngLat.SelectedIndex == 0)
        //    {
        //        FormatLongitudeToCelsiusMinuteSecond(double.Parse(txtInput.Text.Trim()));
        //    }
        //    else
        //    {
        //        FormatLatitudeToCelsiusMinuteSecond(double.Parse(txtInput.Text.Trim()));
        //    }
        //}

        /// <summary>
        /// 转换经度为°′″
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private string FormatLongitudeToCelsiusMinuteSecond(double longitude)
        {
            double jd = longitude;
            var tipJD = string.Empty;

            if (jd >= 0) //东经E
            {
                tipJD = "E";
            }
            else
            {
                tipJD = "W";
                jd = 0 - jd;
            }

            int Celsius_JD, Minute_JD, Second_JD;
            string strCelsius_JD, strMinute_JD, strSecond_JD;

            Celsius_JD = (int)Math.Floor(jd);
            if (Celsius_JD < 10) //只有一位，补两个0
            {
                strCelsius_JD = "00" + Celsius_JD;
            }
            else if (Celsius_JD < 100) //只有两位，补一个0
            {
                strCelsius_JD = "0" + Celsius_JD;
            }
            else
            {
                strCelsius_JD = Celsius_JD.ToString();
            }
            strCelsius_JD += "°";

            Minute_JD = (int)Math.Floor((jd - Celsius_JD) * 60);
            if (Minute_JD < 10) //只有一位，补一个0
            {
                strMinute_JD = "0" + Minute_JD;
            }
            else
            {
                strMinute_JD = Minute_JD.ToString();
            }
            strMinute_JD += "′";
            //decimal sum = Convert.ToDecimal((jd - Celsius_JD) * 60);
            Second_JD = (int)Math.Floor((Convert.ToDecimal((jd - Celsius_JD) * 60) - Minute_JD) * 60);
            if (Second_JD < 10)
            {
                strSecond_JD = "0" + Second_JD;
            }
            else
            {
                strSecond_JD = Second_JD.ToString();
            }
            strSecond_JD += "″";
            var strDMSJD = string.Format("{0}{1}{2}{3}", strCelsius_JD, strMinute_JD, Second_JD, tipJD);
            return strDMSJD;
        }

        /// <summary>
        /// 转换纬度为°′″
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        private string FormatLatitudeToCelsiusMinuteSecond(double latitude)
        {
            var wd = latitude;
            var tipWD = string.Empty;

            if (wd >= 0) //北纬N
            {
                tipWD = "N";
            }
            else
            {
                tipWD = "S"; //南纬S
                wd = 0 - wd;
            }

            int Celsius_WD, Minute_WD, Second_WD;
            string strCelsius_WD, strMinute_WD, strSecond_WD;

            Celsius_WD = (int)Math.Floor(wd);
            if (Celsius_WD < 10) //只有一位，补一个0
            {
                strCelsius_WD = "0" + Celsius_WD;
            }
            else
            {
                strCelsius_WD = Celsius_WD.ToString();
            }
            strCelsius_WD += "°";

            Minute_WD = (int)Math.Floor((wd - Celsius_WD) * 60);
            if (Minute_WD < 10) //只有一位，补一个0
            {
                strMinute_WD = "0" + Minute_WD;
            }
            else
            {
                strMinute_WD = Minute_WD.ToString();
            }
            strMinute_WD += "′";
            Second_WD = (int)Math.Floor((Convert.ToDecimal((wd - Celsius_WD) * 60) - Minute_WD) * 60);
            if (Second_WD < 10)
            {
                strSecond_WD = "0" + Second_WD;
            }
            else
            {
                strSecond_WD = Second_WD.ToString();
            }
            strSecond_WD += "″";
            var strDMSWD = string.Format("{0}{1}{2}{3}", strCelsius_WD, strMinute_WD, strSecond_WD, tipWD);

            return strDMSWD;
        }

        /// <summary>
        /// 转换经度为度分秒
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private string FormatLongitudeToCelsiusMinuteSecondHanzi(double longitude)
        {
            var jd = longitude;
            var tipJD = string.Empty;

            if (jd >= 0) //东经E
            {
                tipJD = "E";
            }
            else
            {
                tipJD = "W";
                jd = 0 - jd;
            }

            int Celsius_JD, Minute_JD, Second_JD;
            string strCelsius_JD, strMinute_JD, strSecond_JD;

            Celsius_JD = (int)Math.Floor(jd);
            if (Celsius_JD < 10) //只有一位，补两个0
            {
                strCelsius_JD = "00" + Celsius_JD;
            }
            else if (Celsius_JD < 100) //只有两位，补一个0
            {
                strCelsius_JD = "0" + Celsius_JD;
            }
            else
            {
                strCelsius_JD = Celsius_JD.ToString();
            }
            strCelsius_JD += "度";

            Minute_JD = (int)Math.Floor((jd - Celsius_JD) * 60);
            if (Minute_JD < 10) //只有一位，补一个0
            {
                strMinute_JD = "0" + Minute_JD;
            }
            else
            {
                strMinute_JD = Minute_JD.ToString();
            }
            strMinute_JD += "分";
            Second_JD = (int)Math.Floor((Convert.ToDecimal((jd - Celsius_JD) * 60) - Minute_JD) * 60);
            if (Second_JD < 10)
            {
                strSecond_JD = "0" + Second_JD;
            }
            else
            {
                strSecond_JD = Second_JD.ToString();
            }
            strSecond_JD += "秒";
            var strDMSJD = string.Format("{0}{1}{2}{3}", strCelsius_JD, strMinute_JD, Second_JD, tipJD);
            return strDMSJD;
        }

        /// <summary>
        /// 转换纬度为度分秒
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        private string FormatLatitudeToCelsiusMinuteSecondHanzi(double latitude)
        {
            var wd = latitude;
            var tipWD = string.Empty;

            if (wd >= 0) //北纬N
            {
                tipWD = "N";
            }
            else
            {
                tipWD = "S"; //南纬S
                wd = 0 - wd;
            }

            int Celsius_WD, Minute_WD, Second_WD;
            string strCelsius_WD, strMinute_WD, strSecond_WD;

            Celsius_WD = (int)Math.Floor(wd);
            if (Celsius_WD < 10) //只有一位，补一个0
            {
                strCelsius_WD = "0" + Celsius_WD;
            }
            else
            {
                strCelsius_WD = Celsius_WD.ToString();
            }
            strCelsius_WD += "度";

            Minute_WD = (int)Math.Floor((wd - Celsius_WD) * 60);
            if (Minute_WD < 10) //只有一位，补一个0
            {
                strMinute_WD = "0" + Minute_WD;
            }
            else
            {
                strMinute_WD = Minute_WD.ToString();
            }
            strMinute_WD += "分";
            Second_WD = (int)Math.Floor((Convert.ToDecimal((wd - Celsius_WD) * 60) - Minute_WD) * 60);
            if (Second_WD < 10)
            {
                strSecond_WD = "0" + Second_WD;
            }
            else
            {
                strSecond_WD = Second_WD.ToString();
            }
            strSecond_WD += "秒";
            var strDMSWD = string.Format("{0}{1}{2}{3}", strCelsius_WD, strMinute_WD, strSecond_WD, tipWD);

            return strDMSWD;
        }

        #endregion

        #region 经纬度切换

        /// <summary>
        /// 经纬度切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radLngLat_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            StartConvert();
        }

        /// <summary>
        /// 经纬格式切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radConversion_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            StartConvert();
            //LongLatCheck();
            ////经度
            //if (radLngLat.SelectedIndex == 0)
            //{
            //    switch (radConversion.SelectedIndex)
            //    {
            //        case 0:
            //            editOutPut.Text = FormatLongitudeToCelsiusHanzi(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 1:
            //            editOutPut.Text = FormatLongitudeToCelsiusMinuteHanzi(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 2:
            //            editOutPut.Text = FormatLongitudeToCelsiusMinuteSecondHanzi(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 3:
            //            editOutPut.Text = FormatLongitudeToCelsius(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 4:
            //            editOutPut.Text = FormatLongitudeToCelsiusMinute(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 5:
            //            editOutPut.Text = FormatLongitudeToCelsiusMinuteSecond(double.Parse(txtInput.Text.Trim()));
            //            break;
            //    }
            //}
            ////纬度
            //else
            //{
            //    switch (radConversion.SelectedIndex)
            //    {
            //        case 0:
            //            editOutPut.Text = FormatLatitudeToCelsiusHanzi(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 1:
            //            editOutPut.Text = FormatLatitudeToCelsiusMinuteHanzi(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 2:
            //            editOutPut.Text = FormatLatitudeToCelsiusMinuteSecondHanzi(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 3:
            //            editOutPut.Text = FormatLatitudeToCelsius(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 4:
            //            editOutPut.Text = FormatLatitudeToCelsiusMinute(double.Parse(txtInput.Text.Trim()));
            //            break;
            //        case 5:
            //            editOutPut.Text = FormatLatitudeToCelsiusMinuteSecond(double.Parse(txtInput.Text.Trim()));
            //            break;
            //    }
            //}
        }
        #endregion

        #region 经纬度验证
        /// <summary>
        /// 输入经纬度验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_TextChanged_1(object sender, EventArgs e)
        {
            StartConvert();
        }

        /// <summary>
        /// 经纬度验证
        /// </summary>
        private void LongLatCheck()
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                var str = txtInput.Text;
                if (radLngLat.SelectedIndex == 0)
                {

                    if (Regex.IsMatch(str, @"^(-?((180)|(((1[0-7]\d)|(\d{1,2}))(\.\d+)?)))$"))
                    {
                        dxErrorProvider1.SetError(txtInput, null);
                    }
                    else
                    {
                        dxErrorProvider1.SetError(txtInput, "经度格式不符合标准。\r\n经度（-180～180）");
                        editOutPut.Text = "";
                    }
                }
                else
                {
                    if (Regex.IsMatch(str, @"^(-?((90)|((([0-8]\d)|(\d{1}))(\.\d+)?)))$"))
                    {
                        dxErrorProvider1.SetError(txtInput, null);
                    }
                    else
                    {
                        dxErrorProvider1.SetError(txtInput, "纬度格式不符合标准。\r\n纬度（-90～90）");
                        editOutPut.Text = "";
                    }
                }
            }
            else
            {
                dxErrorProvider1.SetError(txtInput, null);
            }
        }

        private void StartConvert()
        {
            LongLatCheck();
            if (!dxErrorProvider1.HasErrors)
            {
                try
                {
                    //经度
                    if (radLngLat.SelectedIndex == 0)
                    {
                        switch (radConversion.SelectedIndex)
                        {
                            case 0:
                                editOutPut.Text = FormatLongitudeToCelsiusHanzi(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 1:
                                editOutPut.Text = FormatLongitudeToCelsiusMinuteHanzi(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 2:
                                editOutPut.Text = FormatLongitudeToCelsiusMinuteSecondHanzi(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 3:
                                editOutPut.Text = FormatLongitudeToCelsius(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 4:
                                editOutPut.Text = FormatLongitudeToCelsiusMinute(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 5:
                                editOutPut.Text = FormatLongitudeToCelsiusMinuteSecond(double.Parse(txtInput.Text.Trim()));
                                break;
                        }
                    }
                    //纬度
                    else
                    {
                        switch (radConversion.SelectedIndex)
                        {
                            case 0:
                                editOutPut.Text = FormatLatitudeToCelsiusHanzi(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 1:
                                editOutPut.Text = FormatLatitudeToCelsiusMinuteHanzi(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 2:
                                editOutPut.Text = FormatLatitudeToCelsiusMinuteSecondHanzi(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 3:
                                editOutPut.Text = FormatLatitudeToCelsius(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 4:
                                editOutPut.Text = FormatLatitudeToCelsiusMinute(double.Parse(txtInput.Text.Trim()));
                                break;
                            case 5:
                                editOutPut.Text = FormatLatitudeToCelsiusMinuteSecond(double.Parse(txtInput.Text.Trim()));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        #endregion

        
    }
}
