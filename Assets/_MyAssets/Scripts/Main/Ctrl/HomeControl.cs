using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static EvConst;

public class HomeControl : MonoBehaviour
{
    private string path;
    private void Awake()
    {
        path = Application.dataPath + "\\test.txt";
    }
    private void Start()
    {
        Debug.Log("reload 本场景内容--------");
    }

    /// <summary>
    /// 获取项目开始日期,转化为天数
    /// </summary>
    /// <param name="action"></param>
    public int SetDayNum()
    {
        var fileAddress = Path.Combine(Application.streamingAssetsPath, "BaseInfo.txt");
        FileInfo fInfo0 = new FileInfo(fileAddress);
        string dateJson = string.Empty;
        if (fInfo0.Exists)
        {
            StreamReader r = new StreamReader(fileAddress);
            //StreamReader默认的是UTF8的不需要转格式了，因为有些中文字符的需要有些是要转的，下面是转成String代码
            //byte[] data = new byte[1024];
            // data = Encoding.UTF8.GetBytes(r.ReadToEnd());
            // s = Encoding.UTF8.GetString(data, 0, data.Length);
            dateJson = r.ReadToEnd();
        }
        JsonData jsonData = new JsonData();
        try
        {
            jsonData = JsonMapper.ToObject(dateJson);
        }
        catch (JsonException)
        {
            Debug.Log("Json解析异常");
        }
        if (!((IDictionary)jsonData).Contains("startDate"))
            return -1;
        string tempdate = (string)jsonData["startDate"];
        //string tempdate = "2020-8-2";
        //简单判断是否为日期格式
        try
        {
            Convert.ToDateTime(tempdate);
        }
        catch (FormatException)
        {
            Debug.Log("日期输入异常");
            return -1;
        }
        //2019-09-30
        DateTime dt1 = Convert.ToDateTime(tempdate);
        ////Test
        //DateTime dt1 = Convert.ToDateTime("2020-8-5");
        DateTime dt2 = DateTime.Now;
        return Convert.ToInt32((dt2 - dt1).TotalDays) - 1;
    }

    /// <summary>
    /// 获取当前日期
    /// </summary>
    /// <returns></returns>
    public string GetCurrentDate()
    {
        return string.Format("{0}年{1}月{2}日", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }

    /// <summary>
    /// 获取当前天气信息
    /// </summary>
    /// <param name="_action"></param>
    public void GetCurrentWeather(Action<string,string> _action)
    {
        StartCoroutine(DataManager.GetInstance.GetWeatherData(requestText =>
        {
            JsonData jsonData = new JsonData();
            jsonData = JsonMapper.ToObject(requestText);
            string temperature = (string)jsonData["result"]["temperature"];
            string weather = (string)jsonData["result"]["weather"];
            string wind = (string)jsonData["result"]["wind"];
            string windp = (string)jsonData["result"]["winp"];
            string weatherData = string.Format("{0}  {1}", weather, temperature);
            string windData = string.Format("{0}  {1}", wind, windp);
            _action(weatherData, windData);
        }));
    }

    /// <summary>
    /// 获取当前工人信息
    /// </summary>
    /// <param name="_action"></param>
    public void GetWorkerTotalInfo(Action<Dictionary<string,int>> _action)
    {
        Dictionary<string, int> workerDic = new Dictionary<string, int>();
        StartCoroutine(DataManager.GetInstance.GetWorkerStatistics((requestText) =>
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
                return;
            JsonData jd = jsonData["data"];
            int totalCount = jd.Count;
            for (int i = 0; i < totalCount; i++)
            {
                workerDic.Add((string)jd[i]["type"], int.Parse((string)jd[i]["count"]));
            }
            _action(workerDic);
        }));
    }

