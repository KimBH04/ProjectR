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

        var makeingShop = rooms.Cast<Room>().Where(x => x != null && x.depth > 1).ToArray();
        makeingShop[Random.Range(0, makeingShop.Length)].type = Room.RoomType.Shop;

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
                    switch (rooms[z, x].type)
                    {
                        case Room.RoomType.Start:
                            Instantiate(
                                startRoom,
                                new Vector3(
                                    xpos * 10 * standardScale.x + xpos * padding,
                                    0f,
                                    zpos * 10 * standardScale.z + zpos * padding),
                                Quaternion.identity).GetComponent<RoomData>().depth = rooms[z, x].depth;
                            break;

                        case Room.RoomType.Battle:
                            Instantiate(
                                battleRoom,
                                new Vector3(
                                    xpos * 10 * standardScale.x + xpos * padding,
                                    0f,
                                    zpos * 10 * standardScale.z + zpos * padding),
                                Quaternion.identity).GetComponent<RoomData>().depth = rooms[z, x].depth;
                            break;

                        case Room.RoomType.Shop:
                            Instantiate(
                                shopRoom,
                                new Vector3(
                                    xpos * 10 * standardScale.x + xpos * padding,
                                    0f,
                                    zpos * 10 * standardScale.z + zpos * padding),
                                Quaternion.identity).GetComponent<RoomData>().depth = rooms[z, x].depth;
                            break;

                        case Room.RoomType.Boss:
                            Instantiate(
                                bossRoom,
                                new Vector3(
                                    xpos * 10 * standardScale.x + xpos * padding,
                                    0f,
                                    zpos * 10 * standardScale.z + zpos * padding),
                                Quaternion.identity).GetComponent<RoomData>().depth = rooms[z, x].depth;
                            break;

                        default:
                            Debug.Log($"Uknown Room Type {xpos} {zpos}");
                            break;
                    }
                }
            }
        }
    }

    private class Room
    {
        public RoomType type = RoomType.Battle;
        public int depth = 0;
        public Room left, up, right, down;

        private readonly Room[] dirForRooms = new Room[4];

        public const int LEFT = 0,
                         UP = 1,
                         RIGHT = 2,
                         DOWN = 3;

        public Room()
        {
            dirForRooms[0] = left;
            dirForRooms[1] = up;
            dirForRooms[2] = right;
            dirForRooms[3] = down;
        }

        public bool ConnectRoom(Room to, int dir)
        {
            if (dirForRooms[dir] == null)
            {
                dirForRooms[dir] = to;
                to.dirForRooms[(dir + 2) % 4] = this;
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
}
