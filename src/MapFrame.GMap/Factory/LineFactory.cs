/**************************************************************************
 * 类名：LineFactory.cs
 * 描述：线工厂类
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.GMap.Element;
using System;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// 线工厂类
    /// </summary>
    class LineFactory : IElementFactory
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineFactory()
        {
        }

        /// <summary>
        /// 创建线图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, GMapOverlay gmapOverlay)
        {
            KmlLineString line = kml.Placemark.Graph as KmlLineString;
            if (line == null) return null;
            if (line.PositionList == null || line.PositionList.Count == 0) return null;

            // 画线
            Line_GMap lineRoute = new Line_GMap(kml.Placemark.Name,line);

            // 将图元添加到图层
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.Invoke(new Action(delegate
                {
                    gmapOverlay.Routes.Add(lineRoute);
                }));
            }
            else
            {
                gmapOverlay.Routes.Add(lineRoute);
            }
            return lineRoute;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, GMapOverlay gmapOverlay)
        {
            // 将图元从图层移除
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.Invoke(new Action(delegate
                {
                    gmapOverlay.Routes.Remove(element as GMapRoute);
                }));
            }
            else
                gmapOverlay.Routes.Remove(element as GMapRoute);

            return true;
        }

    }
}
