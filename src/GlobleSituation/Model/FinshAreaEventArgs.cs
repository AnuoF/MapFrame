using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 绘图完成事件
    /// </summary>
    public class FinshAreaEventArgs : EventArgs
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 端点信息
        /// </summary>
        public List<MapLngLat> listMappoit { get; set; }
    }
}
