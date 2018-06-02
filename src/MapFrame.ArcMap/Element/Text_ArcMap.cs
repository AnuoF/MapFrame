
/**************************************************************************
 * 类名：Text_ArcMap.cs
 * 描述：ArcGis文字类
 * 作者：lx
 * 日期：Aug 23,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MapFrame.Core.Model;
using System;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using System.Drawing;
using MapFrame.ArcMap.Factory;

namespace MapFrame.ArcMap.Element
{
    /// <summary>
    /// ArcGis文字类
    /// </summary>
    class Text_ArcMap : TextElementClass, IMFText
    {
        /// <summary>
        /// 图元是否显示
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 高亮
        /// </summary>
        private bool pHightlight = false;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 闪烁判定
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool pIsFlash = false;
        /// <summary>
        /// 颜色
        /// </summary>
        private System.Drawing.Color bTextColor;
        /// <summary>
        /// 字体大小
        /// </summary>
        private float bOutLineSize;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string _elementName;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 文字位置
        /// </summary>
        private MapLngLat position;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private FactoryArcMap mapFactory = null;
        /// <summary>
        /// 图元所属图层
        /// </summary>
        private Core.Interface.IMFLayer belongLayer;
        /// <summary>
        /// 文字符号
        /// </summary>
        private ITextSymbol textSymbol;
        /// <summary>
        /// 字体
        /// </summary>
        private stdole.StdFont font;

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        ///// <param name="kmlText"></param>
        ///// <param name="layer"></param>
        //public Text_ArcMap(KmlText kmlText, CompositeGraphicsLayerClass compositeGraphicsLayer)
        //{
        //    System.Drawing.Color c = System.Drawing.Color.FromArgb(kmlText.Color);
        //    bTextColor = c;
        //    base.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
        //    base.Size = kmlText.Size;
        //    bOutLineSize = kmlText.Size;
        //    base.FontName = kmlText.Font;
        //    base.Text = kmlText.Content;
        //    base.Geometry = new PointClass() { X = kmlText.Position.Lng, Y = kmlText.Position.Lat };

        //    this.Description = kmlText.Description;

        //    position = new MapLngLat();
        //    position = kmlText.Position;//坐标点
        //    pIsFlash = false;
        //    flashTimer = new System.Timers.Timer();
        //    flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
        //}

        public Text_ArcMap(AxMapControl _mapControl, KmlText kmlText,FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;

            //System.Drawing.Color c = System.Drawing.Color.Red;

            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke((Action)delegate()
                {
                    System.Drawing.Color c = kmlText.Color;

                    textSymbol = new TextSymbolClass();
                    font = new stdole.StdFontClass();
                  
                    font.Name = kmlText.Font;
                    font.Size = (decimal)kmlText.Size;
                    switch (kmlText.FontStyle)
                    {
                        case FontStyle.Bold:
                            font.Bold = true;
                            font.Italic = false;
                            font.Strikethrough = false;
                            font.Underline = false;
                            break;
                        case FontStyle.Italic:
                            font.Bold = false;
                            font.Italic = true;
                            font.Strikethrough = false;
                            font.Underline = false;
                            break;
                        case FontStyle.Strikeout:
                           font.Bold = false;
                            font.Italic = false;
                            font.Strikethrough = true;
                            font.Underline = false;
                            break;
                        case FontStyle.Underline:
                            font.Bold = false;
                            font.Italic = false;
                            font.Strikethrough = false;
                            font.Underline = true;
                            break;
                    }
                    textSymbol.Font = font as stdole.IFontDisp;
                    textSymbol.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
                    //textSymbol.Color.RGB = c.B * 65536 + c.G * 256 + c.G;
                    base.Symbol = textSymbol;
                    base.Text = kmlText.Content;
                    base.ScaleText = true;
                    base.Geometry = new PointClass() { X = kmlText.Position.Lng, Y = kmlText.Position.Lat };
                });
            }
            else
            {
                System.Drawing.Color c = kmlText.Color;

                textSymbol = new TextSymbolClass();
                font = new stdole.StdFontClass();
                font.Name = kmlText.Font;
                font.Size = (decimal)kmlText.Size;
                switch (kmlText.FontStyle)
                {
                    case FontStyle.Bold:
                        font.Bold = true;
                        font.Italic = false;
                        font.Strikethrough = false;
                        font.Underline = false;
                        break;
                    case FontStyle.Italic:
                        font.Bold = false;
                        font.Italic = true;
                        font.Strikethrough = false;
                        font.Underline = false;
                        break;
                    case FontStyle.Strikeout:
                        font.Bold = false;
                        font.Italic = false;
                        font.Strikethrough = true;
                        font.Underline = false;
                        break;
                    case FontStyle.Underline:
                        font.Bold = false;
                        font.Italic = false;
                        font.Strikethrough = false;
                        font.Underline = true;
                        break;
                }
                textSymbol.Font = font as stdole.IFontDisp;
                textSymbol.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
                //textSymbol.Color.RGB = c.B * 65536 + c.G * 256 + c.G;
                base.Symbol = textSymbol;

                base.Text = kmlText.Content;
                base.ScaleText = true;
                base.Geometry = new PointClass() { X = kmlText.Position.Lng, Y = kmlText.Position.Lat };

            }

            this.Description = kmlText.Description;
            //记录
            bTextColor = kmlText.Color; ;
            bOutLineSize = kmlText.Size;

            position = new MapLngLat();
            position = kmlText.Position;//坐标点

            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 1000;

        }

        #region  ITextArcMap接口

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 所属图层
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set { belongLayer = value; }
        }

        /// <summary>
        /// 设置文字
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool SetContext(string context)
        {
            base.Text = context;
            Update();
            return true;
        }

        /// <summary>
        /// 获取文本内容
        /// </summary>
        /// <returns>文本内容</returns>
        public string GetContext()
        {
            return base.Text;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(System.Drawing.Color color)
        {
            //base.Color = new RgbColorClass() { Red = color.R, Green = color.G, Blue = color.B };
            textSymbol.Color = new RgbColorClass() { Red = color.R, Green = color.G, Blue = color.B };
            base.Symbol = textSymbol;
            bTextColor = color;
            Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="argb"></param>
        public void SetColor(int argb)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(argb);
            bTextColor = color;
            //base.Color = new RgbColorClass() { Red = color.R, Green = color.G, Blue = color.B };
            textSymbol.Color = new RgbColorClass() { Red = color.R, Green = color.G, Blue = color.B };
            base.Symbol = textSymbol;
            Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int r, int g, int b)
        {
            base.Color = new RgbColorClass() { Red = r, Green = g, Blue = b };
            bTextColor = System.Drawing.Color.FromArgb(r, g, b);
            textSymbol.Color = new RgbColorClass() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
            base.Symbol = textSymbol;
            Update();
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
            System.Drawing.Color color = System.Drawing.Color.FromArgb(a, r, g, b);
            bTextColor = color;
            //base.Color = new RgbColorClass() { Red = color.R, Green = color.G, Blue = color.B };
            textSymbol.Color = new RgbColorClass() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
            base.Symbol = textSymbol;
            Update();
        }

        /// <summary>
        /// 获取当前颜色
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Color GetColor()
        {
            IColor arcColor = base.Color;
            int rgbNum = arcColor.RGB;
            return System.Drawing.ColorTranslator.FromOle(rgbNum);
        }

        /// <summary>
        /// 设置文字大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetSize(float size)
        {
            font.Size = (decimal)size;
            textSymbol.Font = font as stdole.IFontDisp;
            base.Symbol = textSymbol;
            bOutLineSize = size;
            Update();
            return true;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public bool SetFont(string familyName)
        {
            //base.FontName = familyName;
            font.Name = familyName;
            font.Size = (decimal)bOutLineSize;
            textSymbol.Font = font as stdole.IFontDisp;
            base.Symbol = textSymbol;
            Update();
            return true;
        }

        /// <summary>
        /// 获取字体
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Font GetFont()
        {
            System.Drawing.Font font1 = new System.Drawing.Font(font.Name,(float)textSymbol.Size);
            return font1;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="emSize"></param>
        /// <returns></returns>
        public bool SetFont(string familyName, float emSize)
        {
            font.Name = familyName;
            font.Size = (decimal)emSize;
            textSymbol.Font = font as stdole.IFontDisp;
            base.Symbol = textSymbol;
            Update();
            return true;
        }

        public bool SetFont(string familyName, float emSize, System.Drawing.FontStyle fontStyle = FontStyle.Regular)
        {
            font.Name = familyName;
            font.Size = (decimal)emSize;
            switch (fontStyle)
            {
                case FontStyle.Regular:
                    font.Bold = false;
                    font.Italic = false;
                    font.Strikethrough = false;
                    font.Underline = false;
                    break;
                case FontStyle.Bold:
                    font.Bold = true;
                    font.Italic = false;
                    font.Strikethrough = false;
                    font.Underline = false;
                    break;
                case FontStyle.Italic:
                    font.Bold = false;
                    font.Italic = true;
                    font.Strikethrough = false;
                    font.Underline = false;
                    break;
                case FontStyle.Strikeout:
                    font.Bold = false;
                    font.Italic = false;
                    font.Strikethrough = true;
                    font.Underline = false;
                    break;
                case FontStyle.Underline:
                    font.Bold = false;
                    font.Italic = false;
                    font.Strikethrough = false;
                    font.Underline = true;
                    break;
            }
            textSymbol.Font = font as stdole.IFontDisp;
            base.Symbol = textSymbol;
            Update();
            return true;
        }
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <returns></returns>
        public Core.Model.MapLngLat GetLngLat()
        {
            //IPoint point = base.Geometry as IPoint;
            //MapLngLat mapPoint = new MapLngLat() { Lng = point.X, Lat = point.Y };
            return position;
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
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
            get { return pHightlight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return pIsFlash; }
        }

        /// <summary>
        /// 图元是否可见true显示,false隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">是否高亮</param>
        public void HightLight(bool isHightLight)
        {
            lock (lockObj)
            {
                if (isHightLight)
                {
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(Math.Abs(255 - this.bTextColor.R), Math.Abs(255 - this.bTextColor.G), Math.Abs(255 - this.bTextColor.B));
                    //base.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
                    //base.Size = bOutLineSize + 5;
                    font.Size = (decimal)(bOutLineSize + 5);
                    textSymbol.Font = font as stdole.IFontDisp;
                    textSymbol.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
                    base.Symbol = textSymbol;
                    Update();
                }
                else
                {
                    //SetColor(bTextColor);
                    //SetSize(bOutLineSize);
                    font.Size = (decimal)bOutLineSize;
                    textSymbol.Font = font as stdole.IFontDisp;
                    textSymbol.Color = new RgbColorClass() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
                    base.Symbol = textSymbol;
                    Update();

                }
            }
            pHightlight = isHightLight;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="_isFlash">是否闪烁</param>
        /// <param name="interval">闪烁时间</param>
        public void Flash(bool _isFlash,int interval = 500)
        {
            if (pIsFlash == _isFlash) return;   // 防止被多次调用
            pIsFlash = _isFlash;
            if (_isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                base.Color = new RgbColorClass() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
                base.Size = bOutLineSize;
                Update();
            }
        }

        /// <summary>
        /// 时间间隔事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Drawing.Color c = System.Drawing.Color.FromArgb(Math.Abs(255 - this.bTextColor.R), Math.Abs(255 - this.bTextColor.G), Math.Abs(255 - this.bTextColor.B));
            if (!isTimer)   // 闪烁
            {
                if ((this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).InvokeRequired)
                {
                    (this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).Invoke(new System.Action(
                        delegate
                        {
                            textSymbol.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
                            textSymbol.Size = bOutLineSize + 5;
                            base.Symbol = textSymbol;
                        }));
                }
                else
                {
                    textSymbol.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };
                    textSymbol.Size = bOutLineSize + 5;
                    base.Symbol = textSymbol;
                }
            }
            else    // 停止闪烁
            {
                if ((this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).InvokeRequired)
                {
                    (this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).Invoke(new System.Action(
                        delegate
                        {
                            textSymbol.Color = new RgbColor() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
                            textSymbol.Size = bOutLineSize;
                            base.Symbol = textSymbol;
                        }));
                }
                else
                {
                    //base.Color = new RgbColor() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
                    //base.Size = bOutLineSize;
                    textSymbol.Color = new RgbColor() { Red = bTextColor.R, Green = bTextColor.G, Blue = bTextColor.B };
                    textSymbol.Size = bOutLineSize;
                    base.Symbol = textSymbol;
                }
            }
            Update();
            isTimer = !isTimer;
        }

        /// <summary>
        /// 图元显示
        /// </summary>
        /// <param name="isVisible">是否显示</param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            ILayer layer = mapFactory.GetLayerByName(belongLayer.LayerName);
            CompositeGraphicsLayerClass graphLayer = layer as CompositeGraphicsLayerClass;
            if (isVisible)//显示
            {
                graphLayer.AddElement(this, 1);
                //CompositeGraphicsLayer.AddElement(this, 1);
            }
            else
            {
                graphLayer.DeleteElement(this);
            }
            this.isVisible = isVisible;
            Update();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (this.BelongLayer != null)
            {
                this.BelongLayer.Refresh();
            }
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
            set { _elementName = value; base.Name = _elementName; }
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
            pHightlight = false;
            pIsFlash = false;
            isVisible = true;
            position = null;
            mapControl = null;
            mapFactory = null;
        }
        #endregion

        /// <summary>
        /// 主线程做事
        /// </summary>
        /// <param name="action">要做的内容</param>
        /// <param name="synchronization">是否同步执行</param>
        private void Dosomething(Action action, bool synchronization)
        {
            if (mapControl == null) return;
            if (synchronization)
            {
                if (mapControl.InvokeRequired)
                    mapControl.Invoke(action);
                else
                    action();
            }
            else
            {
                if (mapControl.InvokeRequired)
                    mapControl.BeginInvoke(action);
                else
                    action();
            }
        }



        public void UpdatePosition(MapLngLat position)
        {
            throw new NotImplementedException();
        }
    }
}
