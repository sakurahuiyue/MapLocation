using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    MaskManager maskManager;

    Transform canvas;
    Transform UIPanelParent;
    Transform menuBase;
    List<Transform> menuBtnsList;
    List<Transform> menuPanelList;
    Button cancelBtn;
    Transform loginInfoBtn;
    //显示当前用户所在位置与当前编辑模式
    Transform messagePanel;
    private void Awake()
    {
        maskManager = FindObjectOfType<MaskManager>();
        Inite();
    }

    private void Start()
    {
        SetListeners();
    }

    private void Inite()
    {
        //获取引用
        canvas = GameObject.Find("View/Canvas").transform;
        UIPanelParent = canvas.Find("UIPanels");
        menuBase = UIPanelParent.Find("MenuBase");
        Transform btnGroup = menuBase.Find("MenuGroup");

        //常规事务按钮
        cancelBtn = UIPanelParent.Find("TitleBase/CancelLoginBtn").GetComponent<Button>();
        loginInfoBtn = UIPanelParent.Find("TitleBase/LoginInfoBtn");

        ShowCurrentUserName();

        messagePanel = UIPanelParent.Find("MessagePanels");
        //tipMsg = messagePanel.Find("TipMsg");
        //菜单按钮录入
        menuBtnsList = new List<Transform>();
        foreach (Transform _item in btnGroup)
        {
            menuBtnsList.Add(_item);
        }
        //对应panel录入
        menuPanelList = new List<Transform>();
        Transform mainPanelParent = UIPanelParent.Find("MainPanels");
        foreach (Transform _item in mainPanelParent)
        {
            menuPanelList.Add(_item);
        }
    }

    private void SetListeners()
    {
        //基本设置初始化
        //添加按钮监听
        for (int i = 0; i < menuBtnsList.Count; i++)
        {
            int index = i;
            menuBtnsList[index].GetComponent<Button>().onClick.AddListener(() => ShowUIPanel(index, menuPanelList[index]));
            //Debug.Log(string.Format("menubtn:{0} addlistener---", index));
        }
        //登入信息修改
        loginInfoBtn.GetComponent<ObjectClick>().onEnter.AddListener(() =>
        {

        });
        //退出程序
        cancelBtn.onClick.AddListener(() =>
        {
            //Debug.Log("cancel---", cancelBtn);
            //Application.Quit();
            SetCancelAlert();
        });
    }
    //警示监听
    void SetCancelAlert()
    {
        string projectTitle = UIPanelParent.Find("TitleBase/Title").GetComponent<Text>().text;
        SetGeneralTips(string.Format("是否要退出{0}", projectTitle), (Transform tran) =>
        {
            maskManager.ShowWholePanel();
            //隐藏正在使用的UIpanel
            Transform isworkPanel = null;
            foreach (Transform item in UIPanelParent.Find("MainPanels"))
            {
                if (!item.name.Equals(messagePanel.name) && item.gameObject.activeInHierarchy)
                {
                    isworkPanel = item;
                }
                item.gameObject.SetActive(false);
            }
            tran.Find("Confirm").GetComponent<Button>().onClick.AddListener(() =>
            {
                // Application.Quit();
                SceneManager.LoadScene(0);
            });
            tran.Find("Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                maskManager.HideWholePanel();
                tran.gameObject.SetActive(false);
                Destroy(tran.gameObject);
                if (isworkPanel != null)
                    isworkPanel.gameObject.SetActive(true);
            });
            tran.Find("Shut").GetComponent<Button>().onClick.AddListener(() =>
            {
                maskManager.HideWholePanel();
                tran.gameObject.SetActive(false);
                Destroy(tran.gameObject);
                if (isworkPanel != null)
                    isworkPanel.gameObject.SetActive(true);
            });
        });
    }
    //统一警示弹出框实例
    private void SetGeneralTips(string _tipStr,Action<Transform> _action)
    {
        Transform tipMsg = messagePanel.Find("TipMsg");
        if (tipMsg == null)
        {
            GameObject tipMsgPrefab = Resources.Load<GameObject>("Prefabs/TipMsg");
            tipMsg = Instantiate(tipMsgPrefab, messagePanel).transform;
        }
        tipMsg.gameObject.SetActive(true);
        tipMsg.Find("Content").GetComponent<Text>().text = _tipStr;
        _action(tipMsg);
    }

    /// <summary>
    /// 显示目标panel
    /// </summary>
    /// <param name="_currentPanel"></param>
    public void ShowUIPanel(int _index,Transform _currentPanel)
    {
        //设置当前state
        GameManager.GetInstance.SetViewState(_index);
        HideOtherPanels(_currentPanel);
        _currentPanel.gameObject.SetActive(true);
    }
    /// <param name="_currentPanel"></param>
    public void HideOtherPanels(Transform _currentPanel)
    {
        ////已激活,不刷新
        if (_currentPanel.gameObject.activeInHierarchy)
           return;

        //已激活的内容 需重新刷新

        //隐藏其他
        foreach (Transform _item in _currentPanel.parent)
        {
            _item.gameObject.SetActive(false);
        }
    }

    private void Update() {
        ShowCurrentUserName();
    }
    //设置当前用户
    public void ShowCurrentUserName()
    {
        Text userNameText = UIPanelParent.Find("TitleBase/UserNameText").GetComponent<Text>();
        userNameText.text = DataManager.GetInstance.user.username;
    }
}

