/**************************************************************************
 * 类名：CircleFactory.cs
 * 描述：圆图元工厂类
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Model;
using MapFrame.ArcGlobe.Element;
using MapFrame.Core.Interface;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 圆图元工厂类
    /// </summary>
    class CircleFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public CircleFactory(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kml"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, ILayer layer)
        {
            KmlCircle circleKml = kml.Placemark.Graph as KmlCircle;
            if (circleKml.Position == null) return null;

            int index = -1;
            Circle_ArcGlobe circleElement = null;

            this.Dosomething((Action)delegate()
            {
                //图元
                IGlobeGraphicsLayer graphicsLayer = layer as IGlobeGraphicsLayer;

                circleElement = new Circle_ArcGlobe(graphicsLayer, circleKml);
                IGlobeGraphicsElementProperties properties = new GlobeGraphicsElementPropertiesClass();
                properties.Rasterize = circleKml.Rasterize;
                graphicsLayer.AddElement(circleElement, properties, out index);
                circleElement.Index = index;
                circleElement.ElementName = kml.Placemark.Name;
            }, true);

            return circleElement;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, ILayer layer)
        {
            if (element == null) return true;
            IGraphicsContainer graphicsLayer = layer as IGraphicsContainer;
            if (graphicsLayer == null) return true;

            Circle_ArcGlobe circleElement = element as Circle_ArcGlobe;
            graphicsLayer.DeleteElement(circleElement);

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
