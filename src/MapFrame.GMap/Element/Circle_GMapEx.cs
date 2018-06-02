using System;
using System.Collections.Generic;
using MapFrame.Core.Model;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using GMap.NET;
using System.Drawing;

namespace MapFrame.GMap.Element
{
    class Circle_GMapEx : GMapPolygon, IMFCircle
    {
        /// <summary>
        /// 半径
        /// </summary>
        private double radius = 0;
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private volatile MapLngLat centerLnglat = null;
        /// <summary>
        /// 闪烁用的计时器
        /// </summary>
        private System.Timers.Timer refreshTimer = null;
        /// <summary>
        /// 填充色
        /// </summary>
        private Color fillColor;
        /// <summary>
        /// 图元所属图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pointList"></param>
        /// <param name="kmlCircle"></param>
        /// <param name="elementName"></param>
        public Circle_GMapEx(List<PointLatLng> pointList, KmlCircle kmlCircle, string elementName)
            : base(pointList, elementName)
        {
            this.radius = kmlCircle.Radius;
            this.centerLnglat = kmlCircle.Position;
            this.ElementName = elementName;
            this.ElementType = ElementTypeEnum.Circle;
            this.Description = "圆";
            this.fillColor = kmlCircle.FillColor;
            SolidBrush b = new SolidBrush(kmlCircle.FillColor);
            base.Fill = b;
            Pen pen = new Pen(kmlCircle.StrokeColor, kmlCircle.StrokeWidth);
            base.Stroke = pen;
            this.IsHitTestVisible = true;   // 鼠标经过可见
            base.Tag = this;

            refreshTimer = new System.Timers.Timer(500);
            refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(refreshTimer_Elapsed);
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 获取圆的圆心位置
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetCenterDot()
        {
            return centerLnglat;
        }

        /// <summary>
        /// 获取半径
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            return radius;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="centerDot"></param>
        public void UpdatePosition(MapLngLat centerDot)
        {
            this.centerLnglat = centerDot;
            base.Points.Clear();
            for (int i = 0; i < 360; i++)
            {
                double seg = Math.PI * i / 180;
                double a = centerDot.Lng + radius * Math.Cos(seg) / 100000;
                double b = centerDot.Lat + radius * Math.Sin(seg) / 100000;
                PointLatLng lnglat = new PointLatLng(b, a);
                base.Points.Add(lnglat);
            }
            this.Overlay.Control.UpdatePolygonLocalPosition(this);
            this.Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="radius">半径值（单位米）</param>
        public void UpdatePosition(double radius)
        {
            this.radius = radius;
            base.Points.Clear();
            for (int i = 0; i < 360; i++)
            {
                double seg = Math.PI * i / 180;
                double a = this.centerLnglat.Lng + radius * Math.Cos(seg) / 100000;
                double b = this.centerLnglat.Lat + radius * Math.Sin(seg) / 100000;
                PointLatLng lnglat = new PointLatLng(b, a);
                base.Points.Add(lnglat);
            }
            this.Overlay.Control.UpdatePolygonLocalPosition(this);
            this.Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="centerDot">圆心</param>
        /// <param name="radius">半径（单位米）</param>
        public void UpdatePosition(MapLngLat centerDot, double radius)
        {
            this.UpdatePosition(radius);
            this.UpdatePosition(centerDot);
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor">填充的颜色</param>
        public void SetFillColor(Color fillColor)
        {
            this.fillColor = fillColor;
            SolidBrush brush = new SolidBrush(fillColor);
            base.Fill = brush;
            this.Update();
        }

        /// <summary>
        /// 设置轮廓设
        /// </summary>
        /// <param name="color">轮廓色</param>
        /// <param name="width">轮廓线大小</param>
        public void SetStroke(Color color, float width)
        {
            Pen pen = new Pen(color, width);
            base.Stroke = pen;
            this.Update();
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity">透明值</param>
        public void SetOpacity(int _opacity)
        {
            SolidBrush brush = base.Fill as SolidBrush;
            brush.Color = Color.FromArgb(_opacity, brush.Color);
            base.Fill = brush;
            this.Update();
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
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
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightLight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight">是否高亮</param>
        public void HightLight(bool isHightLight)
        {
            if (this.isHightLight == isHightLight) return;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="_isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool _isFlash, int interval = 500)
        {
            if (this.isFlash == _isFlash) return;
            this.isFlash = _isFlash;

            if (_isFlash)
            {
                refreshTimer.Interval = interval;
                refreshTimer.Start();
            }
            else
            {
                refreshTimer.Stop();
                SetFillColor(fillColor);
            }
        }

        // 颜色切换
        bool isRightColor = false;

        /// <summary>
        /// 闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Color color = Color.FromArgb(255 - this.fillColor.R, 255 - this.fillColor.G, 255 - fillColor.B);

            isRightColor = !isRightColor;
            if (isRightColor)
            {
                SolidBrush brush = new SolidBrush(color);
                base.Fill = brush;
            }
            else
            {
                SolidBrush brush = new SolidBrush(this.fillColor);
                base.Fill = brush;
            }

            this.Update();
        }

        /// <summary>
        /// 设置显示隐藏
        /// </summary>
        /// <param name="isVisible">是否显示隐藏</param>
        public void SetVisible(bool isVisible)
        {
            base.IsVisible = isVisible;
        }

        /// <summary>
        /// 刷新图元
        /// </summary>
        public void Update()
        {
            if (BelongLayer != null)
            {
                BelongLayer.Refresh();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            BelongLayer = null;
            layer = null;

            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }
    }
}
