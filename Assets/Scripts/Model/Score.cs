using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据模型类 Score
/// </summary>
public class Score {

    private int _id;
    private int _userid;
    private int _totalCount;
    private int _winCount;
    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public int Userid
    {
        get
        {
            return _userid;
        }

        set
        {
            _userid = value;
        }
    }

    public int TotalCount
    {
        get
        {
            return _totalCount;
        }

        set
        {
            _totalCount = value;
        }
    }

    public int WinCount
    {
        get
        {
            return _winCount;
        }

        set
        {
            _winCount = value;
        }
    }
    public Score() { }

    public Score(int id, int userid, int totalCount, int winCount)
    {
        _id = id;
        _userid = userid;
        _totalCount = totalCount;
        _winCount = winCount;
    }
}
