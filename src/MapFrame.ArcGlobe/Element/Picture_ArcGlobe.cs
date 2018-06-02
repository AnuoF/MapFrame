
/**************************************************************************
 * 类名：Picture_ArcGlobe.cs
 * 描述：图片
 * 作者：cj
 * 日期：Sep 19,2016
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Geometry;
using System.IO;
using MapFrame.Core.Interface;

namespace MapFrame.ArcGlobe.Element
{
    /// <summary>
    /// 图片
    /// </summary>
    class Picture_ArcGlobe : PngPictureElementClass, IMFPicture
    {
        /// <summary>
        /// 图元索引
        /// </summary>
        private int index = -1;
        /// <summary>
        /// 图片宽度
        /// </summary>
        private double width;
        /// <summary>
        /// 图片高度
        /// </summary>
        private double height;
        /// <summary>
        /// 图层
        /// </summary>
        private IGlobeGraphicsLayer globeGraphicsLayer;
        /// <summary>
        /// 图元是否高亮
        /// </summary>
        private bool isHightLight;
        /// <summary>
        /// 图元是否闪烁
        /// </summary>
        private bool isFlash;
        /// <summary>
        /// 图元是否显示
        /// </summary>
        private bool isVisible;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 闪烁判定
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 包裹
        /// </summary>
        private IEnvelope envelope = null;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 目标位置
        /// </summary>
        private MapFrame.Core.Model.MapLngLat positionPoint;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_graphicsLayer">图层</param>
        /// <param name="picKml">图片的kml</param>
        public Picture_ArcGlobe(IGlobeGraphicsLayer _graphicsLayer, Core.Model.KmlPoint picKml)
        {
            //this.ElementType = Core.Model.ElementTypeEnum.Picture;
            //globeGraphicsLayer = _graphicsLayer;
            //width = height = picKml.Scale;
            //positionPoint = picKml.Position;
            //envelope = new EnvelopeClass();
            //envelope.PutCoords(picKml.Position.Lng, picKml.Position.Lat, picKml.Position.Lng + picKml.Scale, picKml.Position.Lat + picKml.Scale);
            //envelope.ZMax = picKml.Position.Alt;
            //IGeometry geometry = envelope as IGeometry;
            //base.Geometry = geometry;
            //base.ImportPictureFromFile(picKml.IconUrl);

            //GlobeGraphicsElementPropertiesClass properties = new GlobeGraphicsElementPropertiesClass();
            //properties.Rasterize = picKml.Rasterize;
            //globeGraphicsLayer.AddElement(this, properties, out this.index);

            //flashTimer = new System.Timers.Timer();
            //flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            //flashTimer.Interval = 500;
        }

        #region  Core.Interface.IPicture

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {
            //IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(lng, lat, lng + width, lat + height);
            envelope.ZMax = alt;
            IGeometry geometry = envelope as IGeometry;
            base.Geometry = geometry;
            //更新坐标
            positionPoint = new Core.Model.MapLngLat() { Lng = lng, Lat = lat, Alt = alt };
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        public void UpdatePosition(Core.Model.MapLngLat lngLat)
        {
            this.UpdatePosition(lngLat.Lng, lngLat.Lat, lngLat.Alt);
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="bitmap">图片位置</param>
        public void SetIcon(string bitmap)
        {
            if (File.Exists(bitmap))
            {
                base.ImportPictureFromFile(bitmap);
            }
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        /// <param name="angle">角度值</param>
        public void SetAngle(float angle)
        {
            
        }

        /// <summary>
        /// 设置比列
        /// </summary>
        /// <param name="scale">大小</param>
        public void SetScale(float scale)
        {
            //IGeometry geometry = base.Geometry;
            //IEnvelope envelope = geometry as IEnvelope;
            ////envelope = geometry as IEnvelope;
            //envelope.Width = scale;
            //envelope.Height = scale;
            //base.Geometry = geometry;
            //height = width = scale;
            envelope.PutCoords(positionPoint.Lng, positionPoint.Lat, positionPoint.Lng + scale, positionPoint.Lat + scale);
            envelope.ZMax = positionPoint.Alt;
            IGeometry geometry = envelope as IGeometry;
            base.Geometry = geometry;
            height = width = scale;
        }

        /// <summary>
        /// 获取经纬度
        /// </summary>
        /// <returns></returns>
        public Core.Model.MapLngLat GetLngLat()
        {
            return positionPoint;
        }

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
        public Core.Interface.IMFLayer BelongLayer
        {
            get;
            set;
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
            get;
            set;
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
        /// 图元是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get 
            {
                return isHightLight;
            }
        }

        /// <summary>
        /// 图元是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get 
            {
                return isFlash;
            }
        }

        /// <summary>
        /// 图元是否显示
        /// </summary>
        public bool IsVisible
        {
            get 
            {
                globeGraphicsLayer.GetIsElementVisible(this.index, out this.isVisible);
                return isVisible;
            }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">是否高亮</param>
        public void HightLight(bool isHightLight)
        {
            if (this.isHightLight == isHightLight) return;
            lock (lockObj)
            {
                if (isHightLight)
                {
                    envelope.PutCoords(positionPoint.Lng, positionPoint.Lat, positionPoint.Lng + height + 2, positionPoint.Lat + height + 2);
                    envelope.ZMax = positionPoint.Alt;
                    IGeometry geometry = envelope as IGeometry;
                    base.Geometry = geometry;
                }
                else
                {
                    envelope.PutCoords(positionPoint.Lng, positionPoint.Lat, positionPoint.Lng + height, positionPoint.Lat + height);
                    envelope.ZMax = positionPoint.Alt;
                    IGeometry geometry = envelope as IGeometry;
                    base.Geometry = geometry;
                }
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
            if (this.isFlash == isFlash) return;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                envelope.PutCoords(positionPoint.Lng, positionPoint.Lat, positionPoint.Lng + height, positionPoint.Lat + height);
                envelope.ZMax = positionPoint.Alt;
                IGeometry geometry = envelope as IGeometry;
                base.Geometry = geometry;
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
            if (!isTimer)
            {
                if ((this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxGlobeControl).InvokeRequired)
                {
                    (this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxGlobeControl).BeginInvoke(new System.Action(
                        delegate
                        {
                            envelope.PutCoords(positionPoint.Lng, positionPoint.Lat, positionPoint.Lng + height + 2, positionPoint.Lat + height + 2);
                            envelope.ZMax = positionPoint.Alt;
                            IGeometry geometry = envelope as IGeometry;
                            base.Geometry = geometry;
                        }));
                }
                else
                {
                    envelope.PutCoords(positionPoint.Lng, positionPoint.Lat, positionPoint.Lng + height + 2, positionPoint.Lat + height + 2);
                    envelope.ZMax = positionPoint.Alt;
                    IGeometry geometry = envelope as IGeometry;
                    base.Geometry = geometry;
                }
            }
            else    // 停止闪烁
            {
                if ((this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxGlobeControl).InvokeRequired)
                {
                    (this.BelongLayer.MapControl as ESRI.ArcGIS.Controls.AxGlobeControl).BeginInvoke(new System.Action(
                        delegate
                        {
                            this.SetScale((float)height);
                        }));
                }
                else
                {
                    this.SetScale((float)height);
                }
            }
            isTimer = !isTimer; 
        }

        /// <summary>
        /// 显示/隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            globeGraphicsLayer.PutIsElementVisible(index, isVisible);
            this.isVisible = isVisible;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            globeGraphicsLayer.UpdateElementByIndex(this.index);
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
            globeGraphicsLayer = null;
            envelope = null;
        }

        #endregion



        public void SetColor(System.Drawing.Color color)
        {
            throw new NotImplementedException();
        }

        public bool IsLableVisible
        {
            get { throw new NotImplementedException(); }
        }

        public Core.Model.ShowTypeEnum LabelShowType
        {
            get { throw new NotImplementedException(); }
        }

        public void SetSize(double size)
        {
            throw new NotImplementedException();
        }

        public object Tag
        {
            get;
            set;
        }
    }
}
