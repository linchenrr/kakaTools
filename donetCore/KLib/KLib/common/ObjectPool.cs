using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLib
{
    public class ObjectPool<T>
    {

        private HashSet<T> dic_obj;

        private Func<T> createFunc;
        private Action<T> resetFunc;

        private int maxSize;

        /// <summary>
        /// 对象池  简单实现
        /// </summary>
        /// <param name="createFunc">创建/获取新对象的函数</param>
        /// <param name="resetFunc">对象被回收时的处理函数</param>
        /// <param name="maxSize">最大缓存实例数，0为不限制</param>
        public ObjectPool(Func<T> createFunc, Action<T> resetFunc, int maxSize = 0)
        {
            if (createFunc == null) throw new ArgumentNullException("createFunc");
            if (resetFunc == null) throw new ArgumentNullException("resetFunc");

            dic_obj = new HashSet<T>();
            this.createFunc = createFunc;
            this.maxSize = maxSize;
        }

        /// <summary>
        /// 预创建指定数量的实例
        /// </summary>
        /// <param name="num">需要创建的数量，此参数不受maxSize的限制</param>
        public void PreCreate(int num)
        {
            for (int i = 0; i < num; i++)
            {
                dic_obj.Add(createFunc());
            }
        }

        public T GetObj()
        {
            foreach (var obj in dic_obj)
            {
                dic_obj.Remove(obj);
                return obj;
            }
            return createFunc();
        }

        public void PutObj(T obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            if (dic_obj.Contains(obj))
                throw new Exception("重复放入了同一个对象:" + obj);

            if (dic_obj.Count >= maxSize)
                return;

            resetFunc(obj);
            dic_obj.Add(obj);
        }

    }

    public class Singleton<T> where T : new()
    {

        static public readonly T Instance = new T();

    }
}