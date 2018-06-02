/**************************************************************************
 * 类名：Circle_ArcMap.cs
 * 描述：ArcGis圆类
 * 作者：lx
 * 日期：September 8,2016
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Carto;
using System.Timers;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using System.Drawing;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Factory;

namespace MapFrame.ArcMap.Element
{
    /// <summary>
    /// ArcGis圆类
    /// </summary>
    class Circle_ArcMap : CircleElementClass, IElement, IMFCircle
    {
        /// <summary>
        /// 高亮
        /// </summary>
        private bool pHightLight = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool pIsFlash = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 记录轮廓颜色
        /// </summary>
        private System.Drawing.Color bOutLineColor;
        /// <summary>
        /// 记录填充色
        /// </summary>
        private System.Drawing.Color bFillColor;
        /// <summary>
        /// 记录轮廓宽度
        /// </summary>
        private float bWidth;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string elementName;
        /// <summary>
        /// 半径(单位：千米)
        /// </summary>
        private double pRadius;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private FactoryArcMap factoryArcMap = null;

        #region 多边形图元
        /// <summary>
        /// 线符号
        /// </summary>
        private ILineSymbol lineSymbol;
        /// <summary>
        /// 填充符号
        /// </summary>
        private IFillSymbol fillSymbol;
        /// <summary>
        /// 片段集合
        /// </summary>
        private ISegmentCollection iSeg;
        /// <summary>
        /// 环形
        /// </summary>
        private IRing pRing;
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private IPoint centerPoint;
        /// <summary>
        /// 几何元素集合
        /// </summary>
        private IGeometryCollection pGeometryColl;
        /// <summary>
        /// 几何元素
        /// </summary>
        private IGeometry pGeometry;
        /// <summary>
        /// 图元 所属图层
        /// </summary>
        private Core.Interface.IMFLayer belongLayer;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        /// <param name="circle"></param>
        public Circle_ArcMap(AxMapControl _mapControl, KmlCircle circle, FactoryArcMap facArc)
        {
            this.mapControl = _mapControl;
            this.factoryArcMap = facArc;

            Dosomething(new Action(delegate
            {
                lineSymbol = new SimpleLineSymbolClass();
                lineSymbol.Color = new RgbColorClass()
                {
                    Red = circle.StrokeColor.R,
                    Green = circle.StrokeColor.B,
                    Blue = circle.StrokeColor.B
                };
                lineSymbol.Width = circle.StrokeWidth;

                fillSymbol = new SimpleFillSymbol();
                fillSymbol.Outline = lineSymbol;
                //填充色
                IColor fillColor = new RgbColorClass()
                {
                    Transparency = circle.FillColor.A,
                    Red = circle.FillColor.R,
                    Green = circle.FillColor.G,
                    Blue = circle.FillColor.B
                };
                fillSymbol.Color = fillColor;

                base.Symbol = fillSymbol;//颜色和风格

                iSeg = new RingClass();
                centerPoint = new PointClass();//圆心坐标
                centerPoint.PutCoords(circle.Position.Lng, circle.Position.Lat);
                MapLngLat around = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)circle.Radius, circle.Position, 180);
                INewLineFeedback backline = new NewLineFeedbackClass();
                IPoint aroundPoint = new PointClass() { X = around.Lng, Y = around.Lat };
                backline.Start(centerPoint);
                backline.AddPoint(aroundPoint);
                var geo = backline.Stop();

                iSeg.SetCircle(centerPoint, geo.Length);
                object o = System.Type.Missing;
                pRing = iSeg as IRing;
                pRing.Close();

                pGeometryColl = new PolygonClass();
                pGeometryColl.AddGeometry(pRing, ref o, ref o);
                pGeometry = pGeometryColl as IGeometry;

                base.Geometry = pGeometry;
            }), true);

            pRadius = circle.Radius;
            bOutLineColor = circle.StrokeColor;
            bFillColor = circle.FillColor;
            bWidth = circle.StrokeWidth;

            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);

            #region MyRegion

            //if (mapControl.InvokeRequired)
            //{
            //    mapControl.Invoke((Action)delegate()
            //    {
            //        lineSymbol = new SimpleLineSymbolClass();
            //        lineSymbol.Color = new RgbColorClass()
            //        {
            //            Red = circle.StrokeColor.R,
            //            Green = circle.StrokeColor.B,
            //            Blue = circle.StrokeColor.B
            //        };
            //        lineSymbol.Width = circle.StrokeWidth;

            //        fillSymbol = new SimpleFillSymbol();
            //        fillSymbol.Outline = lineSymbol;
            //        //填充色
            //        IColor fillColor = new RgbColorClass()
            //        {
            //            Transparency = circle.FillColor.A,
            //            Red = circle.FillColor.R,
            //            Green = circle.FillColor.G,
            //            Blue = circle.FillColor.B
            //        };
            //        fillSymbol.Color = fillColor;

            //        base.Symbol = fillSymbol;//颜色和风格

            //        iSeg = new RingClass();
            //        centerPoint = new PointClass();//圆心坐标
            //        centerPoint.PutCoords(circle.Position.Lng, circle.Position.Lat);
            //        //MapFrame.Core.Model.MapLngLat around = GetPointByDistanceAndAngle(circle.Radius, circle.Position);
            //        MapLngLat around = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)circle.Radius, circle.Position, 180);
            //        INewLineFeedback backline = new NewLineFeedbackClass();
            //        IPoint aroundPoint = new PointClass() { X = around.Lng, Y = around.Lat };
            //        backline.Start(centerPoint);
            //        backline.AddPoint(aroundPoint);
            //        var geo = backline.Stop();

            //        iSeg.SetCircle(centerPoint, geo.Length);
            //        object o = System.Type.Missing;
            //        pRing = iSeg as IRing;
            //        pRing.Close();

            //        pGeometryColl = new PolygonClass();
            //        pGeometryColl.AddGeometry(pRing, ref o, ref o);
            //        pGeometry = pGeometryColl as IGeometry;

            //        base.Geometry = pGeometry;
            //    });
            //}
            //else
            //{
            //    lineSymbol = new SimpleLineSymbolClass();
            //    lineSymbol.Color = new RgbColorClass()
            //    {
            //        Red = circle.StrokeColor.R,
            //        Green = circle.StrokeColor.B,
            //        Blue = circle.StrokeColor.B
            //    };
            //    lineSymbol.Width = circle.StrokeWidth;


            //    fillSymbol = new SimpleFillSymbol();
            //    fillSymbol.Outline = lineSymbol;
            //    //填充色
            //    IColor fillColor = new RgbColorClass()
            //    {
            //        Transparency = circle.FillColor.A,
            //        Red = circle.FillColor.R,
            //        Green = circle.FillColor.G,
            //        Blue = circle.FillColor.B
            //    };
            //    fillSymbol.Color = fillColor;

            //    base.Symbol = fillSymbol;//颜色和风格

            //    iSeg = new RingClass();
            //    centerPoint = new PointClass();//圆心坐标
            //    centerPoint.PutCoords(circle.Position.Lng, circle.Position.Lat);
            //    //MapFrame.Core.Model.MapLngLat around = GetPointByDistanceAndAngle(circle.Radius, circle.Position);
            //    MapLngLat around = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)circle.Radius, circle.Position, 180);
            //    INewLineFeedback backline = new NewLineFeedbackClass();
            //    IPoint aroundPoint = new PointClass() { X = around.Lng, Y = around.Lat };
            //    backline.Start(centerPoint);
            //    backline.AddPoint(aroundPoint);
            //    var geo = backline.Stop();

            //    iSeg.SetCircle(centerPoint, geo.Length);
            //    object o = System.Type.Missing;
            //    pRing = iSeg as IRing;
            //    pRing.Close();

            //    pGeometryColl = new PolygonClass();
            //    pGeometryColl.AddGeometry(pRing, ref o, ref o);
            //    pGeometry = pGeometryColl as IGeometry;

            //    base.Geometry = pGeometry;
            //}
            #endregion
        }

        #region IElement
        /// <summary>
        /// 透明度（0-100）
        /// </summary>
        private int _Opacity;

        void IElement.Activate(IDisplay Display)
        {
            base.Activate(Display);
        }

        void IElement.Deactivate()
        {
            base.Deactivate();
        }

        void IElement.Draw(IDisplay Display, ITrackCancel TrackCancel)
        {
            ITransparencyDisplayFilter filter = new TransparencyDisplayFilterClass
            {
                Transparency = (short)((this._Opacity * 0xff) / 100)
            };
            IDisplayFilter filter2 = null;
            if (Display.Filter != null)
            {
                filter2 = (Display.Filter as IClone).Clone() as IDisplayFilter;
            }
            Display.Filter = filter;
            base.Draw(Display, TrackCancel);
            Display.Filter = filter2;
        }

        bool IElement.HitTest(double x, double y, double Tolerance)
        {
            return base.HitTest(x, y, Tolerance);
        }

        void IElement.QueryBounds(IDisplay Display, IEnvelope Bounds)
        {
            base.QueryBounds(Display, Bounds);
        }

        void IElement.QueryOutline(IDisplay Display, IPolygon Outline)
        {
            base.QueryOutline(Display, Outline);
        }

        IGeometry IElement.Geometry
        {
            get { return base.Geometry; }
            set { base.Geometry = value; }
        }

        bool IElement.Locked
        {
            get { return base.Locked; }
            set { base.Locked = value; }
        }

        ISelectionTracker IElement.SelectionTracker
        { get { return base.SelectionTracker; } }

        /// <summary>
        /// 设置透明度(0-100)
        /// </summary>
        public int Opacity
        {
            get { return this._Opacity; }
            set { this._Opacity = value; }
        }
        #endregion

        #region MapFrame.Core.Interface.ICircleArcMap
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
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity">透明度</param>
        public void SetOpacity(int _opacity)
        {
            this.Opacity = _opacity;
            Update();
        }

        /// <summary>
        /// 所属图层
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set
            {
                belongLayer = value;
                mapControl = value.MapControl as AxMapControl;
            }
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
            get { return pHightLight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return pIsFlash; }
        }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
        }

        /// <summary>
        /// 圆心坐标
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetCenterDot()
        {
            return new MapLngLat() { Lng = centerPoint.X, Lat = centerPoint.Y };
        }

        /// <summary>
        /// 返回半径（单位为100千米)
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            return pRadius;
        }

        /// <summary>
        /// 更新圆心坐标
        /// </summary>
        /// <param name="centerDot"></param>
        public void UpdatePosition(MapLngLat centerDot)
        {
            this.Dosomething((Action)delegate()
            {
                centerPoint = new PointClass() { X = centerDot.Lng, Y = centerDot.Lat };
                MapLngLat center = new MapLngLat(centerPoint.X, centerPoint.Y);
                MapLngLat around = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)pRadius, center, 180);
                INewLineFeedback backline = new NewLineFeedbackClass();
                centerPoint = new PointClass() { X = center.Lng, Y = center.Lat };
                IPoint aroundPoint = new PointClass() { X = around.Lng, Y = around.Lat };
                backline.Start(centerPoint);
                backline.AddPoint(aroundPoint);
                var geo = backline.Stop();
                iSeg = new RingClass();
                iSeg.SetCircle(centerPoint, geo.Length);
                object o = System.Type.Missing;
                pRing = iSeg as IRing;
                pRing.Close();

                pGeometryColl = new PolygonClass();
                pGeometryColl.AddGeometry(pRing, ref o, ref o);
                pGeometry = pGeometryColl as IGeometry;

                base.Symbol = fillSymbol;
                base.Geometry = pGeometry;
            }, true);

            Update();
        }

        /// <summary>
        /// 更新半径 千米
        /// </summary>
        /// <param name="radius"></param>
        public void UpdatePosition(double radius)
        {
            this.Dosomething((Action)delegate()
            {
                pRadius = radius;

                MapLngLat center = new MapLngLat(centerPoint.X, centerPoint.Y);
                MapLngLat around = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)radius, center, 180);
                INewLineFeedback backline = new NewLineFeedbackClass();
                centerPoint = new PointClass() { X = center.Lng, Y = center.Lat };
                IPoint aroundPoint = new PointClass() { X = around.Lng, Y = around.Lat };
                backline.Start(centerPoint);
                backline.AddPoint(aroundPoint);
                var geo = backline.Stop();
                iSeg.SetCircle(centerPoint, geo.Length);
                object o = System.Type.Missing;
                pRing = iSeg as IRing;
                pRing.Close();

                pGeometryColl = new PolygonClass();
                pGeometryColl.AddGeometry(pRing, ref o, ref o);
                pGeometry = pGeometryColl as IGeometry;

                base.Symbol = fillSymbol;
                base.Geometry = pGeometry;
            }, true);

            Update();
        }

        /// <summary>
        /// 更新圆心坐标和半径
        /// </summary>
        /// <param name="centerDot"></param>
        /// <param name="radius"></param>
        public void UpdatePosition(MapLngLat centerDot, double radius)
        {
            centerPoint = new PointClass() { X = centerDot.Lng, Y = centerDot.Lat };
            this.Dosomething((Action)delegate()
            {
                MapLngLat around = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)radius, centerDot, 180);
                INewLineFeedback backline = new NewLineFeedbackClass();
                centerPoint = new PointClass() { X = centerDot.Lng, Y = centerDot.Lat };
                IPoint aroundPoint = new PointClass() { X = around.Lng, Y = around.Lat };
                backline.Start(centerPoint);
                backline.AddPoint(aroundPoint);
                var geo = backline.Stop();
                iSeg.SetCircle(centerPoint, geo.Length);
                object o = System.Type.Missing;
                pRing = iSeg as IRing;
                pRing.Close();

                pGeometryColl = new PolygonClass();
                pGeometryColl.AddGeometry(pRing, ref o, ref o);
                pGeometry = pGeometryColl as IGeometry;

                base.Geometry = pGeometry;
                base.Symbol = fillSymbol;
            }, true);

            Update();
        }

        /// <summary>
        /// 更新填充色
        /// </summary>
        /// <param name="fillColor"></param>
        public void SetFillColor(System.Drawing.Color fillColor)
        {
            this.Dosomething((Action)delegate()
            {
                fillSymbol.Color = new RgbColorClass() { Red = fillColor.R, Green = fillColor.G, Blue = fillColor.B };
                bFillColor = fillColor;
                fillSymbol.Outline = lineSymbol;

                base.Symbol = fillSymbol;
            }, true);

            Update();
        }

        /// <summary>
        /// 更新填充色和轮廓宽度
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public void SetStroke(System.Drawing.Color color, float width)
        {
            this.Dosomething((Action)delegate()
            {
                lineSymbol.Color = new RgbColorClass()
                {
                    Red = color.R,
                    Green = color.B,
                    Blue = color.B
                };
                bOutLineColor = color;
                bWidth = width;
                lineSymbol.Width = width;
                fillSymbol.Outline = lineSymbol;

                base.Symbol = fillSymbol;
            }, true);

            Update();
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
                    this.Dosomething((Action)delegate()
                    {
                        Color c = Color.FromArgb(Math.Abs(255 - this.bOutLineColor.R), Math.Abs(255 - this.bOutLineColor.G), Math.Abs(255 - this.bOutLineColor.B));

                        lineSymbol.Color = new RgbColorClass()
                        {
                            Red = c.R,
                            Green = c.G,
                            Blue = c.B
                        };
                        lineSymbol.Width = bWidth + 5;
                        fillSymbol.Outline = lineSymbol;
                    }, true);
                }
                else
                {
                    this.Dosomething((Action)delegate
                    {
                        lineSymbol.Color = new RgbColorClass() { Red = bOutLineColor.R, Green = bOutLineColor.B, Blue = bOutLineColor.B };
                        lineSymbol.Width = bWidth;
                        fillSymbol.Outline = lineSymbol;
                    }, true);
                }
            }

            base.Symbol = fillSymbol;
            Update();
            this.pHightLight = isHightLight;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash"></param>
        /// <param name="interval"></param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (pIsFlash == isFlash) return;//防止被多次调用
            pIsFlash = isFlash;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                fillSymbol.Color = new RgbColorClass()
                {
                    Red = bFillColor.R,
                    Green = bFillColor.B,
                    Blue = bFillColor.B
                };

                base.Symbol = fillSymbol;
                Update();
            }
        }

        /// <summary>
        /// 闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Color c = Color.FromArgb(Math.Abs(255 - this.bFillColor.R), Math.Abs(255 - this.bFillColor.G), Math.Abs(255 - this.bFillColor.B));

            if (!isTimer)
            {
                if ((this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).InvokeRequired)
                {
                    (this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).Invoke(new System.Action(
                        delegate
                        {
                            fillSymbol.Color = new RgbColorClass()
                            {
                                Red = c.R,
                                Green = c.G,
                                Blue = c.B
                            };
                        }));
                }
                else
                {
                    fillSymbol.Color = new RgbColorClass()
                    {
                        Red = c.R,
                        Green = c.G,
                        Blue = c.B
                    };
                }

            }
            else    // 停止闪烁
            {
                if ((this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).InvokeRequired)
                {
                    (this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxMapControl).Invoke(new System.Action(
                        delegate
                        {
                            fillSymbol.Color = new RgbColorClass()
                            {
                                Red = bFillColor.R,
                                Green = bFillColor.G,
                                Blue = bFillColor.B
                            };
                        }));
                }
                else
                {
                    fillSymbol.Color = new RgbColorClass()
                    {
                        Red = bFillColor.R,
                        Green = bFillColor.G,
                        Blue = bFillColor.B
                    };
                }
            }

            base.Symbol = fillSymbol;
            Update();

            isTimer = !isTimer;
        }

        /// <summary>
        /// 显示隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            ILayer layer = factoryArcMap.GetLayerByName(belongLayer.LayerName);
            CompositeGraphicsLayerClass graphLayer = layer as CompositeGraphicsLayerClass;
            this.isVisible = isVisible;
            if (isVisible)//显示
            {
                this.Dosomething((Action)delegate()
                {
                    graphLayer.AddElement(this, 1);
                }, true);
            }
            else
            {
                this.Dosomething((Action)delegate()
                {
                    graphLayer.DeleteElement(this);

                }, true);
            }

            Update();
            this.isVisible = isVisible;
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
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return elementName; }
            set { elementName = value; base.Name = elementName; }
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
            lineSymbol = null;
            fillSymbol = null;
            iSeg = null;
            pRing = null;
            pGeometry = null;
            pGeometryColl = null;
            pIsFlash = false;
            pHightLight = false;
            isVisible = false;
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
