using MapFrame.Core.Model;
using System.Drawing;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 目标类
    /// </summary>
    class Plane
    {
        /// <summary>
        /// 模型名字
        /// </summary>
        public string Name { get; set; }

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
        /// 航向
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 目标图片路径
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 标牌内容
        /// </summary>
        public string LabelText { get; set; }

        /// <summary>
        /// 目标类型
        /// </summary>
        public byte TargetType { get; set; }


        private Color color = Color.Red;
        /// <summary>
        /// 目标颜色
        /// </summary>
        public Color PlaneColor
        {
            get { return color; }
            set { color = value; }
        }

    }
}
