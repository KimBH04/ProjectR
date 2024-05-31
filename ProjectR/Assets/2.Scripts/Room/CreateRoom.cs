using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    [Header("Value settings")]
    [SerializeField, Min(2)] private int roomCount = 10;
    [Space]
    [SerializeField] private Vector3 standardScale = Vector3.one;

    [Header("Objects")]
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject[] battleRooms;
    [SerializeField] private GameObject bossRoom;
    [SerializeField] private GameObject shopRoom;
    [Space]
    [SerializeField] private GameObject openedVerticalWall;
    [SerializeField] private GameObject blockedVerticalWall;
    [SerializeField] private GameObject openedHorizontalWall;
    [SerializeField] private GameObject blockedHorizontalWall;
    [Space]
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform frontWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private Transform backWall;
    [Space]
    [SerializeField] private GameObject leftEffect;
    [SerializeField] private GameObject frontEffect;
    [SerializeField] private GameObject rightEffect;
    [SerializeField] private GameObject backEffect;
    [Space]
    [SerializeField] private Transform navPlane;

    private static CreateRoom instance;

    public static Transform center;
    public static Transform[] walls = new Transform[4];
    public static GameObject[] effects = new GameObject[4];

    private Room[,] rooms;

    private int roomMaxSize;

    private void Awake()
    {
        center = transform.Find("Blocks");
        walls[0] = leftWall;
        walls[1] = frontWall;
        walls[2] = rightWall;
        walls[3] = backWall;
        instance = this;

        effects[0] = leftEffect;
        effects[1] = frontEffect;
        effects[2] = rightEffect;
        effects[3] = backEffect;
    }

    private IEnumerator Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }

        roomMaxSize = roomCount * 2 + 1;
        rooms = new Room[roomMaxSize, roomMaxSize];

        float startTime = Time.time;
        Application.targetFrameRate = int.MaxValue;

        yield return StartCoroutine(SettingRooms());

        Coroutine buildWall = StartCoroutine(BuildWalls());
        Coroutine buildRoom = StartCoroutine(BuildRooms());
        yield return buildWall;
        yield return buildRoom; // 대기

        Application.targetFrameRate = 60;
        Debug.Log($"Total {Time.time - startTime}'s");

        if (player != null)
        {
            player.transform.position = Vector3.up;
            player.SetActive(true);
            PlayerController.canSkill = true;
        }
        else
        {
            Debug.Log("It is debugging mode");
        }
    }

    #region Room maker
    private IEnumerator SettingRooms()
    {   //                              left     front   right   back
        (int r, int c)[] directions = { (0, -1), (1, 0), (0, 1), (-1, 0) };

        Room start = rooms[roomCount, roomCount] = new Room()
        {
            type = Room.RoomType.Start
        };

        int dir = Random.Range(0, 3);
        var (r, c) = (roomCount + directions[dir].r, roomCount + directions[dir].c);
        start.ConnectRoom(
            rooms[r, c] = new Room()
            {
                depth = 1
            },
            dir);

        int maxDepth = 0;
        Room bossRoom = rooms[r, c];
        List<(int, int)> roomPositions = new List<(int, int)> { (r, c) };
        for (int cnt = 1; cnt < roomCount; cnt++)
        {
            Room newRoom = new Room();

        ReTry:
            int ranidx = Random.Range(0, cnt);
            int randir = Random.Range(0, 4);
            var (ranrow, rancol) = roomPositions[ranidx];
            var (nextRow, nextCol) = (ranrow + directions[randir].r, rancol + directions[randir].c);
            if (rooms[nextRow, nextCol] == null && rooms[ranrow, rancol].ConnectRoom(newRoom, randir))
            {
                rooms[nextRow, nextCol] = newRoom;
                newRoom.depth = rooms[ranrow, rancol].depth + 1;
                roomPositions.Add((nextRow, nextCol));

                if (maxDepth <= newRoom.depth)
                {
                    maxDepth = newRoom.depth;
                    bossRoom = newRoom;
                }
            }
            else
            {
                yield return null;
                goto ReTry;
            }
        }

        var makingShop = rooms.Cast<Room>().Where(x => x != null && x.depth > 1 && x.type != Room.RoomType.Boss).ToArray();
        makingShop[Random.Range(0, makingShop.Length)].type = Room.RoomType.Shop;

        bossRoom.type = Room.RoomType.Boss;
    }

    private IEnumerator BuildRooms()
    {
        for (int z = 0; z < roomMaxSize; z++)
        {
            for (int x = 0; x < roomMaxSize; x++)
            {
                if (rooms[z, x] != null)
                {
                    int xpos = x - roomCount, zpos = z - roomCount;
                    Vector3 pos = new Vector3(
                        xpos * 10f * standardScale.x,
                        0f, // 10 : Default plane size
                        zpos * 10f * standardScale.z);
                    rooms[z, x].movedHere.AddListener(() => navPlane.position = pos);

                    Instantiate(
                        rooms[z, x].type switch
                        {
                            Room.RoomType.Start => startRoom,
                            Room.RoomType.Battle => battleRooms[Random.Range(0, battleRooms.Length)],
                            Room.RoomType.Shop => shopRoom,
                            Room.RoomType.Boss => bossRoom,
                            _ => throw new UnityException($"Uknown Room Type: {xpos} {zpos}")
                        },
                        pos,
                        Quaternion.identity).GetComponentInChildren<RoomData>().Data = rooms[z, x];
                    yield return null;
                }
            }
        }
    }

    private IEnumerator BuildWalls()
    {
        for (int i = 0; i < roomMaxSize; i++)
        {
            for (int j = 1; j < roomMaxSize; j++)
            {
                int ipos = i - roomCount, jpos = j - roomCount;

                // i = x, j = z
                if (rooms[j, i] == null)
                {
                    if (rooms[j - 1, i] != null)
                    {
                        Instantiate(
                            blockedHorizontalWall,
                            new Vector3(
                                ipos * 10f * standardScale.x,
                                0f,
                                jpos * 10f * standardScale.z - standardScale.z * 5f),
                            Quaternion.identity);
                        yield return null;
                    }
                }
                else
                {
                    Instantiate(
                        rooms[j, i][Room.BACK] == null ? blockedHorizontalWall : openedHorizontalWall,
                        new Vector3(
                            ipos * 10f * standardScale.x,
                            0f,
                            jpos * 10f * standardScale.z - standardScale.z * 5f),
                        Quaternion.identity);
                    yield return null;
                }

                // i = z, j = x
                if (rooms[i, j] == null)
                {
                    if (rooms[i, j - 1] != null)
                    {
                        Instantiate(
                            blockedVerticalWall,
                            new Vector3(
                                jpos * 10f * standardScale.x - standardScale.x * 5f,
                                0f,
                                ipos * 10f * standardScale.z),
                            Quaternion.identity);
                        yield return null;
                    }
                }
                else
                {
                    Instantiate(
                        rooms[i, j][Room.LEFT] == null ? blockedVerticalWall : openedVerticalWall,
                        new Vector3(
                            jpos * 10f * standardScale.x - standardScale.x * 5f,
                            0f,
                            ipos * 10f * standardScale.z),
                        Quaternion.identity);
                    yield return null;
                }
            }
        }
    }
    #endregion

    public static void OpenWalls(bool left, bool front, bool right, bool back)
    {
        if (left)
        {
            walls[0].DOMoveY(-20f, 2.3f).SetEase(Ease.OutSine);
            effects[0].SetActive(true);
        }
        if (front)
        {
            walls[1].DOMoveY(-20f, 2.3f).SetEase(Ease.OutSine);
            effects[1].SetActive(true);
        }
        if (right)
        {
            walls[2].DOMoveY(-20f, 2.3f).SetEase(Ease.OutSine);
            effects[2].SetActive(true);
        }
        if (back)
        {
            walls[3].DOMoveY(-20f, 2.3f).SetEase(Ease.OutSine);
            effects[3].SetActive(true);
        }
    }

    public static void CloseWalls(Vector3 position, bool left, bool front, bool right, bool back)
    {
        center.position = position;
        if (left)
        {
            walls[0].DOMoveY(0f, 0.5f).SetEase(Ease.OutSine);
            effects[0].SetActive(true);
        }
        if (front)
        {
            walls[1].DOMoveY(0f, 0.5f).SetEase(Ease.OutSine);
            effects[1].SetActive(true);
        }
        if (right)
        {
            walls[2].DOMoveY(0f, 0.5f).SetEase(Ease.OutSine);
            effects[2].SetActive(true);
        }
        if (back)
        {
            walls[3].DOMoveY(0f, 0.5f).SetEase(Ease.OutSine);
            effects[3].SetActive(true);
        }
    }
}