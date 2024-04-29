using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    [Header("Value settings")]
    [SerializeField] private int roomCount = 10;
    [Space]
    [SerializeField] private Vector3 roomScale = Vector3.one;
    [SerializeField] private Vector3 loadScale = Vector3.one;
    [SerializeField] private float padding = 1f;

    [Header("Objects")]
    [SerializeField] private GameObject roomObject;
    [SerializeField] private GameObject loadObject;

    private List<RoomData> roomDatas;

    private void Start()
    {
        roomObject.transform.localScale = roomScale;
        loadObject.transform.localScale = loadScale;

        CreateRooms();
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
        RoomData nextStart = Instantiate(roomObject,
            new Vector3(
                directions[rand].x * roomScale.x * 10 + directions[rand].x * padding,
                0f,
                directions[rand].z * roomScale.z * 10 + directions[rand].z * padding),
            Quaternion.identity)
            .GetComponent<RoomData>();
        startRoom.ConnectRoom(nextStart, (Direction)rand);
        nextStart.depth = 1;

        // 방과 방 사이 길 생성
        Instantiate(loadObject,
            (startRoom.transform.position + nextStart.transform.position) / 2f,
            Quaternion.identity);

        visited[roomCount + directions[rand].x, roomCount + directions[rand].z] = true;
        
        // 처음 생성된 방과 방 위치
        roomDatas = new List<RoomData>(roomCount) { nextStart };
        List<(int r, int c)> roomPositions = new List<(int r, int c)>()
        {
            (roomCount + directions[rand].x, roomCount + directions[rand].z)
        };

        // 보스방 생성하기
        RoomData bossRoom = nextStart;
        int maxDepth = 0;

        for (int max = 1; max < roomCount; max++)
        {
            RoomData newRoom = Instantiate(roomObject).GetComponent<RoomData>();

        ReTry:
            int randIdx = Random.Range(0, max);
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
                Instantiate(loadObject,
                    (newRoom.transform.position + anyRoom.transform.position) / 2f,
                    Quaternion.identity);

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
}
