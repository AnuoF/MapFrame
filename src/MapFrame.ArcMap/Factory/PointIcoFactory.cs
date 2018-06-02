/**************************************************************************
 * 类名：PictureFactory.cs
 * 描述：图标工厂类
 * 作者：Allen
 * 日期：Aug 30,2016
 * 
 * ************************************************************************/


using ESRI.ArcGIS.Carto;
using MapFrame.ArcMap.Element;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 图标工厂类
    /// </summary>
    class PointIcoFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        private FactoryArcMap mapFactory = null;

        public PointIcoFactory(AxMapControl _mapControl, FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">图标的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, ESRI.ArcGIS.Carto.ILayer layer)
        {
            KmlPoint pointKml = kml.Placemark.Graph as KmlPoint;
            if (pointKml == null) return null;
            if (pointKml.Position == null) return null;

            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            PointIco_ArcMap pictureElement = new PointIco_ArcMap(mapControl, pointKml, mapFactory);
            pictureElement.ElementType = ElementTypeEnum.Picture;
            graphicLayer.AddElement(pictureElement, 0);

            return pictureElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">要移除的图元</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, ESRI.ArcGIS.Carto.ILayer layer)
        {
            if (element == null) return true;
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return true;

            PictureElementClass pictureElement = element as PictureElementClass;
            graphicLayer.DeleteElement(pictureElement);

            return true;
        }
    }
}
