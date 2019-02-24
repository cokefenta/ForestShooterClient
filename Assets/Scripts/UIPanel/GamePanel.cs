using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum TimerState { Run,StandBy}
public enum StartPlayState { Execute,StandBy}
public enum GameState { Win,Fail,StandBy}

public enum QuitInBattleState { Execute,StandBy }
public class GamePanel : BasePanel {
    public Text timerText;//计时器
    public Text resultText;//结果
    private int _timer;
    private TimerState _timerState = TimerState.StandBy;
    private StartPlayState _startPlayState = StartPlayState.StandBy;
    private GameState _gameState = GameState.StandBy;
    private QuitInBattleState _quitInBattleState = QuitInBattleState.StandBy;

    public Button returnToRoomListBtn;//回到房间列表按钮
    public Button closeBtn;//游戏中途退出按钮【返回到房间列表】

    private QuitInBattleRequest _quitInBattleRequest;
    private void Start()
    {
        //初始化组件
        _quitInBattleRequest = this.GetComponent<QuitInBattleRequest>();
        //注册点击事件
        returnToRoomListBtn.onClick.AddListener(delegate() { ReturnToRoomListButtonClicked(); });
        closeBtn.onClick.AddListener(delegate() { CloseButtonClicked(); });
    }

    private void Update()
    {
        #region 处理显示倒计时
        if (_timerState== TimerState.Run)
        {
            Debug.Log("显示时间");
            ShowTimer(_timer);//【找到原因，这个函数有问题】
            _timerState = TimerState.StandBy;
        }
        #endregion
        #region 处理开始战斗
        if (_startPlayState == StartPlayState.Execute)
        {
            //TODO:战斗开始
            //为当前控制的role添加，Move.Attack组件
            GameFacade.Instance.PlayerManager.AddComponentToCurrentControllRole();
            //开始同步
            GameFacade.Instance.PlayerManager.CreatAndStartSyncRequest();

            _startPlayState = StartPlayState.StandBy;
        }
        #endregion

        #region 处理战斗结束-面板显示
        if (_gameState == GameState.Win)
        {
            ShowResult("你赢了");
            //:处理游戏结束-后面的内容，保证再次开始正常
            GameFacade.Instance.PlayerManager.GameOver();
            //隐藏中断游戏按钮
            closeBtn.gameObject.SetActive(false);
            _gameState = GameState.StandBy;
        }
        else if (_gameState == GameState.Fail)
        {
            ShowResult("你输了");
            //:处理游戏结束-后面的内容，保证再次开始正常
            GameFacade.Instance.PlayerManager.GameOver();
            //隐藏中断游戏按钮
            closeBtn.gameObject.SetActive(false);
            _gameState = GameState.StandBy;
        }
        #endregion
        #region 战斗中退出游戏
        if (_quitInBattleState == QuitInBattleState.Execute)
        {
            //重置游戏场景
            GameFacade.Instance.PlayerManager.GameOver();
            //回到房间列表
            GameFacade.Instance.UiManager.PopPanel();
            GameFacade.Instance.UiManager.PopPanel();
            _quitInBattleState = QuitInBattleState.StandBy;
        }
        #endregion
    }
    /// <summary>
    /// 返回房间列表按钮按下
    /// </summary>
    private void ReturnToRoomListButtonClicked()
    {
        //执行两次，要回到房间列表界面，因为这个房间已经被销毁-不能停留在房间界面-否则退不出去，也开始不了游戏
        GameFacade.Instance.UiManager.PopPanel();
        GameFacade.Instance.UiManager.PopPanel();
    }
    /// <summary>
    /// 游戏中退出游戏
    /// </summary>
    private void CloseButtonClicked()
    {
        //发送中途退出游戏请求
        _quitInBattleRequest.SendRequest();
    }
    /// <summary>
    /// 显示计时器-【找到原因这个函数有问题】
    /// </summary>
    /// <param name="time"></param>
    public void ShowTimer(int time)
    {
        //播放音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.Alert);
        //设置初始大小和不透明度【保证再次调用时看得到】
        timerText.transform.localScale = Vector3.one;
        timerText.color = new Color(253f/255f,131f/255f,52f/255,1);
        timerText.text = time.ToString();
        //播放动画-播放完后禁用这个游戏物体-节约性能
        timerText.transform.DOScale(0, 0.4f).SetDelay(0.4f);
        timerText.DOFade(0, 0.4f).SetDelay(0.4f);;
        Debug.Log("倒计时：" + time);
    }
    /// <summary>
    /// 显示战斗结果
    /// </summary>
    /// <param name="msg">消息</param>
    public void ShowResult(string msg)
    {
        resultText.enabled = true;
        resultText.text = msg;
        returnToRoomListBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 处理显示计时器请求的响应
    /// </summary>
    /// <param name="seconds">当前秒数</param>
    public void HandleShowTimerResponse(int seconds)
    {
        Debug.Log("HandleShowTimerResponse执行了");
        _timer = seconds;
        //更改标志位-后交给Update处理
        _timerState = TimerState.Run;
    }
    /// <summary>
    /// 处理开始游戏请求的响应
    /// </summary>
    public void HandleStartPlayResponse()
    {
        //改变标志位-后交给Update处理
        _startPlayState = StartPlayState.Execute;
    }
    /// <summary>
    /// 处理游戏结束请求的响应
    /// </summary>
    public void HandleGameOverResponse(bool isWin)
    {
        if (isWin)
        {
            _gameState = GameState.Win;
        }
        else {
            _gameState = GameState.Fail;
        }
        
    }
    /// <summary>
    /// 处理在游戏中退出
    /// </summary>
    public void HandleQuitInBatttleResponse()
    {
        //更改标志位-后续交给Update
        _quitInBattleState = QuitInBattleState.Execute;
    }
    #region 重写的函数
    public override void OnEnter()
    {
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        //每次进来先清空/关闭结果显示和 按钮显示
        resultText.enabled = false;
        resultText.text = "";
        returnToRoomListBtn.gameObject.SetActive(false);
        closeBtn.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}
