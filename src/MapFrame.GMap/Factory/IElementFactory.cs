/**************************************************************************
 * 类名：IElementFactory.cs
 * 描述：图元工厂接口
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/


using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.GMap.Factory
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
        /// <param name="gmapOverlay">图层</param>
        /// <returns>图元</returns>
        IMFElement CreateElement(Kml kml, GMapOverlay gmapOverlay);

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        bool RemoveElement(IMFElement element, GMapOverlay gmapOverlay);
    }
}
