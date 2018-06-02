using System;
using MapFrame.Mgis.Element;
using AxHOSOFTMapControlLib;

namespace MapFrame.Mgis.Factory
{
    /// <summary>
    /// 圆工厂
    /// </summary>
    class CircleFactory : IElementFactory
    {
        /// <summary>
        /// MGIS地图对象
        /// </summary>
        private AxHOSOFTMapControl mapControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public CircleFactory(AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
        }
        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="layerName">图元名称</param>
        /// <returns></returns>
        public Core.Interface.IMFElement CreateElement(Core.Model.Kml kml, string layerName)
        {
            //MapFrame.Core.Model.KmlCircle kmlCircle = kml.Placemark.Graph as MapFrame.Core.Model.KmlCircle;
            //Circle_Mgis circleMgis = new Circle_Mgis(kml,mapControl);
            //if (kmlCircle.Position == null || kmlCircle.RandomPosition == null || kml.Placemark.Name == string.Empty) return null;
            //List<MapLngLat> listPoints = new List<MapLngLat>();
            //listPoints.Add(kmlCircle.Position);
            //listPoints.Add(kmlCircle.RandomPosition);

            //float[] vertex = new float[4];
            //IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * 4);

            //vertex[0] = (float)listPoints[0].Lng;
            //vertex[1] = (float)listPoints[0].Lat;

            //vertex[2] = (float)listPoints[1].Lng;
            //vertex[3] = (float)listPoints[1].Lat;
            //Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            //mapControl.MgsDrawLineSymByJBID(kml.Placemark.Name, 16, (ulong)(ptrVert.ToInt64()), 2);

            //Marshal.FreeHGlobal(ptrVert);
            //if (kmlCircle.StrokeColor.ToArgb() != System.Drawing.Color.Black.ToArgb() || kmlCircle.FillColor.ToArgb() != 0) 
            //{
            //    mapControl.MgsUpdateSymFillColor(kml.Placemark.Name, kmlCircle.FillColor.R, kmlCircle.FillColor.G, kmlCircle.FillColor.B, kmlCircle.FillColor.A);
            //    mapControl.MgsUpdateSymColor(kml.Placemark.Name, kmlCircle.StrokeColor.R, kmlCircle.StrokeColor.G, kmlCircle.StrokeColor.B, kmlCircle.StrokeColor.A);
            //}
            //circleMgis.SetCircleName(kml.Placemark.Name);
            //circleMgis.SetListPoint(listPoints);
            //circleMgis.ElementType = ElementTypeEnum.Circle;

            Circle_Mgis circleMgis = new Circle_Mgis(kml, mapControl);
            return circleMgis;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元对象</param>
        /// <param name="layerName">图元名称</param>
        /// <returns></returns>
        public bool RemoveElement(Core.Interface.IMFElement element, string layerName)
        {
            Circle_Mgis circleMgis = element as Circle_Mgis;
            return mapControl.MgsDelObject(circleMgis.ElementName) == 1 ? true : false;
            //return mapControl.destroyMoveObject(Convert.ToUInt64(circleMgis.ElementPtr)) == 1 ? true : false;
        }
    }
}
