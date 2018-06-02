/**************************************************************************
 * 类名：Model3dFactory.cs
 * 描述：3D模型图元工厂类
 * 作者：Allen
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using ESRI.ArcGIS.Analyst3D;
using System.IO;
using System.Collections.Generic;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcGlobe.Element;

namespace MapFrame.ArcGlobe.Factory
{
    /// <summary>
    /// 3D模型图元工厂类
    /// </summary>
    class Model3dFactory : IElementFactory
    {
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;
        /// <summary>
        /// 模型对象字典
        /// </summary>
        private Dictionary<string, IImport3DFile> filePathDic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public Model3dFactory(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
            filePathDic = new Dictionary<string, IImport3DFile>();
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        /// <param name="kml">模型的kml</param>
        /// <param name="layer">模型所在的图层</param>
        /// <returns></returns>
        public IMFElement CreateElement(Kml kml, ILayer layer)
        {
            KmlModel3d modelkml = kml.Placemark.Graph as KmlModel3d;
            if (modelkml == null) return null;
            if (modelkml.Position == null) return null;
            if (!File.Exists(modelkml.ModelFilePath)) return null;
            int index = -1;

            //图层
            IGlobeGraphicsLayer graphicLayer = layer as IGlobeGraphicsLayer;
            if (graphicLayer == null) return null;

            //实例化图元
            Model3d_ArcGlobe modelElement = null;

            //添加图元
            this.Dosomething((Action)delegate()
            {
                IImport3DFile import3Dfile = null;
                if (!filePathDic.ContainsKey(modelkml.ModelFilePath))
                {
                    import3Dfile = new Import3DFileClass();
                    import3Dfile.CreateFromFile(modelkml.ModelFilePath);
                    filePathDic.Add(modelkml.ModelFilePath, import3Dfile);
                }
                else//模型已创建
                {
                    import3Dfile = filePathDic[modelkml.ModelFilePath];
                }

                modelElement = new Model3d_ArcGlobe(graphicLayer, modelkml, import3Dfile);

                //设置属性
                GlobeGraphicsElementPropertiesClass properties = new GlobeGraphicsElementPropertiesClass();
                properties.Rasterize = modelkml.Rasterize;

                graphicLayer.AddElement(modelElement, properties, out index);
                modelElement.Index = index;
                modelElement.ElementName = kml.Placemark.Name;
            }, true);

            return modelElement;
        }

        /// <summary>
        /// 删除图元
        /// </summary>
        /// <param name="element">图元对象</param>
        /// <param name="layer">图元所在的图层</param>
        /// <returns></returns>
        public bool RemoveElement(IMFElement element, ILayer layer)
        {
            Model3d_ArcGlobe modelElement = element as Model3d_ArcGlobe;
            IGraphicsContainer graphicsContainer = layer as IGraphicsContainer;
            this.Dosomething((Action)delegate()
            {
                graphicsContainer.DeleteElement(modelElement);
            }, true);

            return true;
        }

        /// <summary>
        /// 主线程做事
        /// </summary>
        /// <param name="action">要做的内容</param>
        /// <param name="synchronization">是否同步执行</param>
        private void Dosomething(Action action, bool synchronization)
        {
            if (mapControl == null) return;
            if (synchronization)
            {
                if (mapControl.InvokeRequired)
                    mapControl.Invoke(action);
                else
                    action();
            }
            else
            {
                if (mapControl.InvokeRequired)
                    mapControl.BeginInvoke(action);
                else
                    action();
            }
        }
    }
}
