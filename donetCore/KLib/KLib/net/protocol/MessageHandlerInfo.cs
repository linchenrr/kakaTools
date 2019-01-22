using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace protocol
{

    /// <summary>
    /// 弱引用回调函数封装模板类
    /// </summary>
    /// <typeparam name="T">回调函数的参数类型</typeparam>
    public class MessageHandlerInfo<T>
    {

        public int Code;
        public WeakReference Target;
        public MethodInfo Method;

        public MessageHandlerInfo(Action<T> handler)
        {
            this.Code = handler.GetHashCode();
            Target = new WeakReference(handler.Target);
            Method = handler.Method;
        }

        public bool Invoke(T msg)
        {
            if (Method.IsStatic == false)
            {
                if (Target.IsAlive == false)
                    return false;
                if (Target.Target.ToString() == "null")
                    return false;
            }

            try
            {
                Method.Invoke(Target.Target, new object[] { msg });
            }
            catch (Exception e)
            {
                //Logger.LogException(e);
                Console.WriteLine(e);
            }

            return true;
        }

        public bool IsEqual(Action<T> handler)
        {
            return Code == handler.GetHashCode();
        }

    }

    /// <summary>
    /// 弱引用回调函数组管理类(模板)
    /// </summary>
    /// <typeparam name="T">回调函数的参数类型</typeparam>
    public class MessageHandlerGroup<T>
    {

        private List<MessageHandlerInfo<T>> list_handler = new List<MessageHandlerInfo<T>>();

        public void RegisterHandler(Action<T> handler)
        {
            RemoveHandler(handler);
            list_handler.Add(new MessageHandlerInfo<T>(handler));
        }

        public void RemoveHandler(Action<T> handler)
        {
            for (int i = 0; i < list_handler.Count; i++)
            {
                if (list_handler[i].IsEqual(handler))
                {
                    list_handler.RemoveAt(i);
                    break;
                }
            }
        }

        public void CallHandler(T msg)
        {
            int i = 0;
            while (i < list_handler.Count)
            {
                var handlerInfo = list_handler[i];
                //自动清除被回收的HandlerInfo对象
                if (handlerInfo.Invoke(msg) == false)
                {
                    list_handler.RemoveAt(i);
                    continue;
                }
                i++;
            }

        }
    }
}
