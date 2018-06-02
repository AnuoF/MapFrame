/**************************************************************************
 * 类名：Text_GMap.cs
 * 描述：GMap文字图元
 * 作者：Allen
 * 日期：July 4,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// GMap文字图元
    /// </summary>
    public class Text_GMap : GMapMarker, IMFText
    {
        /// <summary>
        /// 文本画笔
        /// </summary>
        private Pen mpen = null;
        /// <summary>
        /// 字体
        /// </summary>
        private Font mFont = null;
        /// <summary>
        /// 文本内容
        /// </summary>
        private string mContext;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 高亮画笔
        /// </summary>
        private Pen mHightLightPen = null;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 字体
        /// </summary>
        private string fontFamily = "宋体";
        /// <summary>
        /// 字体大小
        /// </summary>
        private float fontSize = 10;
        /// <summary>
        /// 字体
        /// </summary>
        private FontStyle fontStyle;
        /// <summary>
        /// 闪烁用的计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;

        public object Tag
        {
            get;
            set;
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

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightLight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="text">kml</param>
        /// <param name="elementName">图元名称</param>
        public Text_GMap(PointLatLng pos, KmlText text, string elementName)
            : base(pos)
        {
            // 设置图元的类型
            this.ElementType = ElementTypeEnum.Text;
            this.ElementName = elementName;
            // 设置图元的描述信息
            this.Description = text.Description;
            this.Size = new System.Drawing.Size(20, 20);
            fontSize = text.Size;//记录字体大小

            Color c = text.Color;
            mpen = new Pen(c, 3);
            mFont = new Font(text.Font, text.Size, text.FontStyle);
            mContext = text.Content;
            this.fontFamily = text.Font;//记录字体
            this.fontStyle = text.FontStyle;//记录字形
            mHightLightPen = new Pen(Color.Green, 2);

            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += flashTimer_Elapsed;

            base.Tag = this;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            // 在地图可视区域中的目标才显示
            if (!this.Overlay.Control.ViewArea.Contains(this.Position)) return;

            Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            if (mpen != null)  // 绘制内容
            {
                SizeF size = g.MeasureString(mContext, mFont);
                this.Size = new System.Drawing.Size((int)size.Width, (int)size.Height);
                g.DrawString(mContext, mFont, mpen.Brush, LocalPosition.X, LocalPosition.Y);
            }

            if (isHightLight)   // 高亮
            {
                g.DrawRectangle(mHightLightPen, new Rectangle(LocalPosition.X, LocalPosition.Y, this.Size.Width, this.Size.Height));
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.BelongLayer = null;

            if (mpen != null)
            {
                mpen.Dispose();
                mpen = null;
            }

            if (mFont != null)
            {
                mFont.Dispose();
                mFont = null;
            }

            if (mHightLightPen != null)
            {
                mHightLightPen.Dispose();
                mHightLightPen = null;
            }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="_isHightLight"></param>
        public void HightLight(bool _isHightLight)
        {
            isHightLight = _isHightLight;
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (isFlash == this.isFlash) return;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                this.IsVisible = true;
            }
        }

        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// 显示、隐藏
        /// </summary>
        /// <param name="isVisible">true，显示；false，隐藏</param>
        public void SetVisible(bool isVisible)
        {
            this.IsVisible = isVisible;
        }

        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="context">文本内容</param>
        /// <returns></returns>
        public bool SetContext(string context)
        {
            this.mContext = context;
            Update();
            return true;
        }

        /// <summary>
        /// 获取文本内容
        /// </summary>
        /// <returns>文本内容</returns>
        public string GetContext()
        {
            return mContext;
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(Color color)
        {
            this.mpen.Color = color;
            Update();
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        public void SetColor(int argb)
        {
            Color c = Color.FromArgb(argb);
            this.mpen.Color = c;
            Update();
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int r, int g, int b)
        {
            Color c = Color.FromArgb(r, g, b);
            this.mpen.Color = c;
            Update();
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color c = Color.FromArgb(a, r, g, b);
            this.mpen.Color = c;
            Update();
        }

        /// <summary>
        /// 获取当前文本颜色
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return mpen.Color;
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetSize(float size)
        {
            this.mFont = new Font(fontFamily, size, fontStyle);
            Update();
            fontSize = size;
            return true;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public bool SetFont(string familyName)
        {
            this.mFont = new Font(familyName, fontSize, fontStyle);
            Update();
            fontFamily = familyName;
            return true;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="familyName">新 System.Drawing.Font 的 System.Drawing.FontFamily 的字符串表示形式。</param>
        /// <param name="emSize">新字体的全身大小（以磅值为单位）。</param>
        /// <param name="fontStyle">字体类型</param>
        /// <returns></returns>
        public bool SetFont(string familyName, float emSize, FontStyle fontStyle = FontStyle.Regular)
        {
            this.mFont = new Font(familyName, emSize, fontStyle);

            fontFamily = familyName;
            fontSize = emSize;
            this.fontStyle = fontStyle;

            Update();

            return true;
        }

        /// <summary>
        /// 获取当前文本的文本格式
        /// </summary>
        /// <returns></returns>
        public Font GetFont()
        {
            return this.mFont;
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 刷新，内部不主动刷新，由外部调用刷新
        /// </summary>
        public void Update()
        {
            if (this.BelongLayer != null)
                this.BelongLayer.Refresh();
        }

        /// <summary>
        /// 获取文字的坐标
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetLngLat()
        {
            MapLngLat lnglat = new MapLngLat(base.Position.Lng, base.Position.Lat);
            return lnglat;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="position">位置</param>
        public void UpdatePosition(MapLngLat position)
        {
            this.Position = new PointLatLng(position.Lat, position.Lng);
        }
    }
}
