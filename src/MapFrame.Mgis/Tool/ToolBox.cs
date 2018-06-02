
using System;
using MapFrame.Core.Interface;
using AxHOSOFTMapControlLib;
using System.Drawing;

namespace MapFrame.Mgis.Tool
{
    class ToolBox : IMFToolBox
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<Core.Model.MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 当前使用的工具
        /// </summary>
        private IMFTool currentTool;
        /// <summary>
        /// 逻辑
        /// </summary>
        private IMapLogic mapLogic;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxHOSOFTMapControl mapControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapObject">地图对象</param>
        /// <param name="_mapLogic">地图逻辑类</param>
        public ToolBox(object mapObject, IMapLogic _mapLogic)
        {
            mapLogic = _mapLogic;
            mapControl = mapObject as AxHOSOFTMapControl;
            //measureDistanceTool = new MeasureDistance(gmapControl);
            //measureAreaTool = new MeasureArea(gmapControl);
            //measureAngleTool = new MeasureAngle(gmapControl);
            //selectTool = new SelectElementEx(gmapControl, _mapLogic);
        }

        /// <summary>
        /// 漫游
        /// </summary>
        public void Roam(bool isDrag = true)
        {
            ReleaseTool();
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_MOVE_HOSOFT");
        }

        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomIn()
        {
            ReleaseTool();
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_ZOOMIN_HOSOFT");
        }

        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomOut()
        {
            ReleaseTool();
            mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_MAP_ZOOMOUT_HOSOFT");
        }

        /// <summary>
        /// 全图显示
        /// </summary>
        public void FullView()
        {
            mapControl.showAllMap();
        }

        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        /// <param name="zoomLevel">显示比例</param>
        public void ZoomToPosition(Core.Model.MapLngLat lngLat, int? zoomLevel = null)
        {
            ReleaseTool();
            mapControl.moveMapTo(lngLat.Lng, lngLat.Lat);
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="measureType"></param>
        public void Measure(Core.Model.MeasureTypeEnum measureType)
        {
            ReleaseTool();
            switch (measureType)
            {
                case Core.Model.MeasureTypeEnum.Angle:
                    mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_CALC_DIR_HOSOFT");
                    break;
                case Core.Model.MeasureTypeEnum.Distance:
                    mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_CALC_DISTANCE3D_HOSOFT");
                    break;
                case Core.Model.MeasureTypeEnum.Area:
                    mapControl.IMGS_WorkStation_SelectTool("GIS_TOOL_CALC_AREA3D_HOSOFT");
                    break;
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        public void Select()
        {
            if (currentTool != null)
            {
                currentTool.ReleaseCommond();//重置工具
            }
            string curPath = "";
            if (System.IO.File.Exists(curPath))//加载鼠标样式
            {
                mapControl.loadMouseCur(curPath);
            }
            currentTool = new SelectElement(mapControl, mapLogic);
            if (currentTool != null)
            {
                currentTool.RunCommond();
                currentTool.CommondExecutedEvent += this.CommondExecutedEvent;//注册命令完成事件
            }
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseTool()
        {
            if (currentTool != null)
            {
                mapControl.setMgsEditFlag(0);
                currentTool.ReleaseCommond();
                currentTool.CommondExecutedEvent -= CommondExecutedEvent;
                currentTool.Dispose();
                currentTool = null;
            }
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="element">图元</param>
        public void EditElement(Core.Interface.IMFElement element)
        {
            ReleaseTool();
            switch (element.ElementType)
            {
                case Core.Model.ElementTypeEnum.Point:
                    currentTool = new EditPoint(mapControl, element);
                    break;
            }

            if (currentTool != null)
            {
                currentTool.CommondExecutedEvent += CommondExecutedEvent;
                currentTool.RunCommond();
            }
        }

        /// <summary>
        /// 编辑图元
        /// </summary>
        /// <param name="elementName"></param>
        public void EditElement(string elementName)
        {
            IMFElement element = mapLogic.GetElement(elementName);
            this.EditElement(element);
        }

        /// <summary>
        /// 绘制图形
        /// </summary>
        /// <param name="type">图形类型</param>
        public void DrawGraphic(Core.Model.ElementTypeEnum type)
        {
            this.ReleaseTool();//重置工具
            switch (type)
            {
                case Core.Model.ElementTypeEnum.Point://点
                    break;
                case Core.Model.ElementTypeEnum.Line://线
                    currentTool = new DrawLine(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Polygon://多边形
                    currentTool = new DrawPolygon(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Rectangle://矩形
                    currentTool = new DrawRectangle(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Circle://圆
                    currentTool = new DrawCircle(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Text://文字
                    currentTool = new DrawText(mapControl);
                    break;
                case Core.Model.ElementTypeEnum.Other://其他
                    break;
            }
            if (currentTool != null)
            {
                (currentTool as IMFDraw).MapLogic = mapLogic;
                currentTool.CommondExecutedEvent += new EventHandler<Core.Model.MessageEventArgs>(currentTool_CommondExecutedEvent);
                currentTool.RunCommond();
            }
        }

        void currentTool_CommondExecutedEvent(object sender, Core.Model.MessageEventArgs e)
        {
            if (CommondExecutedEvent != null)
            {
                CommondExecutedEvent(sender, e);
            }
        }

        /// <summary>
        /// 获取当前工具
        /// </summary>
        /// <returns></returns>
        public Core.Interface.IMFTool GetCurrentTool()
        {
            return currentTool;
        }

        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        /// <returns></returns>
        public Core.Model.MapLngLat SceneToGeographyPoint(int x, int y)
        {
            float lon = 0, lat = 0;
            mapControl.MgsAppXYtoBL(0, x, y, ref lon, ref lat);
            Core.Model.MapLngLat lnglat = new Core.Model.MapLngLat(lon, lat);
            return lnglat;
        }

        /// <summary>
        /// 地理坐标转屏幕坐标
        /// </summary>
        /// <param name="lon">地理经度</param>
        /// <param name="lat">地理纬度</param>
        /// <param name="alt">地理高度</param>
        public Point GeographyToScenePoint(double lon, double lat, double alt = 0)
        {
            float x = 0, y = 0;
            mapControl.MgsAppBLtoXY(0, (float)lon, (float)lat, ref x, ref y);
            Point point = new Point((int)x, (int)y);
            return point;
        }
    }
}
