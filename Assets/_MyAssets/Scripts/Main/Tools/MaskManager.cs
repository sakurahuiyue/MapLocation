using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    Transform wholeMask;
    private void Awake()
    {
        wholeMask = transform.Find("Masks/WholeMask");
    }

    //全屏幕mask相关信息
    public void ShowWholePanel()
    {
        wholeMask.gameObject.SetActive(true);
    }
    public void HideWholePanel()
    {
        wholeMask.gameObject.SetActive(false);
    }
    public bool GetWholeMaskCurrentState()
    {
        return wholeMask.gameObject.activeInHierarchy;
    }
}
