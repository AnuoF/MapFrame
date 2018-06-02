/**************************************************************************
 * 类名：EditPolygon.cs
 * 描述：编辑点
 * 作者：lx
 * 日期：Nov 10,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 图元类型枚举
    /// </summary>
    public enum ElementTypeEnum
    {
        /// <summary>
        /// 点图元
        /// </summary>
        Point = 0,

        /// <summary>
        /// 图片图元
        /// </summary>
        Picture = 1,

        /// <summary>
        /// 线图元
        /// </summary>
        Line,

        /// <summary>
        /// 面图元
        /// </summary>
        Polygon,

        /// <summary>
        /// 文本图元
        /// </summary>
        Text,

        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,

        /// <summary>
        /// 圆图元
        /// </summary>
        Circle,

        /// <summary>
        /// 3D模型
        /// </summary>
        Model3D,

        /// <summary>
        /// 其他图元
        /// </summary>
        Other
    }
}
