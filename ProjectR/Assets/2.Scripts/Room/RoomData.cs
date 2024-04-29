using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public RoomType type = RoomType.Battle;
    public int depth = 0;

    private RoomData left;
    private RoomData up;
    private RoomData right;
    private RoomData down;

    /// <summary>
    /// 방과 방끼리 연결하기
    /// </summary>
    /// <param name="room"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public bool ConnectRoom(RoomData room, Direction dir)
    {
        switch (dir)
        {
            case Direction.Left:
                if (left != null)
                {
                    return false;
                }

                left = room;
                room.right = this;
                break;

            case Direction.Up:
                if (up != null)
                {
                    return false;
                }

                up = room;
                room.down = this;
                break;

            case Direction.Right:
                if (right != null)
                {
                    return false;
                }

                right = room;
                room.left = this;
                break;

            case Direction.Down:
                if (down != null)
                {
                    return false;
                }

                down = room;
                room.up = this;
                break;
        }
        return true;
    }
}

public enum RoomType
{
    Start,
    Battle,
    Shop,
    Boss
}

public enum Direction
{
    Left,
    Up,
    Right,
    Down,
}