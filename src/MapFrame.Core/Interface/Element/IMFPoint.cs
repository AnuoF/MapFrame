/**************************************************************************
 * 类名：IPoint.cs
 * 描述：点图元接口
 * 作者：Allen
 * 日期：July 4,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 点图元接口
    /// </summary>
    public interface IMFPoint : IMFElement
    {
        /// <summary>
        /// 设置方向
        /// </summary>
        /// <param name="angle">与正北的夹角</param>
        void SetAngle(double angle);

        /// <summary>
        /// 更新点的位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        void UpdatePosition(double lng, double lat, double alt = 0);

        /// <summary>
        /// 更新点的位置
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        void UpdatePosition(MapLngLat lngLat);

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="color">颜色</param>
        void SetColor(Color color);

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        void SetColor(int argb);

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        void SetColor(int r, int g, int b);

        /// <summary>
        /// 设置点的颜色
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        void SetColor(int a, int r, int g, int b);

        /// <summary>
        /// 获取图元位置坐标
        /// </summary>
        /// <returns>坐标</returns>
        MapLngLat GetLngLat();


        #region GMap

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size">Size</param>
        void SetSize(Size size);

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        void SetSize(int width, int height = 0);

        /// <summary>
        /// 设置Tip内容
        /// </summary>
        /// <param name="tipText">Tip内容</param>
        void SetTipText(string tipText);

        /// <summary>
        /// 设置Tip显示方式
        /// </summary>
        /// <param name="showType">显示方式</param>
        void SetTipShow(ShowTypeEnum showType);

        #endregion


        #region MGIS

        ///// <summary>
        ///// 获取动目标
        ///// </summary>
        ///// <returns></returns>
        //ulong GetMoveObj();

        #endregion

    }
}
