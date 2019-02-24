using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseManager {
    //账户信息
    public User currentLoginedUser;//当前登录的玩家
    public Score currentUserScore;//当前登录的玩家的战绩
    //游戏中的角色数据
    public Dictionary<RoleType, RoleData> roleDataDict = new Dictionary<RoleType, RoleData>();
    //生成位置
    private Transform _redRoleSpawnPosition;
    private Transform _blueRoleSpawnPosition;
    //当前控制的本地角色
    private RoleType _currentControllRoleType;
    private GameObject _currentControllRoleGo;
    public GameObject CurrentControllRoleGo { get { return _currentControllRoleGo; } }
    //收到同步的远端角色
    private GameObject _remoteRoleGo;
    public GameObject RemoteRoleGo { get { return _remoteRoleGo; } }
    //与同步相关request的挂载体
    private GameObject _playerSyncGo;
    public GameObject PlayerSyncGo { get { return _playerSyncGo; } }
    /// <summary>
    /// 设置当前控制角色
    /// </summary>
    /// <param name="roleType"></param>
    public void SetCurrentControllRoleType(RoleType roleType)
    {
        _currentControllRoleType = roleType;
    }
    public PlayerManager(GameFacade gameFacade) : base(gameFacade)
    {
       
    }

    public override void OnInit()
    {
        _blueRoleSpawnPosition = GameObject.Find("BlueRolePosition").transform;
        _redRoleSpawnPosition= GameObject.Find("RedRolePosition").transform;
        InitRoleDataDict();
    }
    /// <summary>
    /// 初始化角色数据字典
    /// </summary>
    private void InitRoleDataDict()
    {
        //目前只有两个游戏角色
        RoleData hunter_blue = new RoleData(RoleType.Blue, "Prefabs/Hunter_BLUE", "Prefabs/Arrow_Blue");
        RoleData hunter_red = new RoleData(RoleType.Red, "Prefabs/Hunter_RED", "Prefabs/Arrow_Red");
        //加入字典
        roleDataDict.Add(hunter_blue.RoleType, hunter_blue);
        roleDataDict.Add(hunter_red.RoleType, hunter_red);
    }
    /// <summary>
    /// 生成角色-并设置当前控制角色/远端控制角色
    /// </summary>
    public void SpawnAllRoles()
    {
        GameObject hunter_blue=null;
        GameObject hunter_red=null;
        //生成所有角色-并设置本地控制角色
        foreach (RoleData roleData in roleDataDict.Values)
        {
            switch (roleData.RoleType)
            {
                case RoleType.Blue:
                    hunter_blue= GameObject.Instantiate(roleDataDict[RoleType.Blue].RolePrefab, _blueRoleSpawnPosition.position, Quaternion.identity);
                    //并设置他们的标签
                    hunter_blue.tag = "Blue";
                    if (_currentControllRoleType == RoleType.Blue)
                    {
                        _currentControllRoleGo = hunter_blue;
                    }
                    break;
                case RoleType.Red:
                    hunter_red = GameObject.Instantiate(roleDataDict[RoleType.Red].RolePrefab, _redRoleSpawnPosition.position, Quaternion.identity);
                    //并设置他们的标签
                    hunter_red.tag = "Red";
                    if (_currentControllRoleType == RoleType.Red)
                    {
                        _currentControllRoleGo = hunter_red;
                    }
                    break;
            }

        }
        //设置远端角色
        if (_currentControllRoleGo == hunter_blue)
        {
            _remoteRoleGo = hunter_red;
        }
        else if (_currentControllRoleGo == hunter_red)
        {
            _remoteRoleGo = hunter_blue;
        }
      
    }

    /// <summary>
    /// 为当前控制的角色添加组件
    /// </summary>
    public void AddComponentToCurrentControllRole()
    {
        //添加移动组件
        _currentControllRoleGo.AddComponent<PlayerMove>();
        //获取该角色的角色类型-【虽然和这里的_currentControllRoleType相同】
        RoleType roleType = _currentControllRoleGo.GetComponent<RoleInfo>().roleType;
        //添加攻击组件
        PlayerAttack pa= _currentControllRoleGo.AddComponent<PlayerAttack>();
        //设置箭的预制体
        pa.arrowPrefab = roleDataDict.TryGet(roleType).ArrowPrefab;
        

    }
    /// <summary>
    /// 生成同步相关的Request
    /// </summary>
    public void CreatAndStartSyncRequest()
    {
        //创建这个用来挂载Request的游戏物体
        _playerSyncGo = new GameObject("PlayerSyncGameObject");
        //添加MoveRequest同步位置动画请求
        MoveRequest moveRequest= _playerSyncGo.AddComponent<MoveRequest>();
        //设置同步位置动画请求的对象-本地玩家
        moveRequest.currentControllRoleTransform = _currentControllRoleGo.transform;
        moveRequest.currentControllRolePlayerMove = _currentControllRoleGo.GetComponent<PlayerMove>();
        //设置同步位置动画响应的对象-远端玩家
        moveRequest.remoteRoleTransform = _remoteRoleGo.transform;
        moveRequest.remoteRoleAnimator = _remoteRoleGo.GetComponent<Animator>();

        //添加SyncArrowRequest同步箭请求
        _playerSyncGo.AddComponent<SyncArrowRequest>();

        //添加伤害有关处理TakeDamageRequest请求
        _playerSyncGo.AddComponent<TakeDamageRequest>();

        //添加动作同步SyncAnimation的请求-并设置远端玩家的Animator组件
        SyncAnimationRequest syncAnimationRequest= _playerSyncGo.AddComponent<SyncAnimationRequest>();
        syncAnimationRequest.remoteRoleAnimator = _remoteRoleGo.GetComponent<Animator>();
        
        

    }
    /// <summary>
    /// 处理游戏结束后的相关事务
    /// </summary>
    public void GameOver()
    {
        GameObject.Destroy(_currentControllRoleGo);
        GameObject.Destroy(_remoteRoleGo);
        _playerSyncGo.GetComponent<MoveRequest>().CancelInvoke();//直接销毁物体Invoke还在进行，应当先停止再销毁
        GameObject.Destroy(_playerSyncGo);
        //切换摄像机
        GameFacade.Instance.CameraManager.SwitchToAnimationState();

    }
}
