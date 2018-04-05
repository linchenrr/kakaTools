using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KLib
{

    static public class EventDispatcherExtension
    {

        static private Dictionary<int, EventRecod> dic_record = new Dictionary<int, EventRecod>();
        static private int GCCurTime = 0;
        static private int GCTime = 10;

        static public void DispatchEvent(this object target, string type, object data = null)
        {
            target.DispatchEvent(new KEvent(type, data));
        }

        static public void DispatchEvent(this object target, KEvent evt)
        {
            var list = getHandlerList(target, evt.Type);
            if (null == list)
                return;

            evt.Sender = target;

            int i = 0;
            while (i < list.Count)
            {
                var handlerInfo = list[i];
                //自动清除被回收的HandlerInfo对象
                if (handlerInfo.Invoke(evt.clone()) == false)
                {
                    list.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        static public void AddEventListener(this object target, string type, ExtensionEventHandler handler)
        {
            RemoveEventListener(target, type, handler);
            var list = getHandlerList(target, type, true);
            list.Add(new HandlerInfo(handler));
        }

        static public void RemoveEventListener(this object target, string type, ExtensionEventHandler handler)
        {
            var list = getHandlerList(target, type);
            if (null == list)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsEqual(handler))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        static public void RemoveAllEventListener(this object target)
        {
            var code = target.GetHashCode();
            dic_record.Remove(code);
        }

        static private List<HandlerInfo> getHandlerList(object target, string type, bool createIfNull = false)
        {
            List<HandlerInfo> list = null;

            var code = target.GetHashCode();
            EventRecod record;
            if (dic_record.TryGetValue(code, out record) == false)
            {
                if (createIfNull)
                {
                    if (++GCCurTime >= GCTime)
                    {
                        GC();
                        GCCurTime = 0;
                    }

                    record = new EventRecod();
                    record.Code = code;
                    dic_record[code] = record;
                }
            }
            if (record != null)
                list = record.GetHandlerList(type, createIfNull);
            return list;
        }

        static private void GC()
        {
            var records = dic_record.Values.ToArray();
            foreach (var record in records)
            {
                if (record.IsInvalid)
                {
                    dic_record.Remove(record.Code);
                }
            }
        }

        private class EventRecod
        {

            public int Code;
            public WeakReference DispatcherRef;
            public Dictionary<string, List<HandlerInfo>> HandlerDictionary = new Dictionary<string, List<HandlerInfo>>();

            public List<HandlerInfo> GetHandlerList(string eventType, bool createIfNull)
            {
                List<HandlerInfo> list = null;
                if (HandlerDictionary.TryGetValue(eventType, out list) == false)
                {
                    if (createIfNull)
                    {
                        list = HandlerDictionary[eventType] = new List<HandlerInfo>();
                    }
                }

                return list;
            }

            public bool IsInvalid
            {
                get
                {
                    if (DispatcherRef.IsAlive == false)
                        return true;
                    if (DispatcherRef.Target.ToString() == "null")
                        return true;
                    return false;
                }
            }

        }

        private class HandlerInfo
        {

            public int Code;
            public WeakReference TargetRef;
            public MethodInfo Method;

            public HandlerInfo(ExtensionEventHandler handler)
            {
                this.Code = handler.GetHashCode();
                TargetRef = new WeakReference(handler.Target);
                Method = handler.Method;
            }

            public bool Invoke(KEvent evt)
            {
                if (Method.IsStatic == false)
                {
                    if (TargetRef.IsAlive == false)
                        return false;
                    if (TargetRef.Target.ToString() == "null")
                        return false;
                }

                try
                {
                    Method.Invoke(TargetRef.Target, new[] { evt });
                }
                catch (Exception e)
                {
                    //Logger.LogException(e);
                }

                return true;
            }

            public bool IsEqual(ExtensionEventHandler handler)
            {
                return Code == handler.GetHashCode();
            }

        }
    }

    public class GlobalEventDispatcher
    {
        static private readonly object eventDispatcher = new object();

        static public void DispatchEvent(string type, object data = null)
        {
            eventDispatcher.DispatchEvent(type, data);
        }

        static public void DispatchEvent(KEvent evt)
        {
            eventDispatcher.DispatchEvent(evt);
        }
        static public void AddEventListener(string type, ExtensionEventHandler handler)
        {
            eventDispatcher.AddEventListener(type, handler);
        }
        static public void RemoveEventListener(string type, ExtensionEventHandler handler)
        {
            eventDispatcher.RemoveEventListener(type, handler);
        }

        //获取事件侦听、发送的全局实例
        static public object Dispatcher
        {
            get { return eventDispatcher; }
        }
    }
}
