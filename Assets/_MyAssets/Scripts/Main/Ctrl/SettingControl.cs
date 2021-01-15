using LitJson;
using Mathd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UMP;
using UnityEngine;
using static TrackDrawer;

public class SettingControl : MonoBehaviour
{
    SettingView settingView;
    UIManager uimanager;
    LocationSpawner spawner;
    TrackDrawer drawer;

    private void Awake()
    {
        settingView = FindObjectOfType<SettingView>();
        uimanager = FindObjectOfType<UIManager>();
        spawner = FindObjectOfType<LocationSpawner>();
        drawer = FindObjectOfType<TrackDrawer>();
    }
    private void Start() {
        //初始阶段完成危险区绘制
        DrawDangerousAreas();
    }
    //提交密码
    public void SubmitPassword(string _userName,string _password,Action _action)
    {
        StartCoroutine(DataManager.GetInstance.ModifyUserPassword(_userName, _password, requestText =>
          {
              JsonData jsonData = new JsonData();
              try
              {
                  jsonData = JsonMapper.ToObject(requestText);
              }
              catch (JsonException)
              {
                  Debug.Log("Json解析异常");
              }
              if (((IDictionary)jsonData).Contains("code"))
              {
                  int code = (int)jsonData["code"];
                  if (200.Equals(code))
                      _action();
              }
              else
              {
                  Debug.Log("error");
              }
          }));
    }
    //更新监控选择
    public void UpdateMonitorSettings(string _settingStr,Action _action){
        string _username=DataManager.GetInstance.user.username;
        StartCoroutine(DataManager.GetInstance.UploadMonitorSettings(_username,_settingStr,requestText=>{
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (((IDictionary)jsonData).Contains("code"))
            {
                int code = (int)jsonData["code"];
                if (200.Equals(code))
                    _action();
            }
        }));
    }
    //根据编号查工人信息
    public void GetWorkerInfoById(string _workerId, Action<WorkerInfo> _action)
    {
        StartCoroutine(DataManager.GetInstance.GetWorkerDetail(_workerId, requestText =>
        {
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (((IDictionary)jsonData).Contains("data"))
            {
                WorkerInfo workerInfo=new WorkerInfo();
                workerInfo.workerId=(string)jsonData["data"]["workerId"];
                workerInfo.name=(string)jsonData["data"]["name"];
                workerInfo.type=(string)jsonData["data"]["type"];
                workerInfo.age=(string)jsonData["data"]["age"];
                workerInfo.image=(string)jsonData["data"]["image"];
                _action(workerInfo);
            }
        }));
    }
    //根据编号查设备信息
    public void GetDeviceInfoById(string _deviceId, Action<DeviceInfo> _action)
    {
        StartCoroutine(DataManager.GetInstance.GetWorkerDetail(_deviceId, requestText =>
        {
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (((IDictionary)jsonData).Contains("data"))
            {
                DeviceInfo deviceInfo=new DeviceInfo();
                deviceInfo.gpsId=(string)jsonData["data"]["gpsId"];
                deviceInfo.@operator=(string)jsonData["data"]["operator"];
                deviceInfo.type=(string)jsonData["data"]["type"];
                deviceInfo.deviceId=(string)jsonData["data"]["deviceId"];
                _action(deviceInfo);
            }
        }));
    }

    public void GetWorkerInTargetArea(string _areaId, string _date,Action<List<WorkerInAreaInfo>> _action)
    {
        StartCoroutine(DataManager.GetInstance.GetRecordInfo(_areaId, _date, requestText =>
        {
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (((IDictionary)jsonData).Contains("data"))
            {
                JsonData jd=jsonData["data"];
                List<WorkerInAreaInfo> workerInAreaInfos=new List<WorkerInAreaInfo>();
                for (int i = 0; i < jd.Count; i++)
                {
                    WorkerInAreaInfo workerInAreaInfo = new WorkerInAreaInfo();
                    workerInAreaInfo.id=(i+1).ToString();
                    workerInAreaInfo.date=(string)jd[i]["date"];
                    workerInAreaInfo.type=(string)jd[i]["type"];
                    workerInAreaInfo.direction=(string)jd[i]["area"];
                    workerInAreaInfo.name=(string)jd[i]["operator"];
                    workerInAreaInfo.workerId=(string)jd[i]["deviceId"];
                    workerInAreaInfos.Add(workerInAreaInfo);
                }
                _action(workerInAreaInfos);
            }
        }));
    }

