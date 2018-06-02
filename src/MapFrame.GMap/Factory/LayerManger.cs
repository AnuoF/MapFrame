/**************************************************************************
 * 类名：LayerManger.cs
 * 描述：图层管理类
 * 作者：Allen
 * 日期：July 11,2016
 * 
 * ************************************************************************/

using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using MapFrame.Core.Interface;

namespace MapFrame.GMap.Factory
{
    /// <summary>
    /// 图层管理类
    /// </summary>
    class LayerManger
    {
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private GMapControl mapControl = null;
        /// <summary>
        /// 图层字典
        /// </summary>
        private Dictionary<string, GMapOverlay> layerDic = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private IMapFactory mapFactory = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map">地图控件</param>
        /// <param name="_mapFac">地图工厂</param>
        public LayerManger(GMapControl map,IMapFactory _mapFac)
        {
            mapControl = map;
            mapFactory = _mapFac;
            layerDic = new Dictionary<string, GMapOverlay>();
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public bool AddLayer(string layerName)
        {
            lock (layerDic)
            {
                if (layerDic.ContainsKey(layerName)) return true;

                GMapOverlay layer = new GMapOverlay(layerName);
                mapControl.Overlays.Add(layer);
                layerDic.Add(layerName, layer);
                return true;
            }
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public bool RemoverLayer(string layerName)
        {
            lock (layerDic)
            {
                if (!layerDic.ContainsKey(layerName)) return true;

                GMapOverlay layer = layerDic[layerName];

                if (mapControl.InvokeRequired)
                {
                    mapControl.Invoke(new Action(delegate
                    {
                        mapControl.Overlays.Remove(layer);
                        mapControl.Refresh();
                    }));
                }
                else
                {
                    mapControl.Overlays.Remove(layer);
                    mapControl.Refresh();
                }

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
                        if (mapControl.InvokeRequired)
                        {
                            mapControl.Invoke(new Action(delegate
                            {
                                mapControl.Overlays.Remove(layer);
                                mapControl.Refresh();
                            }));
                        }
                        else
                        {
                            mapControl.Overlays.Remove(layer);
                            mapControl.Refresh();
                        }
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
        public GMapOverlay GetLayer(string layerName)
        {
            lock (layerDic)
            {
                if (!layerDic.ContainsKey(layerName)) return null;
                return layerDic[layerName];
            }
        }

        /// <summary>
        /// 清除当前图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ClearLayer(string layerName)
        {
            lock (layerDic)
            {
                if (layerDic.ContainsKey(layerName))
                {
                    GMapOverlay overlay = layerDic[layerName];
                    if (overlay == null) return;

                    if (mapControl.InvokeRequired)
                    {
                        mapControl.Invoke(new Action(delegate
                        {
                            overlay.Clear();
                        }));
                    }
                    else
                    {
                        overlay.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// 清除所有图层
        /// </summary>
        public void ClearLayer()
        {
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke(new Action(delegate
                {
                    mapControl.Overlays.Clear();
                }));
            }
            else
            {
                mapControl.Overlays.Clear();
            }

            lock (layerDic)
            {
                layerDic.Clear();
            }
        }

        /// <summary>
        /// 显示、隐藏图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible">显示、隐藏</param>
        public void ShowLayer(string layerName, bool visible)
        {
            if (!layerDic.ContainsKey(layerName)) return;
            layerDic[layerName].IsVisibile = visible;
        }
    }
}
