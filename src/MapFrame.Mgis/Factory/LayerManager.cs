
using System;
using System.Collections.Generic;
using AxHOSOFTMapControlLib;

namespace MapFrame.Mgis.Factory
{
    class LayerManager
    {
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl = null;
        /// <summary>
        /// 图层字典
        /// </summary>
        private Dictionary<string, ulong> layerDic = null;
        /// <summary>
        /// 刷新地图委托
        /// </summary>
        public delegate void RefreshMapControlDelegate();
        public RefreshMapControlDelegate RefreshMapDelegate;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        public LayerManager(AxHOSOFTMapControl _mapControl)
        {
            mapControl = _mapControl;
            layerDic = new Dictionary<string, ulong>();
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
                mapControl.MgsAddTsLayer(layerName);
                ulong layerPrt = mapControl.MgsGetLayerPtrByName(layerName);
                layerDic.Add(layerName, layerPrt);
                return true;
            }
        }

        /// <summary>
        /// 移除指定图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public bool RemoverLayer(string layerName)
        {
            lock (layerDic)
            {
                if (!layerDic.ContainsKey(layerName)) return true;

                if (mapControl.InvokeRequired)
                {
                    mapControl.Invoke(new Action(delegate
                    {
                        mapControl.MgsDeleteTsLayer(layerName);
                    }));
                }
                else
                {
                    mapControl.MgsDeleteTsLayer(layerName);
                }

                layerDic.Remove(layerName);
            }

            RefreshMapDelegate();
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
                lock (layerDic)
                {
                    foreach (var layerName in layerDic.Keys)
                    {
                        if (mapControl.InvokeRequired)
                        {
                            mapControl.Invoke(new Action(delegate
                            {
                                mapControl.MgsDeleteTsLayer(layerName);
                            }));
                        }
                        else
                        {
                            mapControl.MgsDeleteTsLayer(layerName);
                        }
                    }

                    layerDic.Clear();
                }

                RefreshMapDelegate();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 清除所有图层上的图元，清除图元直接删除图层（因为没找到Clear图层的接口）
        /// </summary>
        public void ClearLayer()
        {
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke(new Action(delegate
                {
                    lock (layerDic)
                    {
                        foreach (var item in layerDic)
                        {
                            mapControl.MgsDeleteTsLayer(item.Key);
                            layerDic.Remove(item.Key);

                            mapControl.MgsAddTsLayer(item.Key);
                            ulong layerPrt = mapControl.MgsGetLayerPtrByName(item.Key);
                            layerDic.Add(item.Key, layerPrt);
                        }
                    }
                }));
            }
            else
            {
                lock (layerDic)
                {
                    foreach (var item in layerDic)
                    {
                        mapControl.MgsDeleteTsLayer(item.Key);
                        layerDic.Remove(item.Key);

                        mapControl.MgsAddTsLayer(item.Key);
                        ulong layerPrt = mapControl.MgsGetLayerPtrByName(item.Key);
                        layerDic.Add(item.Key, layerPrt);
                    }
                }
            }

            RefreshMapDelegate();
        }

        /// <summary>
        /// 清除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ClearLayer(string layerName)
        {
            if (mapControl.InvokeRequired)
            {
                mapControl.Invoke(new Action(delegate
                {
                    lock (layerDic)
                    {
                        mapControl.MgsDeleteTsLayer(layerName);
                        layerDic.Remove(layerName);

                        mapControl.MgsAddTsLayer(layerName);
                        ulong layerPrt = mapControl.MgsGetLayerPtrByName(layerName);
                        layerDic.Add(layerName, layerPrt);
                    }
                }));
            }
            else
            {
                lock (layerDic)
                {
                    mapControl.MgsDeleteTsLayer(layerName);
                    layerDic.Remove(layerName);

                    mapControl.MgsAddTsLayer(layerName);
                    ulong layerPrt = mapControl.MgsGetLayerPtrByName(layerName);
                    layerDic.Add(layerName, layerPrt);
                }
            }

            RefreshMapDelegate();
        }

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        public ulong GetLayer(string layerName)
        {
            lock (layerDic)
            {
                mapControl.MgsGetLayerPtrByName(layerName);
                if (!layerDic.ContainsKey(layerName)) return 0;
                return layerDic[layerName];
            }
        }

        /// <summary>
        /// 设置图层可见不可见
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visable"></param>
        public void SetLayerVisable(string layerName, bool visable)
        {
            int iVisable = visable == true ? 1 : 0;
            mapControl.setLayerVisible(layerName, iVisable);
        }
    }
}
