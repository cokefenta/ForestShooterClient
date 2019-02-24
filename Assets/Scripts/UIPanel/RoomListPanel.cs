using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public enum RoomListState { Empty,Have,Standby }
public enum UpdatePlayerInfoState { Execute,StandBy }
public class RoomListPanel : BasePanel {
    public Button closeBtn;
    public Button creatRoomBtn;
    public Button reflashRoomListBtn;

    private CanvasGroup _canvasGroup;

    //Text组件
    public Text uidText;
    public Text userNameText;
    public Text totalCountText;
    public Text winCountText;
    public Text roomCountText;
    //Layout的位置
    private Transform _layout;
    //RoomInfo列表
    private List<RoomInfo> _roomInfoList = new List<RoomInfo>();//作为组件化脚本允许列表，字典等直接在定义时就进行new初始化
    private RoomListState _roomListState = RoomListState.Standby;
    //已经加入一个房间的玩家/战绩列表【就两个元素房主和P2】
    private List<User> _userListInRoom=new List<User>();
    private List<Score> _scoreListInRoom = new List<Score>();
    //Request组件
    private ListRoomRequest _listRoomRequest;
    private JoinRoomRequest _joinRoomRequest;
    //标志位
    private ActionState _joinRoomState = ActionState.StandBy;
    private UpdatePlayerInfoState _updatePlayerInfoState = UpdatePlayerInfoState.StandBy;

    void Start()
    {
        //初始化组件
        _listRoomRequest = this.GetComponent<ListRoomRequest>();
        _joinRoomRequest = this.GetComponent<JoinRoomRequest>();
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _layout = this.transform.Find("Img_RoomListBG/Img_List/Emt_Layout");
        //按钮注册监听点击事件
        closeBtn.onClick.AddListener(delegate () { CloseButtonClicked(); });
        creatRoomBtn.onClick.AddListener(delegate () { CreatRoomButtonClicked(); });
        reflashRoomListBtn.onClick.AddListener(delegate () { ReFlashRoomListButtonClicked(); });
    }
    void Update()
    {
        #region 列出房间列表的操作
        if (_roomListState == RoomListState.Have)
        {
            //有房间
            LoadRoomItem(_roomInfoList);
            //设置房间数量指示器
            SetRoomCountText(_roomInfoList.Count);
            _roomListState = RoomListState.Standby;
        }
        else if (_roomListState == RoomListState.Empty)
        {
            //没有房间-
            //先判断Layout下有无子物体，若有得清空。【不然会产生计数器显示0但有条目的BUG】
            if (_layout.transform.childCount > 0)
            {
                foreach (Transform roomItem in _layout.transform)
                {
                    roomItem.GetComponent<RoomItem>().DestroySelf();
                }
            }
            //设置房间数量指示器
            SetRoomCountText(0);
            _roomListState = RoomListState.Standby;
        }
        #endregion

        #region 加入房间的操作-非房主
        if (_joinRoomState == ActionState.Failed)
        {
            //给出提示信息-无法加入房间
            MessagePanel msgPanel = GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("无法加入房间");

            _joinRoomState = ActionState.StandBy;
        }
        else if(_joinRoomState==ActionState.Success) {
            //能够加入房间-开启房间面板，并设置房间信息
            RoomPanel roomPanel = GameFacade.Instance.UiManager.PushPanel(PanelType.Room) as RoomPanel;
            roomPanel.SetP1Info(_userListInRoom[0].Username, _scoreListInRoom[0].TotalCount, _scoreListInRoom[0].WinCount);
            roomPanel.SetP2Info(_userListInRoom[1].Username, _scoreListInRoom[1].TotalCount, _scoreListInRoom[1].WinCount);
            //作为P2玩家不可点击开始游戏-关闭该按钮的可交互性
            roomPanel.startGameBtn.interactable = false;

            _joinRoomState = ActionState.StandBy;

        }


        #endregion

        #region 游戏结束后更新战绩
        if (_updatePlayerInfoState == UpdatePlayerInfoState.Execute)
        {
            UpdateShowPlayerInfo(GameFacade.Instance.PlayerManager.currentLoginedUser.Id, GameFacade.Instance.PlayerManager.currentLoginedUser.Username, GameFacade.Instance.PlayerManager.currentUserScore.TotalCount, GameFacade.Instance.PlayerManager.currentUserScore.WinCount);

            _updatePlayerInfoState = UpdatePlayerInfoState.StandBy;
        }
        #endregion
    }
    private void CloseButtonClicked()
    {
        GameFacade.Instance.UiManager.PopPanel();
    }

    private void CreatRoomButtonClicked()
    {
        //显示RoomPanel
       RoomPanel roomPanel=  GameFacade.Instance.UiManager.PushPanel(PanelType.Room)as RoomPanel;
        //发送创建房间请求
        if (roomPanel.creatRoomRequest == null)
        {
            roomPanel.creatRoomRequest = roomPanel.GetComponent<CreatRoomRequest>();
        }
        roomPanel.creatRoomRequest.SendRequest();
    }

