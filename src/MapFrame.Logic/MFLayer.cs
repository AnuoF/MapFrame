/**************************************************************************
 * 类名：MFLayer.cs
 * 描述：图层类
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Common;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Collections.Generic;
using System.Text;

namespace MapFrame.Logic
{
    /// <summary>
    /// 图层类  
    /// </summary>
    class MFLayer : IMFLayer
    {
        /// <summary>
        /// 工厂接口对象
        /// </summary>
        private IMapFactory _mapFactory;
        /// <summary>
        /// 图元字典
        /// </summary>
        private Dictionary<string, IMFElement> _elementDic;

        private string _name;
        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName
        {
            get { return _name; }
            set { _name = value; }
        }

        private bool _isvisible;
        /// <summary>
        /// 图层是否可见true显示,false隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return _isvisible; }
            set { _isvisible = value; }
        }

        /// <summary>
        /// 图层所在地图控件
        /// </summary>
        public object MapControl
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">图层名称</param>
        /// <param name="_mapFac">工厂接口对象</param>
        public MFLayer(string name, IMapFactory _mapFac)
        {
            _elementDic = new Dictionary<string, IMFElement>();
            _name = name;
            _mapFactory = _mapFac;
        }

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kmlStr">kml字符串</param>
        /// <returns></returns>
        public bool AddElement(string kmlStr)
        {
            bool ishaveUnkonw;
            Kml kml = XmlHelper.XmlDeserialize<Kml>(kmlStr, Encoding.UTF8, out ishaveUnkonw);
            if (kml == null) return false;

            return AddElement(kml);
        }

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <returns></returns>
        public bool AddElement(Kml kml)
        {
            // 检查是否已经添加相同键的图元，如果有，则返回失败
            if (string.IsNullOrEmpty(kml.Placemark.Name)) return false;

            lock (_elementDic)
            {
                if (_elementDic.ContainsKey(kml.Placemark.Name)) return false;
            }

            // 创建图元
            IMFElement element = _mapFactory.AddElement(LayerName, kml);
            // 如果图元创建失败，则返回失败
            if (element == null) return false;
            // 设置图元所属图层
            element.BelongLayer = this;

            lock (_elementDic)
            {
                // 添加到字典
                _elementDic.Add(element.ElementName, element);
            }

            return true;
        }

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="_element">kml对象</param>
        /// <returns></returns>
        public bool AddElement(Kml kml, out IMFElement _element)
        {
            lock (_elementDic)
            {
                _element = null;
                // 检查是否已经添加相同键的图元，如果有，则返回失败
                if (string.IsNullOrEmpty(kml.Placemark.Name)) return false;
                if (_elementDic.ContainsKey(kml.Placemark.Name)) return false;

                // 创建图元
                _element = _mapFactory.AddElement(LayerName, kml);
                // 如果图元创建失败，则返回失败
                if (_element == null) return false;

                // 设置图元所属图层
                _element.BelongLayer = this;
                // 添加到字典
                _elementDic.Add(_element.ElementName, _element);
                return true;
            }
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <returns>true，成功；false，失败</returns>
        public bool RemoveElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName)) return false;

            bool ret = false;
            IMFElement element = null;

            lock (_elementDic)
            {
                if (!_elementDic.ContainsKey(elementName)) return false;
                element = _elementDic[elementName];
            }

            if (element != null)
            {
                ret = _mapFactory.RemoveElement(LayerName, element);
            }

            if (ret)
            {
                lock (_elementDic)
                {
                    _elementDic.Remove(elementName);
                }

                _mapFactory.Refresh();
            }

            return ret;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <returns>true，成功；false，失败</returns>
        /// <param name="element">图元</param>
        public bool RemoveElement(string elementName, ref IMFElement element)
        {
            bool flag = this.RemoveElement(elementName);
            if (flag) element = null;
            return flag;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <returns>true，成功；false，失败</returns>
        public bool RemoveElement(IMFElement element)
        {
            bool flag = this.RemoveElement(element.ElementName);
            if (flag) element = null;
            return flag;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="element">图元</param>
        /// <returns></returns>
        public bool RemoveElement(ref IMFElement element)
        {
            bool flag = RemoveElement(element.ElementName);
            if (flag)
                element = null;
            return flag;
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <returns></returns>
        public IMFElement GetElement(string elementName)
        {
            lock (_elementDic)   // 加锁，防止在删除的同时，还可以查下到已删除的图元
            {
                if (string.IsNullOrEmpty(elementName)) return null;

                if (_elementDic.ContainsKey(elementName))
                    return _elementDic[elementName];
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取当前图层上图元数量
        /// </summary>
        /// <returns></returns>
        public int GetElementCount()
        {
            return _elementDic.Count;
        }

        /// <summary>
        /// 清除图元
        /// </summary>
        public void ClearElement()
        {
            lock (_elementDic)
            {
                _mapFactory.ClearLayer(LayerName);

                foreach (IMFElement ele in _elementDic.Values)
                {
                    ele.Update();
                    ele.Dispose();
                }

                _elementDic.Clear();
            }
        }

        /// <summary>
        /// 显示、隐藏图层
        /// </summary>
        /// <param name="isVisible">显示或隐藏</param>
        public void SetLayerVisible(bool isVisible)
        {
            _isvisible = isVisible;
            _mapFactory.SetLayerVisiable(LayerName, isVisible);
            _mapFactory.Refresh();
        }

        /// <summary>
        /// 设置图元显示或隐藏
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <param name="visible">显示隐藏</param>
        public void SetElementVisible(string elementName, bool visible)
        {
            lock (_elementDic)   // 加锁，防止在删除的同时，还可以查下到已删除的图元
            {
                if (string.IsNullOrEmpty(elementName)) return;

                if (_elementDic.ContainsKey(elementName))
                    _elementDic[elementName].SetVisible(visible);
            }
        }

        /// <summary>
        /// 获取图层上所有图元
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IMFElement> GetElementDictionary()
        {
            lock (_elementDic)
            {
                return _elementDic;
            }
        }

        /// <summary>
        /// 获取图层上所有图元
        /// </summary>
        /// <returns></returns>
        public List<IMFElement> GetElementList()
        {
            lock (_elementDic)
            {
                List<IMFElement> elementList = new List<IMFElement>();

                foreach (IMFElement element in _elementDic.Values)
                {
                    elementList.Add(element);
                }

                return elementList;
            }
        }

        /// <summary>
        /// 更新图层
        /// </summary>
        public void Refresh()
        {
            _mapFactory.Refresh(this);
        }

        /// <summary>
        /// 设置图层栅格化（三维地图使用的接口）
        /// </summary>
        /// <param name="bRasterize">是否栅格化</param>
        public void SetRasterize(bool bRasterize)
        {
            _mapFactory.SetRasterize(LayerName, bRasterize);
        }

        /// <summary>
        /// 获取图层栅格化的属性（三维地图接口）
        /// </summary>
        public void GetRasterize()
        {
            _mapFactory.GetRasterize(LayerName);
        }

        /// <summary>
        /// 设置图层透明度（三维地图接口）
        /// </summary>
        /// <param name="transparency">透明度</param>
        public void SetTransparency(short transparency)
        {
            _mapFactory.SetTransparency(LayerName, transparency);
        }
    }
}
