using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    MaskManager maskManager;
    Transform groundGroup;
    Vector3 P1;
    Vector3 target = Vector3.zero;
    float distance = 15.0f;
    Vector3 offest = Vector3.zero;
    float Speed = 1500.0f;
    Vector3 orginCameraPos = Vector3.zero;
    float orginField = 0;
    private void Awake()
    {
        maskManager = FindObjectOfType<MaskManager>();
        groundGroup=GameObject.Find("GroundGroup").transform;
        orginCameraPos = Camera.main.transform.position;
        orginField = Camera.main.fieldOfView;
    }
    private void Update() {
        //范围限制
        // Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 40, 60);
    }
    private void LateUpdate()
    {
        //x 212.2~
        //范围限制
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 40, 60);
        //激活操作控制前提
        if (GameManager.GetInstance.currentState != GameManager.ViewState.Home
            && GameManager.GetInstance.currentState != GameManager.ViewState.Setting
            && !maskManager.GetWholeMaskCurrentState())
            CameraMove();
        else
        {
            ResetCamera();
        }
        // if(!groundGroup.GetComponent<DJH_IsRendering>().isRendering){
        //     ResetCamera();
        // }
        //Test
        // Debug.Log(string.Format("pos:{0},rot:{1},field:{2}", Camera.main.transform.position, Camera.main.transform.rotation, Camera.main.fieldOfView));
    }
    private void SetCameraFieldOfView()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5F;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5F;
        }
    }
    public void CameraMove()
    {
        SetCameraFieldOfView();
        if (Input.GetMouseButton(1))
        {

            float x;
            float y;
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");
            transform.Translate(new Vector3(-x, -y, 0) * Time.deltaTime * Speed * 5);
            //print("转换过的：" + Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0)));

        }

        if (Input.GetMouseButtonDown(1))
        {

            P1 = transform.position;

        }
        if (Input.GetMouseButtonUp(1))
        {

            ////利用射线检测来获取屏幕中心点坐标
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider)
                {
                    target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    distance = (hit.point - transform.position).magnitude;
                    print(hit.collider.name);
                }
            }
            else
            {

                offest = transform.position - P1;
                target = target + offest;
                distance = (target - transform.position).magnitude;
            }
        }
    }
    
    public void ResetCamera()
    {
        Camera.main.transform.position = orginCameraPos;
        Camera.main.fieldOfView = orginField;
    }
}
