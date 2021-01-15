using Mathd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonitorControl : MonoBehaviour
{
    LocationSpawner spawner;

    Transform monitorSpawner;

    private void Awake()
    {
        spawner = FindObjectOfType<LocationSpawner>();

        monitorSpawner = GameObject.Find("Spawners").transform.Find("MonitorSpawner");
    }

    private void Start()
    {
        //DrawMonitorLocationPos();
    }

    /// <summary>
    /// 本地读取监控点位置信息，绘制监控位置点
    /// </summary>
    public void DrawMonitorLocationPos()
    {
        List<Vector3d> monitorPointList = spawner.GetLocationInfo("MonitorsPoints.txt");
        spawner.DrawLocationMaps(monitorPointList, monitorSpawner);
        List<Transform> monitors = new List<Transform>();
        foreach (Transform item in monitorSpawner)
        {
            monitors.Add(item);
        }
        //命名与监听控制
        for (int i = 0; i < monitors.Count; i++)
        {
            int index = i;
            monitors[i].name = string.Format("monitor_{0}", i);
        }
    }

    //获取当前监控查看的url
    public string GetCurrentMonitorMedia(string monitorId){
        //本地写入关联文件
        // http://192.168.151.3/dispatch.asp?user=admin&pass=0511@erhanghz&page=preview.asp[&slice=1&open={3}]
        string ipCode=string.Empty;
        //GetIpCode

        //TODO…
        //Test

        // return url;
        // return "www.baidu.com";
        // return "http://192.168.2.64/dispatch.asp?user=admin&pass=abc123456&page=preview.asp[&slice=1&open={3}]";
        return "http://"+EvConst.monitorMediaUrls[int.Parse(monitorId.Split('_')[1])]+"/dispatch.asp?user=admin&pass=erhanghz0511&page=preview.asp[&slice=1&open={3}]";
        // return "http://"+EvConst.monitorMediaUrls[int.Parse(monitorId.Split('_')[1])]+"/dispatch.asp?user=admin&pass=0511@erhanghz&page=preview.asp[&slice=1&open={3}]";
    }

    /// <summary>
    /// 移除相关点
    /// </summary>
    public void RemoveMonitorLocationPos()
    {
        if (monitorSpawner == null)
            return;
        foreach (Transform item in monitorSpawner)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
