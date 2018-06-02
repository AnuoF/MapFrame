
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 地图缩放层级改变事件
    /// </summary>
    public class MapZoomChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 地图当前缩放层级
        /// </summary>
        public double Zoom { get; set; }
    }
}
