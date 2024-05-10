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

    private void Start()
    {
        rooms = new Room[roomCount * 2 + 1, roomCount * 2 + 1];
        SettingRooms();
        BuildRooms();
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

    private void BuildRooms()
    {
        for (int z = roomCount * 2; z >= 0; z--)
        {
            for (int x = roomCount * 2; x >= 0; x--)
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
                            xpos * 10 * standardScale.x + xpos * padding,
                            0f, // 10 : Default plane size
                            zpos * 10 * standardScale.z + zpos * padding),
                        Quaternion.identity).GetComponent<RoomData>().data = rooms[z, x];
                }
            }
        }
    }
}