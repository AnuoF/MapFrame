/**************************************************************************
 * 类名：KmlPicture.cs
 * 描述：图标类
 * 作者：Allen
 * 日期：Aug 24,2016
 * 
 * ************************************************************************/

using System.Drawing;
using System.Xml.Serialization;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 图标类
    /// </summary>
    [XmlType(TypeName = "Picture")]
    public class KmlPicture : KmlBaseGraph
    {
        /// <summary>
        /// 图标大小
        /// </summary>
        private float _sacle = 1;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 图标颜色，只针对字体库生成的图标
        /// </summary>
        public Color IconColor
        {
            get; set;
        }

        /// <summary>
        /// 图标缩放比列，默认为1，不缩放
        /// </summary>
        public float Scale
        {
            get { return _sacle; }
            set { _sacle = value; }
        }

        /// <summary>
        /// 图标路径
        /// 可以是图片路径，如D:\test.png；
        /// 或者是字体库中的符号，格式为“ttf文件|字符代码|字体大小”，如D:\TTMITC.TTF|88|16。
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Tip内容
        /// </summary>
        public string TipText { get; set; }

        /// <summary>
        /// 标牌内容
        /// </summary>
        public string LabelText { get; set; }

        /// <summary>
        /// 栅格化  栅格化之后图形贴图地面  未栅格化图形为空间图形
        /// </summary>
        [XmlIgnore]
        public bool Rasterize { get; set; }

        /// <summary>
        /// 字体库路径
        /// </summary>
        public string FontPath { get; set; }

        /// <summary>
        /// 字体代码
        /// </summary>
        public int Code { get; set; }

    }
}
