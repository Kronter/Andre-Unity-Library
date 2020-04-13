using System.Collections.Generic;
using UnityEngine;

namespace Andre.Pooling
{
    public abstract class Pool<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private T prefab;

        private Queue<T> objects = new Queue<T>();

        public T Get()
        {
            if (objects.Count == 0)
                AddObjects(1);

            return objects.Dequeue();
        }

        public void ReturnToPool(T objectToRetrun)
        {
            objectToRetrun.gameObject.SetActive(false);
            objects.Enqueue(objectToRetrun);
        }

        public void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newObject = GameObject.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);
            }
        }
    }

    public abstract class PoolSingelton<T> : Utils.SingletonBase<PoolSingelton<T>> where T : Component
    {
        [SerializeField]
        private T prefab;

        private Queue<T> objects = new Queue<T>();

        public T Get()
        {
            if (objects.Count == 0)
                AddObjects(1);

            return objects.Dequeue();
        }

        public void ReturnToPool(T objectToRetrun)
        {
            objectToRetrun.gameObject.SetActive(false);
            objects.Enqueue(objectToRetrun);
        }

        public void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newObject = GameObject.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);
            }
        }
    }

    public abstract class PoolSingeltonPersistant<T> : Utils.SingletonBasePersistent<PoolSingeltonPersistant<T>> where T : Component
    {
        [SerializeField]
        private T prefab;

        private Queue<T> objects = new Queue<T>();

        public T Get()
        {
            if (objects.Count == 0)
                AddObjects(1);

            return objects.Dequeue();
        }

        public void ReturnToPool(T objectToRetrun)
        {
            objectToRetrun.gameObject.SetActive(false);
            objects.Enqueue(objectToRetrun);
        }

        public void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newObject = GameObject.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);
            }
        }
    }
}
