
using ESRI.ArcGIS.Display;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    public class Model3D
    {
        /// <summary>
        /// 模型的轨迹图层
        /// </summary>
        public string TrackLayerName { get; set; }

        /// <summary>
        /// 模型坐标
        /// </summary>
        public MapLngLat Coordinate { get; set; }

        /// <summary>
        /// 所在图层名
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// 模型大小比例
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 模型文件位置
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// 模型颜色
        /// </summary>
        public IRgbColor ModelColor { get; set; }

        ///// <summary>
        ///// 模型类型
        ///// </summary>
        //public ModelEnum ModelType { get; set; }

        /// <summary>
        /// 航向
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 模型名字
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 目标类型
        /// </summary>
        public byte TargetType { get; set; }

    }
}
