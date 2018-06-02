
using System;
using ZC57S.ZKNet;
using GlobleSituation.Common;
using System.Xml;
using GlobleSituation.Model;

namespace GlobleSituation.Business
{
    public class GXStroreClient : IClientNetService
    {

        private TCPClient client = null;

        public GXStroreClient()
        {
            InitTCPClient();

            EventPublisher.SendInsertDataToStoreEvent += new EventHandler<Model.SendInsertDataToStoreEventArgs>(EventPublisher_SendInsertDataToStoreEvent);
            EventPublisher.SendSearchDataToStoreEvent += new EventHandler<SendSearchDataToStoreEventArgs>(EventPublisher_SendSearchDataToStoreEvent);
        }


        public void OnConnected(IClientNetConnection connection)
        {
            client.SetConnection(connection);
        }

        // 断开之后重连
        public void OnDisconnected(IClientNetConnection connection)
        {
            try
            {
                client.Stop();
                client.Start();
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(GXStroreClient), ex.Message);
            }
        }

        public void OnReceived(IClientNetConnection connection, NetMessage msg)
        {
            DealRecvData(msg.Buffer);
        }

        public void OnSent(IClientNetConnection connection, NetMessage msg)
        {
        }

        public void OnException(NetException exception)
        {
        }

        // 初始化客户端
        private void InitTCPClient()
        {
            try
            {
                string xmlConfig = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/StroreInfo");
                string ip = node.Attributes["Ip"].InnerXml;
                int port = Convert.ToInt32(node.Attributes["Port"].InnerXml);

                client = new TCPClient(ip, port, this);
                client.Start();
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(GXStroreClient), ex.Message);
            }
        }

        // 入库请求
        private void EventPublisher_SendInsertDataToStoreEvent(object sender, Model.SendInsertDataToStoreEventArgs e)
        {
            byte type = 0;    // 类型为入库
            int count = e.DataList.Count;
            int length = 1 + 69 * count;
            byte[] data = new byte[length];   // 带发送的数据

            data[0] = type;

            byte[] arr = new byte[69 * count];

            for (int i = 0; i < count; i++)
            {
                byte[] tmp = e.DataList[i].ToDataBytes();
                Buffer.BlockCopy(tmp, 0, arr, i * 69, 69);
            }

            Buffer.BlockCopy(arr, 0, data, 1, 69 * count);

            client.Send(data);    // 向存储服务发送入库数据
        }

        // 查询请求
        private void EventPublisher_SendSearchDataToStoreEvent(object sender, SendSearchDataToStoreEventArgs e)
        {
            byte type = 1;
            byte[] sqlArr = System.Text.Encoding.UTF8.GetBytes(e.SqlStr);
            byte[] data = new byte[sqlArr.Length + 1];

            data[0] = type;
            Buffer.BlockCopy(sqlArr, 0, data, 1, sqlArr.Length);
            client.Send(data);   // 向存储服务发送查询数据
        }

        /// <summary>
        /// 处理接收的数据（查询结果）
        /// </summary>
        /// <param name="data"></param>
        private void DealRecvData(byte[] data)
        {
            EventPublisher.PublishRecvSearchDataEvent(this, new RecvSearchDataEventArgs() { Data = data });
        }


    }
}
