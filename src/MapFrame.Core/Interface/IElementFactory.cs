/**************************************************************************
 * 类名：IElementFactory.cs
 * 描述：图元工厂接口
 * 作者：Allen
 * 日期：July 5,2016
 * 
 * ************************************************************************/


using MapFrame.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 图元工厂接口
    /// </summary>
    public interface IElementFactory
    {
        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kmlStr">kml字符串</param>
        /// <returns>图元</returns>
        IElement CreateElement(string kmlStr);

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <returns>图元</returns>
        IElement CreateElement(Kml kml);
    }
}
