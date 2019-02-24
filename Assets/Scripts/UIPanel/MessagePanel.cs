using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MessagePanel : BasePanel {
    private  Text tipsText;//Text组件，用于显示提示信息

    
    private void Start()
    {
        tipsText = this.GetComponent<Text>();
       
    }

    private void Update()
    {
        
    }
    public override void OnEnter()
    {
        //保证再开时处于激活
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        //初始化
        tipsText = this.GetComponent<Text>();
        //设置不显示[即禁用]
        tipsText.enabled = false;
    }


    //这些具体Panel的独有方法，如这里的ShowMessage的调用
    //需要通过(GameFacade.UIManager.instantPanel.TryGet(面板类型) as 具体面板类),具体面板类的方法

  

    /// <summary>
    /// 开启显示消息
    /// </summary>
    /// <param name="msg">要显示的信息</param>
    public void ShowTipsMsg(string msg)
    {
        tipsText.color = Color.white;//每次显示前Alpha通道要先调到完全不透明，不然用过一次再开时就看不到文字了
        tipsText.text = msg;
        tipsText.enabled = true;
        //3s后自动消失
        Invoke("AutoHideText", 1);
    }

    private void AutoHideText()
    {
       
        //:添加DoTween效果，且在结束时关闭
        tipsText.DOFade(0, 2).OnComplete(() => { GameFacade.Instance.UiManager.PopPanel(); });
        
    }

    public override void OnExit()
    {
        this.gameObject.SetActive(false);
    }
}
