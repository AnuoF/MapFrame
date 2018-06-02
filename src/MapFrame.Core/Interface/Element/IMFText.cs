/**************************************************************************
 * 类名：IText.cs
 * 描述：文本图元接口
 * 作者：Allen
 * 日期：July 4,2016
 * 
 * ************************************************************************/

using System.Drawing;
using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 文本图元接口
    /// </summary>
    public interface IMFText : IMFElement
    {
        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="context">内容</param>
        /// <returns></returns>
        bool SetContext(string context);

        /// <summary>
        /// 获取文本内容
        /// </summary>
        /// <returns>文本内容</returns>
        string GetContext();

        /// <summary>
        /// 设置文字的颜色
        /// </summary>
        /// <param name="color">颜色</param>
        void SetColor(Color color);

        /// <summary>
        /// 设置文字的颜色
        /// </summary>
        /// <param name="argb">argb</param>
        void SetColor(int argb);

        /// <summary>
        /// 设置文字的颜色
        /// </summary>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        void SetColor(int r, int g, int b);

        /// <summary>
        /// 设置文字的颜色
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        void SetColor(int a, int r, int g, int b);

        /// <summary>
        /// 获取当前文本颜色
        /// </summary>
        /// <returns></returns>
        Color GetColor();

        /// <summary>
        /// 设置字体大小
        /// </summary>
        /// <param name="size">大小</param>
        /// <returns></returns>
        bool SetSize(float size);

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="familyName">新 System.Drawing.Font 的 System.Drawing.FontFamily 的字符串表示形式。</param>
        /// <returns></returns>
        bool SetFont(string familyName);

        /// <summary>
        /// 获取文本格式
        /// </summary>
        /// <returns></returns>
        Font GetFont();

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="familyName">新 System.Drawing.Font 的 System.Drawing.FontFamily 的字符串表示形式。</param>
        /// <param name="emSize">新字体的全身大小（以磅值为单位）。</param>
        /// <param name="fontStyle">字体类型</param>
        /// <returns></returns>
        bool SetFont(string familyName, float emSize, FontStyle fontStyle = FontStyle.Regular);

        /// <summary>
        /// 获取图元位置坐标
        /// </summary>
        /// <returns>坐标</returns>
        MapLngLat GetLngLat();

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="position">位置</param>
        void UpdatePosition(MapLngLat position);


    }
}
