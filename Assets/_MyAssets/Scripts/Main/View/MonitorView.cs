using System.Security.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MonitorView : MonoBehaviour
{
    MonitorControl monitorCtrl;
    MaskManager maskManager;
    UIManager uiManager;
    Transform canvas;
    Transform messagesPanelParent;
    Transform mainLayout;
    Transform buttonsGroup;
    Transform monitorSpawner;
    //监控obj数据
    List<Transform> monitorsList;
    List<Transform> monitorIconsList;

    //Test
    Vector3 orginCameraPos;
    private void Awake()
    {
        monitorCtrl = FindObjectOfType<MonitorControl>();
        uiManager = FindObjectOfType<UIManager>();
        maskManager = FindObjectOfType<MaskManager>();
        canvas = GameObject.Find("View").transform.Find("Canvas");

        messagesPanelParent = GameObject.Find("Canvas").transform.Find("UIPanels/MessagePanels");
        mainLayout = transform.Find("MainLayout");
        buttonsGroup = mainLayout.Find("BtnsGroup");
        monitorSpawner = GameObject.Find("Spawners").transform.Find("MonitorSpawner");
        //
        orginCameraPos=Camera.main.transform.position;
    }
    private void Start(){
        monitorsList=new List<Transform>();
        //绘制点
        monitorCtrl.DrawMonitorLocationPos();
        foreach (Transform item in monitorSpawner)
        {
            monitorsList.Add(item);
        }
        //监听
        // // SetMonitorInfoListeners();
        //初始化icon
        IniteMonitorIcons();
        maskManager.HideWholePanel();
    }
    private void Update()
    {
        UpdateMonitorIcon(monitorsList, monitorIconsList);
    }
    //退出
    private void OnDisable()
    {
        // //移除相关点,减少无效开销
        // monitorCtrl.RemoveMonitorLocationPos();
        //隐藏根节点
        if(monitorSpawner!=null)
            monitorSpawner.gameObject.SetActive(false);
    }
    //进入
    private void OnEnable()
    {
        // //绘制相关点
        // monitorCtrl.DrawMonitorLocationPos();
        // SetMonitorInfoListeners();
        // IniteMonitorIcons();
        // maskManager.HideWholePanel();
        if(monitorSpawner!=null){
            monitorSpawner.gameObject.SetActive(true);
            //
        }
    }

    //-------0910修改
    //切换为UI点击事件
    //各监控点监听
    void SetMonitorInfoListeners()
    {
        //基本信息panel
        // Transform infoPanel = messagesPanelParent.Find("MsgInfo");
        // infoPanel.gameObject.SetActive(false);
        // Button infoCancelBtn = infoPanel.Find("CancelImg").GetComponent<Button>();
        // infoCancelBtn.onClick.RemoveAllListeners();
        // infoCancelBtn.onClick.AddListener(() =>
        // {
        //     infoPanel.gameObject.SetActive(false);
        // });
        //摄像头画面
        Transform msgPanel = transform.parent.parent.Find("MessagePanels");
        Transform imgPanel = msgPanel.Find("ImgInfo");
        //画面退出监听
        Button imgCancelBtn = imgPanel.Find("CancelImg").GetComponent<Button>();
        imgCancelBtn.onClick.RemoveAllListeners();
        imgCancelBtn.onClick.AddListener(() =>
        {
            imgPanel.gameObject.SetActive(false);
        });
        imgPanel.gameObject.SetActive(false);
        //各点摄像头监听事件
        foreach (Transform item in monitorSpawner)
        {
            item.GetComponent<ObjectClick>().onclick.RemoveAllListeners();
            item.GetComponent<ObjectClick>().onclick.AddListener(() =>
            {
                // imgPanel.gameObject.SetActive(true);
                // //设置ImgPanel显示实时画面
                // GetMonitorInstantImg("TestId");
                string url=monitorCtrl.GetCurrentMonitorMedia(item.name);
                Application.OpenURL(url);
            });
        }
    }

    //获取实时视频画面
    private void GetMonitorInstantImg(string _monitorId)
    {
        Debug.Log("GetMonitorImages-----");
    }

    //初始化monitor图标设置
    void IniteMonitorIcons()
    {
        Transform monitorIconPanel = transform.Find("MonitorIconPanel");
        monitorIconsList=new List<Transform>();
        // Image image = monitorIconPanel.Find("Image").GetComponent<Image>();
        GameObject go = Resources.Load("Prefabs/MonitorIcon") as GameObject;
        foreach (Transform item in monitorSpawner)
        {
            GameObject img = Instantiate(go, monitorIconPanel);
            img.name=item.name;
            img.GetComponent<RectTransform>().anchoredPosition = EvConst.GetScreenPosition(item, canvas);
            img.SetActive(true);
            monitorIconsList.Add(img.transform);
        }
        //icon添加监听
        int itemIndex = 0;
        foreach (Transform item in monitorIconPanel)
        {
            item.Find("Text").GetComponent<Text>().text=EvConst.monitorNames[itemIndex];
            item.Find("Image").GetComponent<Button>().onClick.AddListener(()=>{
                string mediaUrl=monitorCtrl.GetCurrentMonitorMedia(item.name);
                Application.OpenURL(mediaUrl);
            });
            ++itemIndex;
        }
    }

    //随着视距变化更新UI位置信息
    private void UpdateMonitorIcon(List<Transform> monitors, List<Transform> monitorIcons)
    {
        if (monitorIcons == null || monitorIcons.Count == 0)
            return;
        for (int i = 0; i < monitors.Count; i++)
        {
            int index = i;
            monitorIcons[index].GetComponent<RectTransform>().anchoredPosition = EvConst.GetScreenPosition(monitors[index], canvas);
        }
    }
}
