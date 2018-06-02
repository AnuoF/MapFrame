/**************************************************************************
 * 类名：LayerManger.cs
 * 描述：图层管理
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using ESRI.ArcGIS.Controls;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;

namespace MapFrame.ArcMap.Factory
{
    /// <summary>
    /// 图层管理
    /// </summary>
    class LayerManger
    {
        /// <summary>
        /// ArcMap地图控件
        /// </summary>
        private AxMapControl axMapControl = null;
        /// <summary>
        /// 图层字典
        /// </summary>
        private Dictionary<string, CompositeGraphicsLayerClass> layerDic = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        MapFrame.Core.Interface.IMapFactory mapFactory = null;

        public LayerManger(AxMapControl _axMapControl, MapFrame.Core.Interface.IMapFactory _mapFac)
        {
            mapFactory = _mapFac;
            axMapControl = _axMapControl;
            layerDic = new Dictionary<string, CompositeGraphicsLayerClass>();
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public bool AddLayer(string layerName)
        {
            if (layerDic.ContainsKey(layerName)) return true;

            CompositeGraphicsLayerClass layer = new CompositeGraphicsLayerClass();
            layer.Name = layerName;
            layer.Visible = true;
            axMapControl.Map.AddLayer(layer);
            layerDic.Add(layerName, layer);

            return true;
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public bool RemoverLayer(string layerName)
        {
            if (!layerDic.ContainsKey(layerName)) return true;

            ILayer layer = layerDic[layerName];
            axMapControl.Map.DeleteLayer(layer);
            layerDic.Remove(layerName);

            return true;
        }

        /// <summary>
        /// 移除所有图层
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllLayer()
        {
            try
            {
                foreach (var layer in layerDic.Values)
                {
                    axMapControl.Map.DeleteLayer(layer);
                }
                layerDic.Clear();

                return true;
            }
            catch (System.Exception)
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
            return layerDic[layerName];//修改
        }

        /// <summary>
        /// 清除当前图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ClearLayer(string layerName)
        {
            if (!layerDic.ContainsKey(layerName)) return;

            CompositeGraphicsLayerClass graLayer = layerDic[layerName];
            if (graLayer != null)
                graLayer.DeleteAllElements();
        }

        /// <summary>
        /// 清除所有图层
        /// </summary>
        public void ClearLayer()
        {
            foreach (CompositeGraphicsLayerClass layer in layerDic.Values)
            {
                if (layer != null)
                    layer.DeleteAllElements();
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

            ILayer layer = layerDic[layerName];
            if (layer != null)
                layer.Visible = visible;
        }

    }
}
