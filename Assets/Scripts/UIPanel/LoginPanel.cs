using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Text.RegularExpressions;
using Common;
public enum ActionState { StandBy,Failed,Success}
public class LoginPanel : BasePanel {

    private CanvasGroup _canvasGroup;
    public Button closeBtn;//关闭面板按钮
    public Button executeLoginBtn;//执行登录
    public Button regsiterBtn;//注册按钮
    public InputField usernameInputField;//用户名输入框
    public InputField passwordInputField;//密码输入框
    private LoginRequest _loginRequestObj;//登录请求对象

    //这两个对象放在PlayerManager中管理
    //private User _currentLogedUser;//当前登录的玩家
    //private Score _currentUserScore;//当前登录的玩家的战绩

    /// <summary>
    /// 点击登录后的状态【由于新开线程不能直接调用主线程中的内容，就用该标志位来确定点击登录之后的状态】
    /// </summary>
    private ActionState _actionState = ActionState.StandBy;
    private void Start()
    {
        //初始化CanvasGroup组件
        _canvasGroup = this.GetComponent<CanvasGroup>();
        //初始化登录请求对象
        _loginRequestObj = this.GetComponent<LoginRequest>();
        //绑定监听点击事件
        closeBtn.onClick.AddListener(delegate() { CloseButtonClicked(); });
        executeLoginBtn.onClick.AddListener(delegate() { ExecuteLoginButtonClicked(); });
        regsiterBtn.onClick.AddListener(delegate () { RegisterButtonClicked(); });
    }

    private void Update()
    {
        if (_actionState == ActionState.Failed)
        {
            MessagePanel msgPanel= GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("登陆失败");

            _actionState = ActionState.StandBy;//保证只执行一次
        }else if(_actionState==ActionState.Success)
        {
           // :登录成功后打开房间列表面板-    
          RoomListPanel roomListPanel=GameFacade.Instance.UiManager.PushPanel(PanelType.RoomList) as RoomListPanel;
            //:显示玩家战绩
            roomListPanel.UpdateShowPlayerInfo(GameFacade.Instance.PlayerManager.currentLoginedUser.Id, GameFacade.Instance.PlayerManager.currentLoginedUser.Username, GameFacade.Instance.PlayerManager.currentUserScore.TotalCount, GameFacade.Instance.PlayerManager.currentUserScore.WinCount);

            _actionState = ActionState.StandBy;//保证只执行一次
        }
    }
    /// <summary>
    /// 处理登录的响应-并接收战绩数据
    /// </summary>
    /// <param name="isSuccessful">是否登录成功</param>
    /// <param name="data">登录成功情况下的战绩数据</param>
    public void HandleLoginResponse(bool isSuccessful)
    {
        if (!isSuccessful)
        {
            //登陆失败-给出提示信息【不可以直接调用-因为网络模块是新开的线程-不可调用Unity主线程的内容】
            //MessagePanel msgPanel= GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            //msgPanel.ShowTipsMsg("登陆失败");

            //改变标志位，标志位变动后由这里的Update去调用
            _actionState = ActionState.Failed;
            Debug.Log("登录失败");

        }
        else if (isSuccessful)
        {
            Debug.Log("登录成功");
            //改变标志位，标志位变动后由这里的Update去调用     
            _actionState = ActionState.Success;
        }
    }

    #region 按钮点击事件
    private void CloseButtonClicked()
    {
        //按钮音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ButtonClicked);
        //该面板出栈-会执行该面板的OnExit方法，需重写OnExit
        GameFacade.Instance.UiManager.PopPanel();
    }

    private void ExecuteLoginButtonClicked()
    {
        //按钮音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ButtonClicked);

        //:执行登录操作
        Debug.Log("输入的用户名" + usernameInputField.text);
        Debug.Log("输入的密码" + passwordInputField.text);
        //前端校验-这里只用进行判断为空的校验，在注册的时候才进行格式，长度等校验
        if (usernameInputField.text == "" || passwordInputField.text == "")
        {
            MessagePanel msgPanel= GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("用户名或密码为空");
            return;
        }
        //通过loginRequest对象发送请求
        _loginRequestObj.SendRequest(usernameInputField.text, passwordInputField.text);

    }
    
    private void RegisterButtonClicked()
    {
        //按钮音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ButtonClicked);
        //:打开注册面板
        GameFacade.Instance.UiManager.PushPanel(PanelType.Register);
    }
    #endregion

  


    #region 重写的方法 UI相关的逻辑
    public override void OnEnter()
    {
        //此为保证已经开过一次了，但又关闭过【执行OnExit被取消激活了】，再次开启时保证要激活状态
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        //初始化CanvasGroup组件.【为保证安全canvasGroup组件由于一开始就要用，可能这个时候OnEnter的执行会先于Start，导致报空，所以把初始化放在OnEnter更好】
        if (_canvasGroup == null)
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }
        //:动画-缩放动画-小到大-不透明度从-无到有【完全透明到不透明】需要canvasGroup组件
        //设置属性初始值
        this.transform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0;
        this.transform.DOScale(1, 0.4f);
        _canvasGroup.DOFade(1, 0.4f);
    }

    //注册面板开启时要暂停
    public override void OnPause()
    {
        //关闭交互性
        _canvasGroup.interactable = false;
        //可取消激活【禁用】-节约性能。【并没有出栈】
        this.gameObject.SetActive(false);
    }

    //注册面板关闭时要恢复
    public override void OnResume()
    {
        //开启交互性
        _canvasGroup.interactable = true;
        //恢复激活
        this.gameObject.SetActive(true);

    }
    //PopPanel要调用
    public override void OnExit()
    {
        
        //:消失动画-缩放动画-从大到小
        this.transform.DOScale(0, 0.4f);
        _canvasGroup.DOFade(0, 0.4f).OnComplete(() => { this.gameObject.SetActive(false);  });
    }
    #endregion
}
