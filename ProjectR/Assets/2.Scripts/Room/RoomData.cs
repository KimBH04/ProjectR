using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class RoomData : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WaveContainer[] waves;

    [SerializeField] private Room data;

    [SerializeField] private GameObject mapDisplay;
    [SerializeField] private MeshRenderer displayMesh;

    [SerializeField] private GameObject[] fruits;
    [SerializeField] private GameObject boss;

    private bool visited = false;
    private int enemyCount = 0;

    private static Room previousRoom;
    
    public int EnemyCount => enemyCount;

    private List<Transform> _availableSpawnPoints;

    private void Awake()
    {
        _availableSpawnPoints = new List<Transform>(spawnPoints);
    }

    public Room Data
    {
        get
        {
            return data;
        }
        set
        {
            if (data != null)
            {
                data.OnRoomLeave.Invoke();
                data.OnRoomLeave.RemoveAllListeners();
            }
            data = value;
            data.MapDisplay = mapDisplay;
            data.DisplayMesh = displayMesh;
            if (data.type == Room.RoomType.Battle && waves.Length > 0)
            {
                AudioManager.Instance.PlayBgm(AudioManager.EBgm.Stage1);
                WaveContainer waveContainer = waves[Random.Range(0, waves.Length)];
                enemyCount = waveContainer.Count;
                for (int i = 0; i < enemyCount; i++)
                {
                    int index = i;  // handling variable capture
                    data.movedHere.AddListener(() =>
                    {
                        if (TryGetRandomPoint(out Vector3 pos))
                        {
                            string monsterName = GetRandomMonster(waveContainer);
                            var enemy = EnemyPools.AppearObject(monsterName, pos).GetComponent<Enemy>();
                            enemy.onDieEvent.RemoveAllListeners();
                            enemy.onDieEvent.AddListener(EnemyCounter);

                            if (enemy is Slime biglime)
                            {
                                enemyCount += 2;

                                biglime.mini[0].onDieEvent.RemoveAllListeners();
                                biglime.mini[0].onDieEvent.AddListener(EnemyCounter);

                                biglime.mini[1].onDieEvent.RemoveAllListeners();
                                biglime.mini[1].onDieEvent.AddListener(EnemyCounter);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("No have spawnPoint");
                            enemyCount--;
                        }
                    });
                }
            }
            else if(data.type == Room.RoomType.Shop)
            {
                data.movedHere.AddListener(() =>
                {
                    AudioManager.Instance.PlayBgm(AudioManager.EBgm.Shop);
                });
                
                data.OnRoomLeave.AddListener(() =>
                {
                    AudioManager.Instance.PlayBgm(AudioManager.EBgm.Stage1);
                });
                
                
            }
            else if (data.type == Room.RoomType.Boss)
            {
               
                data.movedHere.AddListener(() =>
                {
                    AudioManager.Instance.PlayBgm(AudioManager.EBgm.Stage1Boss);
                    Instantiate(boss, transform.position, Quaternion.identity)
                        .GetComponent<Enemy>()
                        .onDieEvent
                        .AddListener(EnemyCounter);
                });
            }
        }
    }
    
    private string GetRandomMonster(WaveContainer waveContainer)
    {
        float totalProbability = 0f;
        foreach (var enemy in waveContainer.Enemies)
        {
            totalProbability += enemy.spawnProbability;
        }

        float randomPoint = Random.value * totalProbability;

        foreach (var enemy in waveContainer.Enemies)
        {
            if (randomPoint < enemy.spawnProbability)
            {
                return enemy.name;
            }
            else
            {
                randomPoint -= enemy.spawnProbability;
            }
        }

        return waveContainer.Enemies[waveContainer.Count - 1].name;
    }

    private bool TryGetRandomPoint(out Vector3 v)
    {
        int cnt = _availableSpawnPoints.Count;
        if (cnt == 0)
        {
            v = Vector3.zero;
            return false;
        }

        int randIdx = Random.Range(0, cnt);
        v = _availableSpawnPoints[randIdx].position;
        _availableSpawnPoints.RemoveAt(randIdx);
        return true;
    }

    public void MovedHere()
    {
        if (visited)
        {
            return;
        }
        
        if (previousRoom != null && previousRoom.type == Room.RoomType.Shop)
        {
            previousRoom.OnRoomLeave.Invoke();
        }

        data.movedHere.Invoke();
        visited = true;

        for (int i = 0; i < 2; i++)
        {
            if (TryGetRandomPoint(out Vector3 pos))
            {
                int randIdx = Random.Range(0, fruits.Length);
                Instantiate(fruits[randIdx], pos, Quaternion.identity);
            }
        }

        if (data.type == Room.RoomType.Battle || data.type == Room.RoomType.Boss)
        {
            CreateRoom.CloseWalls(
                transform.position,
                data[Room.LEFT] != null,
                data[Room.FRONT] != null,
                data[Room.RIGHT] != null,
                data[Room.BACK] != null);
        }

        

        previousRoom = data;
    }
    
    private void EnemyCounter()
    {
        enemyCount--;
        if (enemyCount == 0)
        {
            CreateRoom.OpenWalls(
                data[Room.LEFT] != null,
                data[Room.FRONT] != null,
                data[Room.RIGHT] != null,
                data[Room.BACK] != null);
        }
        Debug.Log(enemyCount);
    }
}

[System.Serializable]
public class Room
{
    public RoomType type = RoomType.Battle;
    public int depth = 0;

    public UnityEvent movedHere = new UnityEvent();
    public UnityEvent OnRoomLeave = new UnityEvent();

    public GameObject MapDisplay { get; set; }
    public MeshRenderer DisplayMesh { get; set; }

    private readonly Room[] dirForRooms = new Room[4];

    /// <summary>
    /// 방향 인덱스
    /// </summary>
    public const int LEFT = 0,
                     FRONT = 1,
                     RIGHT = 2,
                     BACK = 3;

    public Room this[int idx] => dirForRooms[idx];

    /// <summary>
    /// 방 연결
    /// </summary>
    /// <param name="to"> 목적지 </param>
    /// <param name="dir"> 방향 </param>
    /// <returns></returns>
    public bool ConnectRoom(Room to, int dir)
    {
        if (dirForRooms[dir] == null)
        {
            dirForRooms[dir] = to;
            to.dirForRooms[(dir + 2) % 4] = this; 
            movedHere.AddListener(() =>
            {
                to.MapDisplay.SetActive(true);
                DisplayMesh.material.color = Color.white;
            });
            to.movedHere.AddListener(() =>
            {
                MapDisplay.SetActive(true);
                to.DisplayMesh.material.color = Color.white;
            });
            return true;
        }
        return false;
    }

    public enum RoomType
    {
        Start,
        Battle,
        Shop,
        Boss
    }
}