    /// <summary>
    /// 获取当前车辆信息
    /// </summary>
    /// <param name="_action"></param>
    public void GetVehicleTotalInfo(Action<Dictionary<string, int>> _action)
    {
        Dictionary<string, int> vehicleDic = new Dictionary<string, int>();
        StartCoroutine(DataManager.GetInstance.GetDeviceStatistics((requestText) =>
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
                return;
            JsonData jd = jsonData["data"];
            int totalCount = jd.Count;
            for (int i = 0; i < totalCount; i++)
            {
                vehicleDic.Add((string)jd[i]["type"], int.Parse((string)jd[i]["count"]));
            }
            _action(vehicleDic);
        }));
    }
    //获取当前最大页数
    public void GetMaxPageContent(int _limit,int _page, InfoType _type,HandleState _state,Action<int> _action){
        int maxPage=0;
        StartCoroutine(DataManager.GetInstance.GetAlertInfo(_limit,_page,_type, _state, requestText =>
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
            JsonData data = jsonData["data"];
            maxPage = (int)data["pages"];
            _action(maxPage);
        }));
    }
    //来自请求的警报信息
    public void GetAlertInfo(int _limit,int _page, InfoType _type,HandleState _state, Action<List<AlertInfo>> _action)
    {
        StartCoroutine(DataManager.GetInstance.GetAlertInfo(_limit,_page,_type, _state, requestText =>
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
                return;
            if (!((IDictionary)jsonData["data"]).Contains("list"))
                return;
            JsonData data = jsonData["data"];
            JsonData listItems = data["list"];
            // string infolist=(string)jsonData["data"]["list"];
            // //转换为实体对象
            List<AlertInfo> alerInfoList = new List<AlertInfo>();
            try
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    AlertInfo alertInfoItem = new AlertInfo();
                    alertInfoItem.id = (int)listItems[i]["id"];
                    alertInfoItem.title = (string)listItems[i]["title"];
                    alertInfoItem.image = (string)listItems[i]["image"];
                    alertInfoItem.type = (string)listItems[i]["type"];
                    alertInfoItem.state = (string)listItems[i]["state"];
                    alertInfoItem.date = (string)listItems[i]["date"];
                    alertInfoItem.detail = (string)listItems[i]["detail"];
                    alertInfoItem.remark = (string)listItems[i]["remark"];
                    alertInfoItem.username = (string)listItems[i]["username"];
                    alerInfoList.Add(alertInfoItem);
                }
            }
            catch (NullReferenceException)
            {
                Debug.Log("Json解析异常");
            }

            //Test
            foreach (var item in alerInfoList)
            {
                Debug.Log(string.Format("title:{0},date:{1},state:{2},imge:{3},detail:{4}",
                    item.title, item.date, item.state, item.image, item.detail));
            }
            _action(alerInfoList);
        }));
    }

    public void DealAlertInfo(string _id,string _remark,Action _action){
        string _username=DataManager.GetInstance.user.username;
        StartCoroutine(DataManager.GetInstance.DealAlertInfo(_id,_remark,_username,requestText=>{
            JsonData jsonData = new JsonData();
            try
            {
                jsonData = JsonMapper.ToObject(requestText);
            }
            catch (JsonException)
            {
                Debug.Log("Json解析异常");
            }
            if (!((IDictionary)jsonData).Contains("code"))
                return;
            int code = (int)jsonData["code"];
            if ("200".Equals(code))
            {
                _action();
            }
        }));
    }
    
    // public void GetLocalAlertInfo(string _type,string _state,Action _action)
    // {
    //     StartCoroutine(DataManager.GetInstance.GetAlertInfo(_type, _state, requestText =>
    //     {
    //         JsonData jsonData = new JsonData();
    //         try
    //         {
    //             jsonData = JsonMapper.ToObject(requestText);
    //         }
    //         catch (JsonException)
    //         {
    //             Debug.Log("Json解析异常");
    //         }
    //         if (!((IDictionary)jsonData).Contains("data"))
    //             return;
    //         if (!((IDictionary)jsonData["data"]).Contains("list"))
    //             return;
    //         //转换为实体对象
    //         List<AlertInfo> alerInfoList = JsonMapper.ToObject<List<AlertInfo>>(localJsondata);
    //         //Test
    //         foreach (var item in alerInfoList)
    //         {
    //             Debug.Log(string.Format("title:{0},date:{1},state:{2},imge:{3},detail:{4}",
    //                 item.title, item.date, item.state, item.image, item.detail));
    //         }
    //     }));
    // }


    /// <summary>
    /// 根据url下载图片
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_action"></param>
    public void GetImageSprite(string _url,Action<Sprite> _action)
    {
        StartCoroutine(DataManager.GetInstance.DownloadFile(_url, (Texture2D texture) =>
         {
             Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
             _action(sprite);
         }));
    }
}