    private void ReFlashRoomListButtonClicked()
    {
        _listRoomRequest.SendRequest();
    }
    /// <summary>
    /// 更新显示玩家的个人信息
    /// </summary>
    /// <param name="userid">用户id</param>
    /// <param name="username">用户名</param>
    /// <param name="totalCount">总场数</param>
    /// <param name="winCount">胜利数</param>
    public void UpdateShowPlayerInfo(int userid, string username, int totalCount, int winCount)
    {
        uidText.text = "UID:" + userid;
        userNameText.text = "玩家名:" + username;
        totalCountText.text = "游玩总次数:" + totalCount;
        winCountText.text = "胜利次数:" + winCount;
    }
    /// <summary>
    /// 加载房间条目RoomItem
    /// </summary>
    /// <param name="roomInfoList">房间列表</param>
    private void LoadRoomItem(List<RoomInfo> roomInfoList)
    {
        //如果原来有房间，先销毁原来有的所有RoomItem
        if (_layout.transform.childCount > 0)
        {
            foreach (Transform roomItem in _layout.transform)
            {
                roomItem.GetComponent<RoomItem>().DestroySelf();
            }
        }
        GameObject roomItemPrefab = Resources.Load("UI/Img_RoomItem") as GameObject;
        for (int i = 0; i <= roomInfoList.Count - 1; i++)
        {
            //实例化房间条目RoomItem
            GameObject roomItem = Instantiate(roomItemPrefab);
            roomItem.transform.SetParent(_layout.transform, false);
            //设置RoomItem信息-并把自己引用给过去
            roomItem.GetComponent<RoomItem>().SetRoomInfo(roomInfoList[i].RoomId, roomInfoList[i].HostOwnerName, roomInfoList[i].HostOwnerTotalCount, roomInfoList[i].HostOwnerWinCount,this);
        }
    }
    /// <summary>
    /// 设置列表上方的房间数量指示器
    /// </summary>
    /// <param name="roomCount"></param>
    private void SetRoomCountText(int roomCount)
    {
        roomCountText.text = "房间个数:" + roomCount;
    }

    /// <summary>
    /// 处理列出房间响应
    /// </summary>
    /// <param name="roomCount">房间个数</param>
    /// <param name="roomInfoList">房间信息列表</param>
    public void HandleListRoomResponse(int roomCount, List<RoomInfo> roomInfoList)
    {
        if (roomCount == 0)
        {
            Debug.Log("没有房间");
            //没有房间-更改标志位，后交给Update处理
            _roomListState = RoomListState.Empty;
        }
        else {
            Debug.Log("有房间");
            //有房间-赋值给该面板的roomInfoList-更改标志位，后交给Update处理
            this._roomInfoList = roomInfoList;
            _roomListState = RoomListState.Have;
        }
    }
    /// <summary>
    /// 处理加入房间请求
    /// </summary>
    /// <param name="userList"></param>
    /// <param name="scoreList"></param>
    public void HandleJoinRoomResponse(bool isSuccessful,List<User> userList,List<Score> scoreList)
    {
        if (isSuccessful)
        {
            //加入房间成功
            _userListInRoom = userList;
            _scoreListInRoom = scoreList;
            //改变标志位-剩下交给Updtate执行
            _joinRoomState = ActionState.Success;
        }
        else {
            //改变标志位-剩下交给Updtate执行
            _joinRoomState = ActionState.Failed;
        }
    }
    /// <summary>
    /// 处理游戏结束后的房间列表左边的玩家信息更新请求
    /// </summary>
    public void HandleUpdatePlayerInfoResponse()
    {
        //只用该标志位-值已经更新到PlayerManager
        _updatePlayerInfoState = UpdatePlayerInfoState.Execute;
    }
    /// <summary>
    /// 进入房间-由RoomItem来调用
    /// </summary>
    /// <param name="roomId"></param>
    public void EnterRoom(int roomId)
    {
        //:加入房间
        _joinRoomRequest.SendRequest(roomId);
    }   
    #region 重写的方法
    public override void OnEnter()
    {
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        //实现开启动画
        //设置动画初始状态
        this.transform.localScale = Vector3.zero;
        if (_canvasGroup == null)
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }
        _canvasGroup.alpha = 0;
        this.transform.DOScale(1, 0.4f);
        _canvasGroup.DOFade(1, 0.4f);

        //进入该面板时就发起获取房间列表请求
        if (_listRoomRequest == null)
        {
            _listRoomRequest = this.GetComponent<ListRoomRequest>();
        }
        _listRoomRequest.SendRequest();
    }

    public override void OnPause()
    {
        //关闭交互性
        _canvasGroup.interactable = false;
        //隐藏不显示
        _canvasGroup.alpha = 0;
    }

    public override void OnResume()
    {
        //恢复交互性
        _canvasGroup.interactable = true;
        //调整alpha显示出来
        _canvasGroup.alpha = 1;
        //重写发送一次请求-获取最新数据
        _listRoomRequest.SendRequest();
    }

    public override void OnExit()
    {
        this.transform.DOScale(0, 0.4f);
        _canvasGroup.DOFade(0, 0.4f).OnComplete(() => { this.gameObject.SetActive(false); });
    }
    #endregion
}
