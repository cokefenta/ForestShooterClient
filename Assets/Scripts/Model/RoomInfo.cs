using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据模型类，即房间信息类，与RoomItem对应
/// </summary>
public class RoomInfo  {
    /// <summary>
    /// 房间id-也就是房主的userid
    /// </summary>
    private int _roomId;
    /// <summary>
    /// 房主名
    /// </summary>
    private string _hostOwnerName;
    /// <summary>
    /// 房主总次数
    /// </summary>
    private int _hostOwnerTotalCount;
    /// <summary>
    /// 房主胜利数
    /// </summary>
    private int _hostOwnerWinCount;

    public RoomInfo(int id,string hostOwnerName, int hostOwnerTotalCount, int hostOwnerWinCount)
    {
        _roomId = id;
        _hostOwnerName = hostOwnerName;
        _hostOwnerTotalCount = hostOwnerTotalCount;
        _hostOwnerWinCount = hostOwnerWinCount;
    }
   
    public string HostOwnerName
    {
        get
        {
            return _hostOwnerName;
        }

        set
        {
            _hostOwnerName = value;
        }
    }

    public int HostOwnerTotalCount
    {
        get
        {
            return _hostOwnerTotalCount;
        }

        set
        {
            _hostOwnerTotalCount = value;
        }
    }

    public int HostOwnerWinCount
    {
        get
        {
            return _hostOwnerWinCount;
        }

        set
        {
            _hostOwnerWinCount = value;
        }
    }

    public int RoomId
    {
        get
        {
            return _roomId;
        }

        set
        {
            _roomId = value;
        }
    }
}
