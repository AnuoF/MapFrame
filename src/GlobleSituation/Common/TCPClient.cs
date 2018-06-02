/*****************************************************
** 文件名：Client.cs
** 版 本：1.0
** 内容简述：框架TCP功能封装
** 创建日期：未知
** 创建人：未知
** 修改记录：
*** 2015年4月30日10:54:34 姚正炜 修改如下 ***
***删改原事件为Connected Disconnected Received Sent Ex事件
*****************************************************/

using System;
using ZC57S.ZKNet;
using System.Threading;

namespace GlobleSituation.Common
{
    /// <summary>
    /// Tcp客户端
    /// </summary>
    class TCPClient
    {
        /// <summary>
        /// 网络客户端
        /// </summary>
        private NetClient _client;

        /// <summary>
        /// 客户端网络连接
        /// </summary>
        private IClientNetConnection _connection;

        /// <summary>
        /// 客户端回调
        /// </summary>
        private IClientNetService _callback;
        
        

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">服务端IP</param>
        /// <param name="port">服务端端口号</param>
        /// <param name="connectedMethod">连接事件处理方法，可选</param>
        public TCPClient(string ip, int port, IClientNetService callback)
        {
            _client = new NetClient();
            _callback = callback;
            CreateClient(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
        }

        /// <summary>
        /// 重新连接服务端
        /// </summary>
        public void ReConnect()
        {
            if (_connection != null)
                _connection.BeginReconnect();
        }

        /// <summary>
        /// 启动客户端
        /// </summary>
        public void Start()
        {
            if (_client != null)
            {
                _client.Start();
            }
        }

        /// <summary>
        /// 停止客户端
        /// </summary>
        public void Stop()
        {
            if (_client != null)
            {
                _client.Stop();
            }
        }

        /// <summary>
        /// 设置链路
        /// </summary>
        /// <param name="conn"></param>
        public void SetConnection(IClientNetConnection conn)
        {
            _connection = conn;
        }

        /// <summary>
        /// 创建指定IP指定端口的链接
        /// </summary>
        /// <param name="_IPEndPoint"></param>
        private void CreateClient(System.Net.IPEndPoint _IPEndPoint)
        {
            INetConnector connector = null;
            connector = NetConnectionCreatorFactory.CreateTcpConnector(_IPEndPoint, 1024 * 1024, "");
            _client.AddConnector(connector, _callback);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息内容，字符串为UTF8编码</param>
        public void Send(byte[] msg)
        {
            if (msg != null && _connection != null)
            {
                _connection.BeginSend(msg);
            }
        }

        #region IDisposable 成员

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_connection != null)
                    _connection.BeginDisconnect();

                if (_client != null)
                    _client.Dispose();
            }
            catch { }
        }
        #endregion
    }
}