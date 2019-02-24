using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 数据模型 User，与服务器的User相同
/// </summary>
public class User {

    private int _id;
    private string _username;
    private string _password;

    public User() { }
    public User(int id, string username, string password)
    {
        _id = id;
        _username = username;
        _password = password;
    }
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

    public string Username
    {
        get
        {
            return _username;
        }

        set
        {
            _username = value;
        }
    }

    public string Password
    {
        get
        {
            return _password;
        }

        set
        {
            _password = value;
        }
    }
}

