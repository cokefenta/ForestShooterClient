using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public enum UpdateRoomState { HaveP2,NoP2,StandBy }
public enum QuitRoomState { Execute,NoExecute,StandBy}
public enum StartGameState { Execute,NoExecute,StandBy}
public class RoomPanel : BasePanel {
    public Text p1UsernameText;
    public Text p1TotalCountText;
    public Text p1WinCountText;

    public Text p2UsernameText;
    public Text p2TotalCountText;
    public Text p2WinCountText;

    public Button startGameBtn;
    public Button exitAndDeleteRoomBtn;

    private CanvasGroup _canvasGroup;
    private ActionState _actionState=ActionState.StandBy;
    private UpdateRoomState _updateRoomState = UpdateRoomState.StandBy;
    private QuitRoomState _quitRoomState = QuitRoomState.StandBy;
    private StartGameState _startGameState = StartGameState.StandBy;

    public CreatRoomRequest creatRoomRequest;
    private QuitRoomRequest _quitRoomRequest;
    private StartGameRequest _startGameRequest;

    private string _p2Username;
    private int _p2TotalCount;
    private int _p2WinCount;
    private void Start()
    {
        //组件初始化
        _canvasGroup = this.GetComponent<CanvasGroup>();
        creatRoomRequest = this.GetComponent<CreatRoomRequest>();
        _quitRoomRequest = this.GetComponent<QuitRoomRequest>();
        _startGameRequest = this.GetComponent<StartGameRequest>();
        //注册点击事件
        startGameBtn.onClick.AddListener(delegate() { StartGameButtonClicked(); });
        exitAndDeleteRoomBtn.onClick.AddListener(delegate() { ExitAndDeleteRoomButtonClicked(); });
    }

     void Update()
    {
        #region 创建房间对roomPanel的具体设置
        if (_actionState == ActionState.Success)
        {
            //设置P1,P2的面板信息
            User user = GameFacade.Instance.PlayerManager.currentLoginedUser;
            Score userScore = GameFacade.Instance.PlayerManager.currentUserScore;
            //Debug.Log("玩家" + user.Username + "战绩" + userScore.WinCount + "/" + userScore.TotalCount);

            SetP1Info(user.Username, userScore.TotalCount, userScore.WinCount);
            ResetP2Info();
            //其作为房主-判断开始按钮的交互性-如果不可交互-则设置为可交互
            if (startGameBtn.interactable == false)
            {
                startGameBtn.interactable = true;
            }
            Debug.Log("已完成对Text的设置");
            _actionState = ActionState.StandBy;
            
        }
        else if (_actionState == ActionState.Failed)
        {
            //打开MsgPanel提示创建房间失败
            MessagePanel msgPanel = GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("创建房间失败");
            _actionState = ActionState.StandBy;
        }
        #endregion

        #region 玩家进入/退出房间对房主的RoomPanel进行更新
        if (_updateRoomState == UpdateRoomState.HaveP2)
        {

            SetP2Info(_p2Username, _p2TotalCount, _p2WinCount);
            _updateRoomState = UpdateRoomState.StandBy;
        }
        else if (_updateRoomState == UpdateRoomState.NoP2)
        {
            ResetP2Info();
            //对p2Name变量置空，解决p2加入后，又退出，p1在没有p2的情况下直接开始游戏的bug
            _p2Username = null;
            _updateRoomState = UpdateRoomState.StandBy;
        }

        #endregion

        #region 房间面板的关闭
        if (_quitRoomState == QuitRoomState.Execute)
        {
            GameFacade.Instance.UiManager.PopPanel();
            //
            _quitRoomState = QuitRoomState.StandBy;
        }
        #endregion

        #region 开始游戏控制
        if (_startGameState == StartGameState.Execute)
        {
            //开始游戏TODO
            Debug.Log("开始游戏");
            //打开GamePanel面板
            GameFacade.Instance.UiManager.PushPanel(PanelType.Game);
            //创建所有角色
            GameFacade.Instance.PlayerManager.SpawnAllRoles();
            //切换相机状态
            GameFacade.Instance.CameraManager.SwitchToFollowState(GameFacade.Instance.PlayerManager.CurrentControllRoleGo.transform);



            _startGameState = StartGameState.StandBy;
        }
        else if (_startGameState == StartGameState.NoExecute)
        {
            //不能开始游戏

            Debug.Log("开始游戏失败");
            MessagePanel msgPanel= GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("无法开始游戏");
            _startGameState = StartGameState.StandBy;
        }
        #endregion
    }
    private void StartGameButtonClicked()
    {
        _startGameRequest.SendRequest(_p2Username);
    }

