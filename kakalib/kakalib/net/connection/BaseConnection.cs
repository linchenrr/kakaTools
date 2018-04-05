using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public delegate void ConnectionEventHandler(string msg);
    public abstract class BaseConnection
    {

        public event Action<byte[]> OnData;
        public event Action OnConnectSuccess;
        public event ConnectionEventHandler OnConnectFail;
        public event ConnectionEventHandler OnConnectClose;

        //待发送的数据包队列
        protected Queue<byte[]> queue_send = new Queue<byte[]>();
        //已收到未处理的数据包队列
        protected Queue<byte[]> queue_receive = new Queue<byte[]>();

        private object lockSendObj = new object();
        private object lockReadObj = new object();

        //是否手动检测触发数据分发
        public bool ManualDistributeData;

        public void Send(byte[] bytes, bool immediately = true)
        {
            if (immediately)
            {
                SendData(bytes);
            }
            else
            {
                lock (lockSendObj)
                {
                    queue_send.Enqueue(bytes);
                }
            }
        }

        //将队列中累积的数据全部发送
        public void Flush()
        {
            lock (lockSendObj)
            {
                while (queue_send.Count > 0)
                {
                    SendData(queue_send.Dequeue());
                }
            }
        }

        public virtual void Connect()
        {

        }

        protected virtual void SendData(byte[] bytes)
        {
            //子类覆写
        }

        protected void DistributeData(byte[] bytes)
        {
            lock (lockReadObj)
            {
                queue_receive.Enqueue(bytes);

                if (ManualDistributeData == false)
                {
                    while (queue_receive.Count > 0)
                    {
                        OnData(queue_receive.Dequeue());
                    }
                }
            }
        }

        public void ProcessReceivedData()
        {
            lock (lockReadObj)
            {
                while (queue_receive.Count > 0)
                {
                    OnData(queue_receive.Dequeue());
                }
            }
        }

        protected virtual void ConnectSucess()
        {
            if (OnConnectSuccess != null)
                OnConnectSuccess();
        }

        protected virtual void ConnectFailed(string msg)
        {
            if (OnConnectFail != null)
                OnConnectFail(msg);
        }

        protected virtual void ConnectClose(string msg)
        {
            if (OnConnectClose != null)
                OnConnectClose(msg);
        }

    }
}
