/**************************************************************************
 * 类名：PointFactory.cs
 * 描述：点工厂类
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.GMap.Element;
using System;
using System.Drawing;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// 点工厂类
    /// </summary>
    class PointFactory : IElementFactory
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public PointFactory()
        {
        }

        /// <summary>
        /// 添加点图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, GMapOverlay gmapOverlay)
        {
            KmlPoint point = kml.Placemark.Graph as KmlPoint;
            if (point == null) return null;
            if (point.Position == null) return null;

            PointLatLng p = new PointLatLng(point.Position.Lat, point.Position.Lng, point.Position.Alt);

            IMFElement element = null;
            // 位置和图标
            Point_GMap pointElement = new Point_GMap(p, point, kml.Placemark.Name);
            // 大小
            pointElement.Size = new Size(pointElement.Size.Width, pointElement.Size.Height);
            // Tip
            if (!string.IsNullOrEmpty(point.TipText))
            {
                pointElement.ToolTipText = point.TipText;
                pointElement.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                pointElement.ToolTip.Format.Alignment = System.Drawing.StringAlignment.Near; // Tip文字左对齐
            }

            element = pointElement;

            //if (string.IsNullOrEmpty(point.IcoUrl))//纯点
            //{
            //    // 位置和图标
            //    Point_GMap pointElement = new Point_GMap(p, point, kml.Placemark.Name);
            //    // 大小
            //    pointElement.Size = new Size(pointElement.Size.Width, pointElement.Size.Height);
            //    // Tip
            //    if (!string.IsNullOrEmpty(point.TipText))
            //    {
            //        pointElement.ToolTipText = point.TipText;
            //        pointElement.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            //        pointElement.ToolTip.Format.Alignment = System.Drawing.StringAlignment.Near; // Tip文字左对齐
            //    }

            //    element = pointElement;
            //}
            //else                                //目标点
            //{
            //    Picture_GMap moveObj = new Picture_GMap(p, point, kml.Placemark.Name);
            //    // Tip
            //    if (!string.IsNullOrEmpty(point.TipText))
            //    {
            //        moveObj.ToolTipText = point.TipText;
            //        moveObj.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            //        moveObj.ToolTip.Format.Alignment = System.Drawing.StringAlignment.Near; // Tip文字左对齐
            //    }

            //    // 设置图元的类型
            //    moveObj.ElementType = ElementTypeEnum.Point;
            //    // 设置图元的描述信息
            //    moveObj.Description = point.Description;
            //    // 鼠标经过可见
            //    moveObj.IsHitTestVisible = true;
            //    element = moveObj;
            //}

            // 添加图元到图层
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.BeginInvoke(new Action(delegate
                {
                    gmapOverlay.Markers.Add((element as GMapMarker));
                }));
            }
            else
            {
                gmapOverlay.Markers.Add((element as GMapMarker));
            }

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
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.BeginInvoke(new Action(delegate
                {
                    GMapMarker marker = element as GMapMarker;
                    gmapOverlay.Markers.Remove(marker);
                }));
            }
            else
                gmapOverlay.Markers.Remove(element as GMapMarker);

            return true;
        }
    }
}
