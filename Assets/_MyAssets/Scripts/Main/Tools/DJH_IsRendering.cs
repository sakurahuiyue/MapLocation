using UnityEngine;
using System.Collections;

public class DJH_IsRendering : MonoBehaviour
{

    public bool isRendering = false;
    private float lastTime = 0;
    private float curtTime = 0;

    // void Update()  
    // {  
    //     isRendering=curtTime!=lastTime?true:false;  
    //     lastTime=curtTime;  
    // }  

    // void OnWillRenderObject()  
    // {  
    //     curtTime=Time.time;  
    // }  
    private void OnBecameInvisible()
    {
        isRendering = false;
    }
    private void OnBecameVisible()
    {
        isRendering = true;
    }

}

