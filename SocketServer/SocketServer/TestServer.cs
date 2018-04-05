using KLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    public class TestServer
    {

        //数据事件委托
        public delegate void DataReceivedHandler(byte[] bytes, ClientObject sender);
        //数据事件委托类型的事件
        public event DataReceivedHandler OnData;

        private Socket mainSocket = null;
        //监听进程
        private Thread thread_listen;
        //已连接客户端列表
        public Dictionary<Socket, ClientObject> dic_client = new Dictionary<Socket, ClientObject>();
        //是否已经在侦听状态
        private Boolean isListening = false;

        public const int packageHeadLen = 4;

        private void receive(Socket socket)
        {

            Console.WriteLine("receive " + Thread.CurrentThread.ManagedThreadId);

            if (!checkSocketConnect(socket))
                return;

            var client = dic_client[socket];

            try
            {
                client.context_head.SetBuffer(new byte[packageHeadLen], 0, packageHeadLen);
                socket.ReceiveAsync(client.context_head);

                /*
                var bodyLen = ReadVarintUIntFromSocket(client.socket);
                Console.WriteLine("bodyLen " + bodyLen);
                if (bodyLen > 0)
                {
                    if (!checkSocketConnect(client.socket))
                        return;
                    
                    client.context_body.SetBuffer(new byte[bodyLen], 0, (int)bodyLen);
                    client.socket.ReceiveAsync(client.context_body);
                }
                else
                {
                    receive(socket);
                }
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                removeSocket(socket);
            }

        }

        public uint ReadVarintUIntFromSocket(Socket socket)
        {
            var headBuffer = new byte[1];
            int shift = 0;
            uint result = 0;
            while (shift < 32)
            {
                socket.Receive(headBuffer, 1, SocketFlags.None);
                byte b = headBuffer[0];
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new Exception("ReadVarintUInt 长度超出预期");
        }

        private void OnHeadReceived(object sender, SocketAsyncEventArgs context_head)
        {
            var client = (ClientObject)context_head.UserToken;
            int len = BitConverter.ToInt32(context_head.Buffer, 0);
            len = NetUtils.ConvertToEndian(len, Endian.BigEndian);
            if (len > 0)
            {
                if (!checkSocketConnect(client.socket))
                    return;
                client.context_body.SetBuffer(new byte[len], 0, len);
                client.socket.ReceiveAsync(client.context_body);
            }
            else
            {
                removeSocket((Socket)sender);
                //receive((Socket)sender);
            }
        }

        private void OnBodyReceived(object sender, SocketAsyncEventArgs context_body)
        {
            var bytesReceived = context_body.Buffer;
            if (OnData != null)
                OnData(bytesReceived, ((ClientObject)context_body.UserToken));

            receive((Socket)sender);
        }

        public void send(byte[] bytes, object client)
        {
            //写入4字节的包头长度
            var package = new byte[bytes.Length + packageHeadLen];
            Buffer.BlockCopy(BitConverter.GetBytes(NetUtils.ConvertToEndian(bytes.Length, Endian.BigEndian)), 0, package, 0, packageHeadLen);
            Buffer.BlockCopy(bytes, 0, package, packageHeadLen, bytes.Length);
            ((Socket)client).Send(package);

            //改为支持变长的包头长度
            //var stream = new ProtocolBinaryWriter(new MemoryStream());
            //stream.Write((uint)bytes.Length);
            //stream.BaseStream.Position = 0;
            //var headLen = (int)stream.BaseStream.Length;
            //var reader = new BinaryReader(stream.BaseStream);
            //var package = new byte[bytes.Length + headLen];
            //Buffer.BlockCopy(reader.ReadBytes(headLen), 0, package, 0, headLen);
            //Buffer.BlockCopy(bytes, 0, package, headLen, bytes.Length);
            //((Socket)client).Send(package);

            Console.WriteLine("len:" + (uint)bytes.Length);
            Console.WriteLine(package[0] + " " + package[1] + " " + package[2]);
        }

        private bool checkSocketConnect(Socket socket)
        {
            if (!socket.Connected)
            {
                removeSocket(socket);
                return false;
            }
            return true;
        }

        private void removeSocket(Socket socket)
        {
            Console.WriteLine(String.Format("客户端{0}已断开", socket.RemoteEndPoint));
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            dic_client.Remove(socket);
        }

        private void listen()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                var clientSocket = mainSocket.Accept();
                var client = new ClientObject();
                client.context_head.Completed += OnHeadReceived;
                client.context_body.Completed += OnBodyReceived;
                client.socket = clientSocket;
                dic_client.Add(clientSocket, client);
                Console.WriteLine(String.Format("客户端{0}已连接", clientSocket.RemoteEndPoint));

                receive(clientSocket);
            }
        }

        public void startListen(int port)
        {
            if (isListening)
            {
                return;
            }

            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var IP = GetLocalIP();
            Console.WriteLine(IP);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
            mainSocket.Bind(serverEndPoint);
            mainSocket.Listen(100);
            thread_listen = new Thread(new ThreadStart(listen));
            thread_listen.Start();
            isListening = true;
        }

        public void stopListen()
        {
            if (!isListening)
            {
                return;
            }

            foreach (var client in dic_client.Keys)
            {
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch
                {
                }

            }

            try
            {
                thread_listen.Abort();
                mainSocket.Close();
            }
            catch
            {
            }

            isListening = false;
        }

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception("获取本机IP出错:" + ex.Message);
            }
        }

    }

    public class ClientObject
    {
        public int Id;
        public Socket socket;
        public SocketAsyncEventArgs context_head = new SocketAsyncEventArgs();
        public SocketAsyncEventArgs context_body = new SocketAsyncEventArgs();
        public ClientObject()
        {
            Id = GetNextId();
            context_head.UserToken = this;
            context_body.UserToken = this;
        }

        override public string ToString()
        {
            return "client" + Id;
        }

        static private int nextId;

        static public int GetNextId()
        {
            return ++nextId;
        }

    }

}
