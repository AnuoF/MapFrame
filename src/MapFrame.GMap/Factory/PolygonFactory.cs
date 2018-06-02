/**************************************************************************
 * 类名：PolygonFactory.cs
 * 描述：GMap面工厂
 * 作者：CJ
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using GMap.NET;
using MapFrame.GMap.Element;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// GMap面工厂
    /// </summary>
    class PolygonFactory : IElementFactory
    {

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PolygonFactory()
        {
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, GMapOverlay gmapOverlay)
        {
            KmlPolygon kSurface = kml.Placemark.Graph as KmlPolygon;
            if (kSurface == null) return null;
            if (kSurface.PositionList == null || kSurface.PositionList.Count == 0) return null;

            List<PointLatLng> pList = new List<PointLatLng>();
            foreach (MapLngLat lngLat in kSurface.PositionList)
            {
                PointLatLng point = new PointLatLng(lngLat.Lat, lngLat.Lng);
                pList.Add(point);
            }

            //添加面
            Polygon_GMap gPolygon = new Polygon_GMap(pList, kSurface, kml.Placemark.Name);
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.Invoke(new Action(delegate
                {
                    gmapOverlay.Polygons.Add(gPolygon);
                }));
            }
            else
                gmapOverlay.Polygons.Add(gPolygon);

            return gPolygon;
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
                    gmapOverlay.Polygons.Remove(element as GMapPolygon);
                }));
            }
            else
                gmapOverlay.Polygons.Remove(element as GMapPolygon);

            return true;
        }
    }
}
