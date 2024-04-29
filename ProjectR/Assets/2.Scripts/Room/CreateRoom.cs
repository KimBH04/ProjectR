using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private int roomCount = 10;
    [SerializeField] private RoomData startRoom;
    [SerializeField] private GameObject roomObject;
    [SerializeField] private GameObject loadObject;

    private List<RoomData> roomDatas;

    private void Start()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        // 위치 중복 방지
        bool[,] visited = new bool[roomCount * 2 + 1, roomCount * 2 + 1];
        visited[roomCount, roomCount] = true;

        (int r, int c)[] dirs = { (0, -1), (1, 0), (0, 1), (-1, 0) };

        // 처음 방 생성
        int rand = Random.Range(0, 3);
        RoomData nextStart = Instantiate(roomObject).GetComponent<RoomData>();
        startRoom.ConnectRoom(nextStart, (Direction)rand);

        // 방과 방 사이 길 생성
        Instantiate(loadObject,
            (startRoom.transform.position + nextStart.transform.position) / 2f,
            Quaternion.identity);

        visited[roomCount + dirs[rand].r, roomCount + dirs[rand].c] = true;
        
        // 처음 생성된 방과 방 위치
        roomDatas = new List<RoomData>(roomCount) { nextStart };
        List<(int r, int c)> roomPos = new List<(int r, int c)>()
        {
            (roomCount + dirs[rand].r, roomCount + dirs[rand].c)
        };

        for (int max = 1; max < roomCount; max++)
        {
            RoomData newRoom = Instantiate(roomObject).GetComponent<RoomData>();

        ReTry:
            int randIdx = Random.Range(0, max);
            int randDir = Random.Range(0, 4);

            var (r, c) = roomPos[randIdx];
            RoomData anyRoom = roomDatas[randIdx];

            if (!visited[r + dirs[randDir].r, c + dirs[randDir].c] &&
                anyRoom.ConnectRoom(newRoom, (Direction)randDir))
            {
                roomDatas.Add(newRoom);
                roomPos.Add((r + dirs[randDir].r, c + dirs[randDir].c));

                visited[r + dirs[randDir].r, c + dirs[randDir].c] = true;

                // 방과 방 사이 길 생성
                Instantiate(loadObject,
                    (newRoom.transform.position + anyRoom.transform.position) / 2f,
                    Quaternion.identity);
            }
            else
            {
                goto ReTry;
            }
        }
    }
}
