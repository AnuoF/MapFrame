
using System.Collections.Generic;
using GMap.NET.WindowsForms;
using MapFrame.GMap.Model;
using System.Drawing;
using System.Drawing.Imaging;
using MapFrame.GMap.Common;
using System.Drawing.Drawing2D;
using GMap.NET;
using System;
using System.Diagnostics;

namespace MapFrame.GMap.Element
{
    /// <summary>
    /// 点
    /// </summary>
    public class HeatPointMaker : GMapMarker
    {
        /// <summary>
        /// 宽
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// 透明度
        /// </summary>
        public float Opacity { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public ColorRamp ColorRamp { get; set; }
        /// <summary>
        /// 点集合
        /// </summary>
        public List<HeatPoint> HeatPoints { get; set; }
        /// <summary>
        /// bitmap
        /// </summary>
        public Bitmap GrayMap { get; private set; }

        private Bitmap HeatMap = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p">点位置</param>
        public HeatPointMaker(PointLatLng p)
            : base(p)
        {
            Width = 1000;
            Height = 1000;
            Radius = 20;
            Opacity = 50;

            HeatPoints = randomPoints(Width, Height, 1);
        }

        /// <summary>
        /// 重绘
        /// </summary>
        /// <param name="g"></param>
        public override void OnRender(Graphics g)
        {
            //var result = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            //var graphics = Graphics.FromImage(result);

            //SizeLatLng size = this.Overlay.Control.ViewArea.Size;
            //Width = (int)size.WidthLng;
            //Height = (int)size.HeightLat;
            //HeatPoints = randomPoints(Width, Height, 1);

            //var grayRamp = ColorUtil.GetGrayRamp();
            //foreach (var point in this.HeatPoints)
            //{
            //    var r = this.Radius;
            //    var rect = new Rectangle((int)point.X - (int)r, (int)point.Y - (int)r, (int)r * 2, (int)r * 2);

            //    var path = new GraphicsPath();
            //    path.AddEllipse(rect);
            //    var brush = new PathGradientBrush(path);

            //    brush.InterpolationColors = grayRamp;
            //    g.FillEllipse(brush, rect);
            //}

            //Image image = Image.FromFile(@"d:\printer.png");
            //Rectangle bitmapRct = new Rectangle(0, 0, Width, Height);
            //g.DrawImage(HeatMap, 0, 0);

            HeatMap = MakeHeatMap();
            if (HeatMap != null)
            {
                //Rectangle bitmapRct = new Rectangle(0, 0, Width, Height);
                g.DrawImage(HeatMap, 0f, 0f);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// BitMap
        /// </summary>
        /// <returns></returns>
        public Bitmap MakeHeatMap()
        {
            var result = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            this.GrayMap = this.makeGrayMap();

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    var grayVal = this.GrayMap.GetPixel(x, y);
                    var index = grayVal.A;
                    var color = ColorUtil.GetColorInRamp(index, this.ColorRamp);
                    result.SetPixel(x, y, color);

                    //Debug.WriteLine(index);
                }
            }

            return ColorUtil.AdjustOpacity(result, this.Opacity);
        }

        private Bitmap makeGrayMap()
        {
            var result = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(result);

            var grayRamp = ColorUtil.GetGrayRamp();
            foreach (var point in this.HeatPoints)
            {
                var r = this.Radius;
                var rect = new Rectangle((int)point.X - (int)r, (int)point.Y - (int)r, (int)r * 2, (int)r * 2);

                var path = new GraphicsPath();
                path.AddEllipse(rect);
                var brush = new PathGradientBrush(path);

                brush.InterpolationColors = grayRamp;
                graphics.FillEllipse(brush, rect);
            }
            graphics.Dispose();

            return result;
        }

        List<HeatPoint> randomPoints(int width, int height, int count)
        {
            var result = new List<HeatPoint>();

            var r = new Random();
            for (int i = 0; i < count; i++)
            {
                var x = r.Next(width);
                var y = r.Next(height);
                //var w = (float)r.NextDouble()/2;

                var point = new HeatPoint
                {
                    X = x,
                    Y = y,
                    W = 1
                };

                result.Add(point);
            }

            return result;
        }

    }
}
