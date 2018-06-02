/**************************************************************************
 * 类名：KmlText.cs
 * 描述：文字
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Xml.Serialization;
using System.Drawing;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 文字
    /// </summary>
    [XmlType(TypeName = "Text")]
    public class KmlText : KmlBaseGraph
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 位置，格式（经度，纬度）
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 文字内容
        /// </summary>
        public string Content { get; set; }

        private string _font = "宋体";
        /// <summary>
        /// 字体，如：宋体
        /// </summary>
        public string Font
        {
            get { return _font; }
            set { _font = value; }
        }

        private float _size = 9;
        /// <summary>
        /// 文字大小
        /// </summary>
        public float Size
        {
            get { return _size; }
            set { _size = value; }
        }

        private Color _color = Color.Blue;
        /// <summary>
        /// 文字颜色
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private FontStyle _fontStyle = FontStyle.Regular;
        /// <summary>
        /// 文本字形
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        /// <summary>
        /// 是否栅格化   栅格化之后贴近地图，未栅格为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }

        /// <summary>
        /// 文字kml构造函数
        /// </summary>
        public KmlText()
        {
            Position = new MapLngLat();
        }
    }
}
