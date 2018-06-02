/**************************************************************************
 * 类名：IElement.cs
 * 描述：基本图元接口
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 基本图元接口
    /// </summary>
    public interface IMFElement : IDisposable
    {
        /// <summary>
        /// 标签
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// 图元指针如果是ulong就进行转化下（需要进一步确定是否需要这个属性）
        /// </summary>
        string ElementPtr { get; }

        /// <summary>
        /// 图元所属图层
        /// </summary>
        IMFLayer BelongLayer { get; set; }

        /// <summary>
        /// 图元描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 图元名称
        /// </summary>
        string ElementName { get; set; }

        /// <summary>
        /// 图元类型
        /// </summary>
        ElementTypeEnum ElementType { get; set; }

        /// <summary>
        /// 图元是否高亮
        /// </summary>
        bool IsHightLight { get; }

        /// <summary>
        /// 图元是否在闪烁
        /// </summary>
        bool IsFlash { get; }

        /// <summary>
        /// 是否可见true显示,false隐藏
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        void HightLight(bool isHightLight);

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">true，开始；false，停止</param>
        /// <param name="interval">闪烁间隔默认为500</param>
        void Flash(bool isFlash, int interval = 500);

        /// <summary>
        /// 显示、隐藏图元
        /// </summary>
        /// <param name="isVisible">true，显示；false，隐藏</param>
        void SetVisible(bool isVisible);

        /// <summary>
        /// 更新图元
        /// </summary>
        void Update();
    }
}
