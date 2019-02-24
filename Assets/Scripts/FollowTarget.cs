using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public Transform target;
    private Vector3 _offset= new Vector3(0, 11.98111f, -14.10971f);//直接写死，不再动态计算了
    private float _zoomDegree = 10;
    private Camera _mainCamera;
    private float _filedOfCamera;//初始视野

   
	// Use this for initialization
	void Start () {
      
        _mainCamera = this.GetComponent<Camera>();
        _filedOfCamera = _mainCamera.fieldOfView;

    }
    /// <summary>
    /// 设置偏移量-到了跟随位置后调用，才能保证正确
   


    // Update is called once per frame
    void LateUpdate () {
      
            CameraFollw();
            CameraZoom();
        
    }

    private void CameraFollw()
    {
        Vector3 targetPos = target.position + _offset;
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, 0.5f*Time.fixedDeltaTime);
        this.transform.LookAt(target);
    }

    private void CameraZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput < 0)
        {
            _filedOfCamera += _zoomDegree;
            if (_filedOfCamera >= 100)//保险措施 - 防止镜头畸变
            {
                _filedOfCamera = 100;
            }
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _filedOfCamera, Time.fixedDeltaTime);
        } else if (scrollInput > 0)
        {
            _filedOfCamera-=_zoomDegree;
            if (_filedOfCamera <= 10) //保险措施 - 防止镜头畸变
            {
                _filedOfCamera = 10;
            }
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _filedOfCamera,  Time.fixedDeltaTime);
        }
       
    }
      
}
