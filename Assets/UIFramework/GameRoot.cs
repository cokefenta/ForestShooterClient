using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameRoot : MonoBehaviour {
    //测试用
    public Canvas currentCanvas;
	// Use this for initialization
	void Start () {
        // Test();
        //UIManager.Insatance.PushPanel(PanelType.MAIN_MENU);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 用于测试解析是否成功，这里将主面板打开
    /// </summary>
    //public void Test()
   // {
        //实例化显示面板
        //GameObject mainPanel =Instantiate(UIManager.Insatance.uiPanelPrefabDic[PanelType.MAIN_MENU]);
       // mainPanel.transform.SetParent(currentCanvas.transform);


  //  }
}
