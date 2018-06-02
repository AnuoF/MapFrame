/********************************************************************************
** 文件名：TSDataRecv.cs
** 版 本：1.0
** 内容简述：数据接收类
** 创建日期：Nov 7,2016
** 创建人：王喜进 
** 修改记录：
*********************************************************************************/

using GlobleSituation.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using ZC57S.ZKNet;

namespace GlobleSituation.Business
{
    /// <summary>
    /// 数据接收类
    /// </summary>
    class TSDataRecv : IClientNetService
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        //private TCPClient client = null;
        /// <summary>
        /// 态势数据缓存队列
        /// </summary>
        private Queue<NetMessage> tsDataQueue = null;
        /// <summary>
        /// 波束数据缓存队列
        /// </summary>
        private Queue<NetMessage> beamDataQueue = null;
        /// <summary>
        /// 是否处理数据
        /// </summary>
        private bool bDealTsData = false;
        /// <summary>
        /// 是否处理波束数据
        /// </summary>
        private bool bDealBeamData = false;

        private List<TCPClient> clientList = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TSDataRecv()
        {
            clientList = new List<TCPClient>();
            tsDataQueue = new Queue<NetMessage>();
            beamDataQueue = new Queue<NetMessage>();
            ThreadPool.QueueUserWorkItem(obj => { PushData(); });
        }

        /// <summary>
        /// 初始化接收数据
        /// </summary>
        public void InitDataRecv()
        {
            List<ServerInfo> serverList =GetTcpServerInfo();
            if (serverList == null || serverList.Count <= 0) return;

            foreach (ServerInfo server in serverList)
            {
                TCPClient client = null;
                client = new TCPClient(server.Ip, server.Port, this);
                client.Start();

                clientList.Add(client);
            }
        }

        /// <summary>
        /// 开始处理态势数据
        /// </summary>
        public void StartTs()
        {
            bDealTsData = true;
        }

        /// <summary>
        /// 停止处理态势数据
        /// </summary>
        public void StopTs()
        {
            bDealTsData = false;
        }

        /// <summary>
        /// 开始处理波束数据
        /// </summary>
        public void StartBeam()
        {
            bDealBeamData = true;
        }

        /// <summary>
        /// 停止处理波束数据
        /// </summary>
        public void StopBeam()
        {
            bDealBeamData = false;
        }

        #region IClientNetService
        public void OnConnected(IClientNetConnection connection)
        {

        }

        public void OnDisconnected(IClientNetConnection connection)
        {
            foreach (TCPClient client in clientList)
            {
                client.Stop();
                client.Start();
            }
        }

        public void OnException(NetException exception)
        {
        }

        // 接收数据
        public void OnReceived(IClientNetConnection connection, NetMessage msg)
        {
            // 态势数据
            if (msg.Buffer.Length > 40 && bDealTsData == true)
            {
                lock (tsDataQueue)
                    tsDataQueue.Enqueue(msg);
            }

            // 波束数据
            if (msg.Buffer.Length < 40 && bDealBeamData == true)
            {
                lock (beamDataQueue)
                    beamDataQueue.Enqueue(msg);
            }
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
                if (tsDataQueue.Count > 0)
                {
                    lock (tsDataQueue)
                    {
                        NetMessage msg = tsDataQueue.Dequeue();
                        if (msg != null)
                            EventPublisher.PublishTSDataEvent(this, new Model.TSDataEventArgs(msg.Buffer));
                    }
                }
                else if (beamDataQueue.Count > 0)
                {
                    lock (beamDataQueue)
                    {
                        NetMessage msg = beamDataQueue.Dequeue();
                        if (msg != null)
                            EventPublisher.PublishBeamDataComeEvent(this, Model.BeamData.ByteToClass(msg.Buffer));
                    }
                }
                else
                    Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 从配置文件获取IP、Port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private List<ServerInfo> GetTcpServerInfo()
        {
            try
            {
                string xmlConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();

                doc.Load(xmlConfig);
                XmlNodeList nodes = doc.SelectSingleNode("Globe/Config/TCPServer").ChildNodes;

                List<ServerInfo> serverInfoList = new List<ServerInfo>();

                foreach (XmlNode node in nodes)
                {
                    ServerInfo info = new ServerInfo();
                    info.Ip = node.Attributes["Ip"].InnerXml;
                    info.Port = Convert.ToInt32(node.Attributes["Port"].InnerXml);
                    serverInfoList.Add(info);
                }

                return serverInfoList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 服务端信息
    /// </summary>
    class ServerInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip;

        /// <summary>
        /// 端口
        /// </summary>
        public int Port;
    }
}
