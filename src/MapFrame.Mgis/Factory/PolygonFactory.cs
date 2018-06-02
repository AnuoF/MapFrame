

using AxHOSOFTMapControlLib;
using MapFrame.Mgis.Element;

namespace MapFrame.Mgis.Factory
{
    /// <summary>
    /// 多边形工厂
    /// </summary>
    class PolygonFactory : IElementFactory
    {
        /// <summary>
        /// MGIS地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public PolygonFactory(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 添加多边形图元
        /// </summary>
        /// <param name="kml"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, string layerName)
        {
            //MapFrame.Core.Model.KmlPolygon kmlPolygon = kml.Placemark.Graph as MapFrame.Core.Model.KmlPolygon;
            //Polygon_Mgis polygonMgis = new Polygon_Mgis(mapControl);
            //if (kml.Placemark.Name == null || kmlPolygon.PositionList == null) return null;
            //int count = kmlPolygon.PositionList.Count;
            //float[] vertex = new float[count * 2];
            //IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            //for (int i = 0; i < count; i++)
            //{
            //    vertex[2 * i] = (float)kmlPolygon.PositionList[i].Lng;
            //    vertex[2 * i + 1] = (float)kmlPolygon.PositionList[i].Lat;
            //}
            //Marshal.Copy(vertex, 0, ptrVert, vertex.Length);

            //mapControl.MgsDrawLineSymByJBID(kml.Placemark.Name, 11, (ulong)(ptrVert.ToInt64()), count);
            //Marshal.FreeHGlobal(ptrVert);
            //if (kmlPolygon.OutLineColor.ToArgb() != 0 || kmlPolygon.FillColor.ToArgb() != 0)
            //{
            //    mapControl.MgsUpdateSymFillColor(kml.Placemark.Name, kmlPolygon.FillColor.R, kmlPolygon.FillColor.G, kmlPolygon.FillColor.B, kmlPolygon.FillColor.A);
            //    mapControl.MgsUpdateSymColor(kml.Placemark.Name, kmlPolygon.OutLineColor.R, kmlPolygon.OutLineColor.G, kmlPolygon.OutLineColor.B, kmlPolygon.OutLineColor.A);
            //}

            //polygonMgis.ElementType = Core.Model.ElementTypeEnum.Polygon;
            //polygonMgis.SetPolygonName(kml.Placemark.Name);
            //polygonMgis.SetListPoint(kmlPolygon.PositionList);

            Polygon_Mgis polygonMgis = new Polygon_Mgis(kml, mapControl);

            return polygonMgis;
        }

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="element"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, string layerName)
        {
            Polygon_Mgis polygonMgis = element as Polygon_Mgis;
            return mapControl.MgsDelObject(polygonMgis.ElementPtr) == 0 ? true : false;
        }
    }
}
