/**************************************************************************
 * 类名：TextFactory.cs
 * 描述：文字图元工厂
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
    /// 文字图元工厂
    /// </summary>
    class TextFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public TextFactory(AxGlobeControl _mapControl)
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
            KmlText textKml = kml.Placemark.Graph as KmlText;
            if (textKml == null) return null;
            if (textKml.Position == null) return null;

            int index = -1;

            //图层
            IGlobeGraphicsLayer graphicsLayer = layer as IGlobeGraphicsLayer;
            MapFrame.ArcGlobe.Element.Text_ArcGlobe textElement = null;
            this.Dosomething((Action)delegate()
            {
                //图元
                textElement = new MapFrame.ArcGlobe.Element.Text_ArcGlobe(graphicsLayer, textKml);
                GlobeGraphicsElementPropertiesClass properties = new GlobeGraphicsElementPropertiesClass();
                properties.Rasterize = textKml.Rasterize;
                graphicsLayer.AddElement(textElement, properties, out index);
                textElement.Index = index;    //指定索引
                textElement.ElementName = kml.Placemark.Name;
            }, true);

            return textElement;
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

            Text_ArcGlobe textElement = element as Text_ArcGlobe;
            graphicsLayer.DeleteElement(textElement);

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
