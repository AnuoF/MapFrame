/**************************************************************************
 * 类名：TextFactory.cs
 * 描述：文字工厂
 * 作者：Allen
 * 日期：July 11,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using GMap.NET;
using MapFrame.GMap.Element;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// 文字工厂
    /// </summary>
    class TextFactory : IElementFactory
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextFactory()
        {
        }

        /// <summary>
        /// 创建文字图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, GMapOverlay gmapOverlay)
        {
            KmlText text = kml.Placemark.Graph as KmlText;
            if (text == null) return null;
            if (text.Position == null) return null;

            PointLatLng p = new PointLatLng(text.Position.Lat, text.Position.Lng);

            Text_GMap element = new Text_GMap(p, text, kml.Placemark.Name);

            // 添加图元到图层
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.Invoke(new Action(delegate
                {
                    gmapOverlay.Markers.Add(element);
                }));
            }
            else
                gmapOverlay.Markers.Add(element);

            return element;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, GMapOverlay gmapOverlay)
        {
            if (element == null) return true;

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
