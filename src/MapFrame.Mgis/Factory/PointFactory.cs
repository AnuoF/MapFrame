
using MapFrame.Mgis.Element;
using AxHOSOFTMapControlLib;
using System;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.Mgis.Factory
{
    /// <summary>
    /// MGIS地图点工厂
    /// </summary>
    class PointFactory : IElementFactory
    {
        /// <summary>
        /// MGIS地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public PointFactory(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">图元的kml对象</param>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, string layerName)
        {
            KmlPoint kmlPoint = kml.Placemark.Graph as KmlPoint;
            if (kml.Placemark.Graph == null) return null;
            Point_Mgis pointMgis = new Point_Mgis(kml);
            return pointMgis;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元对象</param>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, string layerName)
        {
            Picture_Mgis pointMgis = element as Picture_Mgis;
            return mapControl.destroyMoveObject(Convert.ToUInt64(element.ElementPtr)) == -1 ? false : true;
        }
    }
}
