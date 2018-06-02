/**************************************************************************
 * 类名：Polygon_ArcGlobe.cs
 * 描述：arcglobe多边形
 * 作者：chenjing
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Display;
using System.Drawing;
using ESRI.ArcGIS.Geometry;
using System;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Element
{
    /// <summary>
    /// arcglobe多边形
    /// </summary>
    class Polygon_ArcGlobe : PolygonElementClass, IMFPolygon
    {
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IGlobeGraphicsLayer graphicsLayer = null;
        /// <summary>
        /// 面填充符号
        /// </summary>
        private ISimpleFillSymbol simpleFillSymbol = null;

        /// <summary>
        /// 面轮廓符号
        /// </summary>
        private ISimpleLineSymbol simpleLineSymbol = null;
        /// <summary>
        /// 本图元索引
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
        /// 图元是否在高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 图元是否在闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 是否隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 闪烁判定
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 填充颜色
        /// </summary>
        private Color fillColor;
        /// <summary>
        /// 轮廓颜色
        /// </summary>
        private Color outlineColor;
        /// <summary>
        /// 轮廓大小
        /// </summary>
        private float outLineSize;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 图元属性
        /// </summary>
        private IGlobeGraphicsElementProperties properties = null;
        /// <summary>
        /// 透明度
        /// </summary>
        private byte opacity = 255;

        /// <summary>
        /// 图元栅格化
        /// </summary>
        public bool Rasterize
        {
            get
            {
                this.Dosomething((Action)delegate()
                {
                    graphicsLayer.GetGlobeProperties(this, out properties);

                }, true);
                return properties.Rasterize;
            }
            set
            {
                if (properties == null)
                {
                    this.Dosomething((Action)delegate()
                    {
                        graphicsLayer.GetGlobeProperties(this, out properties);

                    }, true);
                }
                properties.Rasterize = value;
                this.Dosomething((Action)delegate()
                {
                    graphicsLayer.SetGlobeProperties(this, properties);
                }, true);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_graphicsLayer">图层</param>
        /// <parparam name="polygonKml">面的kml</parparam>
        public Polygon_ArcGlobe(IGlobeGraphicsLayer _graphicsLayer, KmlPolygon polygonKml)
        {
            this.graphicsLayer = _graphicsLayer;

            this.ElementType = ElementTypeEnum.Polygon;           //图元类型
            this.Description = polygonKml.Description;            //图元描述

            #region  符号

            simpleFillSymbol = new SimpleFillSymbolClass();
            simpleLineSymbol = new SimpleLineSymbolClass();

            IRgbColor fillColor = new RgbColorClass();
            fillColor.Transparency = polygonKml.FillColor.A;
            fillColor.Red = polygonKml.FillColor.R;
            fillColor.Green = polygonKml.FillColor.G;
            fillColor.Blue = polygonKml.FillColor.B;
            this.opacity = polygonKml.FillColor.A;

            IRgbColor outlineColor = new RgbColorClass();
            outlineColor.Transparency = polygonKml.OutLineColor.A;
            outlineColor.Red = polygonKml.OutLineColor.R;
            outlineColor.Green = polygonKml.OutLineColor.G;
            outlineColor.Blue = polygonKml.OutLineColor.B;

            simpleLineSymbol.Color = outlineColor;
            simpleLineSymbol.Width = polygonKml.OutLineSize;
            simpleFillSymbol.Color = fillColor;
            simpleFillSymbol.Outline = simpleLineSymbol;

            this.fillColor = polygonKml.FillColor;
            this.outlineColor = polygonKml.OutLineColor;
            this.outLineSize = polygonKml.OutLineSize;

            #endregion

            #region  位置

            IGeometry geometry = new PolygonClass();
            IPointCollection pointCollection = geometry as IPointCollection;
            IPoint point = new PointClass();
            foreach (var item in polygonKml.PositionList)
            {
                point.PutCoords(item.Lng, item.Lat);
                point.Z = item.Alt;
                pointCollection.AddPoint(point);
            }

            (pointCollection as IZAware).ZAware = true;

            #endregion

            base.Geometry = pointCollection as IGeometry;           //指定位置
            base.Symbol = simpleFillSymbol;                         //指定符号

            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }

        #region  MapFrame.Core.Interface.IPolygonArcGlobe

        #region 属性

        /// <summary>
        /// 轮廓色
        /// </summary>
        public Color OutLineColor
        {
            get
            {
                return outlineColor;
            }
        }

        /// <summary>
        /// 填充色
        /// </summary>
        public Color FillColor
        {
            get
            {
                return fillColor;
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
        /// 图元所在的图层
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
                }, false);
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
        /// 图元是否在高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return this.isHightLight; }
        }

        /// <summary>
        /// 是否在闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="lnglat"></param>
        public void AddPoint(MapLngLat lnglat)
        {
            this.Dosomething((Action)delegate()
            {
                IPointCollection pointCollection = base.Geometry as IPointCollection;
                IPoint point = new PointClass();
                point.PutCoords(lnglat.Lng, lnglat.Lat);
                point.Z = lnglat.Alt;
                pointCollection.AddPoint(point);
                (pointCollection as IZAware).ZAware = true;
                if (this.Rasterize)
                {
                    this.Rasterize = false;
                    base.Geometry = pointCollection as IGeometry;
                    this.Rasterize = true;
                }
                else
                {
                    base.Geometry = pointCollection as IGeometry;
                }

                Update();

            }, true);
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lnglat"></param>
        public void RemovePoint(MapLngLat lnglat)
        {
            this.Dosomething((Action)delegate()
            {
                IPointCollection pointCollection = base.Geometry as IPointCollection;
                IPoint point = new PointClass();
                point.PutCoords(lnglat.Lng, lnglat.Lat);
                point.Z = lnglat.Alt;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    if (pointCollection.get_Point(i).Compare(point) == 0)
                    {
                        pointCollection.RemovePoints(i, 1);
                        break;
                    }
                }
                base.Geometry = pointCollection as IGeometry;

                Update();

            }, true);
        }

        /// <summary>
        /// 设置轮廓颜色
        /// </summary>
        /// <param name="outlineColor">argb值</param>
        /// <returns></returns>
        public bool SetOutLineColor(int outlineColor)
        {
            Color color = Color.FromArgb(outlineColor);
            SetOutLineColor(color);
            return true;
        }

        /// <summary>
        /// 设置轮廓颜色
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public bool SetOutLineColor(Color color)
        {
            outlineColor = color;

            this.Dosomething((Action)delegate()
            {
                IRgbColor c = new RgbColorClass();
                c.Transparency = color.A;
                c.Red = color.R;
                c.Green = color.G;
                c.Blue = color.B;
                simpleLineSymbol.Color = c;
                simpleFillSymbol.Outline = simpleLineSymbol;
                base.Symbol = simpleFillSymbol;

                Update();

            }, true);

            return true;
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor">argb值</param>
        /// <returns></returns>
        public bool SetFillColor(int fillColor)
        {
            Color color = Color.FromArgb(fillColor);
            SetFillColor(color);
            return true;
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool SetFillColor(Color color)
        {
            fillColor = color;

            this.Dosomething((Action)delegate()
            {
                IRgbColor c = new RgbColorClass();
                c.Transparency = this.opacity;
                c.Red = color.R;
                c.Green = color.G;
                c.Blue = color.B;
                simpleFillSymbol.Color = c;
                base.Symbol = simpleFillSymbol;

                Update();

            }, true);

            return true;
        }

        /// <summary>
        /// 设置轮廓线大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetOutLineSize(float size)
        {
            simpleLineSymbol.Width = size;
            simpleFillSymbol.Outline = simpleLineSymbol;
            outLineSize = size;

            this.Dosomething((Action)delegate()
            {
                base.Symbol = simpleFillSymbol;

                Update();

            }, true);

            return true;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="oldLngLat">原来的位置</param>
        /// <param name="newLngLat">新的位置</param>
        /// <returns></returns>
        public bool UpdatePosition(MapLngLat oldLngLat, MapLngLat newLngLat)
        {
            this.Dosomething((Action)delegate()
            {
                IPoint newPoint = new PointClass();
                newPoint.PutCoords(newLngLat.Lng, newLngLat.Lat);
                newPoint.Z = newLngLat.Alt;
                IPoint oldPoint = new PointClass() { X = oldLngLat.Lng, Y = oldLngLat.Lat, Z = oldLngLat.Alt };
                IPointCollection pointCollection = base.Geometry as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    if (pointCollection.get_Point(i).Compare(oldPoint) == 0)
                    {
                        pointCollection.RemovePoints(i, 1);
                        pointCollection.AddPoint(newPoint);
                        break;
                    }
                }
                base.Geometry = pointCollection as IGeometry;

                Update();

            }, true);

            return true;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="pList">点集合</param>
        /// <returns></returns>
        public bool UpdatePosition(List<MapLngLat> pList)
        {
            IPointCollection pointCollection = base.Geometry as IPointCollection;
            if (pointCollection == null) return false;

            this.Dosomething((Action)delegate()
            {
                pointCollection.RemovePoints(0, pointCollection.PointCount);//清除集合内所有坐标点
                IPoint p = new PointClass();
                foreach (var item in pList)
                {
                    p.PutCoords(item.Lng, item.Lat);
                    p.Z = item.Alt;
                    pointCollection.AddPoint(p);
                }
                if (this.Rasterize) //判断图元是否为栅格化
                {
                    this.Rasterize = false;
                    base.Geometry = pointCollection as IGeometry;
                    this.Rasterize = true;
                }
                else
                    base.Geometry = pointCollection as IGeometry;

                Update();

            }, true);

            return true;
        }

        /// <summary>
        /// 获取多边形的经纬度集合
        /// </summary>
        /// <returns></returns>
        public List<MapLngLat> GetLngLat()
        {
            IPointCollection pointCollection = base.Geometry as IPointCollection;
            List<MapLngLat> lnglatList = new List<MapLngLat>();

            for (int i = 0; i < pointCollection.PointCount; i++)
            {
                MapLngLat lnglat = new MapLngLat(pointCollection.get_Point(i).X, pointCollection.get_Point(i).Y, pointCollection.get_Point(i).Z);
                lnglatList.Add(lnglat);
            }

            return lnglatList;
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">tru高亮,false不高亮</param>
        public void HightLight(bool isHightLight)
        {
            if (this.isHightLight == isHightLight) return;
            this.isHightLight = isHightLight;

            lock (lockObj)
            {
                if (isHightLight)
                {
                    this.Dosomething((Action)delegate()
                    {
                        Color c = Color.FromArgb(Math.Abs(255 - this.fillColor.R), Math.Abs(255 - this.fillColor.G), Math.Abs(255 - this.fillColor.B));
                        simpleFillSymbol.Color = new RgbColorClass()
                        {
                            Red = c.R,
                            Green = c.G,
                            Blue = c.B
                        };
                        base.Symbol = simpleFillSymbol;

                        Update();
                    }, false);
                }
                else
                {
                    SetFillColor(fillColor);
                }
            }
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;//防止被多次调用
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
            this.isFlash = isFlash;
        }

        /// <summary>
        /// 显示/隐藏
        /// </summary>
        /// <param name="isVisible">是否显示</param>
        public void SetVisible(bool isVisible)
        {
            this.isVisible = isVisible;
            this.Dosomething((Action)delegate()
            {
                graphicsLayer.PutIsElementVisible(index, isVisible);
            }, true);
            Update();
            //mapControl.GlobeDisplay.RefreshViewers();
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity"></param>
        public void SetOpacity(int _opacity)
        {
            opacity = (byte)_opacity;

            IRgbColor color = new RgbColorClass();
            color.Transparency = (byte)_opacity;
            color.Red = fillColor.R;
            color.Green = fillColor.G;
            color.Blue = fillColor.B;

            Dosomething((Action)delegate()
            {
                simpleFillSymbol.Color = color;
                base.Symbol = simpleFillSymbol;
            }, true);
        }

        /// <summary>
        /// 获取多边形面积
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            return MapFrame.Core.Common.Utils.GetPolygonArea(this.GetLngLat());
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            graphicsLayer.UpdateElementByIndex(index);
            layer.Refresh();
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
            graphicsLayer = null;
            simpleFillSymbol = null;
            simpleLineSymbol = null;
            index = -1;
            isHightLight = false;
            isFlash = false;
            isVisible = true;
            isTimer = false;
            lockObj = null;
            properties = null;
        }

        #endregion

        #endregion


        /// <summary>
        /// 闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isTimer)
            {
                this.SetVisible(true);
            }
            else    // 停止闪烁
            {
                this.SetVisible(false);
            }
            isTimer = !isTimer;
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

        public object Tag
        {
            get;
            set;
        }
    }
}
