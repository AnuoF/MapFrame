/**************************************************************************
 * 类名：Circle_ArcGlobe.cs
 * 描述：圆形图元接口
 * 作者：lx
 * 日期：Sep 19,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcGlobe.Element
{
    /// <summary>
    //  圆形图元
    /// </summary>
    class Circle_ArcGlobe : PolygonElementClass, IMFCircle
    {
        private IMFLayer layer = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 图元是否显示
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 高亮
        /// </summary>
        private bool isHightlight = false;
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
        private bool isFlash = false;
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
        private float bsize;
        /// <summary>
        /// 半径
        /// </summary>
        private double radius;
        /// <summary>
        /// 图元标识符
        /// </summary>
        private int index;
        /// <summary>
        /// 图元索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        /// <summary>
        /// 图元描述
        /// </summary>
        private string description;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 填充符号
        /// </summary>
        private IFillSymbol fillSymbol;
        /// <summary>
        /// 图层
        /// </summary>
        private IGlobeGraphicsLayer graphicsLayer;
        /// <summary>
        /// 轮廓线
        /// </summary>
        private ILineSymbol lineSymbol;
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private IPoint centerPoint;
        /// <summary>
        /// 栅格化
        /// </summary>
        public bool Rasterize
        {
            get;
            set;
        }

        //private double circleDegrees = 360.0;
        //private int circleDivisions = 180;
        private double vectorComponentOffset = 0.0000000001;
        private object missing;
        /// <summary>
        /// 矢量元素
        /// </summary>
        private IVector3D upperAxisVector3D, normalVector3D, lowerAxisVector3D;
        /// <summary>
        /// 集合元素集合
        /// </summary>
        private IGeometryCollection geometryCollection;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private IPointCollection pointCollection;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="graphicsLayer">图层</param>
        /// <param name="kmlCircle">圆的kml</param>
        public Circle_ArcGlobe(IGlobeGraphicsLayer _graphicsLayer, KmlCircle kmlCircle)
        {
            this.ElementType = Core.Model.ElementTypeEnum.Circle;
            graphicsLayer = _graphicsLayer;
            lineSymbol = new SimpleLineSymbolClass();

            lineSymbol.Color = new RgbColorClass()
            {
                Red = kmlCircle.StrokeColor.R,
                Green = kmlCircle.StrokeColor.G,
                Blue = kmlCircle.StrokeColor.B
            };

            if (kmlCircle.StrokeWidth == 0)
            {
                kmlCircle.StrokeWidth = 2;
            }

            lineSymbol.Width = kmlCircle.StrokeWidth;
            fillSymbol = new SimpleFillSymbolClass();
            fillSymbol.Outline = lineSymbol;
            fillSymbol.Color = new RgbColorClass()
            {
                Red = kmlCircle.FillColor.R,
                Green = kmlCircle.FillColor.G,
                Blue = kmlCircle.FillColor.B
            };

            radius = kmlCircle.Radius;
            outlineColor = kmlCircle.StrokeColor;
            fillColor = kmlCircle.FillColor;

            centerPoint = new PointClass();//圆心坐标
            centerPoint.PutCoords(kmlCircle.Position.Lng, kmlCircle.Position.Lat);
            centerPoint.Z = kmlCircle.Position.Alt;

            missing = System.Type.Missing;
            IZAware zAware = (IGeometry)centerPoint as IZAware;
            zAware.ZAware = true;
            upperAxisVector3D = new Vector3DClass();
            upperAxisVector3D.SetComponents(0, 0, 2);
            lowerAxisVector3D = new Vector3DClass();
            lowerAxisVector3D.SetComponents(0, 0, -2);
            lowerAxisVector3D.XComponent -= vectorComponentOffset;
            lowerAxisVector3D.YComponent -= vectorComponentOffset;//TODO
            normalVector3D = upperAxisVector3D.CrossProduct(lowerAxisVector3D) as IVector3D;
            normalVector3D.Magnitude = kmlCircle.Radius;
            double rotationAngleInRadians = 2 * (Math.PI / 180);
            //geometryCollection = new MultiPatchClass();
            pointCollection = new PolygonClass();

            for (int i = 0; i < 180; i++)
            {
                normalVector3D.Rotate(-1 * rotationAngleInRadians, upperAxisVector3D);
                IPoint vertexPoint = new PointClass();
                vertexPoint.X = centerPoint.X + normalVector3D.XComponent;
                vertexPoint.Y = centerPoint.Y + normalVector3D.YComponent;
                vertexPoint.Z = centerPoint.Z;
                pointCollection.AddPoint(vertexPoint, missing, missing);
            }

            base.Symbol = fillSymbol;
            base.Geometry = pointCollection as IGeometry;

            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }

        #region IMFCircle
        
        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 图元所在图层
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
        /// 图元描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
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
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightlight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 图元是否可见true显示,false隐藏
        /// </summary>
        public bool IsVisible
        {
            get
            {
                graphicsLayer.GetIsElementVisible(this.index, out isVisible);
                return isVisible;
            }
        }

        /// <summary>
        /// 获取圆心坐标
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetCenterDot()
        {
            return new Core.Model.MapLngLat() { Lng = centerPoint.X, Lat = centerPoint.Y, Alt = centerPoint.Z };
        }

        /// <summary>
        /// 获取圆的半径
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            return radius;
        }

        /// <summary>
        /// 更新圆心
        /// </summary>
        /// <param name="centerDot"></param>
        public void UpdatePosition(MapLngLat centerDot)
        {
            centerPoint = new PointClass() { X = centerDot.Lng, Y = centerDot.Lat };
            centerPoint.Z = centerDot.Alt;

            IZAware zAware = (IGeometry)centerPoint as IZAware;
            zAware.ZAware = true;
            upperAxisVector3D = new Vector3DClass();
            upperAxisVector3D.SetComponents(0, 0, 10);
            lowerAxisVector3D = new Vector3DClass();
            lowerAxisVector3D.SetComponents(0, 0, -10);
            lowerAxisVector3D.XComponent -= vectorComponentOffset;
            normalVector3D = upperAxisVector3D.CrossProduct(lowerAxisVector3D) as IVector3D;
            normalVector3D.Magnitude = this.radius;
            double rotationAngleInRadians = 2 * (Math.PI / 180);
            pointCollection.RemovePoints(0, pointCollection.PointCount);
            for (int i = 0; i < 180; i++)
            {
                normalVector3D.Rotate(-1 * rotationAngleInRadians, upperAxisVector3D);
                IPoint vertexPoint = new PointClass();
                vertexPoint.X = centerPoint.X + normalVector3D.XComponent;
                vertexPoint.Y = centerPoint.Y + normalVector3D.YComponent;
                vertexPoint.Z = centerPoint.Z;
                pointCollection.AddPoint(vertexPoint, missing, missing);
                //if (i == 0) pointCollection.AddPoint(vertexPoint, missing, missing);
            }

            base.Geometry = pointCollection as IGeometry;
            this.Update();
        }

        /// <summary>
        /// 更新半径
        /// </summary>
        /// <param name="radius"></param>
        public void UpdatePosition(double radius)
        {
            this.radius = radius;
            IZAware zAware = (IGeometry)centerPoint as IZAware;
            zAware.ZAware = true;
            upperAxisVector3D = new Vector3DClass();
            upperAxisVector3D.SetComponents(0, 0, 10);
            lowerAxisVector3D = new Vector3DClass();
            lowerAxisVector3D.SetComponents(0, 0, -10);
            lowerAxisVector3D.XComponent -= vectorComponentOffset;
            normalVector3D = upperAxisVector3D.CrossProduct(lowerAxisVector3D) as IVector3D;
            normalVector3D.Magnitude = this.radius;
            double rotationAngleInRadians = 2 * (Math.PI / 180);
            geometryCollection = new MultiPatchClass();
            pointCollection.RemovePoints(0, pointCollection.PointCount);

            for (int i = 0; i < 180; i++)
            {
                normalVector3D.Rotate(-1 * rotationAngleInRadians, upperAxisVector3D);
                IPoint vertexPoint = new PointClass();
                vertexPoint.X = centerPoint.X + normalVector3D.XComponent;
                vertexPoint.Y = centerPoint.Y + normalVector3D.YComponent;
                vertexPoint.Z = centerPoint.Z;
                pointCollection.AddPoint(vertexPoint, missing, missing);
            }
            base.Geometry = pointCollection as IGeometry;

            this.Update();

        }

        /// <summary>
        /// 更新圆心坐标和半径
        /// </summary>
        /// <param name="centerDot"></param>
        /// <param name="radius"></param>
        public void UpdatePosition(MapLngLat centerDot, double radius)
        {
            centerPoint = new PointClass() { X = centerDot.Lng, Y = centerDot.Lat };
            centerPoint.Z = centerDot.Alt;
            this.radius = radius;

            IZAware zAware = (IGeometry)centerPoint as IZAware;
            zAware.ZAware = true;
            upperAxisVector3D = new Vector3DClass();
            upperAxisVector3D.SetComponents(0, 0, 10);
            lowerAxisVector3D = new Vector3DClass();
            lowerAxisVector3D.SetComponents(0, 0, -10);
            lowerAxisVector3D.XComponent -= vectorComponentOffset;
            normalVector3D = upperAxisVector3D.CrossProduct(lowerAxisVector3D) as IVector3D;
            normalVector3D.Magnitude = this.radius;
            double rotationAngleInRadians = 2 * (Math.PI / 180);
            geometryCollection = new MultiPatchClass();
            pointCollection.RemovePoints(0, pointCollection.PointCount);

            for (int i = 0; i < 180; i++)
            {
                normalVector3D.Rotate(-1 * rotationAngleInRadians, upperAxisVector3D);
                IPoint vertexPoint = new PointClass();
                vertexPoint.X = centerPoint.X + normalVector3D.XComponent;
                vertexPoint.Y = centerPoint.Y + normalVector3D.YComponent;
                vertexPoint.Z = centerPoint.Z;
                pointCollection.AddPoint(vertexPoint, missing, missing);
            }
            base.Geometry = pointCollection as IGeometry;
            this.Update();
        }

        /// <summary>
        /// 更新填充色
        /// </summary>
        /// <param name="_fillColor"></param>
        public void SetFillColor(Color _fillColor)
        {
            fillSymbol.Color = new RgbColorClass() { Red = _fillColor.R, Green = _fillColor.G, Blue = _fillColor.B };
            fillColor = _fillColor;

            base.Symbol = fillSymbol;
            this.Update();
        }

        /// <summary>
        /// 更新填充色和轮廓宽度
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public void SetStroke(Color color, float width)
        {
            lineSymbol.Color = new RgbColorClass()
            {
                Red = color.R,
                Green = color.B,
                Blue = color.B
            };
            outlineColor = color;
            bsize = width;
            lineSymbol.Width = width;
            fillSymbol.Outline = lineSymbol;

            base.Symbol = fillSymbol;
            this.Update();
        }             

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            if (this.isHightlight == isHightLight) return;
            lock (lockObj)
            {
                if (isHightLight)
                {
                    Color c = Color.FromArgb(Math.Abs(255 - this.fillColor.R), Math.Abs(255 - this.fillColor.G), Math.Abs(255 - this.fillColor.B));
                    fillSymbol.Color = new RgbColorClass()
                    {
                        Red = c.R,
                        Green = c.G,
                        Blue = c.B
                    };
                    base.Symbol = fillSymbol;
                }
                else
                {
                    SetFillColor(fillColor);
                }
            }
            this.isHightlight = isHightLight;
            this.Update();
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
        /// 闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Color c = Color.FromArgb(Math.Abs(255 - this.fillColor.R), Math.Abs(255 - this.fillColor.G), Math.Abs(255 - this.fillColor.B));
            if (!isTimer)
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
        /// 显示或隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            this.Dosomething((Action)delegate()
            {
                graphicsLayer.PutIsElementVisible(index, isVisible);
            }, true);
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
            }, true);
            (mapControl.GlobeDisplay as IGlobeDisplayLayers2).RefreshLayer(graphicsLayer as ILayer);
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
            }
            layer = null;
            lineSymbol = null;
            fillSymbol = null;
            graphicsLayer = null;
            isFlash = false;
            isHightlight = false;
            isVisible = false;
            upperAxisVector3D = null;
            normalVector3D = null;
            lowerAxisVector3D = null;
            geometryCollection = null;
            pointCollection = null;
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity"></param>
        public void SetOpacity(int _opacity)
        {
            IRgbColor color = new RgbColorClass();
            color.Transparency = (byte)_opacity;
            color.Red = fillColor.R;
            color.Green = fillColor.G;
            color.Blue = fillColor.B;

            Dosomething((Action)delegate()
            {
                fillSymbol.Color = color;
                base.Symbol = fillSymbol;
            }, true);
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

        public object Tag
        {
            get;
            set;
        }
    }
}
