using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlayRequest : BaseRequest {
    private GamePanel _gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartPlay;
        base.Awake();
    }
    private void Start()
    {
        _gamePanel = this.GetComponent<GamePanel>();
    }

    public override void OnResponse(string data)
    {
        _gamePanel.HandleStartPlayResponse();
    }
}
