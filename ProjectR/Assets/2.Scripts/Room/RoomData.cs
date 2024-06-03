using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RoomData : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WaveContainer[] waves;

    [SerializeField] private Room data;

    [SerializeField] private GameObject mapDisplay;
    [SerializeField] private MeshRenderer displayMesh;

    private bool visited = false;
    private int enemyCount = 0;

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
            data = value;
            data.MapDisplay = mapDisplay;
            data.DisplayMesh = displayMesh;
            if (data.type == Room.RoomType.Battle && waves.Length > 0)
            {
                WaveContainer waveContainer = waves[Random.Range(0, waves.Length)];
                enemyCount = waveContainer.Count;
                for (int i = 0; i < enemyCount; i++)
                {
                    int index = i;  // handling variable capture
                    data.movedHere.AddListener(() =>
                    {
                        if(_availableSpawnPoints.Count == 0)
                        {
                            return;
                        }
                        int spawnIndex = Random.Range(0, _availableSpawnPoints.Count);
                        Transform spawnPoint = _availableSpawnPoints[spawnIndex];
                        _availableSpawnPoints.RemoveAt(spawnIndex);
                        
                        var enemy = EnemyPools.AppearObject(waveContainer[index], spawnPoint.position).GetComponent<Enemy>();
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
                    });
                }
            }
        }
    }

    

    public void MovedHere()
    {
        if (visited)
        {
            return;
        }
        data.movedHere.Invoke();
        visited = true;

        if (data.type == Room.RoomType.Battle || data.type == Room.RoomType.Boss)
        {
            CreateRoom.CloseWalls(
                transform.position,
                data[Room.LEFT] != null,
                data[Room.FRONT] != null,
                data[Room.RIGHT] != null,
                data[Room.BACK] != null);
        }
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
    }
}

[System.Serializable]
public class Room
{
    public RoomType type = RoomType.Battle;
    public int depth = 0;

    public UnityEvent movedHere = new UnityEvent();

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