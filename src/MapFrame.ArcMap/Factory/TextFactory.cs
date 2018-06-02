/**************************************************************************
 * 类名：TextFactory.cs
 * 描述：文字工厂
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/
using ESRI.ArcGIS.Carto;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Element;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 文字工厂
    /// </summary>
    class TextFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private FactoryArcMap mapFactory = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        /// <param name="_mapFactory">地图工厂</param>
        public TextFactory(AxMapControl _mapControl,FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;
        }

        /// <summary>
        ///创建图元
        /// </summary>
        /// <param name="kml"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, ILayer layer)
        {
            KmlText kmlText = kml.Placemark.Graph as KmlText;
            if (kmlText == null) return null;
            if (kmlText.Position == null) return null;

            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            Text_ArcMap textElement = new Text_ArcMap(mapControl,kmlText,mapFactory);
            textElement.ElementType = ElementTypeEnum.Text;
            graphicLayer.AddElement(textElement, 0);

            return textElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return true;

            TextElementClass textElement = element as TextElementClass;
            graphicLayer.DeleteElement(textElement);

            return true;
        }
    }
}
