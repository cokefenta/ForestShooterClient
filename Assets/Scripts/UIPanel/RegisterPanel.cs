using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RegisterPanel : BasePanel {
    //UI控件
    public Button closeBtn;//关闭按钮
    public Button executeRegisterBtn;//执行登录按钮
    public InputField usernameInputField;//用户名输入框
    public InputField passwordInputField;//密码输入框
    public InputField repeatPasswordInputField;//重复密码输入框
    //组件
    private CanvasGroup _canvasGroup;
    private RegisterRequest _registerRequest;
    //标志位-用来开面板
    private ActionState _actionState = ActionState.StandBy;

    private void Start()
    {
        //组件初始化
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _registerRequest = this.GetComponent<RegisterRequest>();

        //注册监听点击事件
        closeBtn.onClick.AddListener(delegate () { CloseButtonClicked(); });
        executeRegisterBtn.onClick.AddListener(delegate () { ExecuteRegisterButtonClicked(); });
    }
    private void Update()
    {
        if (_actionState == ActionState.Success)
        {
            //给出注册成功提示
            MessagePanel msgPanel=GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("注册成功");

            _actionState = ActionState.StandBy;
        }
        else if (_actionState == ActionState.Failed)
        {
            //给出注册失败提示
            MessagePanel msgPanel = GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("注册失败-已存在该用户");

            _actionState = ActionState.StandBy;
        }
    }

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    private void CloseButtonClicked()
    {
        //按钮音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ButtonClicked);
        GameFacade.Instance.UiManager.PopPanel();
    }
    /// <summary>
    /// 执行登录按钮点击事件
    /// </summary>
    private void ExecuteRegisterButtonClicked()
    {
        //按钮音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ButtonClicked);
        //:前端校验
        if (usernameInputField.text != "" && passwordInputField.text != "" && repeatPasswordInputField.text != "")
        {
            if (passwordInputField.text == repeatPasswordInputField.text)
            {
                // :发送注册请求
                _registerRequest.SendRequest(usernameInputField.text, passwordInputField.text);

            }
            else {
                MessagePanel msgPanel = GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
                msgPanel.ShowTipsMsg("两次密码不同");
            }
        }
        else {
            MessagePanel msgPanel= GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("下面3项都不为空");
            return;
        }
    }


    public void HandleRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            Debug.Log("注册成功");
            //:向用户给出提示-通过修改标志位，由Update检测去调用开启面板
            _actionState = ActionState.Success;
        }
        else if (returnCode == ReturnCode.Fail)
        {
            Debug.Log("当前用户已经存在");
            //:向用户给出提示-通过修改标志位，由Update检测去调用开启面板
            _actionState = ActionState.Failed;
        }
    }

    #region 重写的UI框架函数
    public override void OnEnter()
    {
        //保证关闭后又开启时处于激活状态
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        //动画设定
        //设置初始状态

        //保证组件初始化成功
        if (_canvasGroup == null)
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }
        _canvasGroup.alpha = 0;
        this.transform.localScale = Vector3.zero;
        //开始动画【渐变+缩放】
        _canvasGroup.DOFade(1, 0.4f);
        this.transform.DOScale(1, 0.4f);

    }

    public override void OnPause()
    {
        //关闭交互性
        _canvasGroup.interactable = false;
    }

    public override void OnResume()
    {
        //开启交互性
        _canvasGroup.interactable = true;
    }

    public override void OnExit()
    {
        //关闭动画和取消激活
        _canvasGroup.DOFade(0, 0.4f);
        this.transform.DOScale(0, 0.4f).OnComplete(() => { this.gameObject.SetActive(false); });
    }
    #endregion
}