    // 获取历史警报信息
    public void QueryHistoryAlertInfo(string _type, string _date, string _username, string _page, Action<List<HistoryAlertInfo>> _action)
    {
        Debug.Log("gethistoryinfo");
        StartCoroutine(DataManager.GetInstance.GetHistoryAlertInfo(_type, _date, _username, _page, EvConst.pageLimit.ToString(), requestText =>
        {
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (((IDictionary)jsonData).Contains("data"))
            {
                JsonData data = jsonData["data"];
                if (((IDictionary)data).Contains("list"))
                {
                    JsonData jd = data["list"];
                    Debug.Log(jd.Count);
                    //转化为对象
                    List<HistoryAlertInfo> historyAlertInfos = new List<HistoryAlertInfo>();
                    for (int i = 0; i < jd.Count; i++)
                    {
                        HistoryAlertInfo historyAlertInfo = new HistoryAlertInfo();
                        historyAlertInfo.id = (int)jd[i]["id"];
                        historyAlertInfo.title = (string)jd[i]["title"];
                        historyAlertInfo.type = (string)jd[i]["type"];
                        historyAlertInfo.state = (string)jd[i]["state"];
                        historyAlertInfo.date = (string)jd[i]["date"];
                        historyAlertInfo.detail = (string)jd[i]["detail"];
                        historyAlertInfo.remark = (string)jd[i]["remark"];
                        historyAlertInfo.username = (string)jd[i]["username"];
                        historyAlertInfos.Add(historyAlertInfo);
                    }
                    _action(historyAlertInfos);
                }

            }
        }));
    }
    //查询当前页是否是最后一页
    public void QueryCurrentPageState(string _type, string _date, string _username, string _page,Action<bool> _action){
        StartCoroutine(DataManager.GetInstance.GetHistoryAlertInfo(_type, _date, _username, _page, EvConst.pageLimit.ToString(), requestText =>
        {
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (!((IDictionary)jsonData).Contains("data"))
            {
                Debug.Log("pagedata");
                JsonData data = jsonData["data"];
                if (!((IDictionary)data).Contains("isLastPage")){
                    bool isLastPage = (bool)data["isLastPage"];
                    _action(isLastPage);
                }
            }
        }));
    }
    //人员查找,返回经纬坐标
    public Vector3d GetWorkerPosition(string workerNum)
    {
        StartCoroutine(DataManager.GetInstance.GetWorkerDetail(workerNum, str =>
        {
            Debug.Log("获取人员坐标------TODO");
        }));
        return Vector3d.zero;
    }
    //危险区控制,返回危险区域4个点经纬坐标
    public void DrawDangerousAreas()
    {
        Transform areaParent = FindObjectOfType<TrackDrawer>().transform.Find("DangerousAreas");
        string[] dataArr = ReadFileInfo("trackarea.txt");
        //string[] dataArr = ReadFileInfo("testtrack.txt");
        //foreach (var item in dataArr)
        //{
        //    Debug.Log(item + "-----");
        //}
        for (int i = 0; i < dataArr.Length; i++)
        {
            List<Vector3d> areaList = GetLocationInfo(dataArr[i]);
            //List<Vector3d> areaList = GetLocationInfo("testtrack.txt");
            //绘制危险区边界线
            if (areaList.Count == 2)
                drawer.DrawVehicleTracks(areaList, areaParent, TrackType.open,Color.red);
            else
                drawer.DrawVehicleTracks(areaList, areaParent, TrackType.close, Color.red);
            //Test
            //绘制危险区边界点,便于查看
            // spawner.DrawLocationMaps(areaList, areaParent);
        }
        //为方便查看效果，隐藏其中的点的相关位置
        // Debug.Log(areaParent.childCount);
        foreach (Transform child in areaParent){
            if ("SphereTest(Clone)".Equals(child.name))
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 读取本地危险区的真实坐标
    /// </summary>
    /// <param name="_fileName"></param>
    /// <returns></returns>
    private List<Vector3d> GetLocationInfo(string str)
    {
        List<Vector3d> pointList = new List<Vector3d>();
        string[] tempArr = str.Split('|');
        foreach (var item in tempArr)
        {
            pointList.Add(Vector3d.Parse(item));
            //Debug.Log("----------------------"+item);
        }
        return pointList;
    }
    //查询工人与设备信息
    private string[] ReadFileInfo(string _fileName)
    {
        var fileAddress = Path.Combine(Application.streamingAssetsPath, _fileName);
        FileInfo fInfo0 = new FileInfo(fileAddress);
        string s = string.Empty;
        if (fInfo0.Exists)
        {
            StreamReader r = new StreamReader(fileAddress);
            s = r.ReadToEnd();
            string[] strText = s.Split('#');
            return strText;
        }
        else
            return null;
    }
    public void ModifyHomeMonitorSequences(List<int> sequenceArr){
        Transform mediaParent = GameObject.Find("MediaPlayers").transform;
        //192.168.2.64
        for (int i = 0; i < sequenceArr.Count; i++)
        {
            mediaParent.GetChild(i).GetComponent<UniversalMediaPlayer>().Path
            = string.Format("rtsp://admin:erhanghz0511@{0}/cam/realmonitor?channel=1&subtype=0",EvConst.monitorMediaUrls[sequenceArr[i]]);
        }
        mediaParent.GetChild(0).GetComponent<UniversalMediaPlayer>().Play();
        mediaParent.GetChild(1).GetComponent<UniversalMediaPlayer>().Play();
        // mediaParent.GetChild(1).GetComponent<UniversalMediaPlayer>().Path
        // = string.Format("rtsp://admin:abc123456@{0}/cam/realmonitor?channel=1&subtype=0",EvConst.monitorMediaUrls[sequenceArr[1]]);
    }
}

public class WorkerInAreaInfo{
    public string id{get;set;}
    public string date{get;set;}
    public string type{get;set;}
    public string direction{get;set;}
    public string name{get;set;}
    public string workerId{get;set;}
}

public class HistoryAlertInfo{
    public int id{get;set;}
    public string title{get;set;}
    public string type{get;set;}
    public string state{get;set;}
    public string date{get;set;}
    public string detail{get;set;}
    public string remark{get;set;}
    public string username{get;set;}
    public string page{get;set;}
}