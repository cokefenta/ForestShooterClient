using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRoomRequest : BaseRequest {
    private RoomPanel _roomPanel;
    public override void Awake()
    {
        //设置RequestCode【可不用因为该Request不用Send】和ActionCode
        requestCode = RequestCode.Room;
        actionCode = ActionCode.UpdateRoom;
        base.Awake();
    }
   
    void Start () {
        _roomPanel = this.GetComponent<RoomPanel>();
	}
    /// <summary>
    /// 处理更新房间响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        //解析广播过来的房间中玩家数据
        string[] playerInfoStrArr = data.Split('*');
        foreach (string temp in playerInfoStrArr)
        {
            Debug.Log(temp);
        }
        //先判断数量
        if (playerInfoStrArr.Length >= 2)
        {
            //P1【房主】已经是设置好了的-只用解析P2的
            string[] p2infoStrArr = playerInfoStrArr[1].Split('#');
            string p2username = p2infoStrArr[1];//下标从1开始，0是id
            int p2TotalCount = int.Parse(p2infoStrArr[2]);
            int p2WinCount = int.Parse(p2infoStrArr[3]);
            //设置P2的信息-剩下交给RoomPanel来处理
            _roomPanel.HandleUpdateRoomResponse(p2username, p2TotalCount, p2WinCount);
        }
        else {
            //数据中只有房主的信息，没有P2信息，P2已经退出了-剩下交给RoomPanel来处理
            _roomPanel.HandleUpdateRoomResponse("null", -1, -1);
            Debug.Log("P2已经退出了");
        }
    }
}
