using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomRequest : BaseRequest {
    private RoomListPanel _roomListPanel;
    public override void Awake()
    {
        //设置RequestCode和ActionCode
        requestCode = RequestCode.Room;
        actionCode = ActionCode.JoinRoom;
        base.Awake();
    }

     void Start()
    {
        _roomListPanel = this.GetComponent<RoomListPanel>();
    }
    /// <summary>
    /// 发送加入房间请求
    /// </summary>
    /// <param name="roomId">房间id</param>
    public void SendRequest(int roomId)
    {
        string roomIdStr = roomId.ToString();
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode,actionCode,roomIdStr);
    }
    /// <summary>
    /// 处理加入房间请求的响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        //解析响应数据
        string[] dataStrArr = data.Split('-');//1级分割
        ReturnCode returnCode = (ReturnCode)int.Parse(dataStrArr[0]);
        if (returnCode == ReturnCode.Success)
        {
            //解析服务器分配的角色类型
            RoleType roleType = (RoleType)int.Parse(dataStrArr[1]);
            //设置角色类型
            GameFacade.Instance.PlayerManager.SetCurrentControllRoleType(roleType);
            //用于存储解析出来的数据
            List<User> userList = new List<User>();
            List<Score> scoreList = new List<Score>();
            //加入房间成功
            string[] usersdataStrArr = dataStrArr[2].Split('*');//二级分割
            foreach (string temp in usersdataStrArr)
            {
                string[] userdata = temp.Split('#');//三级分割
                string username = userdata[1];//从1下标开始，0是id，目前用不上
                int totalCount = int.Parse(userdata[2]);
                int winCount = int.Parse(userdata[3]);
                User user = new User();
                user.Username = username;
                Score score = new Score();
                score.TotalCount = totalCount;
                score.WinCount = winCount;
                userList.Add(user);
                scoreList.Add(score);
            }
            //后续操作交给RoomListPanel
            _roomListPanel.HandleJoinRoomResponse(true, userList, scoreList);
        }
        else if (returnCode == ReturnCode.Fail)
        {
            //加入房间失败.
            //后续操作交给RoomListPanel
            _roomListPanel.HandleJoinRoomResponse(false, null, null);
        }
    }
}
