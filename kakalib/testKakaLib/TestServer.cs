using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testKakaLib
{
    public partial class TestServer : Form
    {
        Socket server;
        public TestServer()
        {
            InitializeComponent();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {

            IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9696);

            server = new Socket(serverPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(serverPoint);

            server.Listen(500);

            accept_async();

        }

        private void accept_async()
        {

            var accept = new SocketAsyncEventArgs();

            accept.Completed += accept_Completed;

            server.AcceptAsync(accept);

        }

        void accept_Completed(object sender, SocketAsyncEventArgs e)
        {

            accept_async();

            var client = e.AcceptSocket;

            e.Completed -= accept_Completed;

            e.Completed += receive_Completed;

            var buffer = new byte[1024];

            e.SetBuffer(buffer, 0, buffer.Length);

            client.ReceiveAsync(e);

        }

        void receive_Completed(object sender, SocketAsyncEventArgs e)
        {

            var client = sender as Socket;

            if (e.BytesTransferred == 0)
            {

                txt_content.Text += "socket is closed";

                client.Close();

            }

            else
            {

                txt_content.Text += Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);

                client.Send(e.Buffer, e.BytesTransferred, SocketFlags.None);

                client.ReceiveAsync(e);

            }

        }
    }
}
