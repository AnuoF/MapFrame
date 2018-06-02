using MapFrame.Core.Interface;

using MapFrame.Core.Model;

namespace MapFrame.Mgis.Factory
{
    /// <summary>
    /// 图元工厂接口
    /// </summary>
    interface IElementFactory
    {
        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="layerName">图层名称</param>
        /// <returns>图元</returns>
        IMFElement CreateElement(Kml kml,string layerName);

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        bool RemoveElement(IMFElement element, string layerName);
    }
}
