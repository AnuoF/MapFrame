/**************************************************************************
 * 类名：IMapFactory.cs
 * 描述：地图工厂接口，用于定义与地图引擎相关的操作,根据不同的引擎加载不同的接口
 *       实现对象，如 FactoryGMap
 * 作者：Allen
 * 日期：July 11,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 地图工厂接口
    /// </summary>
    public interface IMapFactory
    {
        /// <summary>
        /// 获取地图控件
        /// </summary>
        /// <returns></returns>
        object GetMapControl();

        /// <summary>
        /// 获取地图对象接口
        /// </summary>
        /// <returns></returns>
        IMFMap GetIMFMap();

        #region Layer
        /// <summary>
        /// 刷新
        /// </summary>
        void Refresh();

        /// <summary>
        /// 刷新指定图层
        /// </summary>
        /// <param name="layer"></param>
        void Refresh(IMFLayer layer);

        /// <summary>
        /// 添加图层，调地图引擎
        /// </summary>
        /// <param name="layerName">图层名称</param>
        bool AddLayer(string layerName);

        /// <summary>
        /// 移除图层，调地图引擎
        /// </summary>
        /// <param name="layerName">图层名称</param>
        bool RemoveLayer(string layerName);

        /// <summary>
        /// 移除所有图层
        /// </summary>
        bool RemoveAllLayer();

        /// <summary>
        /// 清除当前图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        void ClearLayer(string layerName);

        /// <summary>
        /// 清除所有图层
        /// </summary>
        void ClearLayer();

        /// <summary>
        /// 显示、隐藏图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible"></param>
        void SetLayerVisiable(string layerName, bool visible);

        /// <summary>
        /// 设置图层栅格化（三维地图使用的接口）
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="bRasterize">是否栅格化</param>
        void SetRasterize(string layerName, bool bRasterize);

        /// <summary>
        /// 获取图层栅格化的属性（三维地图接口）
        /// </summary>
        /// <param name="layerName">图层名称</param>
        void GetRasterize(string layerName);

        /// <summary>
        /// 设置图层透明度（三维地图接口）
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="transparency">透明度</param>
        void SetTransparency(string layerName, short transparency);
        #endregion

        #region Element

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="kml">kml类对象</param>
        /// <returns></returns>
        IMFElement AddElement(string layerName, Kml kml);

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="element">图元</param>
        /// <returns></returns>
        bool RemoveElement(string layerName, IMFElement element);

        #endregion
    }
}
