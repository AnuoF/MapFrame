
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobleSituation.Model;
using MapFrame.Core.Model;
using System.Data;

namespace GlobleSituation.Common
{
    class Utils
    {
        #region 变量
        /// <summary>
        /// 是否接受预警数据
        /// </summary>
        public static bool bStartWarn = true;
        /// <summary>
        /// 是否启动预警声音
        /// </summary>
        public static bool bStartSound = true;
        /// <summary>
        /// 是否启动预警气泡
        /// </summary>
        public static bool bStartTip = true;
        /// <summary>
        /// 航迹点最大数
        /// </summary>
        public static int TrackPointNum = 10;
        /// <summary>
        /// 预警目标数量，大于零则存在预警目标
        /// </summary>
        public static int WarnCount = 0;

        /// <summary>
        /// 当前显示模式：实时或历史态势
        /// </summary>
        public static ShowMode Mode = ShowMode.REAL_TIME;
        #endregion

        #region 方法
        
        /// <summary>
        /// 根据距离和方位角获取点
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
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
                newPoint.Alt = 0;
                return newPoint;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(Utils), ex);
                return null;
            }
        }

        /// <summary>
        /// 将指定对象序列化为二进制。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="iscompress">是否压缩。</param>
        public static byte[] SerializeDataTableToBytes(DataTable dt, bool iscompress)
        {
            dt.RemotingFormat = System.Data.SerializationFormat.Binary;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ms.Position = 0;
                System.Runtime.Serialization.IFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                if (iscompress)
                {
                    using (System.IO.Compression.DeflateStream ds = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Compress)) bf.Serialize(ds, dt);
                }
                else
                {
                    bf.Serialize(ms, dt);
                }
                return ms.ToArray();
            }
        }

        //解压byte[]还原成DataTable的方法
        /// <summary>
        /// 将二进制反序列化为指定的类型。
        /// </summary>
        /// <typeparam name="T">反序列化的目标类型。</typeparam>
        /// <param name="bytes">要反序列化的二进制数据。</param>
        /// <param name="isdecompress">是否解压缩。</param>
        public static DataTable DeserializeDataTableFromBytes(byte[] bytes, bool isdecompress)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
            {
                System.Runtime.Serialization.IFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                if (isdecompress)
                {
                    using (System.IO.Compression.DeflateStream ds = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress))
                    {
                        return (DataTable)bf.Deserialize(ds);
                    }
                }
                else
                {
                    return (DataTable)bf.Deserialize(ms);
                }
            }
        }

        #endregion
    }
}
