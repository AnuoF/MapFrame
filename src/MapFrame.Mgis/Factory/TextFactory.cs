

using AxHOSOFTMapControlLib;
using MapFrame.Mgis.Element;
using MapFrame.Core.Interface;

namespace MapFrame.Mgis.Factory
{
    /// <summary>
    /// MGis文字图元工厂
    /// </summary>
    class TextFactory : IElementFactory
    {
        /// <summary>
        /// MGIS地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public TextFactory(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 创建文字图元
        /// </summary>
        /// <param name="km"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml km, string layerName)
        {
            //MapFrame.Core.Model.KmlText kmlText = km.Placemark.Graph as MapFrame.Core.Model.KmlText;
            //Text_Mgis textMgis = new Text_Mgis(mapControl);
            //if (km.Placemark.Name == null || kmlText.Content == string.Empty) return null;
            //mapControl.MgsDrawSymTextByJBID(km.Placemark.Name, kmlText.Content,(float)kmlText.Position.Lng, (float)kmlText.Position.Lat);
            //System.Drawing.Color c = System.Drawing.Color.FromArgb(kmlText.Color);
            //mapControl.MgsUpdateSymColor(km.Placemark.Name, c.R, c.G, c.B, c.A);
            //mapControl.MgsUpdateSymSize(km.Placemark.Name, (float)kmlText.Size);
            //textMgis.SetTextName(km.Placemark.Name);
            //textMgis.ElementType = Core.Model.ElementTypeEnum.Text;
            Text_Mgis textMgis = new Text_Mgis(km, mapControl);
            return textMgis;
        }

        /// <summary>
        /// 删除指定文字图元
        /// </summary>
        /// <param name="element"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, string layerName)
        {
            IMFText textMgis = element as IMFText;
            return mapControl.MgsDelObject(textMgis.ElementPtr) == 0 ? true : false;
        }
    }
}
