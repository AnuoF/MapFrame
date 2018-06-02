using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Mgis.Common
{
    /// <summary>
    /// 全局对象类
    /// </summary>
    class Utils
    {
        private static int _elementIndex = 0;
        /// <summary>
        /// 图元索引
        /// </summary>
        public static int ElementIndex
        {
            get
            {
                _elementIndex++;
                return _elementIndex;
            }
        }

        /// <summary>
        /// 是否向外发布事件
        /// </summary>
        public static bool bPublishEvent = true;

        ///// <summary>
        ///// 地球半径，单位千米
        ///// </summary>
        //private const double Earth = 6378.137;
        ///// <summary>
        ///// 根据两点坐标求距离(GMap单位为米,Mgis单位为公里)
        ///// </summary>
        ///// <param name="listPoint">坐标点</param>
        ///// <returns></returns>
        //public static double GetDistance(MapFrame.Core.Model.MapLngLat p1,MapFrame.Core.Model.MapLngLat p2)
        //{
        //    double radlat1=  rad(p1.Lat);
        //    double radlat2 = rad(p2.Lat);
        //    double a = radlat1 - radlat2;
        //    double b = rad(p1.Lng) - rad(p2.Lng);
        //    double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(a / 2, 2) + Math.Cos(radlat1) * Math.Cos(radlat2) * Math.Pow(Math.Sin(b / 2), 2)));
        //    s = s * Earth;
        //    return Math.Round(s * 10000) / 10000;
        //}

        ///// <summary>
        ///// 计算长度
        ///// </summary>
        ///// <param name="pointList"></param>
        ///// <returns></returns>
        //public static double CalculateLength(List<MapFrame.Core.Model.MapLngLat> pointList)
        //{
        //    double length = 0;
        //    for (int i = 0; i < pointList.Count - 1; i++)
        //    {
        //        double segment = 0;
        //        segment = GetDistance(pointList[i], pointList[i + 1]);
        //        length += segment;
        //    }
        //    return length;
        //}
        ///// <summary>
        ///// 弧度
        ///// </summary>
        ///// <param name="d"></param>
        ///// <returns></returns>
        //private static double rad(double d)
        //{
        //    return d * Math.PI / 180;
        //}
    }
}
