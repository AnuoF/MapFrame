using System;
using ZC57S.ZKNet;

namespace GlobleSituation.Model
{
    /// <summary>
    /// 态势数据
    /// </summary>
    public class TSDataEventArgs : EventArgs
    {
        /// <summary>
        /// 数据
        /// </summary>
        public RealData Data { get; set; }

        /// <summary>
        /// 区域名称（在预警时使用）
        /// </summary>
        public string AreaName { get; set; }


        public TSDataEventArgs() { }
        public TSDataEventArgs(byte[] data)
        {
            Data = RealData.ToRealData(data);
        }
    }
}
