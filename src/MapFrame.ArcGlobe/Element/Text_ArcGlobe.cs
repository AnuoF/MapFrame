/**************************************************************************
 * 类名：Text_ArcGlobe.cs
 * 描述：圆形图元接口
 * 作者：lx
 * 日期：Sep 19,2016
 * 
 * ************************************************************************/

using System;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GlobeCore;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Element
{
    class Text_ArcGlobe : Text3DElementClass, IMFText
    {
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 高亮
        /// </summary>
        private bool isHightlight = false;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 图元是否显示
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 闪烁判定
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 颜色
        /// </summary>
        private Color fontColor;
        /// <summary>
        /// 字体大小
        /// </summary>
        private float fontSize;
        /// <summary>
        /// 图元标识符
        /// </summary>
        private int index = -1;
        /// <summary>
        /// 图元索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 填充风格
        /// </summary>
        private IFillSymbol fillSymbol = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IGlobeGraphicsLayer graphicsLayer = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 图元属性
        /// </summary>
        private IGlobeGraphicsElementProperties properties = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="graphicsLayer"></param>
        /// <param name="textKml">文字kml</param>
        public Text_ArcGlobe(IGlobeGraphicsLayer graphicsLayer, KmlText textKml)
        {
            this.graphicsLayer = graphicsLayer;

            this.ElementType = ElementTypeEnum.Text;              //图元类型
            this.Description = textKml.Description;                          //图元描述

            #region 位置

            IPoint point = new PointClass() { X = textKml.Position.Lng, Y = textKml.Position.Lat, Z = textKml.Position.Alt };
            (point as IZAware).ZAware = true;
            //base.Geometry = point;

            #endregion

            #region  符号

            base.FixedAspectRatio = true;//表明是否固定界限比例
            //this.ReferenceScale = 100;//设置固定比例,不知道具体作用
            base.AnchorPoint = point;//锚
            base.Text = textKml.Content;
            base.Height = textKml.Size;
            base.FontName = textKml.Font;
            base.AxisRotation = esriT3DRotationAxis.esriT3DRotateAxisX; //旋转轴
            base.RotationAngle = -90;//旋转角度
            Color _color = textKml.Color;
            IRgbColor color = new RgbColorClass()
            {
                Red = _color.R,
                Green = _color.G,
                Blue = _color.B
            };
            fillSymbol = new SimpleFillSymbolClass();
            fillSymbol.Color = color;
            base.Symbol = fillSymbol;

            this.fontColor = textKml.Color;
            this.fontSize = textKml.Size;
            #endregion

            isFlash = false;
            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }

        #region  MapFrame.Core.Interface.IText
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 设置文字
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool SetContext(string context)
        {
            base.Text = context;
            this.Update();
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
        public void SetColor(Color color)
        {
            fillSymbol.Color = new RgbColorClass() { Red = color.R, Green = color.G, Blue = color.B, Transparency = color.A };
            this.fontColor = color;
            base.Symbol = fillSymbol;
            this.Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="argb"></param>
        public void SetColor(int argb)
        {
            SetColor(Color.FromArgb(argb));
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r">Red值</param>
        /// <param name="g">Green值</param>
        /// <param name="b">Blue值</param>
        public void SetColor(int r, int g, int b)
        {
            SetColor(Color.FromArgb(r, g, b));
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a">A值</param>
        /// <param name="r">Red值</param>
        /// <param name="g">Green值</param>
        /// <param name="b">Blue值</param>
        public void SetColor(int a, int r, int g, int b)
        {
            SetColor(Color.FromArgb(a, r, g, b));
        }

        /// <summary>
        /// 获取当前颜色
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return ColorTranslator.FromOle(fillSymbol.Color.RGB);
        }

        /// <summary>
        /// 设置文字大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetSize(float size)
        {
            base.Height = size;
            this.fontSize = size;
            this.Update();
            return true;
        }

        /// <summary>
        /// 设置文字字体
        /// </summary>
        /// <param name="familyName">字体名称</param>
        /// <returns></returns>
        public bool SetFont(string familyName)
        {
            base.FontName = familyName;
            this.Update();
            return true;
        }

        /// <summary>
        /// 获取字体
        /// </summary>
        /// <returns></returns>
        public Font GetFont()
        {
            Font font = new Font(base.FontName, (float)base.Height);
            return font;
        }

        /// <summary>
        /// 设置文字字体
        /// </summary>
        /// <param name="familyName">字体名称</param>
        /// <param name="emSize">文字大小</param>
        /// <returns></returns>
        public bool SetFont(string familyName, float emSize)
        {
            base.FontName = familyName;
            base.Height = emSize;
            this.Update();
            return true;
        }

        /// <summary>
        /// 获取位置信息
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetLngLat()
        {
            IPoint point = base.AnchorPoint;
            return new MapLngLat() { Lng = point.X, Lat = point.Y, Alt = point.Z };
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 图元所属图层
        /// </summary>
        public IMFLayer BelongLayer
        {
            get { return layer; }
            set
            {
                layer = value;
                mapControl = value.MapControl as AxGlobeControl;
            }
        }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 获取图元名称
        /// </summary>
        public string ElementName
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                this.Dosomething((Action)delegate()
                {
                    this.graphicsLayer.PutElementName(this, value);
                }, true);
            }
        }

        /// <summary>
        /// 获取图元类型
        /// </summary>
        public ElementTypeEnum ElementType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否高亮显示
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightlight; }
        }

        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible
        {
            get
            {
                graphicsLayer.GetIsElementVisible(this.index, out this.isVisible);
                return isVisible;
            }
        }

        /// <summary>
        /// 高亮显示
        /// </summary>
        /// <param name="isHightLight">true为高亮</param>
        public void HightLight(bool isHightLight)
        {
            lock (lockObj)
            {
                if (isHightLight)
                {
                    Color c = Color.FromArgb(Math.Abs(255 - this.fontColor.R), Math.Abs(255 - this.fontColor.G), Math.Abs(255 - this.fontColor.B));
                    fillSymbol.Color = new RgbColorClass() { Red = c.R, Green = c.G, Blue = c.B };

                    base.Symbol = fillSymbol;
                    base.Height = fontSize + 5;
                }
                else
                {
                    fillSymbol.Color = new RgbColorClass() { Red = fontColor.R, Green = fontColor.G, Blue = fontColor.B };
                    base.Height = fontSize;
                    base.Symbol = fillSymbol;
                }
            }
            isHightlight = isHightLight;
            this.Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;
            this.isFlash = isFlash;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                this.SetVisible(true);
            }
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isTimer)   // 闪烁
            {
                this.SetVisible(false);
            }
            else    // 停止闪烁
            {
                this.SetVisible(true);
            }
            isTimer = !isTimer;
        }

        /// <summary>
        /// 隐藏图元
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            graphicsLayer.PutIsElementVisible(index, isVisible);
            this.isVisible = isVisible;
            this.Update();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            this.Dosomething((Action)delegate()
            {
                graphicsLayer.UpdateElementByIndex(index);
                base.Update();
                if (this.layer != null)
                {
                    this.layer.Refresh();
                }
            }, true);
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
                flashTimer = null;
            }
            mapControl = null;
            isHightlight = false;
            isVisible = true;
            isTimer = false;
            isFlash = false;
            index = -1;
            lockObj = null;
            fillSymbol = null;
            graphicsLayer = null;
            layer = null;
            properties = null;
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
