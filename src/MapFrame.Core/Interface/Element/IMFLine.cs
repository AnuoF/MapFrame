/**************************************************************************
 * 类名：ILine.cs
 * 描述：线接口
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;
using System.Drawing;
using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 线接口
    /// </summary>
    public interface IMFLine : IMFElement
    {
        /// <summary>
        /// 设置线宽
        /// </summary>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        bool SetWidth(float width);

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="color">颜色</param>
        void SetColor(Color color);

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        void SetColor(int argb);

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        void SetColor(int r, int g, int b);

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        void SetColor(int a, int r, int g, int b);

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="pList">线的端点集合</param>
        /// <returns>true，成功；false，失败</returns>
        bool UpdatePosition(List<MapLngLat> pList);

        /// <summary>
        /// 获取线的坐标集合
        /// </summary>
        /// <returns>true，成功；false，失败</returns>
        List<MapLngLat> GetLngLat();

        /// <summary>
        /// 添加点，如何刷新还是完善
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        void AddPoint(MapLngLat lngLat);

        /// <summary>
        /// 移除点，如何刷新还是完善
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        void RemovePoint(MapLngLat lngLat);

        /// <summary>
        /// 获取线的长度，单位米
        /// </summary>
        /// <returns>长度</returns>
        double GetDistance();

    }
}
