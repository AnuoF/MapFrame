/**************************************************************************
 * 类名：IMoveObjectMgis.cs
 * 描述：动目标接口（针对MGIS使用）
 * 作者：CJ
 * 日期：July 4,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 动目标接口（针对MGIS使用）
    /// </summary>
    public interface IMoveObjectMgis : IMFPoint
    {
        /// <summary>
        /// 目标是否处于被跟踪状态
        /// </summary>
        bool IsFollowing { get; }

        /// <summary>
        /// 跟踪
        /// </summary>
        /// <param name="flag">true跟踪,false不跟踪</param>
        void Follow(bool flag);

        /// <summary>
        /// 设置正北夹角
        /// </summary>
        /// <param name="angle">正北夹角</param>
        void SetAngle(double angle);

        /// <summary>
        /// 重新设置图片
        /// </summary>
        /// <param name="icon">图片路径</param>
        void SetIcon(string icon);

        #region 标牌
        /// <summary>
        /// 标牌是否可见true显示,false隐藏
        /// </summary>
        bool BiaoPaiVisible { get; }

        /// <summary>
        /// 设置标牌是否显示
        /// </summary>
        /// <param name="visible">true显示,false隐藏</param>
        void SetBiaoPaiVis(bool visible);

        /// <summary>
        /// 设置标牌背景色
        /// </summary>
        /// <param name="bgColor"></param>
        void SetBiaoPaiBkCo(Color bgColor);

        /// <summary>
        /// 设置标牌文字颜色
        /// </summary>
        /// <param name="fontColor"></param>
        void SetBiaoPaiTxtCo(Color fontColor);

        /// <summary>
        /// 设置标牌线颜色
        /// </summary>
        /// <param name="lineColor"></param>
        void SetBiaoPaiLineCo(Color lineColor);

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="index">属性顺序索引</param>
        void SetBiaoPaiContent(string attributeName, string value, int index = -1);

        /// <summary>
        /// 设置标牌附加内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="attachedIndex">附加属性索引</param>
        void SetBiaoPaiAttContent(string attributeName, string value, int attachedIndex = 1);

        #endregion

        #region 轨迹线
        /// <summary>
        /// 设置轨迹线是否可见
        /// </summary>
        /// <param name="visible">true:可见 false:隐藏</param>
        void SetTrackVisible(bool visible);

        /// <summary>
        /// 设置航迹点容量
        /// </summary>
        /// <param name="trackPointNum"></param>
        void SetTrackMaxPoint(int trackPointNum = 65000);

        /// <summary>
        /// 设置航迹点类型
        /// </summary>
        /// <param name="strip"></param>
        void SetTrackLineStrip(int strip = 1);

        /// <summary>
        /// 设置航迹线颜色
        /// </summary>
        /// <param name="lineColor"></param>
        void SetTrackLineColor(Color lineColor);

        /// <summary>
        /// 设置航迹点大小
        /// </summary>
        /// <param name="size"></param>
        void SetTrackPointSize(int size = 5);

        /// <summary>
        /// 设置航迹线宽度
        /// </summary>
        /// <param name="width"></param>
        void SetTrackPointWidth(int width);
        #endregion

        #region Tip

        /// <summary>
        /// tip是否可见true显示,false隐藏
        /// </summary>
        bool TipsVisible { get; }

        /// <summary>
        /// 设置Tip是否显示
        /// </summary>
        /// <param name="visible">true显示,false隐藏</param>
        void SetTipVisible(bool visible);

        /// <summary>
        /// 设置Tip背景颜色
        /// </summary>
        /// <param name="bgColor"></param>
        void SetTipBgColor(Color bgColor);

        /// <summary>
        /// 设置Tip文字颜色
        /// </summary>
        /// <param name="fontColor"></param>
        void SetTipTextColor(Color fontColor);

        /// <summary>
        /// 设置Tip内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="index">属性索引</param>
        void SetTipContent(string attributeName, string value, int index = -1);

        /// <summary>
        /// 设置Tip附加内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="attachIndex">附加属性索引</param>
        void SetTipAttachedContent(string attributeName, string value, int attachIndex = 1);
        #endregion
    }
}
