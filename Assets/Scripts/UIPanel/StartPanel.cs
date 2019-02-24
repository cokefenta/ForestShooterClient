using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel {

    public Button loginBtn;//登录按钮，用于打开登录面板
    private void Start()
    {
        //绑定监听点击事件
        loginBtn.onClick.AddListener(delegate() { LoginButtonClicked();});
    }

    private void LoginButtonClicked()
    {
        //按钮音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ButtonClicked);
        //点击事件-打开面板
        GameFacade.Instance.UiManager.PushPanel(PanelType.Login);
    }

    public override void OnPause()
    {
        //取消该面板内所有UI控件的可交互性和一些其他属性-【该面板只有一个按钮】

        //取消动画
        loginBtn.gameObject.GetComponent<Animator>().enabled = false;
        //交互性取消
        loginBtn.interactable = false;
        //禁用以节约性能
        this.gameObject.SetActive(false);
    }

    public override void OnResume()
    {
        //恢复动画
        loginBtn.gameObject.GetComponent<Animator>().enabled = true;
        //交互性恢复
        loginBtn.interactable = true;
        //恢复启用
        this.gameObject.SetActive(true);
    }
}
