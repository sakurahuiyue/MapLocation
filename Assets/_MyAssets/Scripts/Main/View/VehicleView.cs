using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TrackDrawer;
using static EvConst;

public class VehicleView : MonoBehaviour
{
    VehicleControl deviceCtrl;
    Transform mainLayout;
    Transform deviceInfoPanel;
    Transform deviceSpawner;
    Transform messagesPanelParent;
    Transform deviceTracks;
    Transform dangerousArea;
    Transform dangerousFlags;
    private void Awake()
    {
        deviceCtrl = FindObjectOfType<VehicleControl>();
        deviceSpawner = GameObject.Find("Spawners").transform.Find("DeviceSpawner");
        messagesPanelParent = GameObject.Find("Canvas").transform.Find("UIPanels/MessagePanels");
        deviceInfoPanel=messagesPanelParent.Find("DeviceInfo");
        deviceTracks=GameObject.Find("LineParent").transform.Find("DeviceTrackLines");
        dangerousArea = GameObject.Find("LineParent").transform.Find("DangerousAreas");
        dangerousFlags = GameObject.Find("Danger").transform.Find("DangerousFlags");
    }
    private void Start()
    {
        IniteDevicesInfoPanel(deviceInfoPanel.Find("LayoutParent"));
        // deviceInfoPanel.gameObject.SetActive(false);
        // SetDeviceClickListener(deviceInfoPanel);
        //绘制相关点
        // deviceCtrl.DrawDeviceLocationPos();
    }

    private void OnEnable()
    {
        dangerousArea.gameObject.SetActive(true);
        dangerousFlags.gameObject.SetActive(true);
        ModifyAllSpawersDetails(true);
        StartCoroutine(UpdateWorkerInfo());
    }

    private void OnDisable()
    {
        //隐藏所有worker点的信息，释放资源
        ModifyAllSpawersDetails(false);
        //删除轨迹
        if(deviceTracks!=null){
            foreach (Transform item in deviceTracks)
            {
                Destroy(item.gameObject);
            }
        }
    }
    //设置切换隐藏，释放资源
    private void ModifyAllSpawersDetails(bool _state)
    {
        if (deviceSpawner != null)
            deviceSpawner.gameObject.SetActive(_state);
    }
    IEnumerator UpdateWorkerInfo()
    {
        while (true)
        {
            //重新绘制
            deviceCtrl.RemoveDeviceLocation();
            yield return new WaitForSeconds(1.0f);
            deviceCtrl.DrawDeviceLocationPos();
            yield return new WaitForSeconds(2.0f);
            SetDeviceClickListener(deviceInfoPanel);
            yield return new WaitForSeconds(deviceWaitSeconds);
        }
    }
    //初始化worker信息显示界面
    private void IniteDevicesInfoPanel(Transform _layoutParent)
    {
        GameObject deviceInfoItem = Resources.Load("Prefabs/InfoItem") as GameObject;
        List<Transform> deviceInfoItemsList = new List<Transform>();
        List<string> contentName = new List<string> { "type", "deviceId", "operator"};
        List<string> contentText = new List<string> { "类型", "设备号", "操作者"};
        for (int i = 0; i < contentName.Count; i++)
        {
            GameObject go = Instantiate(deviceInfoItem, _layoutParent);
            go.transform.Find("Type").GetComponent<Text>().text = string.Format("{0}:", contentText[i]);
            go.transform.name = contentName[i];
            deviceInfoItemsList.Add(go.transform);
        }
    }

    void SetDeviceClickListener(Transform infoPanel)
    {
        infoPanel.gameObject.SetActive(false);
        Button infoCancelBtn = infoPanel.Find("CancelImg").GetComponent<Button>();
        infoCancelBtn.onClick.RemoveAllListeners();
        infoCancelBtn.onClick.AddListener(() =>
        {
            infoPanel.gameObject.SetActive(false);
        });
        List<Transform> deviceSpawnersList = new List<Transform>();
        foreach (Transform item in deviceSpawner)
        {
            deviceSpawnersList.Add(item);
        }
        Transform infoLayout = infoPanel.Find("LayoutParent");

        // //Test数据
        // for (int i = 0; i < deviceSpawnersList.Count; i++)
        // {
        //     int index = i;
        //     deviceSpawnersList[index].GetComponent<ObjectClick>().onclick.RemoveAllListeners();
        //     deviceSpawnersList[index].GetComponent<ObjectClick>().onclick.AddListener(() =>
        //     {
        //         infoPanel.gameObject.SetActive(true);
        //         //获取信息
        //         //显示信息，device信息
        //         infoLayout.Find("type/Content").GetComponent<Text>().text = "typetest";
        //         infoLayout.Find("deviceId/Content").GetComponent<Text>().text = "deviceId" + index;
        //         infoLayout.Find("operator/Content").GetComponent<Text>().text = "operator";
        //     });
        // }
        // //Test--------

        deviceCtrl.GetDeviceLocationInfo(deviceInfoList =>
        {
            for (int i = 0; i < deviceSpawnersList.Count; i++)
            {
                int index = i;
                deviceSpawnersList[index].name=deviceInfoList[index].deviceId;
                deviceSpawnersList[index].GetComponent<ObjectClick>().onclick.RemoveAllListeners();
                deviceSpawnersList[index].GetComponent<ObjectClick>().onclick.AddListener(() =>
                {
                    infoPanel.gameObject.SetActive(true);
                    //获取信息
                    //显示信息，员工信息
                    infoLayout.Find("type/Content").GetComponent<Text>().text = deviceInfoList[index].type;
                    infoLayout.Find("deviceId/Content").GetComponent<Text>().text = deviceInfoList[index].deviceId;
                    infoLayout.Find("operator/Content").GetComponent<Text>().text = deviceInfoList[index].@operator;
                    //轨迹绘制
                    SetDeviceTracks(deviceSpawnersList[index], deviceInfoPanel.Find("TrackBtn").GetComponent<Button>());
                });
            }
        });
    }

    /// <summary>
    /// 车辆轨迹绘制
    /// </summary>
    void SetDeviceTracks(Transform _deviceItem,Button _trackBtn)
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        string date = string.Format("{0}-{1:D2}-{2:D2}",year,month,day);
        //spawner生成go的name即deviceId
        string deviceId=_deviceItem.name;
        //路径轨迹的根节点
        Transform trackLineParent = FindObjectOfType<TrackDrawer>().transform.Find("DeviceTrackLines");
        _trackBtn.onClick.AddListener(() =>
        {
            //绘制轨迹
            TrackDrawer trackDrawer = FindObjectOfType<TrackDrawer>();
            deviceCtrl.GetVehicleInfoById(deviceId, deviceInfo =>
            {
                //根据gpsId和date获取位置点集
                deviceCtrl.GetGpsIdHistoryLocation(deviceId, date, realPosList =>
                {
                    trackDrawer.DrawVehicleTracks(realPosList, trackLineParent, TrackType.open, Color.blue);
                });
            });
        });
    }
}
