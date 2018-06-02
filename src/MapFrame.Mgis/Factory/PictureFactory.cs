using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using AxHOSOFTMapControlLib;
using MapFrame.Mgis.Element;
using System.Drawing;

namespace MapFrame.Mgis.Factory
{
    class PictureFactory : IElementFactory
    {
        AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 图片工厂
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        public PictureFactory(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="kml">图片kml</param>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, string layerName)
        {
            KmlPicture kmlPicture = kml.Placemark.Graph as KmlPicture;
            Picture_Mgis pointMgis = new Picture_Mgis();
            if (kmlPicture.Position == null) return null;
            ulong moveObj = 0;
            if (!string.IsNullOrEmpty(kmlPicture.IconUrl))//图片型目标
            {
                moveObj = mapControl.createImageMoveObject(kmlPicture.IconUrl, layerName);
                if (kmlPicture.IconColor.ToArgb() != 0)
                {
                    mapControl.setMoveObjectColor(moveObj, kmlPicture.IconColor.R, kmlPicture.IconColor.G, kmlPicture.IconColor.B, kmlPicture.IconColor.A);//设置目标颜色
                }
            }
            else //字体型目标
            {
                if (kmlPicture.IconColor.ToArgb() == 0)
                {
                    mapControl.setMoveObjectColor(moveObj, Color.Blue.R, Color.Blue.G, Color.Blue.B, Color.Blue.A);//设置目标颜色
                }
                else
                {
                    mapControl.setMoveObjectColor(moveObj, kmlPicture.IconColor.R, kmlPicture.IconColor.G, kmlPicture.IconColor.B, kmlPicture.IconColor.A);//设置目标颜色
                }
                moveObj = mapControl.createMoveObject(kmlPicture.FontPath, kmlPicture.Code, layerName);
            }
            mapControl.setMoveObjectPositon(moveObj, kmlPicture.Position.Lng, kmlPicture.Position.Lat, 1);//设置目标位置
            mapControl.setMoveObjectScale(moveObj, kmlPicture.Scale, kmlPicture.Scale);//设置目标大小
            if (!string.IsNullOrEmpty(kmlPicture.Description))
            {
                mapControl.setMoveObjectTrackUserData(moveObj, kmlPicture.Description);
            }
            mapControl.setMoveObjectProperty(moveObj, "名称", kml.Placemark.Name);
            pointMgis.SetMoveObj(moveObj);

            return pointMgis;
        }

        public bool RemoveElement(IMFElement element, string layerName)
        {
            throw new NotImplementedException();
        }
    }
}
