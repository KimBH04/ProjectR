using UnityEngine;

public class RoomData : MonoBehaviour
{
    public Room data;
}

[System.Serializable]
public class Room
{
    public RoomType type = RoomType.Battle;
    public int depth = 0;
    public Room left, up, right, down;

    private readonly Room[] dirForRooms = new Room[4];

    /// <summary>
    /// 방향 인덱스
    /// </summary>
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