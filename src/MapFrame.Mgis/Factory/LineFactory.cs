using AxHOSOFTMapControlLib;
using MapFrame.Mgis.Element;

namespace MapFrame.Mgis.Factory
{
    /// <summary>
    /// 线工厂
    /// </summary>
    class LineFactory : IElementFactory
    {
        /// <summary>
        /// MGIS地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public LineFactory(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 创建线图元
        /// </summary>
        /// <param name="kml">线kml对象</param>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, string layerName)
        {
            //MapFrame.Core.Model.KmlLineString kmlLine = kml.Placemark.Graph as MapFrame.Core.Model.KmlLineString;
            Line_Mgis lineMgis = new Line_Mgis(kml, mapControl);
            //if (kml.Placemark.Name == null || kmlLine.PositionList == null) return null;
            //int count = kmlLine.PositionList.Count();
            //float[] vertex = new float[count * 2];
            //IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            //for (int i = 0; i < count; i++)
            //{
            //    vertex[2 * i] = (float)kmlLine.PositionList[i].Lng;
            //    vertex[2 * i + 1] = (float)kmlLine.PositionList[i].Lat;
            //}
            //Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            //mapControl.MgsDrawLineSymByJBID(kml.Placemark.Name, 10, (ulong)(ptrVert.ToInt64()), count);
            //Marshal.FreeHGlobal(ptrVert);
            //if (kmlLine.Color.ToArgb() != 0)
            //{
            //    mapControl.MgsUpdateSymColor(kml.Placemark.Name, kmlLine.Color.R, kmlLine.Color.G, kmlLine.Color.B, kmlLine.Color.A);
            //}
            //lineMgis.SetLineName(kml.Placemark.Name);
            //lineMgis.SetListPoint(kmlLine.PositionList);
            //lineMgis.ElementType = ElementTypeEnum.Line;
            return lineMgis;

        }

        /// <summary>
        /// 删除指定线图元
        /// </summary>
        /// <param name="element">线图元</param>
        /// <param name="layerName">图元名称</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, string layerName)
        {
            Line_Mgis lineMgis = element as Line_Mgis;
            return mapControl.MgsDelObject(lineMgis.ElementPtr) == 0 ? true : false;
        }
    }
}
