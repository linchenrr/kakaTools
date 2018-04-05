using protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public class RPCServer
    {

        private TestServer server;
        private PackageTranslator translator;

        public RPCServer(TestServer server, PackageTranslator translator)
        {
            this.server = server;
            this.translator = translator;
            server.OnData += OnData;
        }

        public void StartListen(int port)
        {
            server.startListen(port);

        }

        public void Send(BaseProtocolVO vo, object client)
        {
            Console.WriteLine("发送消息:" + vo + "   给" + client);
            var bytes = translator.Encode(vo);
            server.send(bytes, ((ClientObject)client).socket);
        }

        public void Broadcast(BaseProtocolVO vo)
        {
            Console.WriteLine("广播消息:" + vo);
            var bytes = translator.Encode(vo);
            foreach (var clientObject in server.dic_client)
            {
                server.send(bytes, clientObject.Value.socket);
            }
        }

        private void OnData(byte[] bytes, ClientObject sender)
        {

            var vo = translator.Decode(bytes);
            vo.customData = sender;

            ProtocolCenter.DispatchMessage(vo);

        }

    }
}
