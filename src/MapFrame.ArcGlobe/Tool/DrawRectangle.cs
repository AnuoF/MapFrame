using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcGlobe.Tool
{
    class DrawRectangle:IMFTool, IMFDraw
    {
        /// <summary>
        /// 绘制完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 地图逻辑接口
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 矩形图元
        /// </summary>
        private IMFPolygon rectangleElement = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawRectangle(AxGlobeControl _mapControl) 
        {
            this.mapControl = _mapControl;
        }

        #region  ITool

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            layer = mapLogic.AddLayer("draw_layer");
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
        }

        #endregion

        #region  IDraw

        /// <summary>
        /// 地图逻辑接口
        /// </summary>
        public IMapLogic MapLogic
        {
            set { this.mapLogic = value; }
        }

        /// <summary>
        /// 获取绘制完成后的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return rectangleElement;
        }

        #endregion
    }
}
