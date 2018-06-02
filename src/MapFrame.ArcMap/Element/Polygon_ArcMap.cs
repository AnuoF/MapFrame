
/**************************************************************************
 * 类名：Polygon_ArcMap.cs
 * 描述：ArcGis面类
 * 作者：lx
 * 日期：Aug 23,2016
 * 
 * ************************************************************************/
using System;
using System.Collections.Generic;
using System.Timers;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using MapFrame.Core.Model;
using ESRI.ArcGIS.esriSystem;
using System.Drawing;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Factory;

namespace MapFrame.ArcMap.Element
{
    /// <summary>
    /// ArcGis面类
    /// </summary>
    class Polygon_ArcMap : PolygonElementClass, IElement, IMFPolygon
    {
        private AxMapControl mapControl;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 开始闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string _name;
        ///// <summary>
        ///// 填充色的RGB值
        ///// </summary>
        //private int fillColor;
        ///// <summary>
        ///// 轮廓色的RGB值
        ///// </summary>
        //private int outLineColor;
        /// <summary>
        /// 轮廓颜色
        /// </summary>
        private System.Drawing.Color outLineColor;
        /// <summary>
        /// 填充色
        /// </summary>
        private System.Drawing.Color fillColor;
        /// <summary>
        /// 轮廓宽度
        /// </summary>
        private float outLineWidth;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 是否隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 多边形坐标点集合
        /// </summary>
        private List<MapLngLat> pointList;
        /// <summary>
        /// ArcMap工厂
        /// </summary>
        private FactoryArcMap mapFactory = null;

        #region 多边形图元
        /// <summary>
        /// 轮廓线符号
        /// </summary>
        private ILineSymbol lineSymbol;
        /// <summary>
        /// 填充符号
        /// </summary>
        private IFillSymbol fillSymbol;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private IPointCollection pointCollection;
        /// <summary>
        /// 多边形
        /// </summary>
        private ESRI.ArcGIS.Geometry.IPolygon polygon;
        #endregion

        /// <summary>
        /// 面的构造函数
        /// </summary>
        /// <param name="_layer">图层</param>
        /// <param name="kmlPolygon">图元（kml）</param>
        /// <param name="mapfac">地图工厂</param>
        public Polygon_ArcMap(AxMapControl _mapcontrol, KmlPolygon kmlPolygon, FactoryArcMap _mapFac)
        {
            this.mapControl = _mapcontrol;
            mapFactory = _mapFac;

            Dosomething((Action)(delegate
            {
                lineSymbol = new SimpleLineSymbolClass();
                //轮廓颜色
                IColor outlineColor = new RgbColorClass()
                {
                    Red = kmlPolygon.OutLineColor.R,
                    Green = kmlPolygon.OutLineColor.G,
                    Blue = kmlPolygon.OutLineColor.B
                };
                lineSymbol.Color = outlineColor;
                lineSymbol.Width = kmlPolygon.OutLineSize;
                //存储颜色和宽度
                outLineColor = kmlPolygon.OutLineColor;
                outLineWidth = kmlPolygon.OutLineSize;

                fillSymbol = new SimpleFillSymbol();
                fillSymbol.Outline = lineSymbol;
                //填充色
                IColor c = new RgbColorClass()
                {
                    Red = kmlPolygon.FillColor.R,
                    Green = kmlPolygon.FillColor.G,
                    Blue = kmlPolygon.FillColor.B
                };
                fillSymbol.Color = c;
                this.fillColor = kmlPolygon.FillColor;

                base.Symbol = fillSymbol;
                polygon = new PolygonClass();
                pointCollection = polygon as IPointCollection;
                foreach (var lngLat in kmlPolygon.PositionList)
                {
                    pointCollection.AddPoint(new PointClass() { X = lngLat.Lng, Y = lngLat.Lat });
                }
                base.Geometry = (IGeometry)pointCollection;

                pointList = new List<MapLngLat>();//坐标点集合
                pointList = kmlPolygon.PositionList;

                this.Description = kmlPolygon.Description;
            }), true);

            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);
        }

        #region IElement
        /// <summary>
        /// 透明度(0-100)
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

        #region MapFrame.Core.Interface.IPolygonArcMap
        public object Tag
        {
            get;
            set;
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
        public MapFrame.Core.Interface.IMFLayer BelongLayer
        {
            get;
            set;
        }

        /// <summary>
        /// 轮廓线颜色的RGB值
        /// </summary>
        public Color OutLineColor
        {
            get { return outLineColor; }
        }

        /// <summary>
        /// 填充色的RGB值
        /// </summary>
        public Color FillColor
        {
            get { return fillColor; }
        }


