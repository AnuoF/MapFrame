/**************************************************************************
 * 类名：IMapLogic.cs
 * 描述：地图逻辑接口
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/


using System.Collections.Generic;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 地图逻辑接口
    /// </summary>
    public interface IMapLogic
    {
        /// <summary>
        /// 获取地图控件接口
        /// </summary>
        /// <returns></returns>
        object GetMapControl();

        /// <summary>
        /// 获取地图控件接口
        /// </summary>
        /// <returns></returns>
        IMFMap GetIMFMap();

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        IMFLayer GetLayer(string layerName);

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="name">图层名称</param>
        /// <returns>图层对象</returns>
        IMFLayer AddLayer(string name);

        /// <summary>
        /// 获取所有图层
        /// </summary>
        /// <returns>图层</returns>
        List<IMFLayer> GetLayers();

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="name">图层名称</param>
        bool RemoveLayer(string name);

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layer">图层对象</param>
        bool RemoveLayer(IMFLayer layer);

        /// <summary>
        /// 移除所有图层
        /// </summary>
        /// <returns></returns>
        bool RemoveAllLayer();

        /// <summary>
        /// 设置图元显示或隐藏
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible">显示或隐藏</param>
        bool SetLayerVisible(string layerName, bool visible);

        /// <summary>
        /// 清除图层
        /// </summary>
        bool ClearLayer();

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <returns>图元</returns>
        IMFElement GetElement(string elementName);

        /// <summary>
        /// 获取工具箱对象
        /// </summary>
        /// <returns>工具箱对象</returns>
        IMFToolBox GetToolBox();

        /// <summary>
        /// 刷新
        /// </summary>
        void Refresh();

        /// <summary>
        /// 区域刷新
        /// </summary>
        /// <param name="layer">图层</param>
        void RefreshLayer(IMFLayer layer);

    }
}
