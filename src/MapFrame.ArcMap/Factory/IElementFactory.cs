/**************************************************************************
 * 类名：IElementFactory.cs
 * 描述：图元接口
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using MapFrame.ArcMap.Common;
using ESRI.ArcGIS.Carto;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 图元接口
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
