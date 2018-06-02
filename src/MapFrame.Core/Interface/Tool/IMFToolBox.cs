/**************************************************************************
 * 类名：IToolBox.cs
 * 描述：工具箱接口
 * 作者：Allen
 * 日期：July 15,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using System;
using System.Drawing;

namespace MapFrame.Core.Interface
{
    /// <summary>
    /// 工具箱接口
    /// </summary>
    public interface IMFToolBox
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        event EventHandler<MessageEventArgs> CommondExecutedEvent;

        /// <summary>
        /// 漫游
        /// </summary>
        /// <param name="isDrag">是否可以拖动地图，默认是可以拖动地图；但根据GMap提出的需求，希望可以不能拖动地图</param>
        void Roam(bool isDrag = true);

        /// <summary>
        /// 放大
        /// </summary>
        void ZoomIn();

        /// <summary>
        /// 缩小
        /// </summary>
        void ZoomOut();

        /// <summary>
        /// 全图显示
        /// </summary>
        void FullView();

        /// <summary>
        /// 定位至某点
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        /// <param name="zoomLevel">缩放级别，默认不缩放</param>
        void ZoomToPosition(MapLngLat lngLat, int? zoomLevel = null);

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="measureType">测量类型，有距离、面积和方位角</param>
        void Measure(MeasureTypeEnum measureType);

        /// <summary>
        /// 选择图元
        /// </summary>
        void Select();

        /// <summary>
        /// 释放地图工具
        /// </summary>
        void ReleaseTool();

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="element">图元</param>
        void EditElement(IMFElement element);

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        void EditElement(string elementName);

        /// <summary>
        /// 绘制图元
        /// </summary>
        /// <param name="type"></param>
        void DrawGraphic(ElementTypeEnum type);

        /// <summary>
        /// 获取当前工具接口对象
        /// </summary>
        /// <returns>ITool</returns>
        IMFTool GetCurrentTool();

        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        /// <returns></returns>
        MapLngLat SceneToGeographyPoint(int x, int y);

        /// <summary>
        /// 地理坐标转屏幕坐标
        /// </summary>
        /// <param name="lon">地理经度</param>
        /// <param name="lat">地理纬度</param>
        /// <param name="alt">地理高度</param>
        Point GeographyToScenePoint(double lon, double lat, double alt = 0);

    }
}
