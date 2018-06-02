/**************************************************************************
 * 类名：MeasureTypeEnum.cs
 * 描述：GMap测量类型枚举
 * 作者：Allen
 * 日期：July 14,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 测量类型枚举
    /// </summary>
    public enum MeasureTypeEnum
    {
        /// <summary>
        /// 长度
        /// </summary>
        Distance,

        /// <summary>
        /// 面积
        /// </summary>
        Area,

        /// <summary>
        /// 方位角
        /// </summary>
        Angle
    }
}
