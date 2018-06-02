/**************************************************************************
 * 类名：Point_ArcGlobe.cs
 * 描述：点图元
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Element
{
    /// <summary>
    /// 点图元
    /// </summary>
    class Point_ArcGlobe : MarkerElementClass, IMFPoint
    {
        /// <summary>
        /// 图元要素
        /// </summary>
        private SimpleMarker3DSymbolClass markerSymbol = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IGlobeGraphicsLayer graphicsLayer = null;
        /// <summary>
        /// 图元索引
        /// </summary>
        private int index = -1;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 是否可见true显示,false隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 点的颜色
        /// </summary>
        private Color color;
        /// <summary>
        /// 点的大小
        /// </summary>
        private double size;
        /// <summary>
        /// 图层属性
        /// </summary>
        public IGlobeGraphicsElementProperties ElementProperties;
        /// <summary>
        /// 是否栅格化
        /// </summary>
        public bool Rasterize
        {
            get
            {
                this.Dosomething((Action)delegate()
                {
                    graphicsLayer.GetGlobeProperties(this, out ElementProperties);
                }, true);
                return ElementProperties.Rasterize;
            }
            set
            {
                ElementProperties.Rasterize = value;
                this.Dosomething((Action)delegate()
                    {
                        graphicsLayer.SetGlobeProperties(this, ElementProperties);
                    }, true);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_graphicsLayer">图层</param>
        /// <param name="pointKml">点kml</param>
        public Point_ArcGlobe(IGlobeGraphicsLayer _graphicsLayer, KmlPoint pointKml)
        {
            graphicsLayer = _graphicsLayer;

            this.ElementType = Core.Model.ElementTypeEnum.Point;//图元类型
            this.Description = pointKml.Description;//图元描述

            #region  位置

            IPoint pt = new PointClass();
            pt.PutCoords(pointKml.Position.Lng, pointKml.Position.Lat);
            pt.Z = pointKml.Position.Alt;
            (pt as IZAware).ZAware = true;//设置高度
            base.Geometry = pt;

            #endregion

            #region  符号

            markerSymbol = new SimpleMarker3DSymbolClass();

            //设置颜色
            if (pointKml.Color.ToArgb() == 0)
            {
                IRgbColor color = new RgbColorClass()
                {
                    Transparency = 50,
                    Red = Color.Green.R,
                    Green = Color.Green.G,
                    Blue = Color.Green.B
                };
                markerSymbol.Color = color;
            }
            else
            {
                IRgbColor color = new RgbColorClass()
                {
                    Transparency = pointKml.Color.A,
                    Red = pointKml.Color.R,
                    Green = pointKml.Color.G,
                    Blue = pointKml.Color.B
                };
                markerSymbol.Color = color;
            }

            //设置大小
            if (pointKml.Size.Height == 0)
                markerSymbol.Size = 500;
            else
                markerSymbol.Size = pointKml.Size.Height;

            markerSymbol.Angle = 90;
            markerSymbol.Style = (esriSimple3DMarkerStyle)pointKml.PointStyle;
            base.Symbol = markerSymbol;

            #endregion

            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 3000;
            isFlash = false;
        }

        #region MapFrame.Core.Interface.IPointArcGlobe
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
            get
            {
                return this.layer;
            }
            set
            {
                layer = value;
                mapControl = value.MapControl as AxGlobeControl;
            }
        }


        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 图元描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        // 注意！！自动实现接口的方法时并没有生成Name属性，这样会运行时会产生错误，需加上Name属性
        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                this.Dosomething((Action)delegate()
                {
                    graphicsLayer.PutElementName(this, value);
                }, true);
            }
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
        /// 图元是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return this.isHightLight; }
        }

        /// <summary>
        /// 图元是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return this.isFlash; }
        }

        /// <summary>
        /// 图元是否可见true显示,false隐藏
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
        /// 设置点的大小
        /// </summary>
        /// <param name="size">大小</param>
        public void SetSize(double size)
        {
            if (this.ElementProperties == null) return;     // add by allen 2016.12.26  shanghai

            this.Dosomething((Action)delegate()
            {
                if (this.ElementProperties.Rasterize)
                {
                    this.Rasterize = false;
                    this.size = markerSymbol.Size = size;
                    base.Symbol = markerSymbol;
                    this.Rasterize = true;
                }
                else
                {
                    markerSymbol.Size = size;
                    base.Symbol = markerSymbol;
                }
            }, true);
            this.Update();
        }

        /// <summary>
        /// 设置点的大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
        {
            this.SetSize(size.Width);
        }

        /// <summary>
        /// 设置点的大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetSize(int width, int height)
        {
            this.SetSize(width);
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        /// <param name="angle"></param>
        public void SetAngle(double angle)
        {
            this.Dosomething((Action)delegate()
            {
                markerSymbol.Angle = angle;
                base.Symbol = markerSymbol;
            }, true);
            this.Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {
            this.Dosomething((Action)delegate()
            {
                IPoint pt = new PointClass();
                pt.PutCoords(lng, lat);
                pt.Z = alt;
                (pt as IZAware).ZAware = true;
                base.Geometry = pt;
            }, true);
            this.Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lngLat"></param>
        public void UpdatePosition(MapLngLat lngLat)
        {
            this.Dosomething((Action)delegate()
            {
                IPoint pt = new PointClass();
                pt.PutCoords(lngLat.Lng, lngLat.Lat);
                pt.Z = lngLat.Alt;
                (pt as IZAware).ZAware = true;
                base.Geometry = pt;
            }, true);
            this.Update();
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="color">Color</param>
        public void SetColor(Color color)
        {
            this.Dosomething((Action)delegate()
            {
                IColor c = new RgbColorClass()
                {
                    Transparency = color.A,
                    Red = color.R,
                    Green = color.G,
                    Blue = color.B
                };
                markerSymbol.Color = c;
                base.Symbol = markerSymbol;
                this.color = color;
            }, true);
            this.Update();
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="argb">rgb</param>
        public void SetColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b); ;
            SetColor(color);
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color color = Color.FromArgb(a, r, g, b); ;
            SetColor(color);
        }

        /// <summary>
        /// 获取当前点的坐标
        /// </summary>
        /// <returns>坐标</returns>
        public MapLngLat GetLngLat()
        {
            IPoint point = base.Geometry as IPoint;
            return new MapLngLat(point.X, point.Y, point.Z);
        }

        /// <summary>
        /// 设置tip内容
        /// </summary>
        /// <param name="tipText">内容</param>
        public void SetTipText(string tipText)
        {

        }

        /// <summary>
        /// 设置tip显示
        /// </summary>
        /// <param name="showType">显示方式</param>
        public void SetTipShow(ShowTypeEnum showType)
        {

        }

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="labelText">内容</param>
        public void SetLabelText(string labelText)
        {

        }

        /// <summary>
        /// 设置标牌显示
        /// </summary>
        /// <param name="showType">显示方式</param>
        public void SetLableShow(ShowTypeEnum showType)
        {

        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">是否高亮</param>
        public void HightLight(bool isHightLight)
        {
            if (isHightLight)//高亮
            {
                Color c = Color.FromArgb(Math.Abs(255 - this.color.R), Math.Abs(255 - this.color.G), Math.Abs(255 - this.color.B));
                IRgbColor color = new RgbColorClass();
                color.Transparency = c.A;
                color.Red = c.R;
                color.Green = c.G;
                color.Blue = c.B;
                markerSymbol.Color = color;
                markerSymbol.Size = this.size + 5;
                base.Symbol = markerSymbol;
            }
            else //不高亮
            {
                SetColor(color);
                SetSize(this.size);
            }
            this.isHightLight = isHightLight;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (isFlash == this.isFlash) return;
            flashTimer.Interval = interval;
            if (isFlash)
            {
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                this.SetVisible(true);
            }
            this.isFlash = isFlash;
        }

        /// <summary>
        /// 显示/隐藏
        /// </summary>
        /// <param name="isVisible">是否可见true显示,false隐藏</param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            graphicsLayer.PutIsElementVisible(index, isVisible);
            this.isVisible = isVisible;
            this.Update();
        }

        /// <summary>
        /// 事件间隔事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isTimer)//闪烁
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
        /// 刷新
        /// </summary>
        public void Update()
        {
            this.Dosomething((Action)delegate()
            {
                graphicsLayer.UpdateElementByIndex(index);
                layer.Refresh();
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
            markerSymbol = null;
            graphicsLayer = null;
            index = -1;
            isFlash = false;
            isVisible = true;
            isHightLight = false;
            isTimer = false;
            mapControl = null;
            layer = null;
            ElementProperties = null;
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

        
    }
}