        /// <summary>
        /// 设置轮廓线颜色
        /// </summary>
        /// <param name="outlineColor">轮廓线颜色的RGB值</param>
        /// <returns></returns>
        public bool SetOutLineColor(int outlineColor)
        {
            outLineColor = System.Drawing.Color.FromArgb(outlineColor);

            Dosomething((Action)(delegate
            {
                lineSymbol.Color = new RgbColorClass() { Red = outLineColor.R, Green = outLineColor.G, Blue = outLineColor.B };
                this.fillSymbol.Outline = lineSymbol;
                base.Symbol = fillSymbol;

            }), true);

            Update();

            return true;
        }

        /// <summary>
        /// 设置轮廓线颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public bool SetOutLineColor(System.Drawing.Color color)
        {
            this.outLineColor = color;

            Dosomething((Action)(delegate
            {
                this.lineSymbol.Color = new RgbColorClass() { Red = outLineColor.R, Green = outLineColor.G, Blue = outLineColor.B };
                fillSymbol.Outline = lineSymbol;
                base.Symbol = fillSymbol;
            }), true);

            Update();

            return true;
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        /// <param name="fillColor">填充色的RGB值</param>
        /// <returns></returns>
        public bool SetFillColor(int fillColor)
        {
            this.fillColor = System.Drawing.Color.FromArgb(fillColor);

            Dosomething((Action)(delegate
            {
                fillSymbol.Color = new RgbColorClass() { Red = this.fillColor.R, Green = this.fillColor.G, Blue = this.fillColor.B };
                base.Symbol = fillSymbol;
            }), true);

            Update();

            return true;
        }

        /// <summary>
        /// 填充颜色
        /// </summary>
        /// <param name="color">填充色</param>
        /// <returns></returns>
        public bool SetFillColor(System.Drawing.Color color)
        {
            Dosomething((Action)(delegate
            {
                fillSymbol.Color = new RgbColorClass() { Red = fillColor.R, Green = fillColor.G, Blue = fillColor.B };
                base.Symbol = fillSymbol;

            }), true);

            fillColor = color;
            Update();

            return true;
        }

        /// <summary>
        /// 设置轮廓线的宽度
        /// </summary>
        /// <param name="size">宽度值</param>
        /// <returns></returns>
        public bool SetOutLineSize(float size)
        {
            outLineWidth = size;

            Dosomething((Action)(delegate
            {
                lineSymbol.Width = outLineWidth;
                fillSymbol.Outline = lineSymbol;
                base.Symbol = fillSymbol;

            }), true);

            Update();

            return true;
        }

        /// <summary>
        /// 更新多边形的某一点坐标
        /// </summary>
        /// <param name="oldLngLat">老坐标</param>
        /// <param name="newLngLat">新坐标</param>
        /// <returns></returns>
        public bool UpdatePosition(Core.Model.MapLngLat oldLngLat, Core.Model.MapLngLat newLngLat)
        {
            Dosomething((Action)(delegate
            {
                IPoint oldPoint = new PointClass() { X = oldLngLat.Lng, Y = oldLngLat.Lat };
                IPoint newPoint = new PointClass() { X = newLngLat.Lng, Y = newLngLat.Lat };
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    if (pointCollection.get_Point(i).Compare(oldPoint) == 0)
                    {
                        pointCollection.RemovePoints(i, 1);
                        pointCollection.AddPoint(newPoint);
                        //坐标点集合
                        pointList.Remove(oldLngLat);
                        pointList.Add(newLngLat);
                        break;
                    }
                }
                base.Geometry = (IGeometry)pointCollection;
            }), true);

            Update();

            return true;
        }

        /// <summary>
        /// 更新坐标点集合
        /// </summary>
        /// <param name="pList">坐标点集合</param>
        /// <returns></returns>
        public bool UpdatePosition(List<Core.Model.MapLngLat> pList)
        {
            Dosomething((Action)(delegate
            {
                IPolygon polygon = new PolygonClass();
                IPoint newPoint = new PointClass();
                IPointCollection newPointCollection = polygon as IPointCollection;
                foreach (var p in pList)
                {
                    newPoint.PutCoords(p.Lng, p.Lat);
                    newPointCollection.AddPoint(newPoint);
                }
                pointCollection.SetPointCollection(newPointCollection);
                base.Geometry = (IGeometry)pointCollection;
            }), true);

            Update();
            pointList = pList;

            return true;
        }