    private void ExitAndDeleteRoomButtonClicked()
    {
        _quitRoomRequest.SendRequest();
    }

    

    //给外界调用的函数
    /// <summary>
    /// 设置P1的战绩信息
    /// </summary>
    /// <param name="p1Username">P1玩家名</param>
    /// <param name="p1TotalCount">P1总场数</param>
    /// <param name="p1WinCount">P1胜利数</param>
    public void SetP1Info(string p1Username,int p1TotalCount,int p1WinCount)
    {
        p1UsernameText.text = p1Username;
        p1TotalCountText.text = "总场数:" + p1TotalCount;
        p1WinCountText.text = "胜利数:" + p1WinCount;
    }
    /// <summary>
    /// 设置P2的战绩信息
    /// </summary>
    /// <param name="p2Username">P1玩家名</param>
    /// <param name="p2TotalCount">P1总场数</param>
    /// <param name="p2WinCount">P1胜利数</param>
    public void SetP2Info(string p2Username, int p2TotalCount, int p2WinCount)
    {

            p2UsernameText.text = p2Username;      
            p2TotalCountText.text = "总场数:" + p2TotalCount;    
            p2WinCountText.text = "胜利数:" + p2WinCount;
            
    }

    /// <summary>
    /// 用于初次开启房间面板对P2信息的重置
    /// 以及P2玩家进入房间后又离开后对P2信息进行重置
    /// </summary>
    public void ResetP2Info()
    {
        p2UsernameText.text = "等待玩家加入...";
        p2TotalCountText.text = "";
        p2WinCountText.text = "";
    }
    #region 响应的具体处理函数

    /// <summary>
    /// 处理创建房间响应
    /// </summary>
    /// <param name="isSuccessful"></param>
    public void HandleCreatRoomResponse(bool isSuccessful)
    {
        if (isSuccessful)
        {
            Debug.Log("创建房间成功");
            //更改标志位-剩下交给Update处理
            _actionState = ActionState.Success;
        }
        else {
            //更改标志位-剩下交给Update处理
            _actionState = ActionState.Failed;
        }
    }
    /// <summary>
    /// 处理退出房间的响应
    /// </summary>
    public void HandleQuitRoomResponse()
    {
        //关闭RoomPanel-更改标志位-交给Update
        _quitRoomState = QuitRoomState.Execute;

    }
    /// <summary>
    /// 处理更新房间响应
    /// </summary>
    public void HandleUpdateRoomResponse(string p2Username,int p2TotalCount,int p2WinCount)
    {
        //先判断传来的数据
        if (!p2Username.Equals("null") && p2TotalCount != -1 && p2WinCount != -1)
        {
            //说明有P2的数据
            _p2Username = p2Username;
            _p2TotalCount = p2TotalCount;
            _p2WinCount = p2WinCount;
            //更改标志位-剩下交给Update处理
            _updateRoomState = UpdateRoomState.HaveP2;
        }
        else {
            //说明没有P2的数据-剩下交给Update处理
            _updateRoomState = UpdateRoomState.NoP2;
        }

    }
    /// <summary>
    /// 处理开始游戏响应
    /// </summary>
    /// <param name="isOk"></param>
    public void HandleStartGameResponse(bool isOk)
    {
        if (isOk)
        {

            //可以开始游戏-后续交给Update-更改标志位
            _startGameState = StartGameState.Execute;

        }
        else {
            //不能开始游戏-后续交给Update-更改标志位
            _startGameState = StartGameState.NoExecute;
        }
    }
    #endregion
    #region 重写的函数
    public override void OnEnter()
    {
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        //进入动画
        if (_canvasGroup == null)
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }
        //设置动画初始状态
        _canvasGroup.alpha = 0;
        this.transform.localScale = Vector3.zero;
        //进行动画
        _canvasGroup.DOFade(1, 0.4f);
        this.transform.DOScale(1, 0.4f);
        ////发送CreatRoom请求【不可再此处调用-因为该面板加载有两种情况-加入别人房间这种情况不需要发送该请求】
        //if (_creatRoomRequest == null)
        //{
        //    _creatRoomRequest = this.GetComponent<CreatRoomRequest>();
        //}
        //_creatRoomRequest.SendRequest();
    }
    public override void OnPause()
    {
        _canvasGroup.interactable = false;
        //节约性能-暂时禁用
        this.gameObject.SetActive(false);
    }

    public override void OnResume()
    {
        _canvasGroup.interactable = true;
        //恢复启用
        this.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        //关闭动画
        _canvasGroup.DOFade(0, 0.4f);
        this.transform.DOScale(0, 0.4f).OnComplete(() => { this.gameObject.SetActive(false); });
    }
    #endregion
}
