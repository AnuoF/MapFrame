/**************************************************************************
 * 类名：IPolygon.cs
 * 描述：面接口
 * 作者：陈静
 * 日期：July 14,2016
 * 
 * ************************************************************************/
using System.Collections.Generic;
using System.Drawing;
using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 面接口
    /// </summary>
    public interface IMFPolygon : IMFElement
    {
        /// <summary>
        /// 轮廓颜色
        /// </summary>
        Color OutLineColor { get; }

        /// <summary>
        /// 填充色
        /// </summary>
        Color FillColor { get; }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="lnglat">坐标</param>
        void AddPoint(MapLngLat lnglat);

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lnglat">坐标</param>
        void RemovePoint(MapLngLat lnglat);

        /// <summary>
        /// 设置轮廓颜色
        /// </summary>
        /// <param name="outlineColor">轮廓颜色</param>
        /// <returns></returns>
        bool SetOutLineColor(int outlineColor);

        /// <summary>
        /// 设置轮廓颜色
        /// </summary>
        /// <param name="color">轮廓颜色</param>
        /// <returns></returns>
        bool SetOutLineColor(Color color);

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor">填充色</param>
        /// <returns></returns>
        bool SetFillColor(int fillColor);

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="color">填充色</param>
        /// <returns></returns>
        bool SetFillColor(Color color);

        /// <summary>
        /// 设置轮廓线粗细
        /// </summary>
        /// <param name="size">大小</param>
        /// <returns></returns>
        bool SetOutLineSize(float size);

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="oldLngLat">旧的位置</param>
        /// <param name="newLngLat">新的位置</param>
        /// <returns></returns>
        bool UpdatePosition(MapLngLat oldLngLat, MapLngLat newLngLat);

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="pList">点集合</param>
        /// <returns></returns>
        bool UpdatePosition(List<MapLngLat> pList);

        /// <summary>
        /// 获取多边形坐标集合
        /// </summary>
        /// <returns></returns>
        List<MapLngLat> GetLngLat();

        #region ArcMap

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity"></param>
        void SetOpacity(int _opacity);

        /// <summary>
        /// 获取多边形的面积
        /// </summary>
        /// <returns></returns>
        double GetArea();

        ///// <summary>
        ///// 面积
        ///// </summary>
        //double Area { get; }

        #endregion

    }
}
