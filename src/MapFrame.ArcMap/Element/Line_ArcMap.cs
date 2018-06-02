/**************************************************************************
 * 类名：LineElementClass.cs
 * 描述：arcgis线
 * 作者：cj
 * 日期：Aug 30,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Display;
using System.Drawing;
using System.Timers;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Factory;

namespace MapFrame.ArcMap.Element
{
    /// <summary>
    /// arcgis线
    /// </summary>
    class Line_ArcMap : LineElementClass, IElement, IMFLine
    {
        /// <summary>
        /// 记录线的颜色
        /// </summary>
        private System.Drawing.Color outLineColor;
        /// <summary>
        /// 记录线的宽度
        /// </summary>
        private float outLineWidth;
        /// <summary>
        /// 线符号
        /// </summary>
        private ISimpleLineSymbol lineSymbol = null;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 显示隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFalsh = false;
        /// <summary>
        /// 计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 图元类别
        /// </summary>
        private Core.Model.ElementTypeEnum elementType;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private IPointCollection pointCollection = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        List<Core.Model.MapLngLat> pointList = null;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private FactoryArcMap mapFactory = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        /// <param name="kmlLine">线</param>
        /// <param name="_mapFactory">地图工厂</param>
        public Line_ArcMap(AxMapControl _mapControl, KmlLineString kmlLine, FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;

            Dosomething(new Action(delegate
            {
                lineSymbol = new SimpleLineSymbolClass();
                IColor color = new RgbColorClass()
                {
                    Transparency = kmlLine.Color.A,
                    Red = kmlLine.Color.R,
                    Green = kmlLine.Color.G,
                    Blue = kmlLine.Color.B
                };
                lineSymbol.Color = color;
                lineSymbol.Width = kmlLine.Width;
                lineSymbol.Style = esriSimpleLineStyle.esriSLSDash;
                IPolyline polyLine = new PolylineClass();
                pointCollection = polyLine as IPointCollection;
                IPoint p = new PointClass();
                foreach (var item in kmlLine.PositionList)
                {
                    p.PutCoords(item.Lng, item.Lat);
                    p.Z = item.Alt;
                    pointCollection.AddPoint(p);
                }
                base.Symbol = lineSymbol;
                base.Geometry = (IGeometry)pointCollection;
            }), true);

            //记录颜色、宽度、坐标集合
            outLineColor = kmlLine.Color;
            outLineWidth = kmlLine.Width;
            pointList = new List<MapLngLat>();
            pointList = kmlLine.PositionList;

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

        #region  MapFrame.Core.Interface.ILineArcMap
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 图元所属图层
        /// </summary>
        private Core.Interface.IMFLayer belongLayer;

        /// <summary>
        /// 所属图层
        /// </summary>
        public MapFrame.Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set
            {
                belongLayer = value;
                mapControl = value.MapControl as AxMapControl;
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
        /// 设置线宽
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool SetWidth(float width)
        {
            outLineWidth = width;
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke((Action)delegate()
                {
                    lineSymbol.Width = width;
                    Update();
                });
            }
            else
            {
                lineSymbol.Width = width;
                Update();
            }
            return true;
        }

        /// <summary>
        /// 设置线颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(System.Drawing.Color color)
        {
            outLineColor = color;
            //颜色转化
            IColor lineColor = new RgbColorClass()
            {
                Transparency = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke((Action)delegate()
                {
                    lineSymbol.Color = lineColor;
                    //base.Symbol = lineSymbol;
                    Update();
                });
            }
            else
            {
                lineSymbol.Color = lineColor;
                //base.Symbol = lineSymbol;
                Update();
            }
        }

        /// <summary>
        /// 设置线颜色
        /// </summary>
        /// <param name="argb">颜色的argb值</param>
        public void SetColor(int argb)
        {
            //颜色转化
            System.Drawing.Color color = System.Drawing.Color.FromArgb(argb);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r">R值</param>
        /// <param name="g">G值</param>
        /// <param name="b">B值</param>
        public void SetColor(int r, int g, int b)
        {
            //颜色转化
            System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a">透明度值</param>
        /// <param name="r">R值</param>
        /// <param name="g">G值</param>
        /// <param name="b">B值</param>
        public void SetColor(int a, int r, int g, int b)
        {
            //颜色转化
            System.Drawing.Color color = System.Drawing.Color.FromArgb(a, r, g, b);
            SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color">颜色</param>
        private void SetColor(IColor color)
        {
            if (color.RGB == 0) return;
            outLineColor = System.Drawing.ColorTranslator.FromOle(color.RGB);

            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke((Action)delegate()
                {
                    lineSymbol.Color = color;
                    Update();
                });
            }
            else
            {
                lineSymbol.Color = color;
                Update();
            };

        }

        /// <summary>
        /// 更新线位置
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public bool UpdatePosition(List<Core.Model.MapLngLat> pList)
        {
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke((Action)delegate()
                {
                    IPoint p = new PointClass();
                    pointCollection.RemovePoints(0, pointCollection.PointCount);
                    for (int i = 0; i < pList.Count; i++)
                    {
                        p.PutCoords(pList[i].Lng, pList[i].Lat);
                        pointCollection.AddPoint(p);
                    }

                    base.Geometry = pointCollection as IGeometry;
                });
            }
            else
            {
                IPoint p = new PointClass();
                pointCollection.RemovePoints(0, pointCollection.PointCount);
                for (int i = 0; i < pList.Count; i++)
                {
                    p.PutCoords(pList[i].Lng, pList[i].Lat);
                    pointCollection.AddPoint(p);
                }

                base.Geometry = pointCollection as IGeometry;
            }
            Update();

            pointList = pList;//更新坐标集合
            return true;
        }

        /// <summary>
        /// 获取线的经纬列表
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
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 线的名字
        /// </summary>
        public string ElementName
        {
            get { return base.Name; }
            set { base.Name = value; base.Name = value; }
        }

        /// <summary>
        /// 图元类别
        /// </summary>
        public Core.Model.ElementTypeEnum ElementType
        {
            get { return ElementTypeEnum.Line; }
            set { elementType = value; }
        }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
        }

        /// <summary>
        /// 是否高亮了
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightLight; }
        }

        /// <summary>
        /// 目标高亮
        /// </summary>
        /// <param name="_isHightLight">是否高亮</param>
        public void HightLight(bool _isHightLight)
        {
            lock (lockObj)
            {
                if (mapControl.InvokeRequired)
                {
                    mapControl.Invoke((Action)delegate()
                    {
                        if (_isHightLight)//高亮
                        {
                            Color c = Color.FromArgb(Math.Abs(255 - this.outLineColor.R), Math.Abs(255 - this.outLineColor.G), Math.Abs(255 - this.outLineColor.B));

                            lineSymbol.Color = new RgbColorClass()
                            {
                                Red = c.R,
                                Green = c.G,
                                Blue = c.B
                            };
                            lineSymbol.Width = outLineWidth + 3;

                        }
                        else //不高亮
                        {
                            lineSymbol.Color = new RgbColorClass()
                            {
                                Red = outLineColor.R,
                                Blue = outLineColor.B,
                                Green = outLineColor.G
                            };

                            lineSymbol.Width = outLineWidth;
                        }
                        Update();
                    });
                }
                else
                {
                    if (_isHightLight)//高亮
                    {
                        Color c = Color.FromArgb(Math.Abs(255 - this.outLineColor.R), Math.Abs(255 - this.outLineColor.G), Math.Abs(255 - this.outLineColor.B));

                        lineSymbol.Color = new RgbColorClass()
                        {
                            Red = c.R,
                            Green = c.G,
                            Blue = c.B
                        };
                        lineSymbol.Width = outLineWidth + 3;

                    }
                    else //不高亮
                    {
                        lineSymbol.Color = new RgbColorClass()
                        {
                            Red = outLineColor.R,
                            Blue = outLineColor.B,
                            Green = outLineColor.G
                        };

                        lineSymbol.Width = outLineWidth;
                    }
                    Update();
                }

            }
            this.isHightLight = _isHightLight;
        }

        /// <summary>
        /// 是否闪烁了
        /// </summary>
        public bool IsFlash
        {
            get { return isFalsh; }
        }

        /// <summary>
        /// 目标闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁时长</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFalsh == isFlash) return;
            this.isFalsh = isFlash;
            if (isFalsh)
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
        private void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.isVisible = !isVisible;
            this.SetVisible(isVisible);
        }

        /// <summary>
        /// 显示图元
        /// </summary>
        /// <param name="isVisible">是否显示</param>
        public void SetVisible(bool isVisible)
        {
            ILayer layer = mapFactory.GetLayerByName(belongLayer.LayerName);
            CompositeGraphicsLayerClass graphLayer = layer as CompositeGraphicsLayerClass;
            this.Dosomething((Action)delegate()
            {
                if (isVisible)//显示
                {
                    graphLayer.AddElement(this, 1);
                }
                else
                {
                    graphLayer.DeleteElement(this);
                }
            }, true);

            Update();
            this.isVisible = isVisible;
        }

        /// <summary>
        /// 获取线的长度(单位：千米)
        /// </summary>
        /// <returns></returns>
        public double GetDistance()
        {
            if (GetLngLat() != null)
            {
                return MapFrame.Core.Common.Utils.CalculateLineLength(GetLngLat());
            }
            else
                return 0;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            this.Dosomething((Action)delegate()
            {
                base.Symbol = lineSymbol;
            }, true);

            if (this.BelongLayer != null)
                this.BelongLayer.Refresh();
            return;
        }

        /// <summary>
        /// 释放该类
        /// </summary>
        public void Dispose()
        {
            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
            }
            isHightLight = false;
            isFalsh = false;
            isTimer = false;
            lineSymbol = null;
            isVisible = true;
            pointCollection = null;
            if (pointList != null)
            {
                pointList.Clear();
                pointList = null;
            }
        }
        #endregion

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="lngLat">地图点</param>
        public void AddPoint(MapLngLat lngLat)
        {
            this.Dosomething((Action)delegate()
           {
               IPoint p = new PointClass();
               p.PutCoords(lngLat.Lng, lngLat.Lat);
               p.Z = lngLat.Alt;

               pointCollection.AddPoint(p);
               base.Geometry = (IGeometry)pointCollection;
           }, true);

            Update();
            pointList.Add(lngLat);//添加到坐标集合
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lngLat"></param>
        /// <returns></returns>
        public void RemovePoint(MapLngLat lngLat)
        {
            this.Dosomething((Action)delegate()
          {
              IPoint p = new PointClass() { X = lngLat.Lng, Y = lngLat.Lat, Z = lngLat.Alt };
              for (int i = 0; i < pointCollection.PointCount; i++)
              {
                  if (pointCollection.get_Point(i).Compare(p) == 0)
                  {
                      pointCollection.RemovePoints(i, 1);
                      base.Geometry = pointCollection as IGeometry;
                      pointList.Remove(lngLat);//移除坐标点集合
                      break;
                  }
              }
          }, true);

            Update();
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
