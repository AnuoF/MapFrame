using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFrame.Core.Model;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 修改区域事件消息结构体
    /// </summary>
    public class ChangeAreaEventArgs : EventArgs
    {
        public string AreaName;

        public List<MapLngLat> listPiont;
    }
}
