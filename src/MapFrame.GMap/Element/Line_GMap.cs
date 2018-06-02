/**************************************************************************
 * 类名：Line_GMap.cs
 * 描述：GMap线图元
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Timers;
using MapFrame.Core.Common;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// GMap线图元
    /// </summary>
    public class Line_GMap : GMapRoute, IMFLine
    {
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 自定义画笔
        /// </summary>
        private Pen mPen = null;
        /// <summary>
        /// 画线的点数组
        /// </summary>
        private Point[] pointArr = null;

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
        /// 图元索引
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
        /// 图元是否在闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">图元名称</param>
        /// <param name="line">线属性</param>
        public Line_GMap(string name, KmlLineString line)
            : base(name)
        {

            this.ElementName = name;
            this.ElementType = ElementTypeEnum.Line;
            this.Description = line.Description;
            // 鼠标经过可见
            base.IsHitTestVisible = true;
            List<PointLatLng> pointList = new List<PointLatLng>();
            foreach (MapLngLat latLng in line.PositionList)
            {
                PointLatLng p = new PointLatLng(latLng.Lat, latLng.Lng);
                pointList.Add(p);
            }

            this.Points.AddRange(pointList);
            this.ElementName = name;

            float penWidth = line.Width > 0 ? line.Width : 1;

            Color penColor = line.Color;
            mPen = new Pen(penColor, penWidth);

            isFlash = false;
            // 计时器
            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += flashTimer_Elapsed;
            flashTimer.Interval = 500;

            base.Tag = this;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            lock (this.Points)
            {
                pointArr = this.Overlay.Control.UpdateRouteToArray(this);
            }

            ThreadPool.QueueUserWorkItem(obj =>
            {
                this.UpdateRoutePosition();
            });

            if (pointArr == null) return;
            if (pointArr.Length < 2) return;

            lock (mPen)
            {
                g.DrawLines(mPen, pointArr);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (mPen != null)
            {
                lock (mPen)
                {
                    mPen.Dispose();
                    mPen = null;
                }
            }

            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
                flashTimer = null;
            }

            BelongLayer = null;
            isHightLight = false;
            isFlash = false;
            pointArr = null;
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            lock (mPen)
            {
                if (isHightLight)
                {
                    mPen.Width = mPen.Width + 3;
                }
                else
                {
                    mPen.Width = mPen.Width - 3;
                }
            }

            this.isHightLight = isHightLight;
            Update();
        }

        /// <summary>
        /// 显示、隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            this.IsVisible = isVisible;
        }

        /// <summary>
        /// 设置线宽
        /// </summary>
        /// <param name="width">宽度</param>
        public bool SetWidth(float width)
        {
            if (width > 0)
            {
                lock (mPen)
                    mPen.Width = width;

                Update();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="color">Color</param>
        public void SetColor(Color color)
        {
            lock (mPen)
            {
                mPen.Color = color;
            }

            Update();
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        public void SetColor(int argb)
        {
            Color c = Color.FromArgb(argb);
            SetColor(c);
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">gren</param>
        /// <param name="b">blue</param>
        public void SetColor(int r, int g, int b)
        {
            Color c = Color.FromArgb(r, g, b); ;
            SetColor(c);
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="a">alpha</param>
        /// <param name="r">red</param>
        /// <param name="g">gren</param>
        /// <param name="b">blue</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color c = Color.FromArgb(a, r, g, b);
            SetColor(c);
        }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="lngLat"></param>
        public void AddPoint(MapLngLat lngLat)
        {
            ThreadPool.QueueUserWorkItem(obj =>
                {
                    lock (this.Points)
                    {
                        this.Points.Add(new PointLatLng(lngLat.Lat, lngLat.Lng));
                    }
                });

            this.Update();
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lngLat"></param>
        public void RemovePoint(MapLngLat lngLat)
        {
            ThreadPool.QueueUserWorkItem(obj =>
                {
                    lock (this.Points)
                    {
                        this.Points.Remove(new PointLatLng(lngLat.Lat, lngLat.Lng));
                    }
                });

            this.Update();
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
        /// 更新线位置
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public bool UpdatePosition(List<MapLngLat> pList)
        {
            lock (this.Points)
            {
                this.Points.Clear();
                foreach (var item in pList)
                {
                    PointLatLng p = new PointLatLng(item.Lat, item.Lng);
                    this.Points.Add(p);
                }
            }

            Update();
            return true;
        }

        /// <summary>
        /// 刷新，内部用主动刷新，由外部调用刷新
        /// </summary>
        public void Update()
        {
            if (this.BelongLayer != null)
                this.BelongLayer.Refresh();
        }

        /// <summary>
        /// 获取线的经纬度集合
        /// </summary>
        /// <returns></returns>
        public List<MapLngLat> GetLngLat()
        {
            lock (this.Points)
            {
                List<MapLngLat> pointList = new List<MapLngLat>();

                foreach (PointLatLng p in this.Points)
                {
                    MapLngLat point = new MapLngLat(p.Lng, p.Lat);
                    pointList.Add(point);
                }

                return pointList;
            }
        }

        /// <summary>
        /// 更新线条位置
        /// </summary>
        private void UpdateRoutePosition()
        {
            #region MyRegion

            //for (int i = 0; i < this.Points.Count;i++ )
            //{
            //    // 判断线是否在可视区域
            //    if (this.Overlay.Control.ViewArea.Contains(this.Points[i]) && bIsContain != true)
            //    {
            //        bIsContain = true;
            //    }

            //    GPoint gPoint = this.Overlay.Control.FromLatLngToLocal(this.Points[i]);
            //    //gPoint.OffsetNegative(this.Overlay.Control.Core.renderOffset);

            //    Point p = new Point((int)gPoint.X, (int)gPoint.Y);
            //    pointArr[i] = p;
            //}

            //bIsContain = false;

            //foreach (PointLatLng p in this.Points)
            //{
            //    if (this.Overlay.Control.ViewArea.Contains(p))
            //    {
            //        bIsContain = true;
            //        break;
            //    }
            //}

            //if (bIsContain == false) return;

            if (this.Overlay.Control.InvokeRequired)
            {
                this.Overlay.Control.Invoke(new Action(delegate
                {
                    this.Overlay.Control.UpdateRouteLocalPosition(this);
                }));
            }
            else
            {
                this.Overlay.Control.UpdateRouteLocalPosition(this);
            }
            #endregion
        }

        private void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IsVisible = !IsVisible;
            if (this.Overlay.Control.InvokeRequired)
            {
                this.Overlay.Control.Invoke(new Action(delegate
                {
                    this.IsVisible = IsVisible;
                }));
            }
            else
                this.IsVisible = IsVisible;
        }

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <returns></returns>
        public double GetDistance()
        {
            List<MapLngLat> lengthPoints = new List<MapLngLat>();

            foreach (var p in this.Points)
            {
                lengthPoints.Add(new MapLngLat(p.Lng, p.Lat));
            }
            return Utils.CalculateLineLength(lengthPoints);
        }
    }
}
