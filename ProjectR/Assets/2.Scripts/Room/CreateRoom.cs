using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    [Header("Value settings")]
    [SerializeField] private int roomCount = 10;
    [Space]
    [SerializeField] private Vector3 roomScale = Vector3.one;
    //[SerializeField] private Vector3 roadScale = Vector3.one;
    [SerializeField] private float padding = 1f;

    [Header("Objects")]
    [SerializeField] private GameObject roomObject;
    //[SerializeField] private GameObject roadObject;
    [Space]
    [SerializeField] private GameObject openedVerticalWall;
    [SerializeField] private GameObject blockedVerticalWall;
    [SerializeField] private GameObject openedHorizontalWall;
    [SerializeField] private GameObject blockedHorizontalWall;

    private List<RoomData> roomDatas;

    private void Start()
    {
        roomObject.transform.localScale = roomScale;
        //roadObject.transform.localScale = roadScale;

        CreateRooms();
        CreateWalls();
    }

    private void CreateRooms()
    {
        // 처음 방 생성
        GameObject obj = Instantiate(roomObject);
        obj.name = "StartRoom";

        RoomData startRoom = obj.GetComponent<RoomData>();
        startRoom.type = RoomType.Start;

        // 위치 중복 방지
        bool[,] visited = new bool[roomCount * 2 + 1, roomCount * 2 + 1];
        visited[roomCount, roomCount] = true;

        (int x, int z)[] directions = { (-1, 0), (0, 1), (1, 0), (0, -1) };

        //두 번 째 방 생성
        int rand = Random.Range(0, 3);
        RoomData nextRoom = Instantiate(roomObject,
            new Vector3(
                directions[rand].x * roomScale.x * 10 + directions[rand].x * padding,
                0f,
                directions[rand].z * roomScale.z * 10 + directions[rand].z * padding),
            Quaternion.identity)
            .GetComponent<RoomData>();
        startRoom.ConnectRoom(nextRoom, (Direction)rand);
        nextRoom.depth = 1;

        // 방과 방 사이 길 생성
        //Instantiate(roadObject,
        //    (startRoom.transform.position + nextRoom.transform.position) / 2f,
        //    Quaternion.identity);

        visited[roomCount + directions[rand].x, roomCount + directions[rand].z] = true;

        // 처음 생성된 방과 방 위치
        roomDatas = new List<RoomData>(roomCount) { startRoom, nextRoom };
        List<(int r, int c)> roomPositions = new List<(int r, int c)>()
        {
            (roomCount, roomCount),                                             // start
            (roomCount + directions[rand].x, roomCount + directions[rand].z)    // next
        };

        // 보스방 생성하기
        RoomData bossRoom = nextRoom;
        int maxDepth = 0;

        for (int max = 2; max < roomCount; max++)
        {
            RoomData newRoom = Instantiate(roomObject).GetComponent<RoomData>();

        ReTry:
            int randIdx = Random.Range(1, max);
            int randDir = Random.Range(0, 4);

            var (x, z) = roomPositions[randIdx];
            RoomData anyRoom = roomDatas[randIdx];

            if (!visited[x + directions[randDir].x, z + directions[randDir].z] &&
                anyRoom.ConnectRoom(newRoom, (Direction)randDir))
            {
                roomDatas.Add(newRoom);
                roomPositions.Add((x + directions[randDir].x, z + directions[randDir].z));

                visited[x + directions[randDir].x, z + directions[randDir].z] = true;

                // 방 위치 지정
                newRoom.transform.position = anyRoom.transform.position +
                    new Vector3(
                        directions[randDir].x * roomScale.x * 10 + directions[randDir].x * padding,
                        0f,
                        directions[randDir].z * roomScale.z * 10 + directions[randDir].z * padding);

                // 방과 방 사이 길 생성
                //Instantiate(roadObject,
                //    (newRoom.transform.position + anyRoom.transform.position) / 2f,
                //    Quaternion.identity);

                newRoom.depth = anyRoom.depth + 1;
                if (maxDepth < newRoom.depth)
                {
                    bossRoom = newRoom;
                    maxDepth = newRoom.depth;
                }
            }
            else
            {
                goto ReTry;
            }
        }

        bossRoom.type = RoomType.Boss;
    }

    private void CreateWalls()
    {
        foreach (RoomData room in roomDatas)
        {
            if (room.ConnectedLeft)
            {
                Instantiate(openedVerticalWall, room.transform.position, Quaternion.Euler(0f, 180f, 0f));
            }
            else
            {
                Instantiate(blockedVerticalWall, room.transform.position, Quaternion.Euler(0f, 180f, 0f));
            }

            if (room.ConnectedUp)
            {
                Instantiate(openedHorizontalWall, room.transform.position, Quaternion.Euler(0f, 180f, 0f));
            }
            else
            {
                Instantiate(blockedHorizontalWall, room.transform.position, Quaternion.Euler(0f, 180f, 0f));
            }

            if (room.ConnectedRight)
            {
                Instantiate(openedVerticalWall, room.transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(blockedVerticalWall, room.transform.position, Quaternion.identity);
            }

            if (room.ConnectedDown)
            {
                Instantiate(openedHorizontalWall, room.transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(blockedHorizontalWall, room.transform.position, Quaternion.identity);
            }
        }
    }
}
