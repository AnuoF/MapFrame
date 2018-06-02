
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFrame.Core.Model;

namespace MapFrame.Core.Common
{
    /// <summary>
    /// 对外提供的公共方法
    /// </summary>
    public class Utils
    {

        /// <summary>
        /// 根据距离和方位角获取点
        /// </summary>
        /// <param name="distance">距离</param>
        /// <param name="point">点</param>
        /// <param name="angle">方位角</param>
        /// <returns>点</returns>
        public static MapLngLat GetPointByDistanceAndAngle(float distance, MapLngLat point, double angle)
        {
            try
            {
                double lng1 = point.Lng;
                double lat1 = point.Lat;
                // 将距离转换成经度的计算公式
                double lon = lng1 + (distance * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat1 * Math.PI / 180));
                // 将距离转换成纬度的计算公式
                double lat = lat1 + (distance * Math.Cos(angle * Math.PI / 180)) / 111;

                MapLngLat newPoint = new MapLngLat();
                newPoint.Lng = lon;
                newPoint.Lat = lat;
                return newPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 地球半径，单位千米
        /// </summary>
        private const double Earth = 6378.137;
        /// <summary>
        /// 根据两点坐标求距离(GMap、Mgis单位为公里)
        /// </summary>
        /// <param name="p1">坐标点1</param>
        /// <param name="p2">坐标点2</param>
        /// <returns></returns>
        public static double GetDistance(MapFrame.Core.Model.MapLngLat p1, MapFrame.Core.Model.MapLngLat p2)
        {
            double radlat1 = rad(p1.Lat);
            double radlat2 = rad(p2.Lat);
            double a = radlat1 - radlat2;
            double b = rad(p1.Lng) - rad(p2.Lng);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(a / 2, 2) + Math.Cos(radlat1) * Math.Cos(radlat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * Earth;
            return (float)Math.Round(s * 10000) / 10000;
        }

        /// <summary>
        /// 计算两点的方位角
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double CalculateAngle(MapLngLat p1, MapLngLat p2)
        {
            // 两种特殊情况  经度相同 或者 纬度相同
            if (p2.Lat == p1.Lat)      // 纬度相等
            {
                if (p2.Lng > p1.Lng)
                    return 90;
                else if (p2.Lng == p1.Lng)
                    return 0;
                else
                    return 270;
            }
            else if (p2.Lng == p1.Lng)  // 经度相等
            {
                if (p2.Lat >= p1.Lat)
                    return 0;
                else
                    return 180;
            }

            //计算角度
            double zqz = Math.Abs((p1.Lng - p2.Lng)) / Math.Abs((p1.Lat - p2.Lat));
            double zqzjd = Math.Atan(zqz);
            double angle = Math.Round(180 / Math.PI * zqzjd, 2);
            if ((p1.Lng - p2.Lng) < 0 && (p1.Lat - p2.Lat) < 0)//第一象限
            {

            }
            else if ((p1.Lng - p2.Lng) < 0 && (p1.Lat - p2.Lat) > 0) //第四象限
            {
                angle = 180 - angle;
            }
            else if ((p1.Lng - p2.Lng) > 0 && (p1.Lat - p2.Lat) > 0) //第三象限
            {
                angle += 180;
            }
            else if ((p1.Lng - p2.Lng) > 0 && (p1.Lat - p2.Lat) < 0) //第二象限
            {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// 获取距离
        /// </summary>
        /// <param name="points">点集合</param>
        /// <returns></returns>
        public static double GetDistance(List<MapLngLat> points)
        {
            return 0;
        }

        /// <summary>
        /// 计算长度(GMap单位为米,Mgis单位为公里)
        /// </summary>
        /// <param name="pointList"></param>
        /// <returns></returns>
        public static double CalculateLineLength(List<MapFrame.Core.Model.MapLngLat> pointList)
        {
            double length = 0;
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                double segment = 0;
                //segment = GetDistance(pointList[i].Lat, pointList[i].Lng, pointList[i + 1].Lat, pointList[i + 1].Lng);
                segment = GetDistance(pointList[i], pointList[i + 1]);
                length += segment;
            }
            return length;
        }

        /// <summary>
        /// 判断点是否在多边形内
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="points">多边形点集合</param>
        /// <returns></returns>
        public static bool IsInsidePolygon(MapLngLat point, List<MapLngLat> points)
        {
            int count = points.Count;
            if (count < 3) return false;

            bool result = false;
            for (int i = 0, j = count - 1; i < count; i++)
            {
                var p1 = points[i];
                var p2 = points[j];
                if (p1.Lat < point.Lat && p2.Lat >= point.Lat || p2.Lat < point.Lat && p1.Lat >= point.Lat)
                {
                    if (p1.Lng + (point.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) < point.Lng)
                    {
                        result = !result;
                    }
                }
                j = i;
            }

            return result;
        }

        /// <summary>
        /// 计算多边形的面积
        /// </summary>
        /// <param name="points">多边形点集合</param>
        /// <returns></returns>
        public static double GetPolygonArea(List<MapLngLat> points)
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the poylgon is oriented clockwise.
            return Math.Abs(SignedPolygonArea(points));
        }

        private static double SignedPolygonArea(List<MapLngLat> points)
        {
            // add the first point to the end.
            int pointsCount = points.Count;
            MapLngLat[] pts = new MapLngLat[pointsCount + 1];
            points.CopyTo(pts, 0);
            pts[pointsCount] = points[0];

            for (int i = 0; i < pointsCount + 1; ++i)
            {
                pts[i].Lat = pts[i].Lat * (System.Math.PI * 6378137 / 180);
                pts[i].Lng = pts[i].Lng * (System.Math.PI * 6378137 / 180);
            }

            // Get the areas.
            double area = 0;
            for (int i = 0; i < pointsCount; i++)
            {
                area += (pts[i + 1].Lat - pts[i].Lat) * (pts[i + 1].Lng + pts[i].Lng) / 2;
            }
            return area;
        }

        /// <summary>
        /// 弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double rad(double d)
        {
            return d * Math.PI / 180;
        }
    }
}
