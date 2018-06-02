using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcGlobe.Tool
{
    class DrawText:IMFTool,IMFDraw
    {
        /// <summary>
        /// 完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 文字图元
        /// </summary>
        private IMFElement textElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 地图逻辑接口
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public DrawText(AxGlobeControl _mapControl) 
        {
            this.mapControl = _mapControl;
        }

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

        /// <summary>
        /// 逻辑接口
        /// </summary>
        public IMapLogic MapLogic
        {
            set { this.mapLogic = value; }
        }

        /// <summary>
        /// 获取绘制后的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return textElement;
        }
    }
}
