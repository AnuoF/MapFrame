/**************************************************************************
 * 类名：IPicture.cs
 * 描述：图片图元接口
 * 作者：Allen
 * 日期：Aug 24,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 图片图元接口
    /// </summary>
    public interface IMFPicture : IMFElement
    {
        /// <summary>
        /// 标牌是否显示
        /// </summary>
        bool IsLableVisible { get; }

        /// <summary>
        /// 标牌显示方式
        /// </summary>
        ShowTypeEnum LabelShowType { get; }

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
        /// 设置点的图片
        /// </summary>
        /// <param name="bitmap">图片路径</param>
        void SetIcon(string bitmap);

        /// <summary>
        /// 设置方向
        /// </summary>
        /// <param name="angle">与正北的夹角</param>
        void SetAngle(float angle);

        /// <summary>
        /// 设置图标缩放比列
        /// </summary>
        /// <param name="scale">比列，大于0的浮点数</param>
        void SetScale(float scale);

        ///// <summary>
        ///// 设置大小
        ///// </summary>
        ///// <param name="size"></param>
        //void SetSize(double size);

        /// <summary>
        /// 获取图元位置坐标
        /// </summary>
        /// <returns>坐标</returns>
        MapLngLat GetLngLat();

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

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="labelText">标牌内容</param>
        void SetLabelText(string labelText);

        /// <summary>
        /// 设置标牌显示方式
        /// </summary>
        /// <param name="showType"></param>
        void SetLableShow(ShowTypeEnum showType);

        /// <summary>
        /// 设置颜色，此接口只对字体库生成的图标有效
        /// </summary>
        /// <param name="color">颜色</param>
        void SetColor(Color color);

    }
}
