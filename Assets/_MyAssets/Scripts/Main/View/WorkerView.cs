using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EvConst;

public class WorkerView : MonoBehaviour
{
    WorkerControl workerCtrl;
    HomeControl homeCtrl;
    UIManager uiManager;

    Transform messagesPanelParent;
    Transform workerSpawner;
    Transform workerInfoPanel;
    Transform dangerousArea;
    Transform dangerousFlags;
    private void Awake()
    {
        workerCtrl = FindObjectOfType<WorkerControl>();
        homeCtrl=FindObjectOfType<HomeControl>();
        uiManager = FindObjectOfType<UIManager>();

        messagesPanelParent = GameObject.Find("Canvas").transform.Find("UIPanels/MessagePanels");
        workerSpawner = GameObject.Find("Spawners").transform.Find("WorkerSpawner");
        workerInfoPanel = messagesPanelParent.Find("WorkerInfo");
        dangerousArea = GameObject.Find("LineParent").transform.Find("DangerousAreas");
        dangerousFlags = GameObject.Find("Danger").transform.Find("DangerousFlags");
    }
    private void Start()
    {
        IniteWorkersInfoPanel(workerInfoPanel.Find("LayoutParent"));
        //绘制相关点
        workerCtrl.DrawWorkerLocationPos();
        // //初始状态危险区域隐藏
        // dangerousArea.gameObject.SetActive(false);
        // dangerousFlags.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //危险区显示
        dangerousArea.gameObject.SetActive(true);
        dangerousFlags.gameObject.SetActive(true);
        //
        ModifyAllSpawersDetails(true);
        Debug.Log("enable");
        // workerCtrl.DrawWorkerLocationPos();
        // SetWorkerClickListener(workerInfoPanel);
        StartCoroutine(UpdateWorkerInfo());
    }

    private void OnDisable()
    {
        //危险区隐藏
        if (dangerousArea != null)
            dangerousArea.gameObject.SetActive(false);
        if (dangerousFlags != null)
            dangerousFlags.gameObject.SetActive(false);
        //隐藏所有worker点的信息，释放资源
        ModifyAllSpawersDetails(false);
        Debug.Log("disable");
    }
    //设置切换隐藏，释放资源
    private void ModifyAllSpawersDetails(bool _state)
    {
        if(workerSpawner!=null)
            workerSpawner.gameObject.SetActive(_state);
    }
    IEnumerator UpdateWorkerInfo()
    {
        while (true)
        {
            Debug.Log("reload current page");
            //重新绘制
            workerCtrl.RemoveWorkerLocation();
            yield return new WaitForSeconds(1.0f);
            workerCtrl.DrawWorkerLocationPos();
            yield return new WaitForSeconds(3.0f);
            SetWorkerClickListener(workerInfoPanel);
            yield return new WaitForSeconds(workerWaitSeconds);
        }
    }
    //初始化worker信息显示界面
    private void IniteWorkersInfoPanel(Transform _layoutParent)
    {
        GameObject workerInfoItem = Resources.Load("Prefabs/InfoItem") as GameObject;
        List<Transform> workerInfoItemsList = new List<Transform>();
        List<string> contentName = new List<string> { "name", "type", "age", "workerId"};
        List<string> contentText = new List<string> { "姓名", "工种", "年龄", "工号"};
        for (int i = 0; i < contentName.Count; i++)
        {
            GameObject go = Instantiate(workerInfoItem, _layoutParent);
            go.transform.Find("Type").GetComponent<Text>().text = string.Format("{0}:", contentText[i]);
            go.transform.name = contentName[i];
            workerInfoItemsList.Add(go.transform);
        }
    }

    void SetWorkerClickListener(Transform infoPanel)
    {
        infoPanel.gameObject.SetActive(false);
        Button infoCancelBtn = infoPanel.Find("CancelImg").GetComponent<Button>();
        infoCancelBtn.onClick.RemoveAllListeners();
        infoCancelBtn.onClick.AddListener(() =>
        {
            infoPanel.gameObject.SetActive(false);
        });
        List<Transform> workerSpawnersList = new List<Transform>();
        workerSpawnersList.Clear();
        foreach (Transform item in workerSpawner)
        {
            workerSpawnersList.Add(item);
        }
        Transform infoLayout = infoPanel.Find("LayoutParent");

        // //Test数据
        // for (int i = 0; i < workerSpawnersList.Count; i++)
        // {
        //     int index = i;
        //     workerSpawnersList[index].GetComponent<ObjectClick>().onclick.RemoveAllListeners();
        //     workerSpawnersList[index].GetComponent<ObjectClick>().onclick.AddListener(() =>
        //     {
        //         infoPanel.gameObject.SetActive(true);
        //         //获取信息
        //         //显示信息，员工信息
        //         infoLayout.Find("name/Content").GetComponent<Text>().text = "nametest";
        //         infoLayout.Find("type/Content").GetComponent<Text>().text = "typetest";
        //         infoLayout.Find("age/Content").GetComponent<Text>().text = "agetest";
        //         infoLayout.Find("workerId/Content").GetComponent<Text>().text = "workerId"+index;
        //         // infoLayout.Find("puId/Content").GetComponent<Text>().text = "puidtest";
        //         // infoLayout.Find("date/Content").GetComponent<Text>().text = "datetest";
        //         //更改spawner的name,type_workerId
        //         workerSpawnersList[index].name = string.Format("{0}-TestWorker", index);
        //     });
        // }
        // //Test--------

        workerCtrl.GetWorkerLocationInfo(workerInfoList =>
        {
            Debug.Log(workerSpawnersList.Count+"   testcount");
            for (int i = 0; i < workerSpawnersList.Count; i++)
            {
                int index = i;
                workerSpawnersList[index].GetComponent<ObjectClick>().onclick.RemoveAllListeners();
                workerSpawnersList[index].GetComponent<ObjectClick>().onclick.AddListener(() =>
                {
                    infoPanel.gameObject.SetActive(true);
                    //prefab的name即工人ID
                    // workerSpawner.GetChild(index).name=workerInfoList[index].workerId;
                    //显示信息，员工信息
                    infoLayout.Find("name/Content").GetComponent<Text>().text = workerInfoList[index].name;
                    infoLayout.Find("type/Content").GetComponent<Text>().text = workerInfoList[index].type;
                    infoLayout.Find("age/Content").GetComponent<Text>().text = workerInfoList[index].age;
                    infoLayout.Find("workerId/Content").GetComponent<Text>().text = workerInfoList[index].workerId;
                    homeCtrl.GetImageSprite(workerInfoList[index].image,sprite=>{
                        infoLayout.parent.Find("Photo").GetComponent<Image>().sprite = sprite;
                    });
                    // workerSpawnersList[index].name = string.Format("{0}-{1}", workerInfoList[index].type, workerInfoList[index].workerId);
                });
            }
        });
    }
}
