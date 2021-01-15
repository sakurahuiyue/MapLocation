using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCharts;
using static EvConst;

public class HomeView : MonoBehaviour
{
    //homectrl引用
    HomeControl homeCrl;
    //主板块
    private Transform baseInfoPanel;
    private Transform dealInfoPanel;

    //base下的子版块
    private Transform dayandweatherPanel;
    private Transform summaryPanel;

    Transform dealInfoLayoutGroup;
    List<Transform> dealInfoList;

    private Transform messagePanel;
    Transform mediaPlayerParent;
    //deal下的子版块
    //每页显示limit条
    int limit = 10;
    int page = 1;
    int maxPageFlag = 1;
    private void Awake()
    {
        dealInfoList=new List<Transform>();
        Inite();
        //baseInfoPanel.localScale = new Vector3(0, 1, 1);
    }
    private void Start()
    {
        //StartCoroutine(SetViewAnimation(SetListeners));
        SetListeners();
        Debug.Log("homeview start-----");
        ////TestgetImg
        //StartCoroutine(DataManager.GetInstance.DownloadFile("https://publish-pic-cpu.baidu.com/0dfdba88-89b2-4f3d-bc34-9031feb7c190.png@q_90,w_450", (Sprite s) =>
        // {
        //     transform.Find("Image").GetComponent<Image>().sprite = s;
        //     Debug.Log("get sprite-----------");
        // }));
        //homeCrl.SetImageSprite("https://publish-pic-cpu.baidu.com/0dfdba88-89b2-4f3d-bc34-9031feb7c190.png@q_90,w_450", transform.Find("Image").GetComponent<Image>());
        //Test
        // transform.Find("Img").GetComponent<Image>().sprite = homeCrl.GetImageSprite("E:\\Test.PNG");
    }

