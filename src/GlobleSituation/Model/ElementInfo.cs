/********************************************************************************
** 文件名：ElementInfo.cs
** 版 本：1.0
** 内容简述：图元信息类
** 创建日期：Nov 11,2016
** 创建人：Allen 
** 修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 图元信息类
    /// </summary>
    class ElementInfo
    {
        public ElementInfo()
        {
            HistoryPoint = new Queue<ElementInfo>();
        }

        /// <summary>
        /// 目标名称
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// 所在图层名称
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public MapLngLat Position { get; set; }

        /// <summary>
        /// 航迹点
        /// </summary>
        public Queue<ElementInfo> HistoryPoint { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 图元类型
        /// </summary>
        public ElementType? ElementType { get; set; }

        /// <summary>
        /// 添加航迹点
        /// </summary>
        /// <param name="elementInfo"></param>
        /// <returns></returns>
        public bool AddTrackPoint(ElementInfo elementInfo)
        {
            HistoryPoint.Enqueue(elementInfo);
            return true;
        }

        /// <summary>
        /// 移除最开始那个航迹点
        /// </summary>
        /// <returns></returns>
        public ElementInfo RemoveTrackPoint()
        {
            return HistoryPoint.Dequeue();
        }


    }
}
