using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTimerRequest : BaseRequest {
    private GamePanel _gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.ShowTimer;
        base.Awake();
        _gamePanel = this.GetComponent<GamePanel>();//与CreatRoomRequest ListRoomRequest一样，间隔太短，要提早初始化，防止报空
    }
    private void Start()
    {
       
    }
    public override void OnResponse(string data)
    {
        //解析到data变为秒数后，交给GamePanel处理后续
        int second = int.Parse(data);
        Debug.Log("解析到的秒数" + second);
        _gamePanel.HandleShowTimerResponse(second);
    }
}
