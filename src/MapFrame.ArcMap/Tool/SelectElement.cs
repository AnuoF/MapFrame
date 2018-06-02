/**************************************************************************
 * 类名：SelectElement.cs
 * 描述：选择图元类
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/
using System;
using System.Collections.Generic;
using MapFrame.Core.Model;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Controls;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 选择图元类
    /// </summary>
    class SelectElement : MapFrame.ArcMap.Interface.IToolArcMap, MapFrame.Core.Interface.ISelect
    {
        private AxMapControl mapControl = null;

        public SelectElement(AxMapControl _mapControl) 
        {
            mapControl = _mapControl;
        }

        public List<Core.Interface.IElement> GetSelectElements()
        {
            return null;
        }

        private void RegistEvent() 
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnMouseUp += mapControl_OnMouseUp;
        }

        void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            
        }

        void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {

        }

        public event EventHandler<MessageEventArgs> CommondExecutedEvent;

        public void RunCommond()
        {
            mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            ICommand command = new ControlsSelectToolClass();
            if (command.Enabled) 
            {
                mapControl.CurrentTool = command as ITool;
            }
            RegistEvent();
        }

        public void ReleaseCommond()
        {
        }

        public void Dispose()
        {
        }
    }
}
