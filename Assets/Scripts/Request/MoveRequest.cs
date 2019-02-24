using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用于处理位置同步
/// </summary>
public class MoveRequest : BaseRequest {
    //同步频率
    public int syncRate = 20;
    //本地玩家的引用【发送给服务器】
    public Transform currentControllRoleTransform;
    public PlayerMove currentControllRolePlayerMove;
  
    //该客户端的远程玩家【将从服务接收到同步数据作用于此】
    public Transform remoteRoleTransform;
    public Animator remoteRoleAnimator;//其不需要PlayerMove，他不受本地玩家控制，也不具备这个组件
    //存储解析到远端玩家信息的内容
    private Vector3 _remoteRolePosition;
    private Vector3 _remoteRoleEulerAngles;
    private float _parametrsForward;
    private bool isNeedSyncRemotePlayerMove = false;//标志位-需要同步远端玩家位置吗
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Move;
        base.Awake();
    }

    private void Start()
    {
        InvokeRepeating("SyncLocalPlayerMove", 1, 1f / syncRate);//延迟1s中在开始，避免刚开始时这个Request还未加入到字典而导致找不到这个枚举对应的Request而报错
    }

    private void FixedUpdate()
    {
        if (isNeedSyncRemotePlayerMove)
        {

            SyncRemotePlayerMove(_remoteRolePosition, _remoteRoleEulerAngles, _parametrsForward);
            isNeedSyncRemotePlayerMove = false;
        }
    }
    /// <summary>
    /// 同步本地玩家的位置信息动画信息【发送给服务器】
    /// </summary>
    private void SyncLocalPlayerMove()
    {
        SendRequest(currentControllRoleTransform.position, currentControllRoleTransform.eulerAngles, currentControllRolePlayerMove.parameterForward);
    }
    /// <summary>
    /// 同步远端玩家的位置动画信息【将解析到的数据应用在游戏物体上】
    /// </summary>
    /// <param name="position"></param>
    /// <param name="eulerAngles"></param>
    /// <param name="parametrsForward"></param>
    private void SyncRemotePlayerMove(Vector3 position,Vector3 eulerAngles,float parametrsForward)
    {
        remoteRoleTransform.position = position;
        remoteRoleTransform.eulerAngles = eulerAngles;
        remoteRoleAnimator.SetFloat("Forward", parametrsForward);
    }
    /// <summary>
    /// 发送同步位置-动画请求
    /// </summary>
    /// <param name="position">位置</param>
    /// <param name="eulerAngles">旋转</param>
    /// <param name="forward">动画状态机parametersForward</param>
    private void SendRequest(Vector3 position, Vector3 eulerAngles,float forward)
    {
        //对这些数据进行拼接【注意不要用.因为有小数】
        string posData = position.x + "#" + position.y + "#" + position.z + "*" + eulerAngles.x + "#" + eulerAngles.y + "#" + eulerAngles.z + "*" + forward;
        GameFacade.Instance.ClientManager.SendRequestToServer(requestCode, actionCode, posData);
    }
    /// <summary>
    /// 解析来自服务器的位置-动画同步数据
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data)
    {
      //解析数据
        string[] dataArr = data.Split('*');
        string[] positionStrArr = dataArr[0].Split('#');
        Vector3 position = new Vector3(float.Parse(positionStrArr[0]), float.Parse(positionStrArr[1]), float.Parse(positionStrArr[2]));
        string[] eulerAngleStrArr = dataArr[1].Split('#');
        Vector3 eulerAngle = new Vector3(float.Parse(eulerAngleStrArr[0]), float.Parse(eulerAngleStrArr[1]), float.Parse(eulerAngleStrArr[2]));
        float parameterForward = float.Parse(dataArr[2]);
        //保存数据
        _remoteRolePosition = position;
        _remoteRoleEulerAngles = eulerAngle;
        _parametrsForward = parameterForward;
        //更改标志位
        isNeedSyncRemotePlayerMove = true;
    }
}
