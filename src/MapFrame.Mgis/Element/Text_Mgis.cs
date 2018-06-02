using System;
using AxHOSOFTMapControlLib;
using System.Timers;
using MapFrame.Core.Model;
using System.Drawing;
using MapFrame.Core.Interface;

namespace MapFrame.Mgis.Element
{
    /// <summary>
    /// 文字图元
    /// </summary>
    class Text_Mgis : IMFText
    {
        /// <summary>
        /// Mgis地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl;
        private MapLngLat textPosition;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string symbolName;
        /// <summary>
        /// 是否隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHight = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 文字大小
        /// </summary>
        private float size = 0;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();

        private string context = string.Empty;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kml">kml 对象</param>
        /// <param name="_mapControl">地图控件</param>
        public Text_Mgis(Kml kml, AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
            KmlText kmlText = kml.Placemark.Graph as KmlText;
            if (kml.Placemark.Name == null || kmlText.Content == string.Empty) return;
            this.symbolName = kml.Placemark.Name;
            this.textPosition = kmlText.Position;
            this.context = kmlText.Content;
            System.Drawing.Color c = kmlText.Color;
            mapControl.MgsDrawSymTextByJBID(symbolName, context, (float)kmlText.Position.Lng, (float)kmlText.Position.Lat);
            mapControl.MgsUpdateSymSize(symbolName, (float)kmlText.Size);
            mapControl.MgsUpdateSymColor(symbolName, c.R, c.G, c.B, c.A);
            mapControl.update();
            this.ElementType = ElementTypeEnum.Text;
            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 设置文字颜色
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool SetContext(string context)
        {
            int result = mapControl.MgsUpdateSymText(symbolName, context);
            this.context = context;
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 获取文字内容
        /// </summary>
        /// <returns></returns>
        public string GetContext()
        {
            //return mapControl.MgsGetSymText(symbolName);
            return this.context;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(System.Drawing.Color color)
        {
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="argb"></param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color color = Color.FromArgb(a, r, g, b);
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// 获取文字颜色
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Color GetColor()
        {
            byte r = 0, g = 0, b = 0, a = 0;
            mapControl.MgsGetSymColor(symbolName, ref r, ref g, ref b, ref a);
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// 设置文字大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetSize(float size)
        {
            int result = mapControl.MgsUpdateSymSize(symbolName, size);
            this.size = size;
            return result == 1 ? true : false;
        }


        /// <summary>
        /// 未解决
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public bool SetFont(string familyName)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Font GetFont()
        {
            throw new NotImplementedException();
        }

        public bool SetFont(string familyName, float emSize)
        {
            throw new NotImplementedException();
        }


        public Core.Model.MapLngLat GetLngLat()
        {
            return this.textPosition;
        }

        /// <summary>
        /// 文字图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return this.symbolName; }
        }

        private IMFLayer belongLayer;
        /// <summary>
        /// 所属图层
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set { belongLayer = value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return symbolName; }
            set { symbolName = value; }
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        public Core.Model.ElementTypeEnum ElementType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return this.isHight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return this.isFlash; }
        }

        /// <summary>
        /// 是否显示隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            lock (lockObj)
            {
                if (isHightLight)
                {
                    mapControl.MgsUpdateSymSize(symbolName, (float)(size + 1));
                }
                else
                {
                    mapControl.MgsUpdateSymSize(symbolName, this.size);
                }
            }
            this.isHight = isHightLight;
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;//防止被多次调用
            this.isFlash = isFlash;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                mapControl.MgsUpdateSymVisibility(symbolName, 0);
            }
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isTimer)
            {
                this.SetVisible(false);
            }
            else
            {
                this.SetVisible(true);
            }
            isTimer = !isTimer;
        }

        /// <summary>
        /// 显示/隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            this.isVisible = isVisible;
            if (isVisible)
            {
                mapControl.MgsUpdateSymVisibility(symbolName, 0);
            }
            else
            {
                mapControl.MgsUpdateSymVisibility(symbolName, 1);
            }
            Update();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (belongLayer != null)
                belongLayer.Refresh();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
            }
            isFlash = false;
            isHight = false;
            isVisible = false;
        }



        public bool SetFont(string familyName, float emSize, FontStyle fontStyle = FontStyle.Regular)
        {
            throw new NotImplementedException();
        }


        public void UpdatePosition(MapLngLat position)
        {
            throw new NotImplementedException();
        }
    }
}
