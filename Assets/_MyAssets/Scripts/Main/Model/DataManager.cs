using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using static EvConst;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance;
    private DataManager()
    {

    }
    //获取实例唯一入口
    public static DataManager GetInstance
    {
        get
        {
            if (_instance == null)
            {
                GameObject Geo = new GameObject("DataManager");
                _instance = Geo.AddComponent<DataManager>();
                DontDestroyOnLoad(Geo);//切换场景不销毁
            }
            return _instance;
        }
    }
    //请求url
    private string fixedUrl = EvConst.projectFixedUrl;

    public UserInfo user;

    private void Awake()
    {
        gameObject.AddComponent<UserInfo>();
        user = GetComponent<UserInfo>();
        user.username = string.Empty;
        user.password = string.Empty;
    }

    // 获取实时天气数据
    public IEnumerator GetWeatherData(Action<string> _action)
    {
        string weaid = EvConst.City.hangzhou.ToString();
        string appkey = EvConst.appkey;
        string sign = EvConst.sign;
        string type = EvConst.type;
        string url = string.Format("http://api.k780.com/?app=weather.today&weaid={0}&appkey={1}&sign={2}&format={3}", weaid, appkey, sign, type);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
            if (!webRequest.downloadHandler.text.Contains("异常"))
                _action(webRequest.downloadHandler.text);
        }
    }

    // 网络下载文件
    public IEnumerator DownloadFile(string _url, Action<Texture2D> _action)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(_url); //创建UnityWebRequest对象，将Url传入
        DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture(true);
        webRequest.downloadHandler = downloadHandlerTexture;
        yield return webRequest.SendWebRequest();                                                                                //开始请求
        if (webRequest.isNetworkError || webRequest.isHttpError)                                                             //如果出错
        {
            Debug.Log(webRequest.error); //输出 错误信息
        }
        else
        {
            while (!webRequest.isDone) //只要下载没有完成，一直执行此循环
            {
                //ProgressBar.value = uwr.downloadProgress; //展示下载进度
                //SliderValue.text = Math.Floor(uwr.downloadProgress * 100) + "%";
                yield return 0;
            }

            if (webRequest.isDone) //如果下载完成了
            {
                //print("完成");
                //var tex2d = new Texture2D(128, 128);
                //tex2d= downloadHandlerTexture.texture;
                //Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, 128, 128), Vector2.zero);
                Texture2D texture = downloadHandlerTexture.texture;
                //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _action(texture);
            }
        }
    }

    // 用户登录
    public IEnumerator Login(string _username,string _password,Action<string> _action)
    {
        string variableUrl = EvConst.loginUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("username", _username);
        form.AddField("password", _password);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log(string.Format("{0};{1}",_username,_password));
            Debug.Log("LoginInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }
    // 上传用户监控选择设置
    public IEnumerator UploadMonitorSettings(string _username, string _settingStr, Action<string> _action)
    {
        string variableUrl = EvConst.selectUserMonsUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("username", _username);
        form.AddField("monitorInfo", _settingStr);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            _action(webRequest.downloadHandler.text);
        }
    }
    // 修改密码
    public IEnumerator ModifyUserPassword(string _username, string _newPassword, Action<string> _action)
    {
        string variableUrl = EvConst.resetPasswordUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("username", _username);
        form.AddField("password", _newPassword);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("Modify Sucess" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    // 根据工人编号查询工人信息
    public IEnumerator GetWorkerDetail(string _workerId, Action<string> _action)
    {
        string variableUrl = EvConst.getWorkerDetaildUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("workerId", _workerId);
        Debug.Log(string.Format("上传的值为{0}", _workerId));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetWorkerDetail" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    // 根据设备编号查询车辆信息
    public IEnumerator GetDeviceDetail(string _deviceId, Action _action)
    {
        string variableUrl = EvConst.getDeviceDetaildUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("workerId", _deviceId);
        Debug.Log(string.Format("上传的值为{0}", _deviceId));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetVehicleDetail" + webRequest.downloadHandler.text);
            _action();
        }
    }

    // 工人工种分类信息
    public IEnumerator GetWorkerStatistics(Action<string> _action)
    {
        string variableUrl = EvConst.getWorkerStatisticsUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        //form.AddField("workerId", _vehicleId);
        //Debug.Log(string.Format("上传的值为{0}", _vehicleId));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetWorkerStatistics" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    //获取所有worker信息
    public IEnumerator GetWorkersList(Action<string> _action)
    {
        string variableUrl = EvConst.getWorkerStatisticsUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            _action(webRequest.downloadHandler.text);
        }
    }
    // 统计设备信息
    public IEnumerator GetDeviceStatistics(Action<string> _action)
    {
        string variableUrl = EvConst.getDeviceStatisticsUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        //form.AddField("workerId", _vehicleId);
        //Debug.Log(string.Format("上传的值为{0}", _vehicleId));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetDeviceStatistics" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    // 获取报警信息
    public IEnumerator GetAlertInfo(int _limit, int _page, InfoType _type,HandleState  _state, Action<string> _action)
    {
        string variableUrl = EvConst.getAlertDetailUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        if (_type != InfoType.defaultType)
        {
            form.AddField("type", Enum.GetName(typeof(InfoType), _type));
        }
        if (_state != HandleState.defalutState)
        {
            form.AddField("state", Enum.GetName(typeof(HandleState), _state));
        }
        form.AddField("limit", _limit.ToString());
        form.AddField("page", _page.ToString());
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            // Debug.Log("提交的参数type:" + _type + "state:" + _state);
            Debug.Log("GetAlertInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    public IEnumerator DealAlertInfo(string _id, string _remark,string _username, Action<string> _action)
    {
        string variableUrl = EvConst.handleAlertUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("remark", _remark);
        form.AddField("username", _username);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        Debug.Log(string.Format("submitItems,id:{0},remark:{1},username:{2}",_id,_remark,_username));
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("DealAlertInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }
    
    // 获取工人实时位置信息
    public IEnumerator GetAllWorkersPosInfo(Action<string> _action)
    {
        string variableUrl = EvConst.getWorkersPosUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        //form.AddField("type", _type);
        //form.AddField("state", _state);
        //Debug.Log(string.Format("上传的值为{0}---{1}", _type, _state));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetAllWorkersInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    //获取设备实时位置信息
    public IEnumerator GetAllDevicesPosInfo(Action<string> _action)
    {
        string variableUrl = EvConst.getDevicesPosUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        //form.AddField("type", _type);
        //form.AddField("state", _state);
        //Debug.Log(string.Format("上传的值为{0}---{1}", _type, _state));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetAllDevicesPosInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    // 根据设备id获取设备信息
    public IEnumerator GetDeviceInfoById(string _deviceId,Action<string> _action)
    {
        string variableUrl = EvConst.getDeviceDetaildUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("deviceId", _deviceId);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetDeviceInfoById" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    // 根据Id获取历史位置信息
    public IEnumerator GetGpsIdHistoryLocation(string _gpsId,string _date,MyType _type, Action<string> _action)
    {
        string variableUrl = EvConst.getHistoryLocationUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("id", _gpsId);
        form.AddField("date", _date);
        form.AddField("type", (int)_type);
        Debug.Log(string.Format("上传的信息：{0},{1},{2}",_gpsId,_date,(int)_type));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetGpsIdHistoryLocation" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }
    
    // 根据区域编号和日期获取区域进出场信息
    public IEnumerator GetRecordInfo(string _areaId, string _date, Action<string> _action)
    {
        string variableUrl = EvConst.getEnterRecordUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("area", _areaId);
        form.AddField("date", _date);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetRecordInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }

    // 获取历史警报信息
    public IEnumerator GetHistoryAlertInfo(string _type, string _date, string _username, string _page,
    string _limit, Action<string> _action)
    {
        string variableUrl = EvConst.queryAlertUrl;
        string url = fixedUrl + variableUrl;
        WWWForm form = new WWWForm();
        form.AddField("type", _type);
        form.AddField("date", _date);
        form.AddField("username", _username);
        form.AddField("page",_page);
        form.AddField("limit",_limit);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetRecordInfo" + webRequest.downloadHandler.text);
            _action(webRequest.downloadHandler.text);
        }
    }
}
