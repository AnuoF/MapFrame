/**************************************************************************
 * 类名：IDraw.cs
 * 描述：绘图接口
 * 作者：Allen
 * 日期：July 20,2016
 * 
 * ************************************************************************/


namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 绘图接口
    /// </summary>
    public interface IMFDraw
    {
        /// <summary>
        /// 图层管理
        /// </summary>
        IMapLogic MapLogic { set; }

        /// <summary>
        /// 获取绘制的图元
        /// </summary>
        /// <returns>图元</returns>
        IMFElement GetDrawElement();
    }
}
