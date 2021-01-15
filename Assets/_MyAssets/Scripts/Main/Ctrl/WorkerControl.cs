using LitJson;
using Mathd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerControl : MonoBehaviour
{
    LocationSpawner spawner;
    Transform workerSpawner;

    private void Awake()
    {
        spawner = FindObjectOfType<LocationSpawner>();
        workerSpawner= GameObject.Find("Spawners").transform.Find("WorkerSpawner");
    } 
    
    //获取worker位置信息
    public void GetWorkerLocationInfo(Action<List<WorkerInfo>> _action)
    {
        List<WorkerInfo> workerInfos = new List<WorkerInfo>();
        StartCoroutine(DataManager.GetInstance.GetAllWorkersPosInfo(requestText =>
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
                for (int i = 0; i < data.Count; i++)
                {
                    WorkerInfo worker = new WorkerInfo();
                    worker.longitude=(string)data[i]["longitude"];
                    worker.latitude=(string)data[i]["latitude"];
                    worker.name = (string)data[i]["name"];
                    worker.type = (string)data[i]["type"];
                    worker.age = (string)data[i]["age"];
                    worker.workerId = (string)data[i]["workerId"];
                    worker.image=(string)data[i]["image"];
                    workerInfos.Add(worker);
                }
                _action(workerInfos);
            }
        }));
    }

    /// <summary>
    /// 绘制worke位置
    /// </summary>
    /// <param name="_locationPosList"></param>
    /// <param name="_action"></param>
    public void DrawWorkerLocationPos()
    {
        List<Vector3d> locationPosList = new List<Vector3d>();
        if (workerSpawner.childCount != 0)
            return;
        //无数据时测试用
        //Test
        // locationPosList = spawner.GetLocationInfo("points.txt");
        // spawner.DrawLocationMaps(locationPosList, workerSpawner);
        //Test

        //分解worker信息,分离出worker位置信息，后期可能还需要分离workerid
        GetWorkerLocationInfo(workerInfoList =>
        {
            for (int i = 0; i < workerInfoList.Count; i++)
            {
                double x = double.Parse(workerInfoList[i].longitude);
                double y = double.Parse(workerInfoList[i].latitude);
                locationPosList.Add(new Vector3d(x, y, 0));
            }
            spawner.DrawLocationMaps(locationPosList, workerSpawner);
            for (int i = 0; i < workerInfoList.Count; i++){
                workerSpawner.GetChild(i).name = workerInfoList[i].workerId;
                workerSpawner.GetChild(i).GetComponent<Renderer>().material.color = Color.red;
            }
        });
    }

    /// <summary>
    /// 清空所有点
    /// </summary>
    public void RemoveWorkerLocation()
    {
        if (workerSpawner == null)
            return;
        if (workerSpawner.childCount == 0)
            return;
        foreach (Transform item in workerSpawner)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
/// <summary>
/// worker信息实体类
/// </summary>
public class WorkerInfo
{
    public string longitude { get; set; }
    public string latitude { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string age { get; set; }
    public string workerId { get; set; }
    public string image { get; set; }
    public string date { get; set; }
}

