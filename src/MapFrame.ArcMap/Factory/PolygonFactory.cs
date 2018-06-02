/**************************************************************************
 * 类名：PolygonFactory.cs
 * 描述：面工厂
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using MapFrame.ArcMap.Element;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Controls;
using System;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 面工厂
    /// </summary>
    class PolygonFactory : IElementFactory
    {
        private AxMapControl mapControl = null;
        private FactoryArcMap mapFoctory = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="mapFac">地图工厂接口</param>
        public PolygonFactory(AxMapControl _mapControl, FactoryArcMap mapFac)
        {
            this.mapControl = _mapControl;
            mapFoctory = mapFac;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">面的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, ILayer layer)
        {
            KmlPolygon kmlPolygon = kml.Placemark.Graph as KmlPolygon;
            if (kmlPolygon == null) return null;
            if (kmlPolygon.PositionList == null || kmlPolygon.PositionList.Count == 0) return null;


            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            Polygon_ArcMap polygonElement = new Polygon_ArcMap(mapControl, kmlPolygon, mapFoctory);

            polygonElement.Opacity = 30;
            graphicLayer.AddElement(polygonElement, 0);
            polygonElement.ElementType = ElementTypeEnum.Polygon;

            return polygonElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">要移除的面图元</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return true;

            PolygonElementClass polygonElement = element as PolygonElementClass;
            graphicLayer.DeleteElement(polygonElement);

            return true;
        }

    }
}
