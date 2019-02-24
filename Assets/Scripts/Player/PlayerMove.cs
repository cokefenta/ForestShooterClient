using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制移动
/// </summary>
public class PlayerMove : MonoBehaviour {
    private float moveSpeed = 3;
    private Animator _animtor;
    private int _forwardID = Animator.StringToHash("Forward");

    //用于同步位置时forward参数的获取
    public float parameterForward = 0;
    private void Awake()
    {
        
    }
    // Use this for initialization
    void Start () {
        _animtor = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        MoveAndRotate();

    }

    public void MoveAndRotate()
    {
        //攻击状态不允许移动
        if (_animtor.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return;
        }
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");
            //移动
            this.transform.Translate(new Vector3(hInput, 0, vInput) * moveSpeed * Time.fixedDeltaTime, Space.World);//允许斜方向上运动
            //朝向-设置始终朝向移动方向【不为0时才有必要】
            if(new Vector3(hInput, 0, vInput)!=Vector3.zero)
            this.transform.localRotation = Quaternion.LookRotation(new Vector3(hInput, 0, vInput));
            //设置动画
            float fowardVal = Mathf.Max(Mathf.Abs(hInput), Mathf.Abs(vInput));
            parameterForward = fowardVal;
            _animtor.SetFloat(_forwardID, fowardVal);
        
    }
}
