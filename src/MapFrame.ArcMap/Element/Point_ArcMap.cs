/**************************************************************************
 * 类名：Point_ArcMap.cs
 * 描述：点图元
 * 作者：Allen
 * 日期：Aug 23,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MapFrame.Core.Model;
using System;
using System.Drawing;
using ESRI.ArcGIS.Controls;
using System.Timers;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Factory;

namespace MapFrame.ArcMap.Element
{
    /// <summary>
    /// 点图元
    /// </summary>
    class Point_ArcMap : MarkerElementClass, IMFPoint
    {
        /// <summary>
        /// 是否可见true显示,false隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string name;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 点的颜色
        /// </summary>
        private Color bColor;
        /// <summary>
        /// marker标记
        /// </summary>
        private ISimpleMarkerSymbol pMarkerSymbol;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        private FactoryArcMap mapFactory = null;
        /// <summary>
        /// 目标位置
        /// </summary>
        private MapLngLat position;

        public Point_ArcMap(AxMapControl _mapControl, KmlPoint point, FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;

            Dosomething((Action)(delegate
            {
                IPoint pt = new PointClass();
                pt.PutCoords(point.Position.Lng, point.Position.Lat);
                base.Geometry = pt;

                pMarkerSymbol = new SimpleMarkerSymbolClass();
                IRgbColor color = new RgbColorClass()
                {
                    Transparency = point.Color.A,
                    Red = point.Color.R,
                    Green = point.Color.G,
                    Blue = point.Color.B
                };
                bColor = point.Color;
                pMarkerSymbol.Color = color;
                pMarkerSymbol.Size = 5;
                pMarkerSymbol.Angle = 90;
                pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                base.Symbol = pMarkerSymbol;
                this.Description = point.Description;
            }), true);

            position = new MapLngLat();
            position = point.Position;//目标位置

            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);
        }

        #region MapFrame.Core.Interface.IPointArcMap
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 设置点的大小
        /// </summary>
        /// <param name="size">大小</param>
        public void SetSize(double size)
        {
            pMarkerSymbol.Size = size;
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            Update();
        }

        /// <summary>
        /// 设置点的大小
        /// </summary>
        /// <param name="size">大小</param>
        public void SetSize(Size size)
        {
            pMarkerSymbol.Size = size.Width;
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            Update();
        }

        /// <summary>
        /// 设置点的大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetSize(int width, int height)
        {
            pMarkerSymbol.Size = width;
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {
            IPoint pt = new PointClass();
            pt.PutCoords(lng, lat);
            base.Geometry = pt;

            position = new MapLngLat(lng, lat);//目标位置
            Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lngLat"></param>
        public void UpdatePosition(Core.Model.MapLngLat lngLat)
        {
            IPoint pt = new PointClass();
            pt.PutCoords(lngLat.Lng, lngLat.Lat);
            base.Geometry = pt;

            position = lngLat;//更新
            Update();
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="color">Color</param>
        public void SetColor(System.Drawing.Color color)
        {
            bColor = color;
            IColor c = new RgbColorClass()
            {
                Transparency = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
            pMarkerSymbol.Color = c;
            Update();
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        public void SetColor(int argb)
        {
            bColor = Color.FromArgb(argb);
            IColor c = new RgbColorClass()
            {
                Transparency = bColor.A,
                Red = bColor.R,
                Green = bColor.G,
                Blue = bColor.B
            };
            pMarkerSymbol.Color = c;

            Update();
        }

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="argb">rgb</param>
        public void SetColor(int r, int g, int b)
        {
            Color c = Color.FromArgb(r, g, b); ;
            SetColor(c);
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
            Color c = Color.FromArgb(a, r, g, b); ;
            SetColor(c);
        }

        /// <summary>
        /// 获取当前点的坐标
        /// </summary>
        /// <returns>坐标</returns>
        public Core.Model.MapLngLat GetLngLat()
        {
            return position;
        }

        #region TIP 标牌

        public void SetTipText(string tipText)
        {
            throw new NotImplementedException();
        }

        public void SetTipShow(Core.Model.ShowTypeEnum showType)
        {
            throw new NotImplementedException();
        }

        public void SetLabelText(string labelText)
        {
            throw new NotImplementedException();
        }

        public void SetLableShow(Core.Model.ShowTypeEnum showType)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 属性

        /// <summary>
        /// 所属图层
        /// </summary>
        public MapFrame.Core.Interface.IMFLayer BelongLayer
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
            get { return name; }
            set
            {
                name = value;
                base.Name = value;
            }
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
        /// 是否可见true显示,false隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
        }
        #endregion

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
                    pMarkerSymbol.Color = new RgbColorClass()
                    {
                        Red = Color.Red.R,
                        Green = Color.Red.G,
                        Blue = Color.Red.B
                    };
                    pMarkerSymbol.Size = 8;
                }
                else
                {
                    pMarkerSymbol.Color = new RgbColorClass()
                    {
                        Red = bColor.R,
                        Green = bColor.G,
                        Blue = bColor.B
                    };
                    pMarkerSymbol.Size = 5;
                }
            }
            this.isHightLight = isHightLight;
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">时间</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                pMarkerSymbol.Color = new RgbColorClass()
                {
                    Red = bColor.R,
                    Green = bColor.G,
                    Blue = bColor.B
                };
                Update();
            }
            this.isFlash = isFlash;
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
                Dosomething((Action)(delegate
                {
                    pMarkerSymbol.Color = new RgbColorClass()
                    {
                        Red = System.Drawing.Color.White.R,
                        Green = System.Drawing.Color.White.G,
                        Blue = System.Drawing.Color.White.B
                    };
                }), true);
            }
            else
            {
                Dosomething((Action)(delegate
                {
                    pMarkerSymbol.Color = new RgbColorClass()
                    {
                        Red = bColor.R,
                        Green = bColor.G,
                        Blue = bColor.B
                    };
                }), true);
            }

            Update();
            isTimer = !isTimer;
        }

        /// <summary>
        /// 显示、隐藏图元
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            ILayer layer = mapFactory.GetLayerByName(BelongLayer.LayerName);
            CompositeGraphicsLayerClass graphLayer = layer as CompositeGraphicsLayerClass;
            this.isVisible = isVisible;

            if (isVisible)//显示
            {
                graphLayer.AddElement(this, 1);
                Update();
            }
            else
            {
                graphLayer.DeleteElement(this);
                Update();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            base.Symbol = pMarkerSymbol;
            if (this.BelongLayer != null)
            {
                this.BelongLayer.Refresh();
            }
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
            pMarkerSymbol = null;
            isFlash = false;
            this.isHightLight = false;
            this.isVisible = true;
            mapFactory = null;
            mapControl = null;
        }
        #endregion
        
        public void SetAngle(double angle)
        {
            throw new NotImplementedException();
        }

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
