/**************************************************************************
 * 类名：LayerManger.cs
 * 描述：图层管理类
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;
using ESRI.ArcGIS.GlobeCore;
using System;
using MapFrame.Core.Interface;
using ESRI.ArcGIS.Analyst3D;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 图层管理类
    /// </summary>
    class LayerManager
    {
        /// <summary>
        /// AxGlobe控件
        /// </summary>
        private AxGlobeControl globeControl = null;
        /// <summary>
        /// 图层字典
        /// </summary>
        private Dictionary<string, ILayer> layerDic = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private IMapFactory mapFactory = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_axGlobeControl">地球控件</param>
        public LayerManager(AxGlobeControl _axGlobeControl, IMapFactory _mapFac)
        {
            mapFactory = _mapFac;
            globeControl = _axGlobeControl;
            layerDic = new Dictionary<string, ILayer>();
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName"></param>
        public bool AddLayer(string layerName)
        {
            lock (layerDic)
            {
                if (layerDic.ContainsKey(layerName)) return false;

                ILayer graphcisLayer = null;
                Dosomething((Action)delegate()
                {
                    graphcisLayer = new GlobeGraphicsLayerClass();
                    graphcisLayer.Name = layerName;
                    globeControl.Globe.AddLayerType(graphcisLayer, esriGlobeLayerType.esriGlobeLayerTypeDraped);
                }, true);
                layerDic.Add(layerName, graphcisLayer);

                return true;
            }
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName"></param>
        public bool RemoveLayer(string layerName)
        {
            lock (layerDic)
            {
                if (!layerDic.ContainsKey(layerName)) return false;

                ILayer layer = layerDic[layerName];
                IScene scene = globeControl.Globe as IScene;
                scene.DeleteLayer(layer);

                layerDic.Remove(layerName);
                return true;
            }
        }

        /// <summary>
        /// 移除所有图层
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllLayer()
        {
            try
            {
                lock (layerDic)
                {
                    foreach (var layer in layerDic.Values)
                    {
                        IScene scene = globeControl.Globe as IScene;
                        scene.DeleteLayer(layer);
                    }

                    layerDic.Clear();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public ILayer GetLayer(string layerName)
        {
            if (!layerDic.ContainsKey(layerName)) return null;
            return layerDic[layerName];
        }

        /// <summary>
        /// 清除当前图层上的图元
        /// </summary>
        /// <param name="layerName"></param>
        public void ClearLayer(string layerName)
        {
            if (!layerDic.ContainsKey(layerName)) return;

            ILayer layer = layerDic[layerName];
            IGraphicsContainer globeGraphicsLayer = layer as IGraphicsContainer;
            globeGraphicsLayer.DeleteAllElements();
        }

        /// <summary>
        /// 清除所有图层的图元
        /// </summary>
        public void ClearLayer()
        {
            lock (layerDic)
            {
                foreach (ILayer layer in layerDic.Values)
                {
                    IGraphicsContainer globeGraphicsLayer = layer as IGraphicsContainer;
                    globeGraphicsLayer.DeleteAllElements();
                }
            }
        }

        /// <summary>
        /// 显示隐藏图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="visible"></param>
        public void ShowLayer(string layerName, bool visible)
        {
            if (!layerDic.ContainsKey(layerName)) return;

            ILayer layer = layerDic[layerName];
            layer.Visible = visible;
        }

        /// <summary>
        /// 设置图层栅格化
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="bRasterize">是否栅格化</param>
        public void SetRasterize(string layerName, bool bRasterize)
        {
            if (!layerDic.ContainsKey(layerName)) return;

            ILayer pLayer = layerDic[layerName];

            Dosomething((Action)delegate()
            {
                IScene m_scene = globeControl.Globe as IScene;    // 提供数据给成员控制场景
                m_scene.ActiveGraphicsLayer = pLayer;             // 活动的图层，如果没有则创建一个
                IGlobeGraphicsLayer pGL = pLayer as IGlobeGraphicsLayer;     //提供数据给地图图形图层

                IGlobeDisplay m_globeDisplay = globeControl.Globe.GlobeDisplay;            // 提供数据给成员操作地图显示
                IGlobeDisplayLayers pGlobeLayer = m_globeDisplay as IGlobeDisplayLayers;   // 提供数据给成员操作地图显示图层
                IGlobeLayerProperties pGlobeLayerProps = pGlobeLayer.FindGlobeProperties(pLayer);   // 提供数据给成员操纵图层属性，返回图层的属性
                IGlobeGraphicsElementProperties pGEP = new GlobeGraphicsElementPropertiesClass();  // 图层的其他属性
                pGEP.DrapeElement = true;
                pGEP.DrapeZOffset = 10;
                pGEP.Rasterize = bRasterize;//是否栅格化
                pGlobeLayerProps.ApplyDisplayProperties(pLayer); //应用属性到此图层
            }, true);
        }

        public void GetRasterize(string layerName)
        {
        }

        /// <summary>
        /// 设置图层透明度
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="transparency">透明度</param>
        public void SetTransparency(string layerName, short transparency)
        {
            if (!layerDic.ContainsKey(layerName)) return;

            ILayer pLayer = layerDic[layerName];

            Dosomething((Action)delegate()
            {
                // 透明度
                ILayerEffects pLayerEffects = pLayer as ILayerEffects;
                pLayerEffects.Transparency = transparency;
            }, true);
        }

        /// <summary>
        /// 主线程做事
        /// </summary>
        /// <param name="action">要做的内容</param>
        /// <param name="synchronization">是否同步执行</param>
        private void Dosomething(Action action, bool synchronization)
        {
            if (globeControl == null) return;
            if (synchronization)
            {
                if (globeControl.InvokeRequired)
                    globeControl.Invoke(action);
                else
                    action();
            }
            else
            {
                if (globeControl.InvokeRequired)
                    globeControl.BeginInvoke(action);
                else
                    action();
            }
        }

    }
}
