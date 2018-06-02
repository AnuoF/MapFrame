using System;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using AxHOSOFTMapControlLib;
using System.Drawing;

namespace MapFrame.Mgis.Element
{
    class Point_Mgis : IMFPoint
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 图元所属图层
        /// </summary>
        private IMFLayer layer = null;

        public Point_Mgis(Kml kml)
        {
            KmlPoint kmlPoint = kml.Placemark.Graph as KmlPoint;
            if (kmlPoint.Position == null || kml.Placemark.Name == string.Empty) return;
            mapControl.MgsDrawDotByJBID(kml.Placemark.Name, 12, 0, 0, 0);
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {

        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lngLat"></param>
        public void UpdatePosition(MapLngLat lngLat)
        {

        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color">Color值</param>
        public void SetColor(Color color)
        {

        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="argb">argb值</param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r">颜色R值</param>
        /// <param name="g">颜色G值</param>
        /// <param name="b">颜色B值</param>
        public void SetColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a">颜色A值</param>
        /// <param name="r">颜色R值</param>
        /// <param name="g">颜色G值</param>
        /// <param name="b">颜色B值</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color color = Color.FromArgb(a, r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 获取点的位置
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetLngLat()
        {
            double lng = 100000000, lat = 100000000;
            mapControl.MgsGetSymPosition(ElementName, ref lng, ref lat);
            MapLngLat lnglat = new MapLngLat(lng, lat);
            return lnglat;
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
                mapControl = value.MapControl as AxHOSOFTMapControl;
            }
        }

        /// <summary>
        /// 图元描述
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get; set;
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        public ElementTypeEnum ElementType
        {
            get; set;
        }

        /// <summary>
        /// 获取图元是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 获取图元是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 获取图元是否显示隐藏
        /// </summary>
        public bool IsVisible
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">是否高亮</param>
        public void HightLight(bool isHightLight)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.IsFlash == isFlash) return;
            mapControl.MgsFlashSym(ElementName, isFlash ? 0 : 1);
        }

        /// <summary>
        /// 设置显示隐藏
        /// </summary>
        /// <param name="isVisible">是否显示隐藏</param>
        public void SetVisible(bool isVisible)
        {
            
        }

        /// <summary>
        /// 图元更新
        /// </summary>
        public void Update()
        {
            if (layer != null)
            {
                layer.Refresh();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            mapControl = null;
            BelongLayer = null;
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }
        

        public void SetSize(System.Drawing.Size size)
        {
            throw new NotImplementedException();
        }

        public void SetSize(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetTipText(string tipText)
        {
            throw new NotImplementedException();
        }

        public void SetTipShow(Core.Model.ShowTypeEnum showType)
        {
            throw new NotImplementedException();
        }
        

        public void SetAngle(double angle)
        {
            throw new NotImplementedException();
        }
    }
}
