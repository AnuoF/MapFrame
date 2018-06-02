

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Display;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    public class Beam
    {
        /// <summary>
        /// 填充色
        /// </summary>
        public IRgbColor FillColor;

        /// <summary>
        /// 圆心位置
        /// </summary>
        public MapLngLat CenterPoint;

        /// <summary>
        /// 卫星坐标
        /// </summary>
        public MapLngLat SatellitePoint;

        /// <summary>
        /// 圆半径
        /// </summary>
        public float Radius;

        /// <summary>
        /// 步进 ----画圆更精细
        /// </summary>
        public int StepValue = 4;

        /// <summary>
        /// 波束名
        /// </summary>
        public string BeamName;

        /// <summary>
        /// 图元名列表
        /// </summary>
        public List<string> ElementNameList;

        /// <summary>
        /// 波束编号
        /// </summary>
        public long BeamID;

        /// <summary>
        /// 卫星编号
        /// </summary>
        public int SatelliteID;

        /// <summary>
        /// 波束图层名字
        /// </summary>
        public string BeamLayerName;

        public Beam()
        {
            ElementNameList = new List<string>();
        }
    }
}
