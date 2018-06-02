/**************************************************************************
 * 类名：ICircle.cs
 * 描述：圆形图元接口
 * 作者：Allen
 * 日期：July 26,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 圆形图元接口
    /// </summary>
    public interface IMFCircle : IMFElement
    {
        /// <summary>
        /// 获取圆心坐标
        /// </summary>
        MapLngLat GetCenterDot();

        /// <summary>
        /// 获取半径
        /// </summary>
        /// <returns></returns>
        double GetRadius();

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="centerDot">圆心坐标</param>
        void UpdatePosition(MapLngLat centerDot);

        /// <summary>
        /// 更新圆的大小
        /// </summary>
        /// <param name="radius">半径（单位米）</param>
        void UpdatePosition(double radius);

        /// <summary>
        /// 更新圆的位置和大小
        /// </summary>
        /// <param name="centerDot">圆心坐标</param>
        /// <param name="radius">半径</param>
        void UpdatePosition(MapLngLat centerDot, double radius);

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor">填充颜色</param>
        void SetFillColor(Color fillColor);

        /// <summary>
        /// 设置轮廓
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="width">线宽</param>
        void SetStroke(Color color, float width);

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity"></param>
        void SetOpacity(int _opacity);

    }
}
