/**************************************************************************
 * 类名：ZoomToPosition.cs
 * 描述：定位至某点
 * 作者：Allen
 * 日期：Aug 15,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MapFrame.GMap.Model;
using System;
using System.Threading;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 定位至某点，实现动画效果
    /// </summary>
    class ZoomToPosition : IDisposable
    {
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 图层
        /// </summary>
        private GMapOverlay overlay = null;
        /// <summary>
        /// 图层名称
        /// </summary>
        private string layerName = "zoom_to_position";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        public ZoomToPosition(GMapControl _mapControl)
        {
            gmapControl = _mapControl;
            overlay = new GMapOverlay(layerName);
        }

        /// <summary>
        /// 定位至某点
        /// </summary>
        /// <param name="latlng">经纬度</param>
        public void ZoomTo(PointLatLng latlng)
        {
            if (gmapControl.InvokeRequired)
            {
                gmapControl.Invoke(new Action(delegate
                {
                    gmapControl.Overlays.Add(overlay);
                    GMarkerGoogle editMarker = new GMarkerGoogle(latlng, GMarkerGoogleType.arrow);
                    overlay.Markers.Add(editMarker);
                }));
            }
            else
            {
                gmapControl.Overlays.Add(overlay);
                GMarkerGoogle editMarker = new GMarkerGoogle(latlng, GMarkerGoogleType.arrow);
                overlay.Markers.Add(editMarker);
            }

            Thread.Sleep(1500);   // 1.5秒后消失

            if (gmapControl.InvokeRequired)
            {
                gmapControl.Invoke(new Action(delegate
                {
                    gmapControl.Overlays.Remove(overlay);
                }));
            }
            else
                gmapControl.Overlays.Remove(overlay);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            //gmapControl.Overlays.Remove(overlay);
        }
    }
}
