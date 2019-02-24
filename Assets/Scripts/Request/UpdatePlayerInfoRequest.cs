using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 该request仅用于游戏结束后玩家战绩更新显示
/// </summary>
public class UpdatePlayerInfoRequest : BaseRequest {
    private RoomListPanel _roomListPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.UpdatePlayerInfo;
        base.Awake();
    }
    private void Start()
    {
        _roomListPanel = this.GetComponent<RoomListPanel>();
    }
    /// <summary>
    /// 处理游戏结束后更新战绩显示响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        string[] dataArr = data.Split('#');
        int totalCount = int.Parse(dataArr[0]);
        int winCount = int.Parse(dataArr[1]);
       //更新数据
        GameFacade.Instance.PlayerManager.currentUserScore.TotalCount = totalCount;
        GameFacade.Instance.PlayerManager.currentUserScore.WinCount = winCount;
        //剩余交给 RoomListPanel来完成
        _roomListPanel.HandleUpdatePlayerInfoResponse();
    }

}
