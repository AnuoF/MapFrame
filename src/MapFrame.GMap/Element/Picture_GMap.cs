/**************************************************************************
 * 类名：Picture_GMap.cs
 * 描述：图标图元
 * 作者：Allen
 * 日期：Aug 24,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.GMap.Windows;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// 图标图元
    /// </summary>
    class Picture_GMap : GMapMarker, IMFPicture
    {
        #region 字段、属性
        /// <summary>
        /// 标牌显示
        /// </summary>
        private bool lableVisible;
        /// <summary>
        /// 图标缩放比列
        /// </summary>
        private float mScale = 1.0f;
        /// <summary>
        /// 方位角
        /// </summary>
        private float mAngle = 0;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHighelight = false;
        /// <summary>
        /// 图元描述
        /// </summary>
        private string description;
        /// <summary>
        /// 切换图标时需要加锁，防止资源被占用
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 标牌是否已经初始化
        /// </summary>
        private bool isTipInit = false;
        /// <summary>
        /// 图片路径，IconUrl
        /// </summary>
        private string imageUrl = "";
        /// <summary>
        /// 标牌显示方式
        /// </summary>
        private ShowTypeEnum labelShowType = ShowTypeEnum.No;
        /// <summary>
        /// 当前点的图标，原始图片
        /// </summary>
        private volatile Bitmap mBitmap = null;
        /// <summary>
        /// 高亮画笔
        /// </summary>
        private Pen mHightLightPen = null;
        /// <summary>
        /// 图片画笔
        /// </summary>
        private Pen mPen = null;
        /// <summary>
        /// 自定义画笔，用于绘制标牌线条和预警线条
        /// </summary>
        private Pen mSelfPen = null;
        /// <summary>
        /// 标牌
        /// </summary>
        private MapLabel mapLabel = null;
        /// <summary>
        /// 开始闪烁
        /// </summary>
        private bool bIsFlash = false;
        private PointLatLng currentLnglat;
        private Point currentPoint;
        // 预警闪烁
        private Pen FlashPen = null;
        private System.Timers.Timer flashTimer = new System.Timers.Timer();
        private int radius;
        private volatile int flashRadius;
        private volatile int flashX;
        private volatile int flashY;


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
            get { return description; }
            set { description = value; }
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

        /// <summary>
        /// 标牌是否显示
        /// </summary>
        public bool IsLableVisible
        {
            get
            {
                return lableVisible;
            }
        }

        #endregion

        ///// <summary>
        ///// 构造函数，没传图片的情况，用GDI绘制点
        ///// </summary>
        ///// <param name="pos">位置</param>
        ///// <param name="kmlPoint">点的kml对象</param>
        ///// <param name="elementName">图元名称</param>
        //public Picture_GMap(PointLatLng pos, KmlPoint kmlPoint, string elementName)
        //    : base(pos)
        //{
        //    this.ElementName = elementName;
        //    this.Description = kmlPoint.Description;
        //    this.ElementType = ElementTypeEnum.Picture;
        //    // 鼠标经过可见
        //    base.IsHitTestVisible = true;
        //    // 画笔
        //    mHightLightPen = new Pen(Brushes.Green, 1);
        //    mPen = new Pen(kmlPoint.Color, 2);

        //    // 图标
        //    if (!string.IsNullOrEmpty(kmlPoint.IcoUrl))
        //    {
        //        LoadBitmap(kmlPoint.IcoUrl);
        //    }

        //    // 图标缩放比列
        //    if (kmlPoint.Size != null)
        //    {
        //        mScale = kmlPoint.Size.Width;
        //    }

        //    FlashPen = new Pen(Brushes.Red, 3);
        //    mSelfPen = new Pen(Color.Red, 2);
        //    flashTimer.Interval = 100;
        //    flashTimer.Elapsed += flashTimer_Elapsed;
        //    bIsFlash = false;

        //    base.Tag = this;
        //}

        /// <summary>
        /// 构造函数，没传图片的情况，用GDI绘制点
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="kmlPicture">点的kml对象</param>
        /// <param name="elementName">图元名称</param>
        public Picture_GMap(PointLatLng pos, KmlPicture kmlPicture, string elementName)
            : base(pos)
        {
            this.ElementName = elementName;
            this.Description = kmlPicture.Description;
            this.ElementType = ElementTypeEnum.Picture;
            // 鼠标经过可见
            base.IsHitTestVisible = true;

            // 画笔
            mHightLightPen = new Pen(Brushes.Green, 1);
            mScale = kmlPicture.Scale;

            if (kmlPicture.IconColor != null)
                mPen = new Pen(kmlPicture.IconColor, 2);
            else
                mPen = new Pen(Color.Red, 2);

            // 图标
            if (!string.IsNullOrEmpty(kmlPicture.IconUrl))
            {
                LoadBitmap(kmlPicture.IconUrl);
            }

            FlashPen = new Pen(Brushes.Red, 3);
            mSelfPen = new Pen(Color.Red, 2);
            flashTimer.Interval = 100;
            flashTimer.Elapsed += flashTimer_Elapsed;
            bIsFlash = false;

            base.Tag = this;
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 主线程绘制
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            // 在地图可视区域中的目标才显示
            //if (!this.Overlay.Control.ViewArea.Contains(this.Position)) return;
            this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);

            #region  方法
            lock (lockObj)
            {
                // 如果图片存在，则绘制图片
                if (mBitmap != null)
                {
                    //Rectangle bitmapRct = new Rectangle(LocalPosition.X - mBitmap.Width / 2, LocalPosition.Y - mBitmap.Height / 2, mBitmap.Width, mBitmap.Height);
                    Rectangle bitmapRct = new Rectangle(LocalPosition.X, LocalPosition.Y, mBitmap.Width, mBitmap.Height);

                    // 画目标
                    g.DrawImage(mBitmap, bitmapRct);

                    // 高亮
                    if (isHighelight && mHightLightPen != null)
                        g.DrawRectangle(mHightLightPen, bitmapRct);
                }
            }

            // 更新标牌位置
            GPoint markerPosition = this.Overlay.Control.FromLatLngToLocal(this.Position);
            Point markerLocation = new Point((int)markerPosition.X, (int)markerPosition.Y);//点的屏幕位置

            //标牌显示
            if (isTipInit == true && mapLabel != null)
            {
                Point lableLocalPosition = new Point(mapLabel.LocalPositionX + this.Size.Width / 2, mapLabel.LocalPositionY + this.Size.Height / 2);
                g.DrawLine(mSelfPen, new Point(this.LocalPosition.X + this.Size.Width / 2, this.LocalPosition.Y + this.Size.Height / 2), lableLocalPosition);

                //刷新地图：经纬度一样，屏幕坐标一样
                if (this.Position == currentLnglat)
                {
                    mapLabel.ShowLable(markerLocation, this.LocalPosition);
                }

                //移动点：经纬度不一样,屏幕坐标不一样，不移动标牌  移动点标牌不动
                if (this.Position != currentLnglat && markerLocation != currentPoint)
                {
                    mapLabel.UpdateLabelLocationAndPosition(markerLocation, this.LocalPosition);
                }
            }

            // 闪烁预警
            if (bIsFlash == true && FlashPen != null)
            {
                Random r = new Random();
                Rectangle rectangle = new Rectangle(flashX, flashY, flashRadius, flashRadius);
                //g.DrawEllipse(FlashPen, new Rectangle(LocalPosition.X - flashRadius / 2, LocalPosition.Y - flashRadius / 2, flashRadius, flashRadius));
                //g.DrawEllipse(FlashPen, new Rectangle(LocalPosition.X + this.Size.Width / 2 - flashRadius / 2, LocalPosition.Y + this.Size.Height / 2 - flashRadius / 2, flashRadius, flashRadius));
                g.DrawEllipse(FlashPen, LocalPosition.X + this.Size.Width / 2 - flashRadius / 2, LocalPosition.Y + this.Size.Height / 2 - flashRadius / 2, flashRadius, flashRadius);
                //g.DrawEllipse(FlashPen, 0,0,0,0);
                //g.DrawEllipse(FlashPen, new Rectangle(LocalPosition.X - flashRadius / 2, LocalPosition.Y - flashRadius / 2, flashRadius, flashRadius));
                //  g.DrawEllipse(FlashPen, new Rectangle(LocalPosition.X + this.Size.Width / 2 - flashRadius / 2, LocalPosition.Y + this.Size.Height / 2 - flashRadius / 2, flashRadius, flashRadius));
            }

            currentLnglat = this.Position;
            currentPoint = markerLocation;
            #endregion
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.BelongLayer = null;

            try
            {
                if (FlashPen != null)
                {
                    FlashPen.Dispose();
                    FlashPen = null;
                }

                if (flashTimer != null)
                {
                    flashTimer.Stop();
                    flashTimer.Dispose();
                    flashTimer = null;
                }

                if (mPen != null)
                {
                    mPen.Dispose();
                    mPen = null;
                }

                if (mBitmap != null)
                {
                    mBitmap.Dispose();
                    mBitmap = null;
                }

                if (mHightLightPen != null)
                {
                    mHightLightPen.Dispose();
                    mHightLightPen = null;
                }

                if (mSelfPen != null)
                {
                    mSelfPen.Dispose();
                    mSelfPen = null;
                }

                if (mapLabel != null)
                {
                    if (mapLabel.InvokeRequired)
                    {
                        mapLabel.Invoke(new Action(delegate
                        {
                            mapLabel.Visible = false;
                            mapLabel.DisposeLabel();
                            mapLabel.Dispose();
                            mapLabel = null;
                        }));
                    }
                    else
                    {
                        mapLabel.Visible = false;
                        mapLabel.DisposeLabel();
                        mapLabel.Dispose();
                        mapLabel = null;
                    }
                }
                BelongLayer = null;
            }
            catch
            {
            }
        }

        #region IPointGMap接口

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
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                this.IsVisible = true;
                Update();
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

            if (mapLabel != null && !mapLabel.IsDisposed)
                mapLabel.SetTargetInfo(this.ElementName, lng, lat);
        }

        /// <summary>
        /// 更新点的位置
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        public void UpdatePosition(MapLngLat lngLat)
        {
            this.Position = new PointLatLng(lngLat.Lat, lngLat.Lng, lngLat.Alt);

            if (mapLabel != null && !mapLabel.IsDisposed)
                mapLabel.SetTargetInfo(this.ElementName, lngLat.Lng, lngLat.Lat);
        }

        /// <summary>
        /// 设置点的图标
        /// </summary>
        /// <param name="url">图标路径</param>
        public void SetIcon(string url)
        {
            mScale = 1f;
            LoadBitmap(url);
            Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(Color color)
        {
            lock (mPen)
            {
                mPen.Color = color;
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
        /// 设置大小
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            mScale = scale;
            LoadBitmap(imageUrl);
            BelongLayer.Refresh();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        public void UpdatePosition(double lng, double lat)
        {
            this.Position = new PointLatLng(lat, lng);

            if (mapLabel != null && !mapLabel.IsDisposed)
                mapLabel.SetTargetInfo(this.ElementName, lng, lat);
            this.Update();
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
        /// 设置方位角
        /// </summary>
        /// <param name="angle">与正北方向的夹角</param>
        public void SetAngle(float angle)
        {
            if (!this.Overlay.Control.ViewArea.Contains(this.Position)) return;
            mAngle = angle;
            LoadBitmap(imageUrl);
            if (!IsFlash)
            {
                Update();
            }
        }

        #endregion

        #region  标牌

        /// <summary>
        /// 初始化标牌
        /// </summary>
        public void InitMapLabel()
        {
            if (mapLabel != null) return;
            base.ToolTipMode = MarkerTooltipMode.Never;

            GPoint markerPosition = this.Overlay.Control.FromLatLngToLocal(this.Position);
            Point markerPoint = new Point((int)markerPosition.X, (int)markerPosition.Y);//点的屏幕坐标
            currentPoint = markerPoint;
            currentLnglat = this.Position;

            mapLabel = new MapLabel(markerPoint);
            mapLabel.ClosedLabelEvent += new EventHandler(mapLabel_Closed);
            mapLabel.UpdateLabelLocationAndPosition(markerPoint, this.LocalPosition);

            mapLabel.SetTargetInfo(this.ElementName, this.Position.Lng, this.Position.Lat);
            mapLabel.SetLabelText(ToolTipText);
            mapLabel.Visible = false;
            this.Overlay.Control.Controls.Add(mapLabel);

            isTipInit = true;
            lableVisible = true;
        }

        /// <summary>
        /// 关闭标牌
        /// </summary>
        public void CloseMapLabel()
        {
            if (mapLabel != null)
            {
                if (mapLabel.InvokeRequired)
                {
                    mapLabel.Invoke(new Action(delegate
                    {
                        mapLabel.DisposeLabel();
                        mapLabel = null;
                        isTipInit = false;
                    }));
                }
                else
                {
                    mapLabel.DisposeLabel();
                    mapLabel = null;
                    isTipInit = false;
                }
            }
            lableVisible = false;
        }

        /// <summary>
        /// 设置Tip内容
        /// </summary>
        /// <param name="tipText">内容</param>
        public void SetTipText(string tipText)
        {
            if (!string.IsNullOrEmpty(tipText))
            {
                base.ToolTipText = tipText;
                base.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                base.ToolTip.Format.Alignment = System.Drawing.StringAlignment.Near; // Tip文字左对齐
            }
        }

        /// <summary>
        /// 设置tip显示方式
        /// </summary>
        /// <param name="showType"></param>
        public void SetTipShow(ShowTypeEnum showType)
        {
            switch (showType)
            {
                case ShowTypeEnum.Always:
                    base.ToolTipMode = MarkerTooltipMode.Always;
                    break;

                case ShowTypeEnum.MouseHover:
                    base.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    break;

                case ShowTypeEnum.No:
                    base.ToolTipMode = MarkerTooltipMode.Never;
                    break;
            }
        }

        /// <summary>
        /// 标牌显示方式
        /// </summary>
        public ShowTypeEnum LabelShowType
        {
            get { return labelShowType; }
        }

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="labelText">标牌内容</param>
        public void SetLabelText(string labelText)
        {
            this.ToolTipText = labelText;
            if (mapLabel == null) return;

            mapLabel.SetLabelText(labelText);
        }

        /// <summary>
        /// 设置标牌显示方式
        /// </summary>
        /// <param name="showType">显示方式</param>
        public void SetLableShow(ShowTypeEnum showType)
        {
            this.labelShowType = showType;

            switch (showType)
            {
                case ShowTypeEnum.Always:
                    InitMapLabel();
                    break;
                case ShowTypeEnum.No:
                    CloseMapLabel();
                    break;
                case ShowTypeEnum.MouseHover://使用默认的tip
                    break;
            }
        }

        /// <summary>
        /// 标牌关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapLabel_Closed(object sender, EventArgs e)
        {
            this.labelShowType = ShowTypeEnum.No;
            this.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            isTipInit = false;
            mapLabel = null;
            lableVisible = false;
        }

        #endregion

        #region  图片处理

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="url">图片路径</param>
        private void LoadBitmap(string url)
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                imageUrl = url;

                if (url.Contains("|"))
                {
                    CreateBitmapFromTtfFile(url);
                }
                else
                {
                    CloneRotateBitmap(url);
                }
            });
        }

        private Dictionary<string, Bitmap> bitmapDic = new Dictionary<string, Bitmap>();

        Bitmap _bitmap = null;
        /// <summary>
        /// 拷贝、旋转图片
        /// </summary>
        /// <param name="url">图片路径</param>
        private void CloneRotateBitmap(string url)
        {
            if (!File.Exists(url)) return;
            lock (lockObj)
            {
                if (bitmapDic.ContainsKey(url))
                {
                    _bitmap = bitmapDic[url];
                }
                else
                {
                    _bitmap = new Bitmap(url);
                    bitmapDic.Add(url, _bitmap);
                }
                if (_bitmap == null) return;

                //清理上一次图片
                if (mBitmap != null)
                {
                    //mBitmap.Dispose();
                    mBitmap = null;
                }

                // 旋转图片
                //mBitmap = _bitmap;
                mBitmap = RotateImage(_bitmap);
                if (mBitmap == null) return;
                this.Size = new Size(mBitmap.Width, mBitmap.Height);     // 设置Marker大小
                radius = Size.Width >= Size.Height ? Size.Width : Size.Height;    // 设置闪烁半径

            }
        }

        /// <summary>
        /// 从TTF文件创建图片
        /// </summary>
        /// <param name="url">TTF字符信息</param>
        private void CreateBitmapFromTtfFile(string url)
        {
            string[] arr = url.Split(new char[] { '|' });
            if (arr.Length < 3) return;

            string ttfPath = arr[0];
            if (!File.Exists(ttfPath)) return;  // 检查字体文件是否存在

            // 字符代码
            int fontCode = Convert.ToInt32(arr[1]);
            // 字体大小
            int fontSize = Convert.ToInt32(arr[2]);

            // 从TTF文件动态加载字体
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(ttfPath);
            Font font = new System.Drawing.Font(pfc.Families[0], fontSize);

            // 获取图片大小
            Bitmap bmpSize = new Bitmap(5, 5);
            var gdiSize = System.Drawing.Graphics.FromImage(bmpSize);
            SizeF size = gdiSize.MeasureString(Char.ConvertFromUtf32(fontCode), font);

            // 创建图片，缩放比列
            Bitmap bmp = new Bitmap((int)(size.Width * mScale), (int)(size.Height * mScale));
            Graphics gdi = System.Drawing.Graphics.FromImage(bmp);
            gdi.Clear(System.Drawing.Color.Transparent);

            // 设置画板的坐标原点为中心点
            gdi.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
            // 以指定角度对画板进行旋转
            gdi.RotateTransform(mAngle);
            // 让文字变得平滑
            gdi.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            // 绘制字符  System.Drawing.Brushes.Red

            if (mPen == null) return;

            lock (mPen)
            {
                gdi.DrawString(Char.ConvertFromUtf32(fontCode), font, mPen.Brush, -bmp.Width / 2, -bmp.Height / 2);
            }

            lock (lockObj)
            {
                if (mBitmap != null)
                {
                    mBitmap.Dispose();
                    mBitmap = null;
                }

                mBitmap = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
                this.Size = new Size(mBitmap.Width, mBitmap.Height);     // 设置Marker大小
                radius = Size.Width >= Size.Height ? Size.Width : Size.Height;    // 设置闪烁半径
            }

            bmpSize.Dispose();
            gdiSize.Dispose();
            font.Dispose();
            gdi.Dispose();
            bmp.Dispose();
        }

        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns></returns>
        private Bitmap RotateImage(Bitmap image)
        {
            try
            {
                const double pi2 = Math.PI / 2.0;

                // Why can't C# allow these to be const, or at least readonly
                // *sigh*  I'm starting to talk like Christian Graus :omg:
                double oldWidth = (double)image.Width;
                double oldHeight = (double)image.Height;

                // Convert degrees to radians
                double theta = ((double)mAngle) * Math.PI / 180.0;
                double locked_theta = theta;

                // Ensure theta is now [0, 2pi)
                while (locked_theta < 0.0)
                    locked_theta += 2 * Math.PI;

                double newWidth, newHeight;
                int nWidth, nHeight; // The newWidth/newHeight expressed as ints

                #region Explaination of the calculations
                /*
             * The trig involved in calculating the new width and height
             * is fairly simple; the hard part was remembering that when 
             * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
             * height are switched.
             * 
             * When you rotate a rectangle, r, the bounding box surrounding r
             * contains for right-triangles of empty space.  Each of the 
             * triangles hypotenuse's are a known length, either the width or
             * the height of r.  Because we know the length of the hypotenuse
             * and we have a known angle of rotation, we can use the trig
             * function identities to find the length of the other two sides.
             * 
             * sine = opposite/hypotenuse
             * cosine = adjacent/hypotenuse
             * 
             * solving for the unknown we get
             * 
             * opposite = sine * hypotenuse
             * adjacent = cosine * hypotenuse
             * 
             * Another interesting point about these triangles is that there
             * are only two different triangles. The proof for which is easy
             * to see, but its been too long since I've written a proof that
             * I can't explain it well enough to want to publish it.  
             * 
             * Just trust me when I say the triangles formed by the lengths 
             * width are always the same (for a given theta) and the same 
             * goes for the height of r.
             * 
             * Rather than associate the opposite/adjacent sides with the
             * width and height of the original bitmap, I'll associate them
             * based on their position.
             * 
             * adjacent/oppositeTop will refer to the triangles making up the 
             * upper right and lower left corners
             * 
             * adjacent/oppositeBottom will refer to the triangles making up 
             * the upper left and lower right corners
             * 
             * The names are based on the right side corners, because thats 
             * where I did my work on paper (the right side).
             * 
             * Now if you draw this out, you will see that the width of the 
             * bounding box is calculated by adding together adjacentTop and 
             * oppositeBottom while the height is calculate by adding 
             * together adjacentBottom and oppositeTop.
             */
                #endregion

                double adjacentTop, oppositeTop;
                double adjacentBottom, oppositeBottom;

                // We need to calculate the sides of the triangles based
                // on how much rotation is being done to the bitmap.
                //   Refer to the first paragraph in the explaination above for 
                //   reasons why.
                if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                    (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
                {
                    adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                    oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                    adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                    oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                }
                else
                {
                    adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                    oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                    adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                    oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                }

                newWidth = adjacentTop + oppositeBottom;
                newHeight = adjacentBottom + oppositeTop;

                nWidth = (int)Math.Ceiling(newWidth * mScale);
                nHeight = (int)Math.Ceiling(newHeight * mScale);

                Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);
                using (Graphics g = Graphics.FromImage(rotatedBmp))
                {
                    // This array will be used to pass in the three points that 
                    // make up the rotated image
                    Point[] points = new Point[3];
                    /*
                     * The values of opposite/adjacentTop/Bottom are referring to 
                     * fixed locations instead of in relation to the
                     * rotating image so I need to change which values are used
                     * based on the how much the image is rotating.
                     * 
                     * For each point, one of the coordinates will always be 0, 
                     * nWidth, or nHeight.  This because the Bitmap we are drawing on
                     * is the bounding box for the rotated bitmap.  If both of the 
                     * corrdinates for any of the given points wasn't in the set above
                     * then the bitmap we are drawing on WOULDN'T be the bounding box
                     * as required.
                     */
                    if (locked_theta >= 0.0 && locked_theta < pi2)
                    {
                        //points = new Point[] {
                        //                     new Point( (int) oppositeBottom, 0 ),
                        //                     new Point( nWidth, (int) oppositeTop ),
                        //                     new Point( 0, (int) adjacentBottom )
                        //                 };
                        points[0].X = (int)oppositeBottom;
                        points[0].Y = 0;
                        points[1].X = nWidth;
                        points[1].Y = (int)oppositeTop;
                        points[2].X = 0;
                        points[2].Y = (int)adjacentBottom;
                    }
                    else if (locked_theta >= pi2 && locked_theta < Math.PI)
                    {
                        //points = new Point[] {
                        //                     new Point( nWidth, (int) oppositeTop ),
                        //                     new Point( (int) adjacentTop, nHeight ),
                        //                     new Point( (int) oppositeBottom, 0 )
                        //                 };
                        points[0].X = nWidth;
                        points[0].Y = (int)oppositeTop;
                        points[1].X = (int)adjacentTop;
                        points[1].Y = nHeight;
                        points[2].X = (int)oppositeBottom;
                        points[2].Y = 0;
                    }
                    else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                    {
                        //points = new Point[] {
                        //                     new Point( (int) adjacentTop, nHeight ),
                        //                     new Point( 0, (int) adjacentBottom ),
                        //                     new Point( nWidth, (int) oppositeTop )
                        //                 };
                        points[0].X = (int)adjacentTop;
                        points[0].Y = nHeight;
                        points[1].X = 0;
                        points[1].Y = (int)adjacentBottom;
                        points[2].X = nWidth;
                        points[2].Y = (int)oppositeTop;
                    }
                    else
                    {
                        //points = new Point[] {
                        //                     new Point( 0, (int) adjacentBottom ),
                        //                     new Point( (int) oppositeBottom, 0 ),
                        //                     new Point( (int) adjacentTop, nHeight )
                        //                 };
                        points[0].X = 0;
                        points[0].Y = (int)adjacentBottom;
                        points[1].X = (int)oppositeBottom;
                        points[1].Y = 0;
                        points[2].X = (int)adjacentTop;
                        points[2].Y = nHeight;
                    }

                    g.DrawImage(image, points);
                    g.Dispose();
                }
                return rotatedBmp;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 预警闪烁

        // 定时器
        private void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            flashRadius += radius / 4;
            if (flashRadius >= 2 * radius)
            {
                flashRadius = radius;
            }

            Update();
        }

        #endregion

    }
}
