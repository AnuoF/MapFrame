/**************************************************************************
 * 类名：PointFactory.cs
 * 描述：点工厂
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using MapFrame.ArcMap.Element;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 点工厂
    /// </summary>
    class PointFactory : IElementFactory
    {
        private AxMapControl mapControl = null;
        private FactoryArcMap mapFactory = null;

        public PointFactory(AxMapControl _mapControl, FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">点的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, ILayer layer)
        {
            KmlPoint point = kml.Placemark.Graph as KmlPoint;
            if (point == null) return null;
            if (point.Position == null) return null;

            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            Point_ArcMap pointElement = new Point_ArcMap(mapControl, point, mapFactory);
            pointElement.ElementType = ElementTypeEnum.Point;
            graphicLayer.AddElement(pointElement, 0);

            return pointElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">要移除的点图元</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return true;

            MarkerElementClass pointElement = element as MarkerElementClass;
            graphicLayer.DeleteElement(pointElement);

            return true;
        }

    }
}
