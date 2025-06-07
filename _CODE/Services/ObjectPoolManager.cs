using System.Collections.Generic;
using UnityEngine;

namespace Services.ObjectPool
{
    public static class ObjectPoolManager
    {
        private static Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private static Dictionary<string, Dictionary<GameObject, Vector3>> initialPositions = new Dictionary<string, Dictionary<GameObject, Vector3>>();

        public static GameObject InstantiatePool(string tag, GameObject prefab, int initialSize, Vector3 createPosition)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                initialPositions[tag] = new Dictionary<GameObject, Vector3>();

                for (int i = 0; i < initialSize; i++)
                {
                    GameObject obj = GameObject.Instantiate(prefab);
                    obj.transform.parent = null;
                    obj.transform.position = createPosition;
                    obj.SetActive(true);

                    objectPool.Enqueue(obj);
                    initialPositions[tag][obj] = createPosition;
                }

                poolDictionary[tag] = objectPool;
                return objectPool.Dequeue();
            }
            return null;
        }


        public static void CreatePool(string tag, GameObject prefab, int initialSize)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < initialSize; i++)
                {
                    GameObject obj = prefab;
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary[tag] = objectPool;
            }
        }

        public static void CreatePool(string tag, Dictionary<int, GameObject> prefab, int initialSize)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < initialSize; i++)
                {
                    Dictionary<int, GameObject> obj = prefab;
                    obj[i].SetActive(false);
                    objectPool.Enqueue(obj[i]);
                }

                poolDictionary[tag] = objectPool;
            }
        }

        public static void CreatePool(string tag, List<GameObject> prefabs, int initialSize)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                initialPositions[tag] = new Dictionary<GameObject, Vector3>();

                for (int i = 0; i < initialSize; i++)
                {
                    GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
                    GameObject obj = GameObject.Instantiate(prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                    initialPositions[tag][obj] = Vector3.zero;
                }

                poolDictionary[tag] = objectPool;
            }
        }


        public static List<GameObject> GetObjectsFromPool(string tag)
        {
            if (poolDictionary.ContainsKey(tag) && poolDictionary[tag].Count > 0)
            {
                int initialPoolSize = poolDictionary[tag].Count;
                List<GameObject> obj = new List<GameObject>();

                for (int i = 0; i < initialPoolSize; i++)
                    obj.Add(poolDictionary[tag].Dequeue());

                for (int i = 0; i < obj.Count; i++)
                    obj[i].SetActive(true);

                return obj;
            }
            else
            {
                Debug.LogWarning("Пул с тегом <color=yellow>" + tag + "</color> пуст или не существует");
                return null;
            }
        }

        public static GameObject GetObjectFromPool(string tag)
        {
            if (poolDictionary.ContainsKey(tag) && poolDictionary[tag].Count > 0)
            {
                GameObject obj = poolDictionary[tag].Dequeue();
                return obj;
            }
            else
            {
                Debug.LogWarning("Пул с тегом <color=yellow>" + tag + "</color> пуст или не существует");
                return null;
            }
        }

        public static GameObject GetNextObjectFromPool(string tag)
        {
            if (poolDictionary.ContainsKey(tag) && poolDictionary[tag].Count > 0)
            {
                int initialPoolSize = poolDictionary[tag].Count;

                for (int i = 0; i < initialPoolSize; i++)
                {
                    GameObject obj = poolDictionary[tag].Dequeue();

                    if (!obj.activeSelf)
                    {
                        poolDictionary[tag].Enqueue(obj);
                        return obj;
                    }
                    else
                        poolDictionary[tag].Enqueue(obj);
                }

                Debug.LogWarning("Все объекты в пуле с тегом <color=yellow>" + tag + "</color> активны.");
                return null;
            }
            else
            {
                Debug.LogWarning("Пул с тегом <color=yellow>" + tag + "</color> пуст или не существует.");
                return null;
            }
        }


        public static void ReturnObjectToPool(string tag, GameObject obj)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                if (initialPositions.ContainsKey(tag) && initialPositions[tag].ContainsKey(obj))
                    obj.transform.position = initialPositions[tag][obj];

                obj.SetActive(false);
                poolDictionary[tag].Enqueue(obj);
            }
            else
            {
                Debug.LogWarning("Пул с тегом <color=yellow>" + tag + "</color> не найден");
            }
        }


        public static int GetPoolCount(string tag)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                return poolDictionary[tag].Count;
            }
            else
            {
                Debug.LogWarning("Пул с тегом <color=yellow>" + tag + "</color> не найден");
                return 0;
            }
        }

        public static void DestroyPool()
        {
            foreach (var pool in poolDictionary)
            {
                Queue<GameObject> objectPool = pool.Value;
               
                while (objectPool.Count > 0)
                {
                    GameObject obj = objectPool.Dequeue();
                    if (obj != null)
                    {
                        GameObject.Destroy(obj); 
                    }
                }
            }

            poolDictionary.Clear();
        }

        public static void ClearPool() =>
            poolDictionary.Clear();
    }
}