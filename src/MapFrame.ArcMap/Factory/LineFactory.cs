/**********************************************************
 * Describe:LineFactory.cs 线工厂
 * Author:
 * DateTime:2016年8月23日 
 * 
 * *******************************************************/

using MapFrame.Core.Model;
using ESRI.ArcGIS.Carto;
using MapFrame.ArcMap.Element;
using System;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 线工厂
    /// </summary>
    class LineFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        private FactoryArcMap mapFactory = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        public LineFactory(AxMapControl _mapControl, FactoryArcMap _mapFactory)
        {
            mapControl = _mapControl;
            this.mapFactory = _mapFactory;
        }

        /// <summary>
        /// 创建线图元
        /// </summary>
        /// <param name="kml">线的kml</param>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, ILayer layer)
        {
            KmlLineString line = kml.Placemark.Graph as KmlLineString;
            if (line == null) return null;
            if (line.PositionList == null || line.PositionList.Count < 1) return null;

            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            Line_ArcMap lineElement = new Line_ArcMap(mapControl, line, mapFactory);
            lineElement.Opacity = 50;
            lineElement.ElementType = ElementTypeEnum.Line;
            graphicLayer.AddElement(lineElement, 0);

            return lineElement;
        }

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="element">要移除的线图元</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, ILayer layer)
        {
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return false;

            Line_ArcMap line = element as Line_ArcMap;
            if (line == null) return false;

            graphicLayer.DeleteElement(line);//删除线图元

            return true;
        }


    }
}
