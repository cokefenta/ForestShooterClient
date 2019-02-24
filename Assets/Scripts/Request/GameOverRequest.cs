using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverRequest : BaseRequest {
    private GamePanel _gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GameOver;
        base.Awake();
    }
     void Start()
    {
        _gamePanel = this.GetComponent<GamePanel>();    
    }
    /// <summary>
    /// 处理游戏结束响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        //解析结果
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        //TODO根据结果
        if (returnCode == ReturnCode.Success)
        {
            //游戏胜利todo-交给GamePanel
            _gamePanel.HandleGameOverResponse(true);

        }
        else if(returnCode==ReturnCode.Fail) {
            //游戏失败todo-交给GamePanel
            _gamePanel.HandleGameOverResponse(false);
        }
    }
}
