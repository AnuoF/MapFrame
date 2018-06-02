/*****************************************************
** 文件名：XmlHelperThreadSafety.cs
** 版 本：1.0
** 内容简述：xml转化为object，并且返回"是否存在未知元素"
** 创建日期：Feb 13,2014
** 创建人：王喜进 
** 修改记录：
*****************************************************/

using System;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace GlobleSituation.Common
{
    /// <summary>
    /// XmlHelper线程安全类
    /// </summary>
    public class XmlHelperThreadSafety
    {
        private XmlSerializer mySerializer = null;
        private bool m_IsHaveUnKnow = false;
        private object SerializerObj = new object();

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="isHaveUnKnow">是否存在未知元素</param>
        /// <returns>反序列化得到的对象</returns>
        public T XmlDeserialize<T>(string s, Encoding encoding, out bool isHaveUnKnow)
        {
            lock (SerializerObj)
            {
                isHaveUnKnow = false;
                if (string.IsNullOrEmpty(s))
                    throw new ArgumentNullException("s");
                if (encoding == null)
                    throw new ArgumentNullException("encoding");

                mySerializer = new XmlSerializer(typeof(T));
                mySerializer.UnknownElement += new XmlElementEventHandler(mySerializer_UnknownElement);
                mySerializer.UnknownAttribute += new XmlAttributeEventHandler(mySerializer_UnknownAttribute);
                mySerializer.UnknownNode += new XmlNodeEventHandler(mySerializer_UnknownNode);
                mySerializer.UnreferencedObject += new UnreferencedObjectEventHandler(mySerializer_UnreferencedObject);

                using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
                {
                    using (StreamReader sr = new StreamReader(ms, encoding))
                    {
                        XmlReader reader = XmlReader.Create(sr);
                        object obj = (T)mySerializer.Deserialize(reader);
                        isHaveUnKnow = m_IsHaveUnKnow;
                        return (T)obj;
                    }
                }
            }
        }

        private void mySerializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            lock (SerializerObj)
            {
                m_IsHaveUnKnow = true;
            }
        }

        private void mySerializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            lock (SerializerObj)
            {
                m_IsHaveUnKnow = true;
            }
        }

        private void mySerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            lock (SerializerObj)
            {
                m_IsHaveUnKnow = true;
            }
        }

        private void mySerializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            lock (SerializerObj)
            {
                m_IsHaveUnKnow = true;
            }
        }
    }
}
