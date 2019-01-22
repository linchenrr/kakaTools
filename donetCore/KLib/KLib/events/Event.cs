using System;
using System.Collections.Generic;
using System.Text;

namespace KLib
{
    public delegate void ExtensionEventHandler(KEvent evt);
    public class KEvent
    {

        private string type;
        public string Type
        {
            get { return type; }
        }

        private Object data;
        public Object Data
        {
            get { return data; }
        }

        private Object sender;
        public Object Sender
        {
            set { sender = value; }
            get { return sender; }
        }

        public KEvent(string type, Object data = null)
        {
            this.type = type;
            this.data = data;
        }

        public virtual KEvent clone()
        {
            var evt = new KEvent(type, data);
            evt.sender = sender;
            return evt;
        }

    }
}