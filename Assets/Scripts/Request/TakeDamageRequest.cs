using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageRequest:BaseRequest  {

    public override void Awake()
    {
        //设置类型
        requestCode = RequestCode.Game;
        actionCode = ActionCode.TakeDamage;
        base.Awake();
    }
    /// <summary>
    /// 发送执行伤害请求
    /// </summary>
    /// <param name="damageVal">伤害值</param>
    public void SendRequest(int damageVal)
    {
        string data = damageVal.ToString();
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode, actionCode, data);
    }


}
