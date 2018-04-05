using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using protocol;
using System.Net.Sockets;

namespace SocketServer
{
    class Program
    {
        static RPCServer server;
        static void Main(string[] args)
        {

            server = new RPCServer(new TestServer(), new PackageTranslator());
            ProtocolCenter.RegisterGlobalMessageHandler(globalHandler);
            ProtocolCenter.RegisterMessageHandler(MessageType.RequestWorldTalk, doRequestWorldTalk);
            ProtocolCenter.RegisterMessageHandler(MessageType.RequestPlayers, doRequestPlayers);
            RequestWorldTalk.RegisterHandler(onRequestWorldTalk);
            RequestPlayers.RegisterHandler(onRequestPlayers);
            server.StartListen(7666);

            while (true)
            {
                Console.ReadLine();
            }

        }

        static void globalHandler(BaseProtocolVO baseVO)
        {
            Console.WriteLine(String.Format("从[{0}]收到协议:{1}", baseVO.customData, baseVO.MessageType));

            //发回所有收到的消息
            //server.Send(baseVO, client);
        }

        static void onRequestWorldTalk(RequestWorldTalk vo)
        {
            //Console.WriteLine("onRequestWorldTalk " + vo.content);
            var response = ResponseWorldTalk.CreateInstance();
            response.content = vo.content;
            response.talkerName = vo.customData.ToString();
            server.Broadcast(response);
        }

        static void doRequestWorldTalk(BaseProtocolVO baseVO)
        {
            //var vo = (RequestWorldTalk)baseVO;
            //var response = ResponseWorldTalk.CreateInstance();
            //response.content = vo.content;
            //server.Send(response, baseVO.customData);
        }

        static void onRequestPlayers(RequestPlayers vo)
        {
            Console.WriteLine("onRequestPlayers " + vo.customData);
        }

        static void doRequestPlayers(BaseProtocolVO baseVO)
        {
            var vo = (RequestPlayers)baseVO;

            var players = new PlayerInfo[3];
            for (int i = 1; i <= 3; i++)
            {
                players[i - 1] = new PlayerInfo()
                {
                    uid = i + 20000,
                    name = "qqq" + i,
                    status = i % 2 == 0,
                    type = (PlayerType)i,
                    maxResetTimes = i,
                    fff = i * 100 + i * 0.11111f,
                    createTime = DateTime.Now,
                    items = new[] { 111, 222, 777 },
                };
            }
            var response = new ResponsePlayers()
            {
                status = true,
                players = players,
            };
            server.Send(response, baseVO.customData);
        }

    }
}
