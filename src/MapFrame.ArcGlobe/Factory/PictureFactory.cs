/**************************************************************************
 * 类名：PictureFactory.cs
 * 描述：图片图元工厂类
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using MapFrame.Core.Model;
using MapFrame.Core.Interface;
using MapFrame.ArcGlobe.Element;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 图片图元工厂类
    /// </summary>
    class PictureFactory : IElementFactory
    {
        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">图片的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, ILayer layer)
        {
            //IGlobeGraphicsLayer graphicLayer = layer as IGlobeGraphicsLayer;
            //Core.Model.KmlPicture modelkml = kml.Placemark.Graph as Core.Model.KmlPicture;
            //if (modelkml == null) return null;
            //if (modelkml.Position == null) return null;

            //MapFrame.ArcGlobe.Element.Picture_ArcGlobe pictureElement = new MapFrame.ArcGlobe.Element.Picture_ArcGlobe(graphicLayer, modelkml);
            //graphicLayer.PutElementName(pictureElement, kml.Placemark.Name);
            return null;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, ILayer layer)
        {
            Picture_ArcGlobe model = element as Picture_ArcGlobe;
            IGraphicsContainer graphicsContainer = layer as IGraphicsContainer;
            graphicsContainer.DeleteElement(model);
            return true;
        }
    }
}
