/********************************************************************************
** 文件名：DataRecv.cs
** 版 本：1.0
** 内容简述：数据接收类
** 创建日期：Nov 7,2016
** 创建人：王喜进 
** 修改记录：
*********************************************************************************/

using GlobleSituation.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using ZC57S.ZKNet;

namespace GlobleSituation.Business
{
    /// <summary>
    /// 数据接收类
    /// </summary>
    class DataRecv: IClientNetService
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        private TCPClient client = null;
        /// <summary>
        /// 数据缓存队列
        /// </summary>
        private Queue<NetMessage> dataQueue = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataRecv()
        {
            dataQueue = new Queue<NetMessage>();
            ThreadPool.QueueUserWorkItem(obj => { PushData(); });
        }

        /// <summary>
        /// 初始化接收数据
        /// </summary>
        public void InitDataRecv()
        {
            string ip;
            int port;
            if (GetTcpServerInfo(out ip, out port) == false)
                return;

            client = new TCPClient(ip, port, this);
            client.Start();
        }

        #region IClientNetService
        public void OnConnected(IClientNetConnection connection)
        {
        }

        public void OnDisconnected(IClientNetConnection connection)
        {
        }

        public void OnException(NetException exception)
        {
        }

        // 接收数据
        public void OnReceived(IClientNetConnection connection, NetMessage msg)
        {
            lock(dataQueue)
                dataQueue.Enqueue(msg);

            Debug.WriteLine(msg.Buffer.Length);
        }

        public void OnSent(IClientNetConnection connection, NetMessage msg)
        {
        }
        #endregion

        /// <summary>
        /// 推送数据
        /// </summary>
        private void PushData()
        {
            while (true)
            {
                if (dataQueue.Count > 0)
                {
                    lock (dataQueue)
                    {
                        NetMessage msg = dataQueue.Dequeue();
                        if (msg != null)
                            EventPublisher.PublishTSDataEvent(this, new Model.TSDataEventArgs(msg.Buffer));
                    }

                    Thread.Sleep(100);
                }
                else
                    Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 从配置文件获取IP、Port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private bool GetTcpServerInfo(out string ip, out int port)
        {
            ip = "127.0.0.1";
            port = 5005;

            string xmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xmlConfig);
                XmlNodeList nodes = doc.SelectSingleNode("Globe/Config").ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["key"].InnerXml == "TCPServer")
                    {
                        ip = node.Attributes["Ip"].InnerXml;
                        port = Convert.ToInt32(node.Attributes["Port"].InnerXml);
                        break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


    }
}