        /// <summary>
        /// 获取面的顶点位置集合
        /// </summary>
        /// <returns></returns>
        public List<Core.Model.MapLngLat> GetLngLat()
        {
            int count = pointCollection.PointCount;
            List<Core.Model.MapLngLat> lnglatList = new List<MapLngLat>();
            Core.Model.MapLngLat lnglat;
            IPoint point = new PointClass();

            for (int i = 0; i < count; i++)
            {
                point = pointCollection.get_Point(i);
                lnglat = new MapLngLat(point.X, point.Y);
                lnglatList.Add(lnglat);
            }

            return lnglatList;
        }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="lngLat"></param>
        public void AddPoint(MapLngLat lngLat)
        {
            Dosomething((Action)(delegate
            {
                ESRI.ArcGIS.Geometry.IPoint newPoint = new ESRI.ArcGIS.Geometry.PointClass() { X = lngLat.Lng, Y = lngLat.Lat };
                pointCollection.AddPoint(newPoint);
                base.Geometry = (IGeometry)pointCollection;
            }), true);

            Update();
            pointList.Add(lngLat);//坐标集合
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lngLat"></param>
        /// <returns></returns>
        public void RemovePoint(MapLngLat lngLat)
        {
            Dosomething((Action)(delegate
            {
                ESRI.ArcGIS.Geometry.IPoint newPoint = new ESRI.ArcGIS.Geometry.PointClass() { X = lngLat.Lng, Y = lngLat.Lat };
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    if (pointCollection.get_Point(i).Compare(newPoint) == 0)
                    {
                        pointCollection.RemovePoints(i, 1);

                        pointList.Remove(lngLat);
                        break;
                    }
                }
                base.Geometry = (IGeometry)pointCollection;

                Update();
            }), true);
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
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return _name; }
            set { _name = value; base.Name = value; }
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

        public bool IsFlash
        {
            get { return isFlash; }
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
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            Dosomething((Action)(delegate
            {
                lock (lockObj)
                {
                    if (isHightLight)
                    {
                        Color c = Color.FromArgb(Math.Abs(255 - this.outLineColor.R), Math.Abs(255 - this.outLineColor.G), Math.Abs(255 - this.outLineColor.B));

                        lineSymbol.Color = new RgbColorClass()
                        {
                            Red = c.R,
                            Green = c.G,
                            Blue = c.B
                        };
                        lineSymbol.Width = outLineWidth + 5;
                        fillSymbol.Outline = lineSymbol;
                        base.Symbol = fillSymbol;
                    }
                    else
                    {
                        SetOutLineColor(outLineColor);
                        SetOutLineSize(outLineWidth);
                    }
                }
            }), true);

            Update();
            this.isHightLight = isHightLight;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔时间，默认1000毫秒</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;

            Dosomething((Action)(delegate
            {
                if (isFlash)
                {
                    flashTimer.Interval = interval;
                    flashTimer.Start();
                }
                else
                {
                    flashTimer.Stop();
                    fillSymbol.Color = new RgbColorClass() { Red = fillColor.R, Green = fillColor.G, Blue = fillColor.B };
                    base.Symbol = fillSymbol;
                    Update();
                }
            }), true);

            this.isFlash = isFlash;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Color c = Color.FromArgb(Math.Abs(255 - this.fillColor.R), Math.Abs(255 - this.fillColor.G), Math.Abs(255 - this.fillColor.B));

            Dosomething((Action)(delegate
            {
                if (!isTimer)
                {
                    fillSymbol.Color = new RgbColorClass()
                    {
                        Red = c.R,
                        Green = c.G,
                        Blue = c.B
                    };
                }
                else
                {
                    SetFillColor(fillColor);
                }

                base.Symbol = fillSymbol;
            }), true);

            Update();
            isTimer = !isTimer;
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            ILayer layer = mapFactory.GetLayerByName(this.BelongLayer.LayerName);
            CompositeGraphicsLayerClass graphLayer = layer as CompositeGraphicsLayerClass;
            if (graphLayer == null) return;

            Dosomething((Action)(delegate
            {
                if (isVisible)//显示
                {
                    graphLayer.AddElement(this, 1);
                }
                else
                {
                    graphLayer.DeleteElement(this);
                }

                Update();
            }), true);

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
            pointCollection = null;
            polygon = null;
            this.isHightLight = false;
            isVisible = true;
            if (pointList != null)
            {
                pointList.Clear();
                pointList = null;
            }
        }
        #endregion


        /// <summary>
        /// 计算面积
        /// </summary>
        /// <returns></returns>
        private double CalculateArea()
        {
            double result = 0;
            IPolygon areaPolygon = pointCollection as IPolygon;
            if (areaPolygon != null)
            {
                areaPolygon.Close();
                IGeometry areaGeometry = areaPolygon as IGeometry;
                ITopologicalOperator topo = areaGeometry as ITopologicalOperator;
                topo.Simplify();
                areaGeometry.Project((this.BelongLayer.MapControl as AxMapControl).Map.SpatialReference);
                IArea area = areaGeometry as IArea;
                result = area.Area;
            }
            return result;
        }

        /// <summary>
        /// 获取多边形面积(面积：万平方千米)
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            return CalculateArea();
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
