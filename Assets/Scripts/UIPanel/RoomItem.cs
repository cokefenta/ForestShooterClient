using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour {
    public Text roomNameText;
    public Text totalCountText;
    public Text winCountText;
    public Button enterBtn;

    private RoomListPanel _roomListPanel;//roomListPanel的引用

    /// <summary>
    /// 房间id
    /// </summary>
    private int _roomId;
   

    void Start () {
        enterBtn.onClick.AddListener(delegate () { EnterButtonClicked(); });
	}
    /// <summary>
    /// 加入按钮点击事件
    /// </summary>
    private void EnterButtonClicked()
    {
        //TODO:处理加入房间-具体交给RoomListPanel中的EnterRoom执行
        _roomListPanel.EnterRoom(_roomId);
    }
    /// <summary>
    /// 设置房间信息
    /// </summary>
    /// <param name="roomId">房间id</param>
    /// <param name="roomName">房主名</param>
    /// <param name="totalCount">房主总次数</param>
    /// <param name="winCount">房主胜利数</param>
    /// <param name="roomListPanel">roomListPanel的引用</param>
    public void SetRoomInfo(int roomId, string roomName,int totalCount,int winCount,RoomListPanel roomListPanel)
    {
        _roomListPanel = roomListPanel;
        _roomId = roomId;
        roomNameText.text = roomName + " 的房间";
        totalCountText.text = "总场数" + "\n" + totalCount;
        winCountText.text = "胜利数" + "\n" + winCount;
    }
    /// <summary>
    /// 销毁自身这个游戏物体-每次加载房间列表前都要将现有的销毁掉
    /// </summary>
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
