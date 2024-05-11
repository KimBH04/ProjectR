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
    [SerializeField] private float padding = 1f;

    [Header("Objects")]
    [SerializeField] private GameObject startRoom;
    [SerializeField] private GameObject battleRoom;
    [SerializeField] private GameObject bossRoom;
    [SerializeField] private GameObject shopRoom;
    [Space]
    [SerializeField] private GameObject openedVerticalWall;
    [SerializeField] private GameObject blockedVerticalWall;
    [SerializeField] private GameObject openedHorizontalWall;
    [SerializeField] private GameObject blockedHorizontalWall;

    private Room[,] rooms;

    private int RoomMaxSize => roomCount * 2 + 1;

    private IEnumerator Start()
    {
        rooms = new Room[RoomMaxSize, RoomMaxSize];
        SettingRooms();
        Coroutine buildRoom = StartCoroutine(BuildRooms());
        Coroutine buildWall = StartCoroutine(BuildWalls());

        // 대기
        yield return buildRoom;
        yield return buildWall;

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

    private void SettingRooms()
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
                goto ReTry;
            }
        }

        var makingShop = rooms.Cast<Room>().Where(x => x != null && x.depth > 1).ToArray();
        makingShop[Random.Range(0, makingShop.Length)].type = Room.RoomType.Shop;

        bossRoom.type = Room.RoomType.Boss;
    }

    private IEnumerator BuildRooms()
    {
        for (int z = 0; z < RoomMaxSize; z++)
        {
            for (int x = 0; x < RoomMaxSize; x++)
            {
                if (rooms[z, x] != null)
                {
                    int xpos = x - roomCount, zpos = z - roomCount;
                    Instantiate(
                        rooms[z, x].type switch
                        {
                            Room.RoomType.Start => startRoom,
                            Room.RoomType.Battle => battleRoom,
                            Room.RoomType.Shop => shopRoom,
                            Room.RoomType.Boss => bossRoom,
                            _ => throw new UnityException($"Uknown Room Type: {xpos} {zpos}")
                        },
                        new Vector3(
                            xpos * 10f * standardScale.x + xpos * padding,
                            0f, // 10 : Default plane size
                            zpos * 10f * standardScale.z + zpos * padding),
                        Quaternion.identity).GetComponentInChildren<RoomData>().Data = rooms[z, x];
                }
                yield return null;
            }
        }
    }

    private IEnumerator BuildWalls()
    {
        for (int i = 0; i < RoomMaxSize; i++)
        {
            for (int j = 1; j < RoomMaxSize; j++)
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
                                ipos * 10f * standardScale.x + ipos * padding,
                                0f,
                                jpos * 10f * standardScale.z + jpos * padding - standardScale.z * 5f),
                            Quaternion.identity);
                    }
                }
                else
                {
                    Instantiate(
                        rooms[j, i][Room.DOWN] == null ? blockedHorizontalWall : openedHorizontalWall,
                        new Vector3(
                            ipos * 10f * standardScale.x + ipos * padding,
                            0f,
                            jpos * 10f * standardScale.z + jpos * padding - standardScale.z * 5f),
                        Quaternion.identity);
                }

                // i = z, j = x
                if (rooms[i, j] == null)
                {
                    if (rooms[i, j - 1] != null)
                    {
                        Instantiate(
                            blockedVerticalWall,
                            new Vector3(
                                jpos * 10f * standardScale.x + jpos * padding - standardScale.x * 5f,
                                0f,
                                ipos * 10f * standardScale.z + ipos * padding),
                            Quaternion.identity);
                    }
                }
                else
                {
                    Instantiate(
                        rooms[i, j][Room.LEFT] == null ? blockedVerticalWall : openedVerticalWall,
                        new Vector3(
                            jpos * 10f * standardScale.x + jpos * padding - standardScale.x * 5f,
                            0f,
                            ipos * 10f * standardScale.z + ipos * padding),
                        Quaternion.identity);
                }
                yield return null;
            }
        }
    }
}