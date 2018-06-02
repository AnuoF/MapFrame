/**************************************************************************
 * 类名：KmlModel3d.cs
 * 描述：3d模型对象
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;
using System.Drawing;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 3d模型对象
    /// </summary>
    [XmlType(TypeName = "Point")]
    public class KmlModel3d : KmlBaseGraph
    {
        private Color _color = Color.Red;
        private double _azimuth = 0;
        private double _scale = 10000;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 经纬度、高度
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 模型颜色
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// 模型文件位置
        /// </summary>
        public string ModelFilePath { get; set; }

        /// <summary>
        /// 模型大小
        /// </summary>
        public double Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        /// <summary>
        /// 偏航角度
        /// </summary>
        public double Azimuth
        {
            get { return _azimuth; }
            set { _azimuth = value; }
        }

        /// <summary>
        /// 栅格化  栅格化之后图形贴图地面  未栅格化图形为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }



        public KmlModel3d()
        {
            Position = new MapLngLat();
        }

    }
}
