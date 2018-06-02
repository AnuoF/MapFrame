/**************************************************************************
 * 类名：IToolEditLine.cs
 * 描述：编辑线接口，用于在编辑线时可指定编辑某点，根据手动测向（多手段态势项目）的需求而添加
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 编辑线接口，用于在编辑线时可指定编辑某点
    /// </summary>
    public interface IToolEditLine : IMFTool
    {
        /// <summary>
        /// 设置编辑点
        /// </summary>
        /// <param name="editPoint">编辑点，针对测向线的需求提供的接口</param>
        void SetEditPoint(MapLngLat editPoint);
    }
}
