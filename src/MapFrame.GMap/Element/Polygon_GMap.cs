/**************************************************************************
 * 类名：Polygon_GMap.cs
 * 描述：GMap面图元
 * 作者：CJ
 * 日期：July 4,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using GMap.NET;
using System.Drawing;
using System.Timers;
using MapFrame.Core.Common;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// GMap面图元
    /// </summary>
    public class Polygon_GMap : GMapPolygon, IMFPolygon
    {
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 开始闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 轮廓颜色
        /// </summary>
        private Color outLineColor;
        /// <summary>
        /// 设置轮廓宽度
        /// </summary>
        private float outLineSize;
        /// <summary>
        /// 填充色
        /// </summary>
        private Color fillColor;
        /// <summary>
        /// 是否为多边形
        /// </summary>
        private bool isPolygon;
        /// <summary>
        /// 是否高亮状态
        /// </summary>
        private bool isHighelight;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pList">多边形点集合</param>
        /// <param name="kPolygon">多边形的kml</param>
        /// <param name="name">多边形的名称</param>
        public Polygon_GMap(List<PointLatLng> pList, KmlPolygon kPolygon, string name)
            : base(pList, name)
        {
            this.ElementName = name;
            this.ElementType = ElementTypeEnum.Polygon;
            this.Description = kPolygon.Description;
            this.Stroke.Width = kPolygon.OutLineSize;

            //设置面的填充色
            fillColor = kPolygon.FillColor;
            SolidBrush b = new SolidBrush(fillColor);
            this.Fill = b;

            //设置面的轮廓颜色
            outLineColor = kPolygon.OutLineColor;
            outLineSize = kPolygon.OutLineSize;
            Pen pen = new Pen(outLineColor, kPolygon.OutLineSize);
            this.Stroke = pen;
            this.IsHitTestVisible = true;   // 鼠标经过可见
            flashTimer = new Timer();
            flashTimer.Elapsed += flashTimer_Elapsed;
            flashTimer.Interval = 500;

            base.Tag = this;
        }

        #region IPolygonGMap
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 所属图层
        /// </summary>
        public IMFLayer BelongLayer
        {
            get;
            set;
        }

        /// <summary>
        /// 图元是否在闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
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
        /// 轮廓颜色
        /// </summary>
        public Color OutLineColor
        {
            get { return outLineColor; }
        }

        /// <summary>
        /// 填充色
        /// </summary>
        public Color FillColor
        {
            get { return fillColor; }
        }

        /// <summary>
        /// 是否为多边形
        /// </summary>
        public bool IsPolygon
        {
            get { return isPolygon; }
            set { isPolygon = value; }
        }

        void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="_isHightLight"></param>
        public void HightLight(bool _isHightLight)
        {
            if (this.isHighelight == _isHightLight) return;
            isHighelight = _isHightLight;

            if (isHighelight)
            {
                Color color = Color.FromArgb(255 - outLineColor.R, 255 - outLineColor.G, 255 - outLineColor.B);
                this.Stroke.Color = color;
                this.Stroke.Width = outLineSize * 2;
                Update();
            }
            else
            {
                SetOutLineColor(outLineColor);
                SetOutLineSize(outLineSize);
            }
        }

        /// <summary>
        /// 显示\隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            this.IsVisible = isVisible;
        }

        /// <summary>
        /// 设置轮廓颜色
        /// </summary>
        /// <param name="outlineColor">轮廓颜色</param>
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
        /// <param name="color">轮廓颜色</param>
        /// <returns></returns>
        public bool SetOutLineColor(Color color)
        {
            outLineColor = color;
            this.Stroke.Color = color;
            Update();
            return true;
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor">填充色</param>
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
        /// <param name="color">填充色</param>
        /// <returns></returns>
        public bool SetFillColor(Color color)
        {
            fillColor = color;
            SolidBrush brush = new SolidBrush(color);
            Fill = brush;
            Update();
            return true;
        }

        /// <summary>
        /// 设置轮廓线粗细
        /// </summary>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public bool SetOutLineSize(float size)
        {
            this.Stroke.Width = size;
            this.outLineSize = size;
            Update();
            return true;
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="_isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool _isFlash, int interval = 500)
        {
            if (_isFlash == this.isFlash) return;
            this.isFlash = _isFlash;

            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                this.IsVisible = true;
            }
        }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHighelight; }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="oldLngLat">旧的位置</param>
        /// <param name="newLngLat">新的位置</param>
        /// <returns></returns>
        public bool UpdatePosition(MapLngLat oldLngLat, MapLngLat newLngLat)
        {
            PointLatLng oLngLat = new PointLatLng(oldLngLat.Lat, oldLngLat.Lng);
            PointLatLng nLngLat = new PointLatLng(newLngLat.Lat, newLngLat.Lng);
            this.Points.Remove(oLngLat);
            this.Points.Add(nLngLat);

            this.Overlay.Control.UpdatePolygonLocalPosition(this);
            Update();

            return true;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public bool UpdatePosition(List<MapLngLat> pList)
        {
            this.Points.Clear();
            foreach (var item in pList)
            {
                PointLatLng p = new PointLatLng(item.Lat, item.Lng);
                this.Points.Add(p);
            }

            this.Overlay.Control.UpdatePolygonLocalPosition(this);
            Update();

            return true;
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
        /// 获取面的顶点位置集合
        /// </summary>
        /// <returns>顶点集合</returns>
        public List<MapLngLat> GetLngLat()
        {
            int cout = base.Points.Count;
            List<MapLngLat> lnglatList = new List<MapLngLat>();
            for (int i = 0; i < cout; i++)
            {
                MapLngLat lnglat = new MapLngLat(base.Points[i].Lng, base.Points[i].Lat);
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
            PointLatLng point = new PointLatLng(lngLat.Lat, lngLat.Lng);
            this.Points.Add(point);
            this.Update();
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lngLat"></param>
        /// <returns></returns>
        public void RemovePoint(MapLngLat lngLat)
        {
            PointLatLng point = new PointLatLng(lngLat.Lat, lngLat.Lng);
            this.Points.Remove(point);
            this.Update();
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            this.BelongLayer = null;

            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
                flashTimer = null;
            }
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity">值</param>
        public void SetOpacity(int _opacity)
        {
            Pen p = new Pen(Fill);
            Fill = new SolidBrush(Color.FromArgb(_opacity, p.Color));
            Update();
        }

        /// <summary>
        /// 获取面积
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            List<MapLngLat> lnglatList = new List<MapLngLat>();
            foreach (var item in base.Points)
            {
                MapLngLat lnglat = new MapLngLat(item.Lng, item.Lat);
                lnglatList.Add(lnglat);
            }

            return Utils.GetPolygonArea(lnglatList);
        }
    }
}
