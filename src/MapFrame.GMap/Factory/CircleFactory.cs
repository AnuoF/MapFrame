/**************************************************************************
 * 类名：CircleFactory.cs
 * 描述：圆图元工厂类
 * 作者：Allen
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.GMap.Element;
using System;
using System.Collections.Generic;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// 圆图元工厂类
    /// </summary>
    class CircleFactory : IElementFactory
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public CircleFactory()
        {
        }

        /// <summary>
        /// 创建圆图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, GMapOverlay gmapOverlay)
        {
            KmlCircle kmlCircle = kml.Placemark.Graph as KmlCircle;
            if (kmlCircle == null) return null;
            if (kmlCircle.Position == null) return null;
            if (kmlCircle.RandomPosition == null && kmlCircle.Radius == 0) return null;

            List<PointLatLng> pointList = new List<PointLatLng>();
            for (int i = 0; i < 360; i++)
            {
                double seg = Math.PI * i / 180;
                double a = kmlCircle.Position.Lng + kmlCircle.Radius * Math.Cos(seg) / 100000;
                double b = kmlCircle.Position.Lat + kmlCircle.Radius * Math.Sin(seg) / 100000;
                PointLatLng lnglat = new PointLatLng(b, a);
                pointList.Add(lnglat);
            }

            Circle_GMapEx circle = new Circle_GMapEx(pointList, kmlCircle, kml.Placemark.Name);

            // 添加到图层
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.Invoke(new Action(delegate
                {
                    gmapOverlay.Polygons.Add(circle);
                }));
            }
            else
                gmapOverlay.Polygons.Add(circle);

            return circle;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, GMapOverlay gmapOverlay)
        {
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.Invoke(new Action(delegate
                {
                    gmapOverlay.Markers.Remove(element as GMapMarker);
                }));
            }
            else
                gmapOverlay.Markers.Remove(element as GMapMarker);

            return true;
        }
    }
}