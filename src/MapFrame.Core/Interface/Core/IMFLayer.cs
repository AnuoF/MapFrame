/**************************************************************************
 * 类名：IMFLayer.cs
 * 描述：图层接口，定义图层和图元的操作接口
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using System.Collections.Generic;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 图层接口
    /// </summary>
    public interface IMFLayer
    {
        /// <summary>
        /// 图层名称
        /// </summary>
        string LayerName { get; set; }

        /// <summary>
        /// 图层所在的地图控件
        /// </summary>
        object MapControl { get; set; }

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kmlStr">kml字符串</param>
        /// <returns>true，成功；false，失败</returns>
        bool AddElement(string kmlStr);

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kml">kml类对象</param>
        /// <returns>true，成功；false，失败</returns>
        bool AddElement(Kml kml);

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kml">kml类对象</param>
        /// <param name="_element">添加成功后的图元</param>
        /// <returns></returns>
        bool AddElement(Kml kml, out IMFElement _element);

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <returns>图元</returns>
        IMFElement GetElement(string elementName);

        /// <summary>
        /// 获取图层上所有图元
        /// </summary>
        /// <returns>图元集合</returns>
        Dictionary<string, IMFElement> GetElementDictionary();

        /// <summary>
        /// 获取图层上所有图元
        /// </summary>
        /// <returns>图元集合</returns>
        List<IMFElement> GetElementList();

        /// <summary>
        /// 获取当前图层上图元数量
        /// </summary>
        /// <returns></returns>
        int GetElementCount();

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="name">目标编号</param>
        /// <returns>true，成功；false，失败</returns>
        bool RemoveElement(string name);

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="name">目标编号</param>
        /// <returns>true，成功；false，失败</returns>
        /// <param name="element">图元</param>
        bool RemoveElement(string name, ref IMFElement element);

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <returns>true，成功；false，失败</returns>
        bool RemoveElement(IMFElement element);

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <returns>true，成功；false，失败</returns>
        bool RemoveElement(ref IMFElement element);

        /// <summary>
        /// 清除图元
        /// </summary>
        void ClearElement();

        /// <summary>
        /// 更新图层
        /// </summary>
        void Refresh();

        /// <summary>
        /// 显示、隐藏图层
        /// </summary>
        /// <param name="isVisible">显示或隐藏</param>
        void SetLayerVisible(bool isVisible);

        /// <summary>
        /// 设置图元显示或隐藏
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <param name="visible">显示隐藏</param>
        void SetElementVisible(string elementName, bool visible);

        /// <summary>
        /// 设置图层栅格化（三维地图使用的接口）
        /// </summary>
        /// <param name="bRasterize">是否栅格化</param>
        void SetRasterize(bool bRasterize);

        /// <summary>
        /// 获取图层栅格化的属性（三维地图接口）
        /// </summary>
        void GetRasterize();

        /// <summary>
        /// 设置图层透明度（三维地图接口）
        /// </summary>
        /// <param name="transparency">透明度</param>
        void SetTransparency(short transparency);
    }
}
