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

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 图标工厂类
    /// </summary>
    class PictureFactory : IElementFactory
    {
        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">图标的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public Core.Interface.IElement CreateElement(Core.Model.Kml kml, ESRI.ArcGIS.Carto.ILayer layer)
        {
            KmlPicture pictureKml = kml.Placemark.Graph as KmlPicture;
            if (pictureKml == null) return null;
            if (pictureKml.Position == null) return null;

            CompositeGraphicsLayerClass graphicLayer = layer as CompositeGraphicsLayerClass;
            if (graphicLayer == null) return null;

            Picture_ArcMap pictureElement = new Picture_ArcMap(graphicLayer, pictureKml);
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
        public bool RemoveElement(Core.Interface.IElement element, ESRI.ArcGIS.Carto.ILayer layer)
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
