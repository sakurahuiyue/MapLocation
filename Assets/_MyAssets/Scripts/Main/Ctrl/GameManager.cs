using LitJson;
using Mathd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private GameManager()
    {

    }
    //获取实例唯一入口
    public static GameManager GetInstance
    {
        get
        {
            if (_instance == null)
            {
                GameObject Geo = new GameObject("GameManager");
                _instance = Geo.AddComponent<GameManager>();
                DontDestroyOnLoad(Geo);//切换场景不销毁
            }
            return _instance;
        }
    }
    public enum ViewState
    {
        Home = 0,
        Monitor = 1,
        Worker = 2,
        Vehicle = 3,
        Setting = 4
    }
    //供外部修改
    public ViewState currentState { get; set; } = 0;
    UIManager uimanager;
    //监听state变化event
    private void Awake()
    {
        uimanager = FindObjectOfType<UIManager>();
        currentState = ViewState.Home;
    }
    private void Start()
    {
    }
    //设置当前view所处的状态类型
    public void SetViewState(int _index)
    {
        if (_index > 4|| currentState == (ViewState)_index)
            return;
        currentState = (ViewState)_index;
        Debug.Log(string.Format("当前所处的状态类型：{0}", currentState));
    }
}
