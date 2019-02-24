using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitRoomRequest : BaseRequest {
    private RoomPanel _roomPanel;
    public override void Awake()
    {
        //设置RequestCode和ActionCode
        requestCode = RequestCode.Room;
        actionCode = ActionCode.QuitRoom;
        base.Awake();
    }

     void Start()
    {
        _roomPanel = this.GetComponent<RoomPanel>();
    }
    /// <summary>
    /// 发送退出房间请求
    /// </summary>
    public override void SendRequest()
    {
        string data = "null";
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode, actionCode, data);
    }
    /// <summary>
    /// 处理退出房间请求的响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)(int.Parse(data));
        if (returnCode == ReturnCode.Success)
        {
            //交给RoomPanel
            _roomPanel.HandleQuitRoomResponse();
        }
    }
}
