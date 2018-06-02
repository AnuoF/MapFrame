/**************************************************************************
 * 类名：Utils.cs
 * 描述：全局对象类
 * 作者：Allen
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapFrame.GMap.Common
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

        #region 保留
        
        ///// <summary>
        ///// 根据距离和方位角获取点
        ///// </summary>
        ///// <param name="distance">距离</param>
        ///// <param name="point">点</param>
        ///// <param name="angle">方位角</param>
        ///// <returns>点</returns>
        //public static MapLngLat GetPointByDistanceAndAngle(float distance, MapLngLat point, double angle)
        //{
        //    try
        //    {
        //        double lng1 = point.Lng;
        //        double lat1 = point.Lat;
        //        // 将距离转换成经度的计算公式
        //        double lon = lng1 + (distance * Math.Sin(angle * Math.PI / 180)) / (111 * Math.Cos(lat1 * Math.PI / 180));
        //        // 将距离转换成纬度的计算公式
        //        double lat = lat1 + (distance * Math.Cos(angle * Math.PI / 180)) / 111;

        //        MapLngLat newPoint = new MapLngLat();
        //        newPoint.Lng = lon;
        //        newPoint.Lat = lat;
        //        return newPoint;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 拷贝图片，深拷贝
        ///// </summary>
        ///// <param name="source">源文件</param>
        ///// <returns>副本</returns>
        //private Bitmap CopyBitmap(Bitmap source)
        //{
        //    int depth = Bitmap.GetPixelFormatSize(source.PixelFormat);
        //    if (depth != 8 && depth != 24 && depth != 32)
        //    {
        //        return null;
        //    }

        //    Bitmap destination = new Bitmap(source.Width, source.Height, source.PixelFormat);
        //    BitmapData source_bitmapdata = null;
        //    BitmapData destination_bitmapdata = null;

        //    try
        //    {
        //        source_bitmapdata = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);
        //        destination_bitmapdata = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.ReadWrite, destination.PixelFormat);

        //        unsafe
        //        {
        //            byte* source_ptr = (byte*)source_bitmapdata.Scan0;
        //            byte* destination_ptr = (byte*)destination_bitmapdata.Scan0;

        //            for (int i = 0; i < (source.Width * source.Height * (depth / 8)); i++)
        //            {
        //                *destination_ptr = *source_ptr;
        //                source_ptr++;
        //                destination_ptr++;
        //            }
        //        }
        //        source.UnlockBits(source_bitmapdata);
        //        destination.UnlockBits(destination_bitmapdata);

        //        return destination;
        //    }
        //    catch (Exception ex)
        //    {
        //        destination.Dispose();
        //        return null;
        //    }
        //}
        #endregion

    }
}