    private void Inite()
    {
        homeCrl = FindObjectOfType<HomeControl>();

        baseInfoPanel = transform.Find("BaseInfo");
        dealInfoPanel = transform.Find("DealInfo");
        dealInfoLayoutGroup = dealInfoPanel.Find("EmergencyPanel/Scroll View/Viewport/Content"); 
        dayandweatherPanel = baseInfoPanel.Find("DandWPanel");
        summaryPanel = baseInfoPanel.Find("SummaryPanel");
        messagePanel = GameObject.Find("MessagePanels").transform;
        mediaPlayerParent = GameObject.Find("MediaPlayers").transform;
    }
    private void SetListeners()
    {
        SetBaseInfoPanel();
        IniteMediaPlay();
    }
    void IniteMediaPlay()
    {
        //初始化测试media
        // mediaPlayerParent.GetChild(0).GetComponent<UniversalMediaPlayer>().Path="rtsp://admin:erhanghz0511@192.168.151.3/cam/realmonitor?channel=1&subtype=0";
        // mediaPlayerParent.GetChild(1).GetComponent<UniversalMediaPlayer>().Path="rtsp://admin:erhanghz0511@192.168.153.3/cam/realmonitor?channel=1&subtype=0";
        mediaPlayerParent.GetChild(0).GetComponent<UniversalMediaPlayer>().Play();
        mediaPlayerParent.GetChild(1).GetComponent<UniversalMediaPlayer>().Play();
    }
    void OnEnable(){
        //更新人员信息
        UpdateInfo();
        //Test 关闭视频
        // SetLiveSituation(MediaState.Play);
        // SetLiveSituation(MediaState.Play);
        //默认显示所有
        StartCoroutine(RefreshInfoPanel(InfoType.defaultType, HandleState.defalutState));
    }
    void OnDisable(){
        Debug.Log("disaable");
        // SetLiveSituation(MediaState.Pause);
    }
    //更新basePanel内容
    private void SetBaseInfoPanel()
    {
        //设置天数值
        int num = homeCrl.SetDayNum();
        if (num != -1)
            dayandweatherPanel.Find("dayText").GetComponent<Text>().text
            = string.Format("{0}天", num);
        //设置当前日期
        dayandweatherPanel.Find("bgLine/CurrentDate").GetComponent<Text>().text = homeCrl.GetCurrentDate();
        //设置天气
        homeCrl.GetCurrentWeather((weatherData, windData) =>
        {
            //修改一次,去除了天气和风力
            dayandweatherPanel.Find("WeatherText").GetComponent<Text>().text = string.Format(" {0}", weatherData);
            dayandweatherPanel.Find("WindText").GetComponent<Text>().text = string.Format(" {0}", windData);
        });
        // //设置工人图表信息
        // homeCrl.GetWorkerTotalInfo(workersDic =>
        //  {
        //      int workerCount = workersDic.Count;
        //      Debug.Log(workerCount);
        //      //设置图表
        //      PieChart pieChart = summaryPanel.Find("WorkerStatistics/PieChart").GetComponent<PieChart>();
        //      List<string> keyList = workersDic.Keys.ToList();
        //      int total = 0;
        //      pieChart.ClearData();
        //      for (int i = 0; i < workerCount; i++)
        //      {
        //          pieChart.AddData(0, workersDic[keyList[i]], keyList[i]);
        //          total += workersDic[keyList[i]];
        //      }
        //      summaryPanel.Find("WorkerStatistics/workerNum/workerNumText").GetComponent<Text>().text = string.Format("{0}人", total.ToString());
        //  });
        // homeCrl.GetVehicleTotalInfo(vehiclesDic =>
        // {
        //     int vehicleCount = vehiclesDic.Count;
        //     BarChart barChart = summaryPanel.Find("VehiclesStatistics/BarChart").GetComponent<BarChart>();
        //     List<string> keyList = vehiclesDic.Keys.ToList();
        //     barChart.ClearData();
        //     int total = 0;
        //     for (int i = 0; i < vehicleCount; i++)
        //     {
        //         barChart.AddXAxisData(keyList[i]);
        //         barChart.AddData(0, vehiclesDic[keyList[i]]);
        //         // barChart.AddData(0, 2, "Test");
        //         // barChart.AddData(0, 2, "Tssss");
        //         // barChart.AddData(0, vehiclesDic[keyList[i]], keyList[i]);
        //         Debug.Log(keyList[i]+"deviceTest-------");
        //         total += vehiclesDic[keyList[i]];
        //     }
        //     summaryPanel.Find("VehiclesStatistics/vehicleNum/vehicleNumText").GetComponent<Text>().text = string.Format("{0}台", total.ToString());
        //     ////
        //     //List<Color32> colors = new List<Color32> { Color32.Lerp(Color.red, Color.green,3f) };
        //     //barChart.themeInfo.colorPalette = colors;
        // });
        StartCoroutine(UpdateChartInfo());
    }
    IEnumerator UpdateChartInfo()
    {
        while (true)
        {
            UpdateInfo();
            //由于延时，重新刷新视频流
            IniteMediaPlay();
            yield return new WaitForSeconds(20.0f);
        }
    }
    void UpdateInfo(){
        //设置工人图表信息
            homeCrl.GetWorkerTotalInfo(workersDic =>
             {
                 int workerCount = workersDic.Count;
                 Debug.Log(workerCount);
                 //设置图表
                 PieChart pieChart = summaryPanel.Find("WorkerStatistics/PieChart").GetComponent<PieChart>();
                 List<string> keyList = workersDic.Keys.ToList();
                 int total = 0;
                 pieChart.ClearData();
                 for (int i = 0; i < workerCount; i++)
                 {
                     pieChart.AddData(0, workersDic[keyList[i]], keyList[i]);
                     total += workersDic[keyList[i]];
                 }
                 summaryPanel.Find("WorkerStatistics/workerNum/workerNumText").GetComponent<Text>().text = string.Format("{0}人", total.ToString());
             });
            homeCrl.GetVehicleTotalInfo(vehiclesDic =>
            {
                int vehicleCount = vehiclesDic.Count;
                BarChart barChart = summaryPanel.Find("VehiclesStatistics/BarChart").GetComponent<BarChart>();
                List<string> keyList = vehiclesDic.Keys.ToList();
                barChart.ClearData();
                int total = 0;
                for (int i = 0; i < vehicleCount; i++)
                {
                    barChart.AddXAxisData(keyList[i]);
                    barChart.AddData(0, vehiclesDic[keyList[i]]);
                // barChart.AddData(0, 2, "Test");
                // barChart.AddData(0, 2, "Tssss");
                // barChart.AddData(0, vehiclesDic[keyList[i]], keyList[i]);
                Debug.Log(keyList[i] + "deviceTest-------");
                    total += vehiclesDic[keyList[i]];
                }
                summaryPanel.Find("VehiclesStatistics/vehicleNum/vehicleNumText").GetComponent<Text>().text = string.Format("{0}台", total.ToString());
            ////
            //List<Color32> colors = new List<Color32> { Color32.Lerp(Color.red, Color.green,3f) };
            //barChart.themeInfo.colorPalette = colors;
        });
    }
    enum MediaState{
        Play,
        Pause
    }
    //现场工作情况设置
    void SetLiveSituation(MediaState inputState)
    {
        // if (mediaPlayerParent == null)
        //     mediaPlayerParent = GameObject.Find("MediaPlayers").transform;
        // foreach (Transform item in mediaPlayerParent)
        // {
        //     // item.GetComponent<UniversalMediaPlayer>().Prepare();
        //     if (inputState == MediaState.Play)
        //         item.GetComponent<UniversalMediaPlayer>().Play();
        //     if (inputState == MediaState.Pause)
        //         // item.GetComponent<UniversalMediaPlayer>().Pause();
        //         {
        //             Debug.Log("pause");
        //             item.GetComponent<UniversalMediaPlayer>().Stop();
        //         }
        // }
    }

