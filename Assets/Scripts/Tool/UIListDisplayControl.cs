using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 用于实时规范列表的显示，防止过度移动显示空白
/// </summary>
public class UIListDisplayControl : MonoBehaviour {
    private RectTransform _rectTransform;
    /// <summary>
    /// 每个条目的高度
    /// </summary>
    public float ItemHeight = 100;//
    /// <summary>
    /// 条目间间隔的高度
    /// </summary>
    public float spacingHeight = 22;
    /// <summary>
    /// 显示区域的高度
    /// </summary>
    public float ShowAreaHeight = 667;
    /// <summary>
    /// 单页最多显示的条目数
    /// </summary>
    public int SinglePageMaxCount = 5;
    /// <summary>
    /// 显示上限值
    /// </summary>
    private float upLimit;
    /// <summary>
    /// 显示下限值
    /// </summary>
    private float downLimit;


	void Start () {
        _rectTransform = this.GetComponent<RectTransform>();
        //设置初始显示上下限
        upLimit = 0;
        downLimit = (this.transform.childCount * ItemHeight + (this.transform.childCount - 1) * spacingHeight)-ShowAreaHeight;

    }
	
	
	void Update () {
        //条目个数过少时，就不使用Scroll Rect了。避免此时拖动会产生颤动
        if (this.transform.childCount <= SinglePageMaxCount)
        {
            if (this.transform.parent.GetComponent<ScrollRect>().enabled == true)
            {
                this.transform.parent.GetComponent<ScrollRect>().enabled = false;
            }
        }
        else {
            if (this.transform.parent.GetComponent<ScrollRect>().enabled == false)
            {
                this.transform.parent.GetComponent<ScrollRect>().enabled = true;
            }
        }


        // Debug.Log(_rectTransform.localPosition);
        //保证位置在上下限之中，防止留白
        if (_rectTransform.localPosition.y <= upLimit)
        {
            _rectTransform.localPosition = new Vector3(0, upLimit, 0);
        }
        else if(_rectTransform.localPosition.y>=downLimit) {
            _rectTransform.localPosition = new Vector3(0, downLimit, 0);
        }

        //如果条目个数是变动，则需更新显示下限【该项目中】
        if(this.transform.childCount>SinglePageMaxCount)
        downLimit = (this.transform.childCount * ItemHeight + (this.transform.childCount - 1) * spacingHeight) - ShowAreaHeight;
    }

    
}
