/**************************************************************************
 * 类名：MapLngLat.cs
 * 描述：经纬度
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 经纬度
    /// </summary>
    public class MapLngLat
    {
        private double _lng = 0;
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng
        {
            get { return _lng; }
            set { _lng = value; }
        }

        private double _lat = 0;
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }

        private double _alt = 0;
        /// <summary>
        /// 高度
        /// </summary>
        public double Alt
        {
            get { return _alt; }
            set { _alt = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度，默认为0</param>
        public MapLngLat(double lng, double lat, double alt = 0)
        {
            _lng = lng;
            _lat = lat;
            _alt = alt;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MapLngLat()
        {

        }

        /// <summary>
        /// 将对象转成字节流
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            byte[] data = new byte[24];

            Buffer.BlockCopy(BitConverter.GetBytes(_lng), 0, data, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(_lat), 0, data, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(_alt), 0, data, 16, 8);

            return data;
        }

        /// <summary>
        /// 将字节流转成类对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MapLngLat ByteToClass(byte[] data)
        {
            MapLngLat lngLat = new MapLngLat();

            lngLat.Lng = BitConverter.ToDouble(data, 0);
            lngLat.Lat = BitConverter.ToDouble(data, 8);
            lngLat.Alt = BitConverter.ToDouble(data, 16);

            return lngLat;
        }
    }
}
