/********************************************************************************
** 文件名：ElementInfo.cs
** 版 本：1.0
** 内容简述：图元信息类
** 创建日期：Nov 11,2016
** 创建人：Allen 
** 修改记录：
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobleSituation.Model;

namespace GlobleSituation.Business
{
    class ArcGlobeElementMgr
    {
        /// <summary>
        /// 地球上绘制的图元集合
        /// </summary>
        private Dictionary<string, ElementInfo> elementDic = null;


        public ArcGlobeElementMgr()
        {
            elementDic = new Dictionary<string, ElementInfo>();
        }

        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="elementName">图元名称</param>
        /// <param name="elementObject">图元信息</param>
        public void AddElement(string elementName, ElementInfo elementInfo)
        {
            if (elementDic.ContainsKey(elementName))
            {
                elementDic[elementName] = elementInfo;
            }
            else
                elementDic.Add(elementName, elementInfo);
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="elementName"></param>
        public void RemoveElement(string elementName)
        {
            if (elementDic.ContainsKey(elementName))
                elementDic.Remove(elementName);
        }

    }
}
