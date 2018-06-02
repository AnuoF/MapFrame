/**************************************************************************
 * 类名：PictureFactory.cs
 * 描述：图标工厂类
 * 作者：Allen
 * 日期：Aug 24,2016
 * 
 * ************************************************************************/

using GMap.NET;
using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using MapFrame.GMap.Element;
using System;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// 图标工厂类
    /// </summary>
    class PictureFactory : IElementFactory
    {
        /// <summary>
        /// 图元管理
        /// </summary>
        private GMapOverlay overlay;

        /// <summary>
        /// 添加点图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Kml kml, global::GMap.NET.WindowsForms.GMapOverlay gmapOverlay)
        {
            if (overlay == null)
            {
                overlay = gmapOverlay;
                overlay.Control.OnMarkerEnter += Control_OnMarkerEnter;
                overlay.Control.OnMarkerLeave += Control_OnMarkerLeave;
            }

            KmlPicture kmlPicture = kml.Placemark.Graph as KmlPicture;
            if (kmlPicture == null) return null;
            if (kmlPicture.Position == null) return null;

            PointLatLng p = new PointLatLng(kmlPicture.Position.Lat, kmlPicture.Position.Lng, kmlPicture.Position.Alt);

            // 位置和图片
            Picture_GMap moveObj = new Picture_GMap(p, kmlPicture, kml.Placemark.Name);
            // Tip
            if (!string.IsNullOrEmpty(kmlPicture.TipText))
            {
                moveObj.ToolTipText = kmlPicture.TipText;
                moveObj.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                moveObj.ToolTip.Format.Alignment = System.Drawing.StringAlignment.Near; // Tip文字左对齐
            }

            // 添加图元到图层
            if (gmapOverlay.Control.InvokeRequired)
            {
                gmapOverlay.Control.BeginInvoke(new Action(delegate
                {
                    gmapOverlay.Markers.Add(moveObj);
                }));
            }
            else
            {
                gmapOverlay.Markers.Add(moveObj);
            }

            return moveObj;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <param name="gmapOverlay">图层</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, global::GMap.NET.WindowsForms.GMapOverlay gmapOverlay)
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

        /// <summary>
        /// 离开目标事件
        /// </summary>
        /// <param name="item"></param>
        private void Control_OnMarkerLeave(GMapMarker item)
        {
            Picture_GMap gmapMoveObj = item as Picture_GMap;
            if (gmapMoveObj == null) return;

            switch (gmapMoveObj.LabelShowType)
            {
                case ShowTypeEnum.No://不显示
                    gmapMoveObj.CloseMapLabel();
                    break;

                case ShowTypeEnum.MouseHover://移上显示
                    gmapMoveObj.CloseMapLabel();
                    break;

                case ShowTypeEnum.Always://永久显示
                    //gmapPoint.InitToolTip();
                    break;
            }
        }

        /// <summary>
        /// 进入目标事件
        /// </summary>
        /// <param name="item"></param>
        private void Control_OnMarkerEnter(GMapMarker item)
        {
            Picture_GMap gmapMoveObj = item as Picture_GMap;
            if (gmapMoveObj == null) return;

            switch (gmapMoveObj.LabelShowType)
            {
                case ShowTypeEnum.No:
                    gmapMoveObj.CloseMapLabel();
                    break;

                case ShowTypeEnum.MouseHover:
                    gmapMoveObj.InitMapLabel();
                    break;

                case ShowTypeEnum.Always:
                    gmapMoveObj.InitMapLabel();
                    break;
            }
        }
    }
}
