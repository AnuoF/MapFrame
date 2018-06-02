using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Mgis.Tool
{
    /// <summary>
    /// 绘制图像类型
    /// </summary>
    public enum DrawType
    {
        /// <summary>
        /// 折线
        /// </summary>
        FoldLine = 10,

        /// <summary>
        /// 多边形
        /// </summary>
        Polygon = 11,

        /// <summary>
        /// 正多边形
        /// </summary>
        //RegularPolygon = 12,

        /// <summary>
        /// 过点曲线
        /// </summary>
        //PassDotCurve = 13,

        /// <summary>
        /// 封闭曲线区域
        /// </summary>
        //SealCurve = 14,

        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle = 15,

        /// <summary>
        /// 圆形
        /// </summary>
        Circle = 16,

        /// <summary>
        /// 弧形
        /// </summary>
        //Arc = 17,

        /// <summary>
        /// 扇形
        /// </summary>
        //Sector = 18,

        /// <summary>
        /// 弓形
        /// </summary>
        //BowShape = 19,

        /// <summary>
        /// 椭圆
        /// </summary>
        //Ellipse = 20,

        /// <summary>
        /// 带方向的矩形
        /// </summary>
        //DirectionRectangle = 27,

        /// <summary>
        /// 矩形内切八字形
        /// </summary>
        //Rectangle8 = 28

    }
}
