/**************************************************************************
 * 类名：Model3d_ArcGlobe.cs
 * 描述：模型
 * 作者：chenjing
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/
using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GlobeCore;
using System.Timers;
using System.Drawing;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Element
{
    /// <summary>
    /// 模型
    /// </summary>
    class Model3d_ArcGlobe : MarkerElementClass, I3DModel
    {
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 3D符号
        /// </summary>
        private IMarker3DSymbol marker3DSymbol = null;
        /// <summary>
        /// 符号
        /// </summary>
        private IMarkerSymbol markerSymbol = null;
        /// <summary>
        /// 图元所属图层
        /// </summary>
        private IMFLayer layer;
        /// <summary>
        /// 图元索引
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
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 是否在闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 图元是否隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 图元是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IGlobeGraphicsLayer graphicLayer = null;
        /// <summary>
        /// 比例
        /// </summary>
        private double scale;
        /// <summary>
        /// 模型颜色
        /// </summary>
        private Color color;
        /// <summary>
        /// 模型位置
        /// </summary>
        private MapLngLat lngLat;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_graphcisLayer">图层</param>
        /// <param name="modelKml">模型kml</param>
        public Model3d_ArcGlobe(IGlobeGraphicsLayer _graphicLayer, KmlModel3d modelKml, IImport3DFile import3Dfile)
        {
            graphicLayer = _graphicLayer;

            this.ElementType = ElementTypeEnum.Model3D;   // 图元类型
            this.Description = modelKml.Description;      // 图元描述

            #region  符号

            IGeometry geometry = import3Dfile.Geometry;
            //将模型转为3D符号
            marker3DSymbol = new Marker3DSymbolClass();
            marker3DSymbol.Shape = geometry;
            markerSymbol = marker3DSymbol as IMarkerSymbol;
            markerSymbol.Size = modelKml.Scale;
            markerSymbol.Angle = modelKml.Azimuth;


            IRgbColor c = new RgbColorClass();
            c.Transparency = modelKml.Color.A;
            c.Red = modelKml.Color.R;
            c.Green = modelKml.Color.G;
            c.Blue = modelKml.Color.B;
            markerSymbol.Color = c;

            this.scale = modelKml.Scale;
            this.color = modelKml.Color;

            #endregion

            #region  位置

            IPoint p = new PointClass();
            p.PutCoords(modelKml.Position.Lng, modelKml.Position.Lat);
            p.Z = modelKml.Position.Alt;
            (p as IZAware).ZAware = true;
            #endregion
            lngLat = modelKml.Position;

            base.Geometry = p;                                  //指定位置
            base.Symbol = markerSymbol;                         //指定符号

            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }

        #region  Core.Interface.IModel
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 模型文件位置
        /// </summary>
        public string ModelFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lnglat">坐标</param>
        public void UpdatePosition(MapLngLat _lnglat)
        {
            this.Dosomething((Action)delegate()
            {
                IPoint point = new PointClass();
                point.PutCoords(_lnglat.Lng, _lnglat.Lat);
                point.Z = _lnglat.Alt;
                (point as IZAware).ZAware = true;
                base.Geometry = point;
                graphicLayer.UpdateElementByIndex(index);   // 刷新图元
            }, true);

            this.lngLat = _lnglat;
        }

        /// <summary>
        /// 更新模型
        /// </summary>
        /// <param name="_lnglat">坐标</param>
        /// <param name="angle">角度</param>
        public void UpdateModel(MapLngLat _lnglat, double angle)
        {
            this.Dosomething((Action)delegate()
            {
                IPoint point = new PointClass();
                point.PutCoords(_lnglat.Lng, _lnglat.Lat);
                point.Z = _lnglat.Alt;
                (point as IZAware).ZAware = true;
                base.Geometry = point;

                markerSymbol.Angle = angle;
                base.Symbol = markerSymbol;

                int outIndex = -1;
                graphicLayer.FindElementIndex(this, out outIndex);
                if (outIndex > -1)
                {
                    graphicLayer.UpdateElementByIndex(index);   // 刷新图元
                }
            }, true);

            this.lngLat = _lnglat;
        }

        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetPosition()
        {
            //IPoint point = marker3DSymbol.Shape as IPoint;
            //MapLngLat lnglat = new MapLngLat(point.X, point.Y, point.Z);
            return lngLat;
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        /// <param name="angle">角度</param>
        public void SetAngle(double angle)
        {
            this.Dosomething((Action)delegate()
            {
                markerSymbol.Angle = angle;
                base.Symbol = markerSymbol;
                graphicLayer.UpdateElementByIndex(index);   // 刷新图元
            }, false);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(System.Drawing.Color color)
        {
            RgbColorClass c = new RgbColorClass();
            c.Transparency = color.A;
            c.RGB = color.B * 65536 + color.G * 256 + color.R;

            markerSymbol.Color = c;
            this.Dosomething((Action)delegate()
            {
                base.Symbol = markerSymbol;
                graphicLayer.UpdateElementByIndex(index);   // 刷新图元
            }, true);

            this.color = color;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="argb">argb值</param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        public void SetColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a">透明度</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color color = Color.FromArgb(a, r, g, b);
            SetColor(color);
        }

        /// <summary>
        /// 设置tip内容
        /// </summary>
        /// <param name="tipText">内容</param>
        public void SetTipText(string tipText)
        {
            //TODO
        }

        public void SetTipShow(ShowTypeEnum showType)
        {
            //TODO
        }

        /// <summary>
        /// 设置模型比例
        /// </summary>
        /// <param name="scale">比例值</param>
        public void SetScale(double scale)
        {
            this.scale = scale;
            markerSymbol.Size = scale;
            this.Dosomething((Action)delegate()
            {
                base.Symbol = markerSymbol;
                graphicLayer.UpdateElementByIndex(index);   // 刷新图元
            }, false);
        }

        #endregion

        #region  Element

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
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 图元名
        /// </summary>
        public string ElementName
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                this.Dosomething((Action)delegate()
                {
                    graphicLayer.PutElementName(this, value);
                    graphicLayer.UpdateElementByIndex(index);   // 刷新图元
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
        /// 图元隐藏/显示
        /// </summary>
        public bool IsVisible
        {
            get
            {
                this.Dosomething((Action)delegate()
                {
                    graphicLayer.GetIsElementVisible(index, out this.isVisible);
                }, true);

                return isVisible;
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
                Color c = Color.FromArgb(Math.Abs(255 - this.color.R), Math.Abs(255 - this.color.G), Math.Abs(255 - this.color.B));
                RgbColorClass color = new RgbColorClass();
                color.Transparency = c.A;
                color.Red = c.R;
                color.Green = c.G;
                color.Blue = c.B;
                markerSymbol.Color = color;
                base.Symbol = markerSymbol;
                this.Update();
            }
            else
            {
                this.SetColor(this.color);
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
            if (this.isFlash == isFlash) return;//防止被多次调用
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
        /// 显示/隐藏
        /// </summary>
        /// <param name="isVisible">是否显示或隐藏</param>
        public void SetVisible(bool isVisible)
        {
            if (this.IsVisible == isVisible) return;

            this.Dosomething((Action)delegate()
            {
                graphicLayer.PutIsElementVisible(index, isVisible);
                graphicLayer.UpdateElementByIndex(index);//刷新图元
            }, true);
        }

        /// <summary>
        /// 闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isTimer)
            {
                this.Dosomething((Action)delegate()
                {
                    this.SetVisible(true);
                }, true);
            }
            else    // 停止闪烁
            {
                this.Dosomething((Action)delegate()
                {
                    this.SetVisible(false);
                }, true);
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
                graphicLayer.UpdateElementByIndex(index);//刷新图元
            }, false);
        }

        /// <summary>
        /// 释放
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
            marker3DSymbol = null;
            markerSymbol = null;
            index = -1;
            isTimer = false;
            isFlash = false;
            isVisible = true;
            isHightLight = false;
            graphicLayer = null;
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
