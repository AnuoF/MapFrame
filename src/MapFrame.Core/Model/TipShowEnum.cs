/**************************************************************************
 * 类名：TipShowEnum.cs
 * 描述：标牌显示方式
 * 作者：CJ
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// 标牌显示方式
    /// </summary>
    public enum ShowTypeEnum
    {
        /// <summary>
        /// 不显示
        /// </summary>
        No = 0,

        /// <summary>
        /// 鼠标放上去显示
        /// </summary>
        MouseHover = 1,

        /// <summary>
        /// 始终显示
        /// </summary>
        Always = 2,
    }
}
