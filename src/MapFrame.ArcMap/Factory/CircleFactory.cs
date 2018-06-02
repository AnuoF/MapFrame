/**************************************************************************
 * 类名：CircleFactory.cs
 * 描述：圆工厂
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using MapFrame.ArcMap.Element;
using System;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 圆工厂
    /// </summary>
    class CircleFactory : IElementFactory
    {
        private AxMapControl mapControl = null;
        private FactoryArcMap factoryArcMap = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="_mapContrl">地图控件</param>
        /// <param name="mapFac">地图工厂接口</param>
        public CircleFactory(AxMapControl _mapContrl, FactoryArcMap facArc)
        {
            this.mapControl = _mapContrl;
            this.factoryArcMap = facArc;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">图元的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, ILayer layer)
        {
            Core.Model.KmlCircle kmlCircle = kml.Placemark.Graph as Core.Model.KmlCircle;
            if (kmlCircle == null) return null;
            if (kmlCircle.Position == null || kmlCircle.Radius <= 0) return null;
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            Circle_ArcMap circleElement = new Circle_ArcMap(mapControl, kmlCircle, factoryArcMap);
            circleElement.Opacity = 30;
            circleElement.ElementType = Core.Model.ElementTypeEnum.Circle;
            graphicLayer.AddElement(circleElement, 0);

            return circleElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">要移除的圆图元</param>
        /// <param name="layer">图元的所在图层</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return true;

            CircleElementClass circleElement = element as CircleElementClass;
            graphicLayer.DeleteElement(circleElement);
            return true;
        }

      
    }
}
