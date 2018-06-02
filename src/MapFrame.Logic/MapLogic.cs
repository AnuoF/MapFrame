/**************************************************************************
 * 类名：MapLogic.cs
 * 描述：地图主逻辑处理类
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Interface;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using MapFrame.Core.Model;

namespace MapFrame.Logic
{
    /// <summary>
    /// 地图主逻辑处理类
    /// </summary>
    public class MapLogic : IMapLogic
    {
        /// <summary>
        /// 工具箱
        /// </summary>
        private IMFToolBox toolBox = null;
        /// <summary>
        /// 地图工厂接口，在这个类中创建
        /// </summary>
        private IMapFactory mapFactory = null;
        /// <summary>
        /// 图层字典
        /// </summary>
        private Dictionary<string, IMFLayer> layerDic = null;
        /// <summary>
        /// 地图引擎类型
        /// </summary>
        private MapEngineType mapEngineType = MapEngineType.GMap;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="engineType">地图类型</param>
        /// <param name="mapObject">地图控件对象</param>
        public MapLogic(MapEngineType engineType, object mapObject = null)
        {
            layerDic = new Dictionary<string, IMFLayer>();

            mapEngineType = engineType;
            LoadMapEngine(mapObject);
        }

        #region IMapLogic Function


        /// <summary>
        /// 获取地图控件
        /// </summary>
        /// <returns></returns>
        public object GetMapControl()
        {
            return mapFactory.GetMapControl();
        }

        /// <summary>
        /// 获取地图接口对象
        /// </summary>
        /// <returns></returns>
        public IMFMap GetIMFMap()
        {
            return mapFactory.GetIMFMap();
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns>图层</returns>
        public IMFLayer AddLayer(string layerName)
        {
            if (layerDic.ContainsKey(layerName))
            {
                return layerDic[layerName];
            }
            else
            {
                object mapControl = mapFactory.GetMapControl();
                var ret = mapFactory.AddLayer(layerName);
                if (ret == false)
                    return null;

                IMFLayer layer = new MFLayer(layerName, mapFactory);
                layer.MapControl = mapControl;

                lock (layerDic)
                {
                    layerDic.Add(layerName, layer);
                }

                return layer;
            }
        }

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns>图层</returns>
        public IMFLayer GetLayer(string layerName)
        {
            lock (layerDic)
            {
                if (layerDic.ContainsKey(layerName))
                {
                    return layerDic[layerName];
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取所有图元
        /// </summary>
        /// <returns></returns>
        public List<IMFLayer> GetLayers()
        {
            lock (layerDic)
            {
                List<IMFLayer> layerList = new List<IMFLayer>();

                foreach (IMFLayer layer in layerDic.Values)
                {
                    layerList.Add(layer);
                }

                return layerList;
            }
        }

        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool RemoveLayer(string layerName)
        {
            if (layerDic.ContainsKey(layerName))
            {
                mapFactory.RemoveLayer(layerName);

                lock (layerDic)
                {
                    layerDic.Remove(layerName);
                }
            }
            return true;
        }

        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="layer">图层</param>
        public bool RemoveLayer(IMFLayer layer)
        {
            string layerName = layer.LayerName;

            if (layerDic.ContainsKey(layerName))
            {
                mapFactory.RemoveLayer(layerName);
                lock (layerDic)
                {
                    layerDic.Remove(layerName);
                }
            }
            return true;
        }

        /// <summary>
        /// 删除所有图层
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllLayer()
        {
            var ret = mapFactory.RemoveAllLayer();

            lock (layerDic)
            {
                layerDic.Clear();
            }

            return ret;
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <returns>图元</returns>
        public IMFElement GetElement(string elementName)
        {
            lock (layerDic)
            {
                IMFElement element = null;

                foreach (IMFLayer layer in layerDic.Values)
                {
                    var ret = layer.GetElement(elementName);
                    if (ret != null)
                    {
                        element = ret;
                        break;
                    }
                }

                return element;
            }
        }

        /// <summary>
        /// 设置图元显示或隐藏
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible">显示或隐藏</param>
        public bool SetLayerVisible(string layerName, bool visible)
        {
            lock (layerDic)
            {
                if (layerDic.ContainsKey(layerName))
                {
                    layerDic[layerName].SetLayerVisible(visible);
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// 清除所有图层
        /// </summary>
        public bool ClearLayer()
        {
            lock (layerDic)
            {
                mapFactory.ClearLayer();
                layerDic.Clear();
                return true;
            }
        }

        /// <summary>
        /// 获取工具箱对象
        /// </summary>
        /// <returns></returns>
        public IMFToolBox GetToolBox()
        {
            return toolBox;
        }

        /// <summary>
        /// 重绘
        /// </summary>
        public void Refresh()
        {
            mapFactory.Refresh();
        }

        /// <summary>
        /// 刷新图层
        /// </summary>
        /// <param name="layer"></param>
        public void RefreshLayer(IMFLayer layer)
        {
            layer.Refresh();
        }


        /// <summary>
        /// 刷新图层
        /// </summary>
        /// <param name="layerName"></param>
        public void RefreshLayer(string layerName)
        {
            lock (layerDic)
            {
                if (layerDic.ContainsKey(layerName))
                    layerDic[layerName].Refresh();
            }
        }

        #endregion

        #region Private Function

        /// <summary>
        /// 加载地图引擎
        /// </summary>
        /// <param name="mapObject">地图控件对象</param>
        private void LoadMapEngine(object mapObject)
        {
            mapFactory = CreateFactory(mapObject);
            toolBox = CreateToolBox(mapFactory.GetMapControl());
        }

        /// <summary>
        /// 创建地图工厂对象
        /// </summary>
        /// <param name="mapObject">地图控件对象</param>
        /// <returns>IFactory</returns>
        private IMapFactory CreateFactory(object mapObject)
        {
            IMapFactory fac = null;

            #region 反射
            object RetObj = null;
            object[] obj = new object[] { mapObject };
            LibraryInfo info = GetLibraryInfoByName("MapEngine", mapEngineType.ToString());
            if (info == null) return null;

            try
            {
                Assembly asm = Assembly.Load(info.LibraryName);
                Type[] tp = asm.GetTypes();
                RetObj = (object)asm.CreateInstance(info.NameSpaceClass, true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
                , null, obj, CultureInfo.CurrentCulture, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("通过反射创建IMapFactory对象异常：{0}", ex.Message));
                return null;
            }
            if (null == RetObj)
            {
                Debug.WriteLine("通过反射创建IMapFactory对象失败！");
                return null;
            }

            fac = RetObj as IMapFactory;
            #endregion

            return fac;
        }

        /// <summary>
        /// 创建地图地图工具箱
        /// </summary>
        /// <param name="mapObject">地图控件对象</param>
        /// <returns></returns>
        private IMFToolBox CreateToolBox(object mapObject)
        {
            IMFToolBox _toolBox = null;

            #region 反射
            object RetObj = null;
            object[] obj = new object[] { mapObject, this };
            LibraryInfo info = GetLibraryInfoByName("MapTool", mapEngineType.ToString());
            if (info == null) return null;

            try
            {
                Assembly asm = Assembly.Load(info.LibraryName);
                Type[] tp = asm.GetTypes();
                RetObj = (object)asm.CreateInstance(info.NameSpaceClass, true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
                , null, obj, CultureInfo.CurrentCulture, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("通过反射创建IToolBox对象异常：{0}", ex.Message));
                return null;
            }
            if (null == RetObj)
            {
                Debug.WriteLine("通过反射创建IToolBox对象失败！");
                return null;
            }

            _toolBox = RetObj as IMFToolBox;
            #endregion

            return _toolBox;
        }

        /// <summary>
        /// 根据类型和名称获取反射库的信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="engineType">地图类型</param>
        /// <returns></returns>
        private LibraryInfo GetLibraryInfoByName(string type, string engineType)
        {
            string xmlPath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "MapFrame.Config.xml");
            if (!File.Exists(xmlPath)) return null;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                string mapType;
                if (string.IsNullOrEmpty(engineType))
                {
                    XmlNode runNode = doc.SelectSingleNode("MapFrame/Run");
                    if (runNode == null) return null;
                    mapType = runNode.Attributes["value"].InnerXml;
                }
                else
                    mapType = engineType;

                LibraryInfo info = null;

                if (type == "MapEngine")
                {
                    XmlNode engineNode = doc.SelectSingleNode("MapFrame/MapEngine");
                    if (engineNode == null) return null;

                    XmlNodeList engineNodeList = engineNode.ChildNodes;
                    foreach (XmlNode n in engineNodeList)
                    {
                        if (n.Attributes["Type"].InnerXml != mapType) continue;

                        info = new LibraryInfo();
                        info.LibraryName = n.Attributes["LibraryName"].InnerXml;
                        info.NameSpaceClass = n.Attributes["NameSpace.Class"].InnerXml;
                        break;
                    }
                }
                else if (type == "MapTool")
                {
                    XmlNode toolNode = doc.SelectSingleNode("MapFrame/MapTool");
                    if (toolNode == null) return null;

                    XmlNodeList toolNodeList = toolNode.ChildNodes;
                    foreach (XmlNode n in toolNodeList)
                    {
                        if (n.Attributes["Type"].InnerXml != mapType) continue;

                        info = new LibraryInfo();
                        info.LibraryName = n.Attributes["LibraryName"].InnerXml;
                        info.NameSpaceClass = n.Attributes["NameSpace.Class"].InnerXml;
                        break;
                    }
                }

                return info;
            }
            catch
            {
                throw;
            }
        }
        #endregion


    }
}
