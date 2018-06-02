/**************************************************************************
 * 类名：I3DModel.cs
 * 描述：模型接口
 * 作者：cj
 * 日期：September 8,2016
 * 
 * ************************************************************************/
using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 三维模型接口
    /// </summary>
    public interface I3DModel : IMFElement
    {
        /// <summary>
        /// 模型文件位置
        /// </summary>
        string ModelFilePath { get; set; }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lnglat">坐标</param>
        void UpdatePosition(MapLngLat lnglat);

        /// <summary>
        /// 更新模型
        /// </summary>
        /// <param name="lnglat">坐标</param>
        /// <param name="angle">角度</param>
        void UpdateModel(MapLngLat lnglat, double angle);

        /// <summary>
        /// 获取坐标（存在问题）
        /// </summary>
        /// <returns></returns>
        MapLngLat GetPosition();

        /// <summary>
        /// 设置模型角度
        /// </summary>
        /// <param name="angle">角度</param>
        void SetAngle(double angle);

        /// <summary>
        /// 设置模型颜色
        /// </summary>
        /// <param name="color"></param>
        void SetColor(Color color);

        /// <summary>
        /// 设置模型颜色
        /// </summary>
        /// <param name="argb">argb</param>
        void SetColor(int argb);

        /// <summary>
        /// 设置模型颜色
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        void SetColor(int r, int g, int b);

        /// <summary>
        /// 设置模型颜色
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        void SetColor(int a, int r, int g, int b);

        /// <summary>
        /// 设置模型比例
        /// </summary>
        /// <param name="scale">比例</param>
        void SetScale(double scale);

        /// <summary>
        /// 设置Tip内容（未实现）
        /// </summary>
        /// <param name="tipText">Tip内容</param>
        void SetTipText(string tipText);

        /// <summary>
        /// 设置Tip显示方式（未实现）
        /// </summary>
        /// <param name="showType">显示方式</param>
        void SetTipShow(ShowTypeEnum showType);

    }
}
