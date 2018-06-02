

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobleSituation.Model;
using MapFrame.Core.Model;

namespace GlobleSituation.Business
{
    class ArcMapElementMgr
    {
        /// <summary>
        /// 地球上绘制的图元集合
        /// </summary>
        private Dictionary<string, ElementInfo> elementDic = null;



        public ArcMapElementMgr()
        {
            elementDic = new Dictionary<string, ElementInfo>();
        }


        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <param name="elementObject">图元信息</param>
        public void AddElement(string elementName, ElementInfo elementInfo)
        {
            if (elementDic.ContainsKey(elementName))
            {
                elementDic[elementName] = elementInfo;
            }
            else
                elementDic.Add(elementName, elementInfo);
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="elementName"></param>
        public void RemoveElement(string elementName)
        {
            if (elementDic.ContainsKey(elementName))
                elementDic.Remove(elementName);
        }

        /// <summary>
        /// 添加航迹点
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <param name="mapPoint">点图元信息</param>
        /// <returns></returns>
        public bool AddElementTrackPoint(string elementName, ElementInfo elementInfo)
        {
            if (!elementDic.ContainsKey(elementName)) return false;
            return elementDic[elementName].AddTrackPoint(elementInfo);
        }

        /// <summary>
        /// 移除航迹点
        /// </summary>
        /// <returns></returns>
        public bool RemoveElementTrackPoint(string elementName)
        {
            if (!elementDic.ContainsKey(elementName)) return false;
            elementDic[elementName].RemoveTrackPoint();
            return true;
        }

        /// <summary>
        /// 是否存在图元
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public bool IsHaveElement(string elementName)
        {
            return elementDic.ContainsKey(elementName);
        }

        /// <summary>
        /// 获取图元信息
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ElementInfo GetElementInfo(string elementName)
        {
            if (elementDic.ContainsKey(elementName))
                return elementDic[elementName];
            else
                return null;
        }

        /// <summary>
        /// 更新目标的位置
        /// </summary>
        /// <param name="elementName">目标名称</param>
        /// <param name="point">点</param>
        public void UpdateElementPosition(string elementName, MapLngLat point)
        {
            if (elementDic.ContainsKey(elementName))
            {
                elementDic[elementName].Position = point;
            }
        }


    }
}
