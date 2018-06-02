 /**************************************************************************
 * 类名：ISelect.cs
 * 描述：框选接口
 * 作者：Allen
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 框选接口
    /// </summary>
    public interface IMFSelect
    {
        /// <summary>
        /// 获取框选的目标
        /// </summary>
        /// <returns></returns>
        List<IMFElement> GetSelectElements();
    }
}
