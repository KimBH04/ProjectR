using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Room[,] rooms;

    private int roomMaxSize;

    private IEnumerator Start()
    {
        roomMaxSize = roomCount * 2 + 1;
        rooms = new Room[roomMaxSize, roomMaxSize];

        float startTime = Time.time;
        Application.targetFrameRate = 1000;

        yield return StartCoroutine(SettingRooms());

        Coroutine buildWall = StartCoroutine(BuildWalls());
        Coroutine buildRoom = StartCoroutine(BuildRooms());
        yield return buildWall;
        yield return buildRoom; // 대기

        Application.targetFrameRate = 60;
        Debug.Log($"Total {Time.time - startTime}'s");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.up;
        }
        else
        {
            Debug.Log("It is debugging mode");
        }
    }

    private IEnumerator SettingRooms()
    {   //                              left     up      right   down
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
                    Instantiate(
                        rooms[z, x].type switch
                        {
                            Room.RoomType.Start => startRoom,
                            Room.RoomType.Battle => battleRooms[Random.Range(0, battleRooms.Length)],
                            Room.RoomType.Shop => shopRoom,
                            Room.RoomType.Boss => bossRoom,
                            _ => throw new UnityException($"Uknown Room Type: {xpos} {zpos}")
                        },
                        new Vector3(
                            xpos * 10f * standardScale.x,
                            0f, // 10 : Default plane size
                            zpos * 10f * standardScale.z),
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
                        rooms[j, i][Room.DOWN] == null ? blockedHorizontalWall : openedHorizontalWall,
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
}