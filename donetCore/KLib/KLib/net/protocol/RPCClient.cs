using KLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace protocol
{

    public class RPCClient
    {

        protected BaseConnection connection;
        protected PackageTranslator packager;

        public event Action OnConnectSuccess;
        public event ConnectionEventHandler OnConnectFail;
        public event ConnectionEventHandler OnConnectClose;

        public event Action<BaseProtocolVO> OnReceiveMessage;

        public RPCClient(BaseConnection connection, PackageTranslator packager)
        {
            this.connection = connection;
            this.packager = packager;
        }

        public void Connect()
        {
            connection.OnData += connectDataHandler;
            connection.OnConnectSuccess += connectionSuccessHandler;
            connection.OnConnectFail += connectionFailHandler;
            connection.OnConnectClose += connectionCloseHandler;

            this.connection.Connect();

        }

        public void Call(BaseProtocolVO vo)
        {

            Console.WriteLine("发送消息:" + vo);
            var bytes = packager.Encode(vo);
            send(bytes);

        }

        protected void send(byte[] data)
        {
            connection.Send(data);
        }

        protected void connectDataHandler(byte[] bytes)
        {
            var vo = packager.Decode(bytes);
            ProtocolCenter.DispatchMessage(vo);
            if (OnReceiveMessage != null)
                OnReceiveMessage(vo);
        }

        private void connectionSuccessHandler()
        {
            if (OnConnectSuccess != null)
                OnConnectSuccess();
        }
        private void connectionFailHandler(string msg)
        {
            if (OnConnectFail != null)
                OnConnectFail(msg);
        }

        private void connectionCloseHandler(string msg)
        {
            if (OnConnectClose != null)
                OnConnectClose(msg);
        }

    }
}
