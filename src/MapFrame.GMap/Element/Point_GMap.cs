/**************************************************************************
 * 类名：Point_GMap.cs
 * 描述：GMap地图点图元
 * 作者：Allen
 * 日期：July 4,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System;
using System.Drawing;
using System.Timers;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// GMap地图点图元
    /// </summary>
    class Point_GMap : GMapMarker, IMFPoint
    {
        #region 字段、属性

        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHighelight = false;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 开始闪烁
        /// </summary>
        private bool bIsFlash = false;
        /// <summary>
        /// 闪烁半径
        /// </summary>
        private int flashRadius;
        /// <summary>
        /// 高亮画笔
        /// </summary>
        private Pen m_HightLightPen = null;
        /// <summary>
        /// 图片画笔
        /// </summary>
        private Pen m_Pen = null;
        /// <summary>
        /// 告警画笔
        /// </summary>
        private Pen flashPen;


        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 所属图层
        /// </summary>
        public IMFLayer BelongLayer
        {
            get;
            set;
        }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHighelight; }
        }

        /// <summary>
        /// 是否在闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return bIsFlash; }
        }

        /// <summary>
        /// 图元描述
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
            get;
            set;
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        public ElementTypeEnum ElementType
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 构造函数，没传图片的情况，用GDI绘制点
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="point">点的kml对象</param>
        /// <param name="elementName">图元名称</param>
        public Point_GMap(PointLatLng pos, KmlPoint point, string elementName)
            : base(pos)
        {
            this.ElementName = elementName;
            this.Description = point.Description;
            this.ElementType = ElementTypeEnum.Point;
            // 鼠标经过可见
            base.IsHitTestVisible = true;
            // 大小
            if (point.Size == null)
            {
                base.Size = new Size(5, 5);
            }
            else
            {
                int width = point.Size.Width;
                int height = point.Size.Height;
                base.Size = new System.Drawing.Size(width, height);
            }

            // 画笔
            m_HightLightPen = new Pen(Brushes.Green, 1);
            if (point.Color.ToArgb() == 0)
            {
                m_Pen = new Pen(Brushes.Blue, 2);
            }
            else
            {
                m_Pen = new Pen(point.Color, 2);
            }

            // 计时器
            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += flashTimer_Elapsed;
            flashTimer.Interval = 100;
            bIsFlash = false;

            base.Tag = this;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            #region  方法

            if (m_Pen != null)
            {
                Rectangle markerRct = new Rectangle(LocalPosition.X - Size.Width / 2, LocalPosition.Y - Size.Height / 2, Size.Width, Size.Height);

                lock (m_Pen)
                {
                    // 画目标
                    g.FillEllipse(m_Pen.Brush, markerRct);
                }

                // 高亮
                if (isHighelight && m_HightLightPen != null)
                    g.DrawRectangle(m_HightLightPen, markerRct);
            }

            #endregion
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.BelongLayer = null;

            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
                flashTimer = null;
            }

            if (m_Pen != null)
            {
                lock (m_Pen)
                {
                    m_Pen.Dispose();
                    m_Pen = null;
                }
            }

            if (m_HightLightPen != null)
            {
                m_HightLightPen.Dispose();
                m_HightLightPen = null;
            }

            if (flashPen != null)
            {
                flashPen.Dispose();
                flashPen = null;
            }
        }

        #region IPointGMap接口
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="_isHightLight"></param>
        public void HightLight(bool _isHightLight)
        {
            isHighelight = _isHightLight;
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="_isFlash">true，开始；false，停止</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool _isFlash, int interval = 500)
        {
            if (bIsFlash == _isFlash) return;   // 防止多次调用

            bIsFlash = _isFlash;

            if (bIsFlash)
            {
                flashTimer.Start();
                flashTimer.Interval = interval;
            }
            else
            {
                flashTimer.Stop();
                if (flashPen != null)
                {
                    flashPen.Dispose();
                    flashPen = null;
                }
            }
        }

        /// <summary>
        /// 显示隐藏图形
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            this.IsVisible = visible;
        }

        /// <summary>
        /// 更新点的位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {
            this.Position = new PointLatLng(lat, lng);
        }

        /// <summary>
        /// 更新点的位置
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        public void UpdatePosition(MapLngLat lngLat)
        {
            this.Position = new PointLatLng(lngLat.Lat, lngLat.Lng);
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size">Size</param>
        public void SetSize(Size size)
        {
            this.Size = size;
            Update();
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetSize(int width, int height)
        {
            this.Size = new Size(width, height);
            Update();
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(double size)
        {
            int width = (int)size;
            this.Size = new Size(width, width);
            Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(Color color)
        {
            lock (m_Pen)
            {
                m_Pen.Color = color;
            }

            Update();
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        public void SetColor(int argb)
        {
            Color c = Color.FromArgb(argb);
            SetColor(c);
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">gren</param>
        /// <param name="b">blue</param>
        public void SetColor(int r, int g, int b)
        {
            Color c = Color.FromArgb(r, g, b); ;
            SetColor(c);
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="a">alpha</param>
        /// <param name="r">red</param>
        /// <param name="g">gren</param>
        /// <param name="b">blue</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color c = Color.FromArgb(a, r, g, b);
            SetColor(c);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (this.BelongLayer != null)
                this.BelongLayer.Refresh();
        }

        /// <summary>
        /// 获取目标位置
        /// </summary>
        /// <returns>经纬度</returns>
        public MapLngLat GetLngLat()
        {
            return new MapLngLat(this.Position.Lng, this.Position.Lat, this.Position.Alt);
        }

        /// <summary>
        /// 设置Tip内容
        /// </summary>
        /// <param name="tipText"></param>
        public void SetTipText(string tipText)
        {
            if (!string.IsNullOrEmpty(tipText))
            {
                this.ToolTipText = tipText;
            }
        }

        /// <summary>
        /// 设置Tip显示方式
        /// </summary>
        /// <param name="showType"></param>
        public void SetTipShow(ShowTypeEnum showType)
        {
            switch (showType)
            {
                case ShowTypeEnum.Always:
                    this.ToolTipMode = MarkerTooltipMode.Always;
                    break;
                case ShowTypeEnum.MouseHover:
                    this.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    break;
                case ShowTypeEnum.No:
                    this.ToolTipMode = MarkerTooltipMode.Never;
                    break;
            }
        }

        #endregion

        private void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (flashPen == null)
            {
                flashPen = new Pen(Brushes.Red, 2);
                flashRadius = 1;
            }
            else
            {
                int radius = 0;
                radius = Size.Width >= Size.Height ? Size.Width : Size.Height;

                flashRadius += radius / 4;
                if (flashRadius >= 2 * radius)
                {
                    flashRadius = radius;
                    flashPen.Color = Color.FromArgb(255, Color.Red);
                }
                else
                {
                    Random rand = new Random();
                    int alpha = rand.Next(255);
                    flashPen.Color = Color.FromArgb(alpha, Color.Red);
                }
            }
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        /// <param name="angle">角度</param>
        public void SetAngle(double angle)
        {

        }
    }
}
