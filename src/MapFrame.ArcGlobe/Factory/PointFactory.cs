/**************************************************************************
 * 类名：PointFactory.cs
 * 描述：点图元工厂
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Controls;
using System;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcGlobe.Element;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 点图元工厂
    /// </summary>
    class PointFactory : IElementFactory
    {
        /// <summary>
        /// 地图对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public PointFactory(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, ILayer layer)
        {
            KmlPoint pointKml = kml.Placemark.Graph as KmlPoint;
            if (pointKml.Position == null) return null;

            int index = -1;

            //图层
            IGlobeGraphicsLayer graphicsLayer = layer as IGlobeGraphicsLayer;

            //图元
            Point_ArcGlobe pointElement = new Point_ArcGlobe(graphicsLayer, pointKml);

            this.Dosomething((Action)delegate()
            {
                IGlobeGraphicsElementProperties properties = new GlobeGraphicsElementPropertiesClass();
                properties.Rasterize = pointKml.Rasterize;                  //栅格化
                graphicsLayer.AddElement(pointElement, properties, out index);

                pointElement.Index = index;                                 //指定索引
                pointElement.ElementName = kml.Placemark.Name;
            }, true);

            return pointElement;
        }

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="element"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            IGraphicsContainer graphicsLayer = layer as IGraphicsContainer;
            if (graphicsLayer == null) return true;

            Point_ArcGlobe pointElement = element as Point_ArcGlobe;
            if (pointElement.Rasterize)
            {
                pointElement.Rasterize = false;
            }
            this.Dosomething((Action)delegate()
                {
                    graphicsLayer.DeleteElement(pointElement);
                }, true);

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
