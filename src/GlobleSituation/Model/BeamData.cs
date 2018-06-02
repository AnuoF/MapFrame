using System;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 卫星波束数据
    /// </summary>
    public class BeamData : EventArgs
    {
        /// <summary>
        /// 卫星ID
        /// </summary>
        public int SatelliteId { get; set; }

        /// <summary>
        /// 波束ID
        /// </summary>
        public int BeamId { get; set; }

        /// <summary>
        /// 位置点的类型 0-卫星；1-波束
        /// </summary>
        public int PointType { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public MapLngLat Point { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            return string.Format("卫星{0}-波束{1}", SatelliteId, BeamId);
        }

        /// <summary>
        /// 转换为字节流
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            byte[] data = new byte[36];

            //卫星ID
            Buffer.BlockCopy(BitConverter.GetBytes(SatelliteId), 0, data, 0, 4);
            //波束ID
            Buffer.BlockCopy(BitConverter.GetBytes(BeamId), 0, data, 4, 4);
            //位置点的类型 0-卫星；1-波束
            Buffer.BlockCopy(BitConverter.GetBytes(PointType), 0, data, 8, 4);
            //位置
            Buffer.BlockCopy(Point.ToByte(), 0, data, 12, Point.ToByte().Length);

            return data;
        }

        /// <summary>
        /// 字节流转成类对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BeamData ByteToClass(byte[] data)
        {
            BeamData beam = new BeamData();

            beam.SatelliteId = BitConverter.ToInt32(data, 0);
            beam.BeamId = BitConverter.ToInt32(data, 4);
            beam.PointType = BitConverter.ToInt32(data, 8);

            byte[] arr = new byte[24];
            Buffer.BlockCopy(data, 12, arr, 0, 24);
            beam.Point = MapLngLat.ByteToClass(arr);

            return beam;
        }

    }
}
