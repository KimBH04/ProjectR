using UnityEngine;
using UnityEngine.Events;

public class RoomData : MonoBehaviour
{
    [SerializeField] private GameObject mapDisplay;
    [SerializeField] private MeshRenderer displayMesh;

    [SerializeField] private WaveContainer[] waves;

    private Room data;

    private bool visited = false;

    public Room Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            data.mapDisplay = mapDisplay;
            data.movedHere.AddListener(() =>
            {
                displayMesh.material.color = Color.white;
            });

            if (data.type == Room.RoomType.Battle && waves.Length > 0)
            {
                WaveContainer waveContainer = waves[Random.Range(0, waves.Length)];
                for (int i = 0; i < waveContainer.Count; i++)
                {
                    int index = i;  // handling variable capture
                    data.movedHere.AddListener(() => Instantiate(waveContainer[index], transform.position, Quaternion.identity));
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
    }
}

public class Room
{
    public RoomType type = RoomType.Battle;
    public int depth = 0;

    public UnityEvent movedHere = new UnityEvent();
    public GameObject mapDisplay;

    private readonly Room[] dirForRooms = new Room[4];

    /// <summary>
    /// 방향 인덱스
    /// </summary>
    public const int LEFT = 0,
                     UP = 1,
                     RIGHT = 2,
                     DOWN = 3;

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
            movedHere.AddListener(() => to.mapDisplay.SetActive(true));
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