    //1min一次请求刷新
    IEnumerator RefreshInfoPanel(InfoType _infoType,HandleState _handleState){
        while(true){
            SetDealInfoPanel(_infoType,_handleState);
            Debug.Log("刷新dealinfo panel");
        yield return new WaitForSeconds(alertWaitSeconds);
        }
    }
    void SetPagesListeners(int maxConut)
    {
        Transform btnsParent = dealInfoPanel.Find("EmergencyPanel");
        Button frontBtn = btnsParent.Find("frontBtn").GetComponent<Button>();
        Button nextBtn = btnsParent.Find("nextBtn").GetComponent<Button>();
        Text pageText = btnsParent.Find("Page").GetComponent<Text>();
        Text pageCountText = btnsParent.Find("PageCount").GetComponent<Text>();
        pageCountText.text = string.Format("共{0}页", maxConut);
        frontBtn.onClick.RemoveAllListeners();
        frontBtn.onClick.AddListener(()=>{
            if (page > 1)
            {
                --page;
                UpdatePageContens(pageText);
            }
        });
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(()=>{
            if (page < maxConut)
            {
                ++page;
                UpdatePageContens(pageText);
            }
        });
    }
    void UpdatePageContens(Text pageText){
        //更新页码
        pageText.text = page.ToString();
        //更新内容
        SetDealInfoPanel(InfoType.defaultType, HandleState.defalutState);
    }
    void SetDealInfoPanel(InfoType _infoType,HandleState _handleState)
    {
        //删除parent节点下的内容
        if (dealInfoLayoutGroup.childCount != 0)
        {
            // Debug.Log("clear and inite");
            foreach (Transform item in dealInfoLayoutGroup)
            {
                if(item!=null){
                    Destroy(item.gameObject);
                }
            }
        }
        homeCrl.GetMaxPageContent(limit,1,_infoType,_handleState,maxPage=>{
            if (maxPageFlag != maxPage)
            {
                maxPageFlag = maxPage;
                SetPagesListeners(maxPage);
            }
        });
        homeCrl.GetAlertInfo(limit,page,_infoType,_handleState,alertInfoList =>
        {
            Debug.Log(alertInfoList.Count+"-----");
            //每切换一次页面就更新一次内容与监听
            for (int i = 0; i < alertInfoList.Count; i++)
            {
                // GameObject go = dealInfoLayoutGroup.GetChild(i).gameObject;
                GameObject go = Instantiate(Resources.Load("Prefabs/EmergencyItem") as GameObject, dealInfoLayoutGroup);
                go.name = "alertItem" + i;
                go.tag = "alertBtn";
                dealInfoList.Add(go.transform);
                //对Item的内容进行更新
                //基本内容
                go.transform.Find("Info/Title").GetComponent<Text>().text = alertInfoList[i].title;
                // go.transform.Find("Info/Detail").GetComponent<Text>().text = alertInfoList[i].detail;
                go.transform.Find("Info/Detail").GetComponent<Text>().text = ReplaceMonitorsLocation(alertInfoList[i].detail);
                if(alertInfoList[i].state=="0"){
                    go.transform.Find("Info/State").GetComponent<Text>().text = "未处理";
                    go.transform.Find("Info/State").GetComponent<Text>().color=Color.red;
                }
                else{
                    go.transform.Find("Info/State").GetComponent<Text>().text = "已处理";
                    go.transform.Find("Info/State").GetComponent<Text>().color=Color.green;
                }
                go.transform.Find("Info/Date").GetComponent<Text>().text = alertInfoList[i].date;
                if(alertInfoList[i].image != "")
                    go.transform.Find("Img").GetComponent<Image>().sprite = EvConst.GetImageSprite(alertInfoList[i].image);
            }
            SetAlertDetails(dealInfoLayoutGroup, alertInfoList);
        });
    }

