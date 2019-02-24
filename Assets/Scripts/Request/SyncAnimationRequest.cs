using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackAnimationState { Execute,StandBy }
public class SyncAnimationRequest : BaseRequest {
    public Animator remoteRoleAnimator;
    private AttackAnimationState _attackAnimationState = AttackAnimationState.StandBy;

    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.SyncAnimation;
        base.Awake();
    }

    private void Update()
    {
        #region 处理远端玩家动画的同步
        if (_attackAnimationState == AttackAnimationState.Execute)
        {
            remoteRoleAnimator.SetTrigger("toAttack");
            _attackAnimationState = AttackAnimationState.StandBy;
        }

        #endregion
    }
    /// <summary>
    /// 发送同步动作请求
    /// </summary>
    public override void SendRequest()
    {
        string data = "null";
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode,actionCode,data);
    }
    /// <summary>
    /// 处理同步动作响应
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
        _attackAnimationState = AttackAnimationState.Execute;
    }
}
