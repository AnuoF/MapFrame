/**************************************************************************
 * 类名：KmlTooltipEnum.cs
 * 描述：Tip显示方式类型
 * 作者：CJ
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
    /// Tip显示方式
    /// </summary>
    enum KmlTooltipEnum
    {
        /// <summary>
        /// 鼠标移上显示
        /// </summary>
        OnMouseOver = 0,

        /// <summary>
        /// 从不显示
        /// </summary>
        Never = 1,

        /// <summary>
        /// 始终显示
        /// </summary>
        Always = 2,
    }
}
