/**************************************************************************
 * 类名：PolygonFactory.cs
 * 描述：面图元工厂
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcGlobe.Element;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 面图元工厂
    /// </summary>
    class PolygonFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public PolygonFactory(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">图元的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, ILayer layer)
        {
            KmlPolygon polygonKml = kml.Placemark.Graph as KmlPolygon;
            if (polygonKml == null) return null;
            if (polygonKml.PositionList == null) return null;

            int index = -1;
            Polygon_ArcGlobe polygonElement = null;

            this.Dosomething((Action)delegate()
            {
                //图层
                IGlobeGraphicsLayer graphicsLayer = layer as IGlobeGraphicsLayer;
                //实例化图元
                polygonElement = new Polygon_ArcGlobe(graphicsLayer, polygonKml);

                //设置属性
                GlobeGraphicsElementPropertiesClass properties = new GlobeGraphicsElementPropertiesClass();
                properties.Rasterize = polygonKml.Rasterize;
                graphicsLayer.AddElement(polygonElement, properties, out index);
                polygonElement.Index = index;                                       //指定索引
                polygonElement.ElementName = kml.Placemark.Name;
            }, true);

            return polygonElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">要移除的图元对象</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            IGraphicsContainer graphicsLayer = layer as IGraphicsContainer;
            if (graphicsLayer == null) return true;

            Polygon_ArcGlobe polygonElement = element as Polygon_ArcGlobe;
            if (polygonElement.Rasterize)
            {
                polygonElement.Rasterize = false;
            }
            graphicsLayer.DeleteElement(polygonElement);

            return true;
        }

        /// <summary>
        /// 主线程做事
        /// </summary>
        /// <param name="action">要做的内容</param>
        /// <param name="synchronization">是否同步执行</param>
        private void Dosomething(Action action, bool synchronization)
        {
            if (mapControl == null) return;
            if (synchronization)
            {
                if (mapControl.InvokeRequired)
                    mapControl.Invoke(action);
                else
                    action();
            }
            else
            {
                if (mapControl.InvokeRequired)
                    mapControl.BeginInvoke(action);
                else
                    action();
            }
        }
    }
}
