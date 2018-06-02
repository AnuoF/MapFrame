/**************************************************************************
 * 类名：FactoryGMap.cs
 * 描述：FactoryArcMap工厂类
 * 作者：Allen
 * 日期：Aug 19,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Model;
using System;
using MapFrame.Core.Interface;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// ArcMap工厂类
    /// </summary>
    public class FactoryArcMap : IMapFactory, IMFMap
    {
        /// <summary>
        /// ArcMap地图控件
        /// </summary>
        private AxMapControl axMapControl;
        /// <summary>
        /// 图层管理
        /// </summary>
        private LayerManger lyMgr = null;
        /// <summary>
        /// 点图元工厂
        /// </summary>
        private PointFactory pointFac = null;
        /// <summary>
        /// 线工厂
        /// </summary>
        private LineFactory lineFac = null;
        /// <summary>
        /// 面工厂
        /// </summary>
        private PolygonFactory polygonFac = null;
        /// <summary>
        /// 文字工厂
        /// </summary>
        private TextFactory textFac = null;
        /// <summary>
        /// 圆图元工厂
        /// </summary>
        private CircleFactory circleFac = null;
        /// <summary>
        /// 图标图元工厂
        /// </summary>
        private PointIcoFactory pointIcoFac = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapObject">地图控件对象</param>
        public FactoryArcMap(object _mapObject)
        {
            if (_mapObject != null)
            {
                axMapControl = _mapObject as AxMapControl;
            }
            else
            {
                // 注册
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
                axMapControl = new AxMapControl();
            }

            lyMgr = new LayerManger(axMapControl, this);

            pointFac = new PointFactory(axMapControl, this);
            lineFac = new LineFactory(axMapControl, this);
            polygonFac = new PolygonFactory(axMapControl, this);
            textFac = new TextFactory(axMapControl, this);
            circleFac = new CircleFactory(axMapControl, this);
            pointIcoFac = new PointIcoFactory(axMapControl, this);
        }

        /// <summary>
        /// 获取地图控件
        /// </summary>
        /// <returns></returns>
        public object GetMapControl()
        {
            return axMapControl;
        }

        /// <summary>
        /// 刷新图层
        /// </summary>
        public void Refresh()
        {
            Dosomething((Action)(delegate
            {
                axMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }), true);
        }

        /// <summary>
        /// 刷新指定图层
        /// </summary>
        /// <param name="layer">图层</param>
        public void Refresh(Core.Interface.IMFLayer layer)
        {
            Dosomething((Action)(delegate
            {
                axMapControl.Refresh(esriViewDrawPhase.esriViewGraphics, layer as CompositeGraphicsLayerClass, null);
            }), true);
        }

        #region ILayer

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool AddLayer(string layerName)
        {
            return lyMgr.AddLayer(layerName);
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool RemoveLayer(string layerName)
        {
            return lyMgr.RemoverLayer(layerName);
        }

        /// <summary>
        /// 移除所有图层
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllLayer()
        {
            return lyMgr.RemoveAllLayer();
        }

        /// <summary>
        /// 清除图层上的图元
        /// </summary>
        /// <param name="layerName"></param>
        public void ClearLayer(string layerName)
        {
            lyMgr.ClearLayer(layerName);
        }

        /// <summary>
        /// 清除所有图层上的图元
        /// </summary>
        public void ClearLayer()
        {
            lyMgr.ClearLayer();
        }

        /// <summary>
        /// 显示、隐藏图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="visible"></param>
        public void SetLayerVisiable(string layerName, bool visible)
        {
            lyMgr.ShowLayer(layerName, visible);
        }

        /// <summary>
        /// 获取ArcMap的图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public ILayer GetLayerByName(string layerName)
        {
            return lyMgr.GetLayer(layerName);
        }
        #endregion

        #region Element

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="kml">Kml对象</param>
        /// <returns></returns>
        public MapFrame.Core.Interface.IMFElement AddElement(string layerName, Core.Model.Kml kml)
        {
            if (kml == null) return null;
            if (kml.Placemark == null) return null;
            if (kml.Placemark.Graph == null) return null;
            if (string.IsNullOrEmpty(kml.Placemark.Name)) return null;

            var layer = lyMgr.GetLayer(layerName);
            if (layer == null) return null;

            IElementFactory elementFactory = null;
            Type type = kml.Placemark.Graph.GetType();

            // 点
            if (type == typeof(KmlPoint))
            {
                KmlPoint pointKml = kml.Placemark.Graph as KmlPoint;
                if (pointKml == null) return null;
                if (string.IsNullOrEmpty(pointKml.IcoUrl))//没有图标纯点
                {
                    elementFactory = pointFac;
                }
                else //有图标
                {
                    elementFactory = pointIcoFac;
                }
            }
            // 线
            else if (type == typeof(KmlLineString))
            {
                elementFactory = lineFac;
            }
            // 面
            else if (type == typeof(KmlPolygon))
            {
                elementFactory = polygonFac;
            }
            // 文字
            else if (type == typeof(KmlText))
            {
                elementFactory = textFac;
            }
            // 圆
            else if (type == typeof(KmlCircle))
            {
                elementFactory = circleFac;
            }

            if (elementFactory == null) return null;

            MapFrame.Core.Interface.IMFElement element = elementFactory.CreateElement(kml, layer);
            if (element != null)
            {
                element.ElementName = kml.Placemark.Name;
            }

            Refresh(layer as Core.Interface.IMFLayer);
            return element;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="element">图元</param>
        /// <returns></returns>
        public bool RemoveElement(string layerName, MapFrame.Core.Interface.IMFElement element)
        {
            if (element == null) return true;
            var layer = lyMgr.GetLayer(layerName);
            if (layer == null) return true;
            IElementFactory elementFactory = null;

            switch (element.ElementType)
            {
                case ElementTypeEnum.Point:
                    elementFactory = pointFac;
                    break;

                case ElementTypeEnum.Line:
                    elementFactory = lineFac;
                    break;

                case ElementTypeEnum.Polygon:
                    elementFactory = polygonFac;
                    break;

                case ElementTypeEnum.Text:
                    elementFactory = textFac;
                    break;

                case ElementTypeEnum.Circle:
                    elementFactory = circleFac;
                    break;

                case ElementTypeEnum.Picture:
                    elementFactory = pointIcoFac;
                    break;
            }

            if (elementFactory == null) return false;

            bool ret = elementFactory.RemoveElement(element, layer);
            element.Dispose();

            this.Refresh();
            return ret;
        }
        #endregion

        #region IMFMap
        public event EventHandler<Core.Model.MFMouseEventArgs> MouseClickEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseMoveEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseDownEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseUpEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseDbClickEvent;

        public event EventHandler<Core.Model.MFElementEnterEventArgs> ElementEnterEvent;

        public event EventHandler<Core.Model.MFElementLeaveEventArgs> ElementLeaveEvent;

        public event EventHandler<Core.Model.MFElementClickEventArgs> ElementClickEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyDownEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyUpEvent;

        public event EventHandler<Core.Model.MFKeyPressEventArgs> KeyPressEvent;

        public event EventHandler<EventArgs> InitFinishEvent;


        public object MapControl
        {
            get
            {
                return axMapControl;
            }
        }

        /// <summary>
        /// 获取地图接口对象
        /// </summary>
        /// <returns></returns>
        public IMFMap GetIMFMap()
        {
            return this;
        }

        public void LoadMap(string mapFile)
        {
            throw new NotImplementedException();
        }

        public MapLngLat FromLocalToLngLat(int x, int y)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Point FromLngLatToLocal(MapLngLat position)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Image Snapshot()
        {
            throw new NotImplementedException();
        }
      
        #endregion

        /// <summary>
        /// 主线程做事
        /// </summary>
        /// <param name="action">要做的内容</param>
        /// <param name="synchronization">是否同步执行</param>
        private void Dosomething(Action action, bool synchronization)
        {
            if (axMapControl == null) return;
            if (synchronization)
            {
                if (axMapControl.InvokeRequired)
                    axMapControl.Invoke(action);
                else
                    action();
            }
            else
            {
                if (axMapControl.InvokeRequired)
                    axMapControl.BeginInvoke(action);
                else
                    action();
            }
        }



        public event EventHandler<MapZoomChangedEventArgs> MapZoomChangedEvent;


        public void SetRasterize(string layerName, bool bRasterize)
        {
            throw new NotImplementedException();
        }

        public void GetRasterize(string layerName)
        {
            throw new NotImplementedException();
        }


        public void SetTransparency(string layerName, short transparency)
        {
            throw new NotImplementedException();
        }


    }
}
