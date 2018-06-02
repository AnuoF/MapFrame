
using System.Collections.Generic;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    public class TrackPoint
    {
        /// <summary>
        /// 目标索引
        /// </summary>
        public uint Index;

        /// <summary>
        /// 航迹点的名称
        /// </summary>
        public string PointName;

        /// <summary>
        /// 位置
        /// </summary>
        public MapLngLat Position;

    }

    /// <summary>
    /// 航迹线
    /// </summary>
    public class TrackLine
    {
        /// <summary>
        /// 所属模型名称
        /// </summary>
        public string ModelName;

        /// <summary>
        /// 目标类型
        /// </summary>
        public byte TargetType;

        /// <summary>
        /// 航迹线的名称
        /// </summary>
        public string LineName;

        /// <summary>
        /// 航迹点
        /// </summary>
        public List<TrackPoint> Points;
    }


}
