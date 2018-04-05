using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KLib
{
    public class SocketConnection : BaseConnection
    {

        public const int packageHeadLen = 4;

        private Socket socket;
        private Thread thread_receive;
        private string host;
        private int port;

        public SocketConnection(string host, int port)
        {
            this.host = host;
            this.port = port;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        override public void Connect()
        {

            if (Connected)
            {
                throw new Exception("尝试对处于连接状态的socket进行连接操作");
            }

            var args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            socket.ConnectAsync(args);

        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {

            if (e.SocketError == SocketError.Success)
            {
                thread_receive = new Thread(receive);
                thread_receive.Start();

                ConnectSucess();
            }
            else
            {
                ConnectFailed(e.SocketError.ToString());
            }

        }


        public bool Connected { get { return socket.Connected; } }

        override protected void SendData(byte[] bytes)
        {

            //写入4字节的包头长度
            var package = new byte[bytes.Length + packageHeadLen];
            Buffer.BlockCopy(BitConverter.GetBytes(NetUtils.ConvertToEndian(bytes.Length, Endian.BigEndian)), 0, package, 0, packageHeadLen);
            Buffer.BlockCopy(bytes, 0, package, packageHeadLen, bytes.Length);

            //改为支持变长的包头长度
            //var stream = new ProtocolBinaryWriter(new MemoryStream());
            //stream.Write((uint)bytes.Length);
            //stream.BaseStream.Position = 0;
            //var headLen = (int)stream.BaseStream.Length;
            //var reader = new BinaryReader(stream.BaseStream);
            //var package = new byte[bytes.Length + headLen];
            //Buffer.BlockCopy(reader.ReadBytes(headLen), 0, package, 0, headLen);
            //Buffer.BlockCopy(bytes, 0, package, headLen, bytes.Length);

            //Logger.LogWarning("包头占用字节:" + headLen);

            var args = new SocketAsyncEventArgs();
            args.Completed += OnSendCompleted;
            args.SetBuffer(package, 0, package.Length);
            socket.SendAsync(args);

        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {

        }

        private byte[] headBytes = new byte[packageHeadLen];
        private void receive()
        {
            while (true)
            {

                byte[] receiveBytes;

                try
                {
                    if (!Connected)
                    {
                        return;
                    }

                    socket.Receive(headBytes, packageHeadLen, SocketFlags.None);
                    var len = BitConverter.ToInt32(headBytes, 0);
                    len = NetUtils.ConvertToEndian(len, Endian.BigEndian);

                    //var len = ReadVarintUIntFromSocket(socket);
                    // ??? 接收的包体不能大于10KB，否则长度错误  但是加上下面这句打log就又正常了..
                    //Logger.Log("len", len);

                    receiveBytes = new byte[len];

                    socket.Receive(receiveBytes, len, SocketFlags.None);


                }
                catch (Exception e)
                {
                    //throw e;
                    ConnectClose(e.ToString());
                    break;
                }

                DistributeData(receiveBytes);

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

        public void Close()
        {
            //此处不能Shutdown，否则服务器不知道断线
            //socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            thread_receive.Abort();
        }

    }
}
