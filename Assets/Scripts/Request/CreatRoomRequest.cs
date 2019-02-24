using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatRoomRequest : BaseRequest {

    private RoomPanel _roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreatRoom;
        base.Awake();
        _roomPanel = this.GetComponent<RoomPanel>();

    }

     void Start()
    {
        
    }

    /// <summary>
    /// 发起创建房间请求，该请求不用参数所以直接重写
    /// </summary>
    public override void SendRequest()
    {
        string data = "null";//为避免空字符串，还是发一个无意义的数据
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode,actionCode,data);
    }

    public override void OnResponse(string data)
    {
        //由于SendRequest执行的时机，点击创建房间后，是在加载RoomPanel时马上获得该Request执行SendRequest，然后又很快的执行OnResponse，所以SendRequest与OnResponse
        //执行间隔很短，OnResponse的执行可能在Start之前完成，就会导致_roomPanel报空
        //所以放在更早的Awake进行初始化。这里不能进行初始化，因为新开线程不能调用主线程的内容
        //
        //解析data
        string[] dataArr = data.Split('#');

        ReturnCode returnCode = (ReturnCode)int.Parse(dataArr[0]);
        if (returnCode == ReturnCode.Success)
        {
            //说明创建房间成功-剩余工作交给RoomPanel进行
            //获得服务器分配的角色
            RoleType roleType = (RoleType)(int.Parse(dataArr[1]));
            //设置该客户端的角色类型
            GameFacade.Instance.PlayerManager.SetCurrentControllRoleType(roleType);
            _roomPanel.HandleCreatRoomResponse(true);
        }
        else if (returnCode == ReturnCode.Fail)
        {
            //说明创建房间失败-剩余工作交给RoomPanel进行
            _roomPanel.HandleCreatRoomResponse(false);
        }
    }
}
