using LitJson;
using Mathd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EvConst;

public class VehicleControl : MonoBehaviour
{
    LocationSpawner spawner;
    Transform deviceSpawner;

    private void Awake()
    {
        spawner = FindObjectOfType<LocationSpawner>();
        deviceSpawner = GameObject.Find("Spawners").transform.Find("DeviceSpawner");
    }

    //获取device位置信息
    public void GetDeviceLocationInfo(Action<List<DeviceInfo>> _action)
    {
        List<DeviceInfo> DeviceInfos = new List<DeviceInfo>();
        StartCoroutine(DataManager.GetInstance.GetAllDevicesPosInfo(requestText =>
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
                    DeviceInfo device = new DeviceInfo();
                    device.longitude = (string)data[i]["longitude"];
                    device.latitude = (string)data[i]["latitude"];
                    device.type = (string)data[i]["type"];
                    device.deviceId = (string)data[i]["deviceId"];
                    device.@operator = (string)data[i]["operator"];
                    DeviceInfos.Add(device);
                }
                _action(DeviceInfos);
            }
        }));
    }
    //绘制device位置信息
    public void DrawDeviceLocationPos()
    {
        List<Vector3d> locationPosList = new List<Vector3d>();
        if (deviceSpawner.childCount != 0)
            return;
        //无数据时测试用
        //Test
        // locationPosList = spawner.GetLocationInfo("points.txt");
        // spawner.DrawLocationMaps(locationPosList, deviceSpawner);
        //Test

        //
        GetDeviceLocationInfo(deviceInfoList =>
        {
            for (int i = 0; i < deviceInfoList.Count; i++)
            {
                double x = double.Parse(deviceInfoList[i].longitude);
                double y = double.Parse(deviceInfoList[i].latitude);
                locationPosList.Add(new Vector3d(x, y, 0));
            }
            spawner.DrawLocationMaps(locationPosList, deviceSpawner);
            for (int i = 0; i < deviceInfoList.Count; i++)
            {
                deviceSpawner.GetChild(i).name = deviceInfoList[i].deviceId;
                deviceSpawner.GetChild(i).GetComponent<Renderer>().material.color=Color.blue;
            }
        });
    }
    //清空所有点
    public void RemoveDeviceLocation()
    {
        if (deviceSpawner == null)
            return;
        if (deviceSpawner.childCount == 0)
            return;
        foreach (Transform item in deviceSpawner)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
    }
    //待修改
    public void GetVehicleInfoById(string vehicleId,Action<DeviceInfo> _action)
    {
        StartCoroutine(DataManager.GetInstance.GetDeviceInfoById(vehicleId,requestText=>
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
            //try
            
            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.deviceId=(string)jd["deviceId"];
            deviceInfo.gpsId=(string)jd["gpsId"];
            deviceInfo.@operator=(string)jd["operator"];
            _action(deviceInfo);
        }));
    }

    public void GetGpsIdHistoryLocation(string _gpsId,string _date,Action<List<Vector3d>> _action)
    {
             Debug.Log("getgps");
        StartCoroutine(DataManager.GetInstance.GetGpsIdHistoryLocation(_gpsId, _date, MyType.Device, requestText =>
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
             List<Vector3d> realPosList = new List<Vector3d>();
             for (int i = 0; i < jd.Count; i++)
             {
                 double x = double.Parse((string)jd[i]["longitude"]);
                 double y = double.Parse((string)jd[i]["latitude"]);
                 realPosList.Add(new Vector3d(x, y, 0));
             }
             _action(realPosList);
         }));
    }
}
public class DeviceInfo
{
    public string longitude { get; set; }
    public string latitude { get; set; }
    public string gpsId { get; set; }
    public string type { get; set; }
    public string deviceId { get; set; }
    public string @operator { get; set; }
}
