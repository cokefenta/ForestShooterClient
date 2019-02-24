using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListRoomRequest : BaseRequest {
    private RoomListPanel _roomListPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ListRoom;
        base.Awake();
        _roomListPanel = this.GetComponent<RoomListPanel>();//这个和CreatRoomRequest一样，发送请求和接收响应间隔很短
        //为保证panel引用能使用，都更早一步进行初始化，放在Awake中
    }

    private void Start()
    {
        
    }
    public override void SendRequest()
    {
        string data = "null";//为避免空字符串，还是发一个无意义的数据
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode, actionCode, data);
    }

    public override void OnResponse(string data)
    {
        if (data.Equals("EMPTY"))
        {
            //没有数据，说明没有房间-剩余交给RoomListPanel处理
            _roomListPanel.HandleListRoomResponse(0, null);
            return;
        }
        //否则有数据
        List<RoomInfo> roomInfoList = new List<RoomInfo>();
        string[] roomArr = data.Split('*');//1级分割
        int roomCount = roomArr.Length;//取得房间个数
        foreach (string temp in roomArr)//解析每个房间的信息-房主名，房主总次数，房主胜利数
        {
           string[] roomInfoArr = temp.Split('#');//二级分割
           RoomInfo roomInfo = new RoomInfo(int.Parse(roomInfoArr[0]), roomInfoArr[1], int.Parse(roomInfoArr[2]),int.Parse( roomInfoArr[3]));
           roomInfoList.Add(roomInfo);
        }
        //数据解析完毕-房间信息已存在这个列表当中-剩余交给RoomListPanel处理
        _roomListPanel.HandleListRoomResponse(roomCount, roomInfoList);
       
            
        
    }
}
