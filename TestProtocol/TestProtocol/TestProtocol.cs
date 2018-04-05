using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib;
using Newtonsoft.Json;
using protocol;

namespace TestProtocol
{
    public class TestProtocol
    {
        SocketConnection connection;
        PackageTranslator translator;
        RPCClient client;
        public void start()
        {

            translator = new PackageTranslator();
            //connection = new SocketConnection("127.0.0.1", 7666);
            connection = new SocketConnection("172.16.50.181", 7666);
            client = new RPCClient(connection, translator);
            ProtocolCenter.RegisterMessageHandler(MessageType.ResponseWorldTalk, ResponseWorldTalk);
            //client.RegisterMessageHandler(MessageType.ResponsePlayers, ResponsePlayers);
            ResponsePlayers.RegisterHandler(onResponsePlayers);
            ProtocolCenter.RegisterGlobalMessageHandler(globalMessageHandler);

            client.OnConnectSuccess += OnConnectSuccess;
            client.OnConnectFail += OnConnectFail;
            client.OnConnectClose += OnConnectClose;
            client.Connect();

            /*
            connection.OnConnectSuccess += OnConnectSuccess;
            connection.OnData += OnData;
            connection.OnConnectClose += OnConnectClose;
            connection.Connect();
            */

            while (true)
            {
                var input = Console.ReadLine();
                if (input.IndexOf("2") >= 0)
                {
                    var msg = new RequestPlayers();
                    client.Call(msg);
                }
                else
                {
                    var sendStr = new RequestWorldTalk();
                    sendStr.content = input;
                    client.Call(sendStr);
                }

            }


        }

        void OnConnectSuccess()
        {
            Console.WriteLine("连接成功");
        }

        void OnConnectFail(string msg)
        {
            Console.WriteLine("连接失败");
            Console.WriteLine(msg);
        }

        void OnConnectClose(string msg)
        {
            Console.WriteLine("连接已关闭");
            Console.WriteLine(msg);
        }

        private void ResponseWorldTalk(BaseProtocolVO vo)
        {
            var msg = (ResponseWorldTalk)vo;
            Console.WriteLine(msg.talkerName + ": " + msg.content);
        }

        private void onResponsePlayers(ResponsePlayers vo)
        {
            Console.WriteLine("onResponsePlayers:" + vo.players.Length);
            foreach (var playerInfo in vo.players)
            {
                Console.WriteLine(JsonConvert.SerializeObject(playerInfo, Formatting.Indented));
            }
        }

        private void globalMessageHandler(BaseProtocolVO baseVO)
        {
            Console.WriteLine("收到消息:" + baseVO);
        }

        void OnData(byte[] bytes)
        {
            var vo = translator.Decode(bytes);
        }

    }
}
