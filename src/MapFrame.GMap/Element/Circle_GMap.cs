/**************************************************************************
 * 类名：Circle_GMap.cs
 * 描述：GMap地图圆图元
 * 作者：Allen
 * 日期：July 26,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using MapFrame.Core.Common;
using System.Diagnostics;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// GMap地图圆图元
    /// </summary>
    [Serializable]
    public class Circle_GMap : GMapPolygon, ISerializable, IMFCircle
    {
        /// <summary>
        /// 闪烁用的计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 高亮画笔
        /// </summary>
        private Pen pen = null;
        /// <summary>
        /// 半径，单位米
        /// </summary>
        private double radius;
        /// <summary>
        /// 编辑点
        /// </summary>
        private List<GPoint> listPoints = null; //存储坐标点
        /// <summary>
        /// 圆心坐标
        /// </summary>
        private MapLngLat position = null;

        public object Tag
        {
            get;
            set;
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
        /// is filled
        /// </summary>
        public bool IsFilled = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p"></param>
        /// <param name="kmlCircle">kml</param>
        /// <param name="elementName">图元名称</param>
        public Circle_GMap(PointLatLng p, KmlCircle kmlCircle, string elementName, List<PointLatLng> points)
            : base(points, elementName)
        {
            position = new MapLngLat(p.Lng, p.Lat);
            this.IsHitTestVisible = true;
            this.ElementName = elementName;
            this.ElementType = ElementTypeEnum.Circle;
            this.Description = kmlCircle.Description;
            if (kmlCircle.RandomPosition != null)
                radius = Utils.GetDistance(kmlCircle.Position, kmlCircle.RandomPosition) * 1000;
            else
                radius = kmlCircle.Radius;

            Stroke = new Pen(kmlCircle.StrokeColor, kmlCircle.StrokeWidth);
            Fill = new SolidBrush(kmlCircle.FillColor);
            pen = new Pen(Brushes.Green, 1);

            listPoints = new List<GPoint>();
            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += flashTimer_Elapsed;
        }

        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// 重绘
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            GPoint centerPoint = this.Overlay.Control.FromLatLngToLocal(new PointLatLng(position.Lat, position.Lng));
            MapLngLat antherPosition = Utils.GetPointByDistanceAndAngle((float)radius, position, 90);
            GPoint antherPoint = this.Overlay.Control.FromLatLngToLocal(new PointLatLng(antherPosition.Lat, antherPosition.Lng));

            int width = Convert.ToInt32(antherPoint.X - centerPoint.X);


            //// 重新获取大小
            //ResetSize();

            Rectangle rect = new Rectangle((int)centerPoint.X, (int)centerPoint.Y, width, width);

            Debug.WriteLine(string.Format("{0}_______{1}", (int)centerPoint.X, (int)centerPoint.Y));

            // 画圆
            if (Stroke != null)
            {
                g.DrawEllipse(Stroke, rect);
            }

            // 填充
            if (IsFilled)
            {
                g.FillEllipse(Fill, rect);
            }

            // 高亮
            if (isHightLight)
            {
                //编辑时高亮，移动控制点位矩形
                g.DrawRectangle(pen, rect);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (Stroke != null)
            {
                Stroke.Dispose();
                Stroke = null;
            }

            if (Fill != null)
            {
                Fill.Dispose();
                Fill = null;
            }

            if (pen != null)
            {
                pen.Dispose();
                pen = null;
            }

            listPoints.Clear();

            listPoints = null;
            BelongLayer = null;
        }

#if !PocketPC

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // TODO: Radius, IsFilled
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="context">内容</param>
        protected Circle_GMap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // TODO: Radius, IsFilled
        }

        #endregion

#endif

        #region ICircle

        /// <summary>
        /// 获取圆心坐标
        /// </summary>
        /// <returns></returns>
        public MapLngLat GetCenterDot()
        {
            return position;
        }

        /// <summary>
        /// 获取半径,单位是米
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            return this.radius;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="centerDot">圆心坐标</param>
        public void UpdatePosition(MapLngLat centerDot)
        {
            this.position = centerDot;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="radius">半径</param>
        public void UpdatePosition(double radius)
        {
            this.radius = radius;

            Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="centerDot">圆心坐标</param>
        /// <param name="radius">半径</param>
        public void UpdatePosition(MapLngLat centerDot, double radius)
        {
            this.position = centerDot;
            this.radius = radius;

            Update();
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor">填充色</param>
        public void SetFillColor(Color fillColor)
        {
            Fill = new SolidBrush(fillColor);
            Update();
        }

        /// <summary>
        /// 设置轮廓
        /// </summary>
        /// <param name="color">轮廓颜色</param>
        /// <param name="width">线宽</param>
        public void SetStroke(Color color, float width)
        {
            Stroke = new Pen(color, width);
            Update();
        }

        #endregion

        #region IElement

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
            get { return base.Tag.ToString(); }
            set { base.Tag = value; }
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
        /// <param name="_isHightLight"></param>
        public void HightLight(bool _isHightLight)
        {
            isHightLight = _isHightLight;
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="_isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool _isFlash, int interval = 500)
        {
            if (this.isFlash == _isFlash) return;
            if (_isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                this.IsVisible = true;
            }
            this.isFlash = _isFlash;
        }

        /// <summary>
        /// 显示/隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            this.IsVisible = isVisible;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (this.BelongLayer != null)
                this.BelongLayer.Refresh();
        }
        #endregion

        ///// <summary>
        ///// 重置大小
        ///// </summary>
        //private void ResetSize()
        //{
        //    if (Overlay == null) return;
        //    //int R = (int)((radius) / Overlay.Control.MapProvider.Projection.GetGroundResolution((int)Overlay.Control.Zoom, Position.Lat)) * 2;//可能有用

        //    int offset = CalcuOffset();
        //    // 设置Marker的偏移量，Rectangle的位置将会产生偏移
        //    this.Offset = new Point(new Size(-this.Size.Width / 2, -(this.Size.Height / 2 + offset)));
        //}

        ///// <summary>
        ///// 计算偏移量
        ///// </summary>
        //private int CalcuOffset()
        //{
        //    listPoints.Clear();
        //    for (float ang = 0; ang <= 270; ang += 90)
        //    {
        //        PointLatLng point = GetPointByDistanceAndAngle((float)radius, this.Position, ang);
        //        GPoint p = Overlay.Control.FromLatLngToLocal(point);
        //        listPoints.Add(p);
        //    }

        //    int width = CalcuWidth();
        //    int height = CalcuHeight();

        //    // 重置Marker的Size
        //    this.Size = new System.Drawing.Size(width, height);

        //    return height - width;
        //}

        ///// <summary>
        ///// 求坐标点
        ///// </summary>
        ///// <param name="distance"></param>
        ///// <param name="point"></param>
        ///// <param name="angle"></param>
        ///// <returns></returns>
        //private PointLatLng GetPointByDistanceAndAngle(float distance, PointLatLng point, double angle)
        //{
        //    double lng1 = point.Lng;
        //    double lat1 = point.Lat;
        //    // 将距离转换成经度的计算公式 * Math.PI / 180
        //    double lon = lng1 + (distance / 1000 * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat1 * Math.PI / 180));
        //    // 将距离转换成纬度的计算公式
        //    double lat = lat1 + (distance / 1000 * Math.Cos(angle * Math.PI / 180)) / 111;

        //    PointLatLng newPoint = new PointLatLng();
        //    newPoint.Lng = lon;
        //    newPoint.Lat = lat;

        //    return newPoint;
        //}

        ///// <summary>
        ///// 计算宽
        ///// </summary>
        ///// <returns></returns>
        //private int CalcuWidth()
        //{
        //    var point1 = listPoints[1];
        //    var point2 = listPoints[3];

        //    return Math.Abs((int)point1.X - (int)point2.X);
        //}

        ///// <summary>
        ///// 计算高
        ///// </summary>
        ///// <returns></returns>
        //private int CalcuHeight()
        //{
        //    var point1 = listPoints[0];
        //    var point2 = listPoints[2];

        //    return Math.Abs((int)point1.Y - (int)point2.Y);
        //}

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
    }
}