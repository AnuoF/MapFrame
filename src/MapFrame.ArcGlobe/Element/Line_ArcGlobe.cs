/**************************************************************************
 * 类名：Line_ArcGlobe.cs
 * 描述：ArcGlobe的线
 * 作者：chenjing
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.GlobeCore;
using System.Drawing;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Element
{
    /// <summary>
    /// ArcGlobe的线
    /// </summary>
    class Line_ArcGlobe : LineElementClass, IMFLine
    {
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 地图对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 线符号
        /// </summary>
        private ISimpleLineSymbol pLineSymbol = null;
        /// <summary>
        /// 图层 
        /// </summary>
        private IGlobeGraphicsLayer graphcisLayer = null;
        /// <summary>
        /// 本图元的索引
        /// </summary>
        private int index = -1;
        /// <summary>
        /// 线的颜色
        /// </summary>
        private Color color;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 闪烁判定
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 图元是否可见true显示,false隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 图元是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 图元是否在闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 图元属性
        /// </summary>
        private IGlobeGraphicsElementProperties properties = null;

        #region  属性

        /// <summary>
        /// 图元索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// 图元是否栅格化
        /// </summary>
        public bool Rasterize
        {
            get
            {
                Dosomething((Action)delegate()
                {
                    graphcisLayer.GetGlobeProperties(this, out properties);
                }, true);
                return properties.Rasterize;
            }
            set
            {
                if (properties == null)
                {
                    Dosomething((Action)delegate()
                    {
                        graphcisLayer.GetGlobeProperties(this, out properties);
                    }, true);
                }
                properties.Rasterize = value;
                Dosomething((Action)delegate()
                {
                    graphcisLayer.SetGlobeProperties(this, properties);
                }, true);
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
        /// 图元名
        /// </summary>
        public string ElementName
        {
            get { return base.Name; }
            set
            {
                graphcisLayer.PutElementName(this, value);
                base.Name = value;
            }
        }

        /// <summary>
        /// 图层
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
        public ElementTypeEnum ElementType
        {
            get;
            set;
        }
        /// <summary>
        /// 图元颜色
        /// </summary>
        public Color ElementColor
        {
            set { this.color = value; }
        }

        /// <summary>
        /// 是否在高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightLight; }
        }

        /// <summary>
        /// 是否在闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 图元是否在显示隐藏
        /// </summary>
        public bool IsVisible
        {
            get
            {
                Dosomething((Action)delegate()
                {
                    graphcisLayer.GetIsElementVisible(this.index, out this.isVisible);
                }, true);
                return isVisible;
            }
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_graphcisLayer">图层</param>
        /// <param name="lineKml">线的kml</param>
        public Line_ArcGlobe(IGlobeGraphicsLayer _graphcisLayer, KmlLineString lineKml)
        {
            graphcisLayer = _graphcisLayer;

            this.ElementType = ElementTypeEnum.Line;//图元类型
            this.Description = lineKml.Description;//描述

            #region  位置

            IPolyline polyLine = new PolylineClass();
            IPointCollection pointCollection = polyLine as IPointCollection;
            IPoint p = new PointClass();
            foreach (var item in lineKml.PositionList)
            {
                p.PutCoords(item.Lng, item.Lat);
                p.Z = item.Alt;
                pointCollection.AddPoint(p);
            }
            IGeometry geometry = (IGeometry)pointCollection;
            (geometry as IZAware).ZAware = true;
            base.Geometry = (IGeometry)pointCollection;

            #endregion

            #region  符号

            pLineSymbol = new SimpleLineSymbolClass();
            if (lineKml.Color.ToArgb() == 0)                                                  //符号颜色
            {
                IColor color = new RgbColorClass()
                {
                    Transparency = Color.Red.A,
                    Red = Color.Red.R,
                    Green = Color.Red.G,
                    Blue = Color.Red.B
                };
                pLineSymbol.Color = color;
            }
            else
            {
                IColor color = new RgbColorClass()
                {
                    Transparency = lineKml.Color.A,
                    Red = lineKml.Color.R,
                    Green = lineKml.Color.G,
                    Blue = lineKml.Color.B
                };
                pLineSymbol.Color = color;
            }

            if (lineKml.Width == 0)                                                     //符号宽度
                lineKml.Width = 2;
            pLineSymbol.Width = lineKml.Width;
            pLineSymbol.Style = (esriSimpleLineStyle)lineKml.LineStyle;     //线的样式
            base.Symbol = pLineSymbol;

            #endregion

            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 1000;
        }

        #region  MapFrame.Core.Interface.ILine
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 设置线宽
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool SetWidth(float width)
        {
            pLineSymbol.Width = width;
            base.Symbol = pLineSymbol;
            this.Update();
            return true;
        }

        /// <summary>
        /// 设置线颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(Color color)
        {
            this.color = color;
            IColor c = new RgbColorClass()
            {
                Transparency = color.A,
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
            pLineSymbol.Color = c;
            Dosomething((Action)delegate()
            {
                base.Symbol = pLineSymbol;
            }, true);
            this.Update();
        }

        /// <summary>
        /// 设置线颜色
        /// </summary>
        /// <param name="argb">argb值</param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public void SetColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="a">透明度</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color color = Color.FromArgb(a, r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 更新线的位置
        /// </summary>
        /// <param name="pList">经纬度列表</param>
        /// <returns></returns>
        public bool UpdatePosition(List<MapLngLat> pList)
        {
            IPointCollection pointCollection = base.Geometry as IPointCollection;

            if (pointCollection != null)
            {
                pointCollection.RemovePoints(0, pointCollection.PointCount);   // 清空之前的点，再添加新点

                for (int i = 0; i < pList.Count; i++)
                {
                    IPoint point = new PointClass();
                    point.PutCoords(pList[i].Lng, pList[i].Lat);
                    point.Z = pList[i].Alt;
                    (point as IZAware).ZAware = true;
                    pointCollection.AddPoint(point);
                }

                Dosomething((Action)delegate()
                {
                    int outIndex = -1;
                    graphcisLayer.FindElementIndex(this, out outIndex);
                    if (outIndex > 0)
                    {
                        base.Geometry = pointCollection as IGeometry;
                        graphcisLayer.UpdateElementByIndex(outIndex);
                    }
                }, true);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取线的经纬度列表
        /// </summary>
        /// <returns></returns>
        public List<MapLngLat> GetLngLat()
        {
            IPointCollection pointCollection = base.Geometry as IPointCollection;
            List<MapLngLat> pList = new List<MapLngLat>();

            if (pointCollection != null)
            {
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    MapLngLat lnglat = new MapLngLat();
                    lnglat.Lng = pointCollection.get_Point(i).X;
                    lnglat.Lat = pointCollection.get_Point(i).Y;
                    pList.Add(lnglat);
                }
            }

            return pList;
        }

        /// <summary>
        /// 添加点  延长线
        /// </summary>
        /// <param name="lngLat"></param>
        public void AddPoint(MapLngLat lngLat)
        {
            IPointCollection pointCollection = base.Geometry as IPointCollection;

            if (pointCollection != null)
            {
                IPoint point = new PointClass();
                point.PutCoords(lngLat.Lng, lngLat.Lat);
                point.Z = lngLat.Alt;
                pointCollection.AddPoint(point);
                Dosomething((Action)delegate()
                {
                    base.Geometry = pointCollection as IGeometry;
                }, true);
            }
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        public void RemovePoint(MapLngLat lngLat)
        {
            IPointCollection pointCollection = base.Geometry as IPointCollection;
            if (pointCollection != null)
            {
                IPoint point = new PointClass() { X = lngLat.Lng, Y = lngLat.Lat, Z = lngLat.Alt };
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    if (pointCollection.get_Point(i).Compare(point) == 0)
                    {
                        pointCollection.RemovePoints(i, 1);
                        Dosomething((Action)delegate()
                        {
                            base.Geometry = pointCollection as IGeometry;
                        }, true);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">是否高亮</param>
        public void HightLight(bool isHightLight)
        {
            if (isHightLight)
            {
                this.Dosomething((Action)delegate()
                {
                    Color color = Color.FromArgb(Math.Abs(255 - this.color.R), Math.Abs(255 - this.color.G), Math.Abs(255 - this.color.B));
                    IRgbColor c = new RgbColorClass()
                    {
                        Transparency = color.A,
                        Red = color.R,
                        Green = color.G,
                        Blue = color.B
                    };
                    pLineSymbol.Color = c;
                    base.Symbol = pLineSymbol;
                }, true);
            }
            else
            {
                this.SetColor(this.color);
            }
            this.isHightLight = isHightLight;
            this.Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁事件</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;//防止被多次调用
            if (isFlash)
            {
                flashTimer.Interval = 1000;
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
        /// <param name="isVisible">是否显示</param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            Dosomething((Action)delegate()
            {
                graphcisLayer.PutIsElementVisible(index, isVisible);
            }, true);
            this.isVisible = isVisible;
            this.Update();
        }

        /// <summary>
        /// 获取线的距离
        /// </summary>
        /// <returns></returns>
        public double GetDistance()
        {
            return MapFrame.Core.Common.Utils.GetDistance(GetLngLat());
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            Dosomething((Action)delegate()
            {
                graphcisLayer.UpdateElementByIndex(this.index);
            }, true);
            if (this.layer != null)
            {
                this.layer.Refresh();
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
                flashTimer = null;
            }
            layer = null;
            mapControl = null;
            pLineSymbol = null;
            graphcisLayer = null;
            index = -1;
            isTimer = false;
            isVisible = true;
            isHightLight = false;
            isFlash = false;
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

        /// <summary>
        /// 闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Color c = Color.FromArgb(Math.Abs(255 - this.color.R), Math.Abs(255 - this.color.G), Math.Abs(255 - this.color.B));
            if (!isTimer)
            {
                if (mapControl.InvokeRequired)
                {
                    mapControl.BeginInvoke((Action)delegate
                    {
                        this.SetVisible(true);
                    });
                }
                else
                {
                    this.SetVisible(true);
                }
            }
            else    // 停止闪烁
            {
                if (mapControl.InvokeRequired)
                {
                    mapControl.BeginInvoke((Action)delegate
                    {
                        this.SetVisible(false);
                    });
                }
                else
                {
                    this.SetVisible(false);
                }
            }
            isTimer = !isTimer;
        }

    }
}
