using System;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcGlobe.Tool
{
    class DrawPoint : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic;
        /// <summary>
        /// 点图元
        /// </summary>
        private IMFElement pointElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer= null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public DrawPoint(AxGlobeControl _mapControl) 
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取绘制后的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return pointElement;
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
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
        }
    }
}
