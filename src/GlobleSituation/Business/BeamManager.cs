
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobleSituation.Model;
using MapFrame.Core.Model;

namespace GlobleSituation.Business
{
    class BeamManager
    {
        private Dictionary<string, List<Beam>> BeamListDic = new Dictionary<string, List<Beam>>();

        public BeamManager(int maxLength = int.MaxValue)
        {

        }

        /// <summary>
        /// 波束是否存在
        /// </summary>
        /// <param name="beam"></param>
        /// <returns></returns>
        public bool HasBeam(string name, Beam beam)
        {
            if (!BeamListDic.ContainsKey(name)) return false;

            if (BeamListDic[name].Find(o => o.BeamID == beam.BeamID) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 添加波束数据
        /// </summary>
        /// <param name="beam"></param>
        /// <returns></returns>
        public bool AddBeam(string name, Beam beam)
        {
            try
            {
                if (BeamListDic.ContainsKey(name))
                {
                    BeamListDic[name].Add(beam);
                }
                else
                {
                    List<Beam> beams = new List<Beam>();
                    beams.Add(beam);
                    BeamListDic.Add(name, beams);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 查找波束
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<Beam> FindBeams(string name, Predicate<Beam> match)
        {
            if (BeamListDic.ContainsKey(name))
            {
                return BeamListDic[name].FindAll(match);
            }
            else
                return null;
        }

        /// <summary>
        /// 更新波束圆心坐标位置
        /// </summary>
        /// <param name="satelliteID"></param>
        /// <param name="point"></param>
        public void UpdataBeamCenterPoint(string name, MapLngLat point)
        {
            if (!BeamListDic.ContainsKey(name)) return;

            foreach (Beam b in BeamListDic[name])
            {
                b.SatellitePoint = point;
            }
        }

        /// <summary>
        /// 更新波束圆心坐标位置
        /// </summary>
        /// <param name="satelliteID"></param>
        /// <param name="beam"></param>
        public void UpdataBeamCenterPoint(string name, Beam beam)
        {
            if (!BeamListDic.ContainsKey(name)) return;

            var b = BeamListDic[name].Find(o => o.BeamID == beam.BeamID);
            if (b != null)
                BeamListDic[name].Remove(b);

            BeamListDic[name].Add(beam);
        }
    }
}