    //警报详情监听
    private void SetAlertDetails(Transform dealInfoLayoutGroup,List<AlertInfo> alertInfoList)
    {
        Transform alertInfoDetailPanel = messagePanel.Find("AlertInfo");
        alertInfoDetailPanel.gameObject.SetActive(false);
        //base info
        //type
        Text title = alertInfoDetailPanel.Find("Type").GetComponent<Text>();
        //date
        Text date = alertInfoDetailPanel.Find("Date").GetComponent<Text>();
        //pso
        Text pos = alertInfoDetailPanel.Find("Panel/PosItem/Pos").GetComponent<Text>();
        //state
        Text state = alertInfoDetailPanel.Find("Panel/StateItem/State").GetComponent<Text>();
        //inputText
        string submitStr=string.Empty;
        InputField inputField=alertInfoDetailPanel.Find("InputText").GetComponent<InputField>();
        Image image = alertInfoDetailPanel.Find("Image").GetComponent<Image>();

        //Test
        int index=0;
        // foreach (Transform _alertItem in dealInfoLayoutGroup)
        for (int i = 0; i < dealInfoLayoutGroup.childCount; i++)
        {
            index = i;
            // Debug.Log(index + "..................");
            dealInfoLayoutGroup.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            string ti = alertInfoList[index].title;
            string da = alertInfoList[index].date;
            // string po=alertInfoList[index].detail;
            string po = ReplaceMonitorsLocation(alertInfoList[index].detail);
            string st = alertInfoList[index].state;
            Sprite sprite = EvConst.GetImageSprite(alertInfoList[index].image);
            // if (alertInfoList[index].state == "0")
            // {
            //     st = "未处理";
            //     inputField.interactable = true;
            // }
            // else
            // {
            //     st = "已处理";
            //     inputField.interactable = false;
            // }
            string re = alertInfoList[index].remark;
             dealInfoLayoutGroup.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                //update info in panel
                // title.text = alertInfoList[index].title;
                // date.text = alertInfoList[index].date;
                // pos.text = alertInfoList[index].detail;
                // state.text = alertInfoList[index].state;
                // inputField.text=alertInfoList[index].remark;\
                Debug.Log("st内容---"+st);
                if (st == "0")
                {
                    state.text = "未处理";
                    inputField.interactable = true;
                }
                if(st=="1")
                {
                    state.text = "已处理";
                    inputField.interactable = false;
                }
                title.text = ti;
                date.text = da;
                pos.text = po;
                // state.text = st;
                inputField.text = re;
                if(sprite != null)
                    image.sprite = sprite;
                alertInfoDetailPanel.gameObject.SetActive(true);
            });
        }
        //read remark
        inputField.onEndEdit.AddListener(inputStr =>
        {
            submitStr = inputStr;
        });
        //warningText
        Text warningText=alertInfoDetailPanel.Find("WarningText").GetComponent<Text>();
        warningText.gameObject.SetActive(false);
        //cancel
        Button cancelBtn = alertInfoDetailPanel.Find("Cancel").GetComponent<Button>();
        // Button cancelImg=alertInfoDetailPanel.Find("CancelImg").GetComponent<Button>();
        //submit
        Button confirmBtn = alertInfoDetailPanel.Find("Confirm").GetComponent<Button>();
        //listeners
        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(() =>
        {
            Debug.Log("cancel");
            warningText.gameObject.SetActive(false);
            alertInfoDetailPanel.gameObject.SetActive(false);
        });
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(() =>
        {
            //submit
            //判断
            if(submitStr==string.Empty){
                warningText.gameObject.SetActive(true);
                return;
            }
            Debug.Log("deal");
            //reset dealState
            // _alertItem.transform.Find("Info/State").GetComponent<Text>().text = "已处理";
            int alertId = alertInfoList[EvConst.currentClickId].id;
            homeCrl.DealAlertInfo(alertId.ToString(), submitStr, () =>
            {
                Debug.Log("submit success");
                //提交后刷新一次dealview
            SetDealInfoPanel(InfoType.defaultType, HandleState.defalutState);
            });
            warningText.gameObject.SetActive(false);
            alertInfoDetailPanel.gameObject.SetActive(false);
        });
    }
    private string ReplaceMonitorsLocation(string monitorIpCpde){
        int index = -2;
        string str = monitorIpCpde;
        if (str.Contains("192."))
        {
            for (int i = 0; i < EvConst.monitorMediaUrls.Length; i++)
            {
                if (EvConst.monitorMediaUrls[i].Equals(monitorIpCpde))
                {
                    index = i;
                }
            }
            return EvConst.monitorNames[index];
        }
        else
        {
            string[] infostr = str.Split(' ');
            str = infostr[infostr.Length - 1];
            return str;
        }
    }
    private void Update() {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        if (obj == null)
            return;
            // Debug.Log(obj.name);
        if ("alertBtn".Equals(obj.transform.tag))
        {
            Transform testParent = obj.transform;
            // int index = dealInfoList.IndexOf(testParent);
            int index = testParent.GetSiblingIndex();
            // Debug.Log("当前transform处于第序列" + index);
            EvConst.currentClickId = index;
        }
            // Debug.Log(index);
    }
}
