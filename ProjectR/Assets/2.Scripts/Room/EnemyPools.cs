using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPools : MonoBehaviour
{
    [SerializeField] private Pool[] pools;
    public static readonly Dictionary<string, ObjectPool<GameObject>> poolDict
        = new Dictionary<string, ObjectPool<GameObject>>();

    private void Awake()
    {
        foreach (var pool in pools)
        {
            var temp = new ObjectPool<GameObject>(
                pool.CreatePoolItem,
                Pool.TakePoolItem,
                Pool.ReleasePoolItem,
                Pool.DestroyPoolItem,
                true, 10);

            poolDict.Add(pool.name, temp);

            for (int i = 0; i < pool.initCount; i++)
            {
                var obj = pool.CreatePoolItem();
                temp.Release(obj);
            }
        }
    }

    /// <summary>
    /// 에너미 풀에서 오브젝트 불러오기 <br/>
    /// 불러온 오브젝트 반드시 초기화
    /// </summary>
    /// <param name="name"> 이름 </param>
    /// <param name="position"> 위치 </param>
    /// <param name="rotation"> 회전 </param>
    /// <returns> 불러온 오브젝트 </returns>
    public static GameObject AppearObject(string name, Vector3 position = default, Quaternion rotation = default)
    {
        print(name);
        GameObject obj = poolDict[name].Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 에너미 풀에 오브젝트 반환
    /// </summary>
    /// <param name="name"> 이름 </param>
    /// <param name="obj"> 에너미 오브젝트</param>
    public static void ReleaseObject(string name, GameObject obj)
    {
        poolDict[name].Release(obj);
    }

    [System.Serializable]
    private class Pool
    {
        public string name;
        [Space]
        public GameObject enemyObject;
        public int initCount = 10;

        #region Pool Methods
        public GameObject CreatePoolItem()
        {
            GameObject obj = Instantiate(enemyObject);
            obj.name = name;
            return obj;
        }

        public static void TakePoolItem(GameObject obj)
        {
            //obj.SetActive(true);
        }

        public static void ReleasePoolItem(GameObject obj)
        {
            obj.SetActive(false);
        }

        public static void DestroyPoolItem(GameObject obj)
        {
            Destroy(obj);
        }
        #endregion
    }
}
