/**************************************************************************
 * 类名：IElementFactory.cs
 * 描述：图元工厂接口
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Factory
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
        /// <param name="layer">图层</param>
        /// <returns>图元</returns>
        MapFrame.Core.Interface.IMFElement CreateElement(Kml kml, ILayer layer);

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        bool RemoveElement(MapFrame.Core.Interface.IMFElement element, ILayer layer);
    }
}
