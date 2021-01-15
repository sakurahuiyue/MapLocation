
using System;
using System.IO;
using DG.Tweening;
using UnityEngine;

public static class EvConst
{
    // // request action url
    // public const string projectFixedUrl="http://localhost:8888/ArtificialRiver/";
    public const string projectFixedUrl="http://211.149.200.157:8083/ArtificialRiver/";
    // public const string projectFixedUrl="http://192.168.2.113:8081/ArtificialRiver/";
    // user
    public const string loginUrl="user/login";
    public const string selectUserMonsUrl="user/setMonitor";
    public const string resetPasswordUrl="user/setpwd";
    // worker
    public const string getWorkerDetaildUrl="worker/getDetail";
    public const string getWorkerStatisticsUrl="statistics/getWorkers";
    public const string getWorkersListUrl="worker/list";
    // device
    public const string getDeviceDetaildUrl="device/getDetail";
    public const string getDeviceStatisticsUrl="statistics/getDevices";
    // alert
    public const string getAlertDetailUrl="alert/getDetail";
    public const string handleAlertUrl="alert/handle";
    public const string queryAlertUrl="alert/query";
    // position
    public const string getWorkersPosUrl="position/getWorkers";
    public const string getDevicesPosUrl="position/getDevices";
    public const string getHistoryLocationUrl="position/getHistoryLocation";
    // record
    public const string getEnterRecordUrl="inOutRecord/getDetail";
    // get weather info
    public enum City{
        zhenjiang,
        hangzhou
    }
    public const string appkey="53807";
    public const string sign="da758d582b3cc05f58233c3b41ab92ca";
    public const string type="json";
    public const string weatherRequestUrl="http://api.k780.com/?app=weather.today&weaid={0}&appkey={1}&sign={2}&format={3}";

    public static int currentClickId = 1;
    //homeview
    //alert刷新间隔(secomds)
    public const float alertWaitSeconds = 60.0f;
    //警报信息查询类型
    public enum InfoType
    {
        fireDisaster = 1,
        wearMistake = 2,
        dangerousArea = 3,
        defaultType = 0
    }
    //处理状态
    public enum HandleState
    {
        finished = 0,
        unfinished = 1,
        defalutState
    }

    //monitorsNames
    public static string monitorSequence=string.Empty;
    public static string[] monitorMediaUrls = { "192.168.150.3", "192.168.151.3", "192.168.155.3", "192.168.152.3", "192.168.153.3", "192.168.154.3" };
    public static string[] monitorNames
    ={"①浙宝城","②胜稼村","③绕城高速","④三厂","⑤吴家村老年公寓","⑥老01桥"};
    public enum MyType
    {
        Worker = 0,
        Device = 1
    }
    //worker
    public const float workerWaitSeconds = 60.0f;
    //device
    public const float deviceWaitSeconds = 60.0f;
    //settings
    public const int pageLimit = 12;
    //读取本地图片
    public static Sprite GetImageSprite(string _localPath)
    {
        if (_localPath == null)
            return null;
        try
        {
            //创建文件读取流
            FileStream fileStream = new FileStream(_localPath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);

            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];

            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);

            //释放文件读取流
            fileStream.Close();

            fileStream.Dispose();
            fileStream = null;
            //创建Texture
        int width = 45;
        int height = 55;

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        //创建Sprite--把Texture转成sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
        }
        catch (Exception)
        {
            Debug.Log("未找到目录");
            return null;
        }
    }
    
    //世界坐标转换为屏幕坐标
    public static Vector2 GetScreenPosition(Transform orginObj,Transform canvas)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(orginObj.position);
        // Canvas canvas = GameObject.Find("View").transform.Find("Canvas").GetComponent<Canvas>();
        RectTransform canvasRtm = canvas.GetComponent<RectTransform>();
        Vector2 uguiPos = Vector2.zero;
        uguiPos.x = screenPos.x * canvasRtm.sizeDelta.x / Screen.width;
        uguiPos.y = screenPos.y * canvasRtm.sizeDelta.y / Screen.height;
        return uguiPos;
    }
    //镜头拉近设置
    public static void MoveCamera(Transform camera, Transform obj, float time)
    {
        Vector3 orginPos = camera.position;
        Vector3 objPos = obj.position;
        Vector3 eulerangle = camera.eulerAngles;
        float radio = 0.5f;
        Vector3 targetPos = new Vector3((orginPos.x + objPos.x) * 0.4f, (orginPos.y + objPos.y) * radio, (orginPos.z + objPos.z) * 0.3f);
        Vector3 targetRotation=new Vector3(eulerangle.x*radio,eulerangle.y*radio,eulerangle.z*radio);
        Debug.Log(orginPos+"----"+targetPos);
        camera.DOMove(targetPos, 1);
        // camera.DORotate(targetRotation, 1);
    }
}
