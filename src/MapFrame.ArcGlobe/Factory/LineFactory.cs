/**************************************************************************
 * 类名：LineFactory.cs
 * 描述：线图元工厂
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Carto;
using MapFrame.Core.Interface;
using MapFrame.ArcGlobe.Element;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 线图元工厂
    /// </summary>
    class LineFactory : IElementFactory
    {
        /// <summary>
        /// 地图对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public LineFactory(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 创建线图元
        /// </summary>
        /// <param name="kml">线的kml</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, ILayer layer)
        {
            KmlLineString lineKml = kml.Placemark.Graph as KmlLineString;
            if (lineKml.PositionList == null || lineKml.PositionList.Count < 1) return null;

            int index = -1;
            //图层
            IGlobeGraphicsLayer graphicLayer = layer as IGlobeGraphicsLayer;
            if (graphicLayer == null) return null;

            //图元
            Line_ArcGlobe lineElement = null;

            Dosomething((Action)delegate()
            {
                //属性
                GlobeGraphicsElementPropertiesClass properties = new GlobeGraphicsElementPropertiesClass();
                properties.Rasterize = lineKml.Rasterize;

                //添加图元
                lineElement = new Line_ArcGlobe(graphicLayer, lineKml);
                graphicLayer.AddElement(lineElement, properties, out index);
                lineElement.Index = index;                                      //指定索引
                lineElement.ElementName = kml.Placemark.Name;
            }, true);

            return lineElement;
        }

        /// <summary>
        /// 移除线图元
        /// </summary>
        /// <param name="element">要移除的线图元</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, ILayer layer)
        {
            GlobeGraphicsLayerClass graphicLayer = layer as GlobeGraphicsLayerClass;
            if (graphicLayer == null) return false;

            Line_ArcGlobe line = element as Line_ArcGlobe;
            if (line == null) return false;

            if (line.Rasterize) //判断图元是否栅格化,栅格化之后删不掉图元，所以要先不栅格化
            {
                line.Rasterize = false;
            }
            Dosomething((Action)delegate()
            {
                graphicLayer.DeleteElement(line);//删除线图元
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
