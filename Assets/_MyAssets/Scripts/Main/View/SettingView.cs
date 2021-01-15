using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EvConst;
using static UnityEngine.UI.Dropdown;
using UI.Dates;

public class SettingView : MonoBehaviour
{
    SettingControl settingCtrl;
    UIManager uiManager;
    Transform canvas;
    Transform settingLayout;//弹出框parent节点
    Transform buttonsGroup;
    Transform baseSettingPanel;
    Transform seekPanel;
    Transform searchInfoPanel;
    Transform workerSpawner;
    Transform deviceSpawner;
    [SerializeField]
    int btnsCount = 4;


    private void Awake()
    {
        settingCtrl = FindObjectOfType<SettingControl>();
        uiManager = FindObjectOfType<UIManager>();
        canvas = GameObject.Find("View").transform.Find("Canvas");
        settingLayout = transform.Find("SettingLayout");
        buttonsGroup = settingLayout.Find("BtnsGroup");
        baseSettingPanel = transform.Find("BaseSettingPanel");
        seekPanel = transform.Find("SeekPanel");
        searchInfoPanel = transform.Find("SearchInfoPanel");

        workerSpawner = GameObject.Find("Spawners").transform.Find("WorkerSpawner");
        deviceSpawner = GameObject.Find("Spawners").transform.Find("DeviceSpawner");
    }
    private void Start()
    {
        //
        IniteBasePanelSettings();
    }
    private void OnEnable()
    {
        //初始化 隐藏spawers
        workerSpawner.gameObject.SetActive(false);
        deviceSpawner.gameObject.SetActive(false);
    }
    void IniteBasePanelSettings()
    {
        settingLayout.gameObject.SetActive(true);
        List<Transform> panelsGroupList = new List<Transform>();
        foreach (Transform item in transform)
        {
            if (item.name.Equals(settingLayout.name))
                continue;
            panelsGroupList.Add(item);
        }
        btnsCount = panelsGroupList.Count;
        List<Transform> buttonsGroupList = new List<Transform>();
        GameObject settingItemPrefab = Resources.Load<GameObject>("Prefabs/SettingBtnItem");
        for (int i = 0; i < btnsCount; i++)
        {
            buttonsGroupList.Add(Instantiate(settingItemPrefab, buttonsGroup).transform);
            int index = i;
            buttonsGroupList[index].Find("Text").GetComponent<Text>().text
                = panelsGroupList[index].Find("ParentPanel/ParentTitle").GetComponent<Text>().text;
            //1对1控制监听事件
            panelsGroupList[index].gameObject.SetActive(false);
            //父级按钮监听
            buttonsGroupList[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                panelsGroupList[index].gameObject.SetActive(true);
                settingLayout.gameObject.SetActive(false);
            });
            Button cancelBtn = panelsGroupList[index].Find("ParentPanel/CancelBtn").GetComponent<Button>();
            cancelBtn.onClick.AddListener(() =>
            {
                panelsGroupList[index].gameObject.SetActive(false);
                settingLayout.gameObject.SetActive(true);
            });

        }
        //SetSystemSettingsPanel(baseSettingPanel);
        //SetSeekPanel();
        //SetSearchInfoPanel();
        //密码修改
        SetPasswordModify(baseSettingPanel);
        //摄像头设置
        SelectMointorSettings(baseSettingPanel);
        //查找设置
        //查工人
        SetSeekInfo(seekPanel, MyType.Worker);
        SetSeekInfo(seekPanel, MyType.Device);
        SetSearchInfoPanel(searchInfoPanel, InfoType.Danger);
        SetSearchInfoPanel(searchInfoPanel, InfoType.Alert);
    }

    /// <summary>
    /// 初始化层级内容
    /// </summary>
    /// <param name="parentPanel">父级panel</param>
    /// <param name="childPanel">子级panel</param>
    /// <param name="index">显示的序列</param>
    /// <param name="targetName">目标序列detailpanel的name</param>
    private void IniteChildBtns(Transform parentPanel, Transform childPanel, int index, string targetName)
    {
        //Transform parentPanel = basePanel.Find("ParentPanel");
        //parentPanel.gameObject.SetActive(true);
        //Transform childPanel = basePanel.Find("ChildPanel");
        //childPanel.gameObject.SetActive(false);
        Button childBtn = parentPanel.Find("ChlidrenGroup").GetChild(index).GetComponent<Button>();
        Text childTitle = childPanel.Find("ChildTitle").GetComponent<Text>();
        string str = childBtn.transform.GetChild(0).GetComponent<Text>().text;
        childBtn.onClick.RemoveAllListeners();
        childBtn.onClick.AddListener(() =>
        {
            parentPanel.gameObject.SetActive(false);
            childPanel.gameObject.SetActive(true);
            if (!childTitle.text.Equals(str))
                childTitle.text = str;
            //some actions
            Transform detail = childPanel.Find("Detail");
            foreach (Transform item in detail)
            {
                item.gameObject.SetActive(false);
            }
            if (detail.Find(targetName) != null)
            {
                detail.Find(targetName).gameObject.SetActive(true);
            }
        });
    }
    //设置通用提交监听
    private void SetChildPanelSubmission(Transform _basePanel, int index, Action _submitAction)
    {
        Transform parentPanel = _basePanel.Find("ParentPanel");
        parentPanel.gameObject.SetActive(true);
        Transform childPanel = _basePanel.Find("ChildPanel");
        Button childCancelBtn = childPanel.Find("Detail").GetChild(index).Find("Cancel").GetComponent<Button>();
        childCancelBtn.onClick.RemoveAllListeners();
        childCancelBtn.onClick.AddListener(() =>
        {
            childPanel.gameObject.SetActive(false);
            parentPanel.gameObject.SetActive(true);
        });
        Button childConfirmBtn = childPanel.Find("Detail").GetChild(index).Find("Confirm").GetComponent<Button>();
        childConfirmBtn.onClick.RemoveAllListeners();
        childConfirmBtn.onClick.AddListener(() =>
        {
            //childPanel.gameObject.SetActive(false);
            //parentPanel.gameObject.SetActive(true);
            _submitAction();
        });
    }
    //系统设置
    void SetPasswordModify(Transform _systemSettingPanel)
    {
        Transform parentPanel = _systemSettingPanel.Find("ParentPanel");
        parentPanel.gameObject.SetActive(true);
        Transform childPanel = _systemSettingPanel.Find("ChildPanel");
        childPanel.gameObject.SetActive(false);
        IniteChildBtns(parentPanel, childPanel, 0, "PasswordDetail");
        //password check
        Text warningText = childPanel.Find("Detail/PasswordDetail/WarningText").GetComponent<Text>();
        warningText.gameObject.SetActive(false);
        string psw = string.Empty;
        string pswCopy = string.Empty;
        InputField pswInput = childPanel.Find("Detail/PasswordDetail/PwdInput").GetComponent<InputField>();
        pswInput.onEndEdit.RemoveAllListeners();
        pswInput.onEndEdit.AddListener(inputStr =>
        {
            psw = inputStr;
        });
        InputField pswInputConfirm = childPanel.Find("Detail/PasswordDetail/PwdConfirm").GetComponent<InputField>();
        pswInputConfirm.onEndEdit.RemoveAllListeners();
        pswInputConfirm.onEndEdit.AddListener(inputStr =>
        {
            pswCopy = inputStr;
            if (inputStr == string.Empty || !pswCopy.Equals(psw))
            {
                //错误提示
                Debug.Log("两次输入不一致");
                warningText.gameObject.SetActive(true);
                return;
            }
            //正确
            warningText.gameObject.SetActive(false);
            Debug.Log(string.Format("输入值:psw:{0},pswCopy:{1}", psw, pswCopy));
        });
        //提交
        SetChildPanelSubmission(_systemSettingPanel, 0, () =>
         {
             if (psw == string.Empty || psw != pswCopy)
                 return;
             Debug.Log("sumited-----");
             Debug.Log(string.Format("输入值:psw:{0},pswCopy:{1}", psw, pswCopy));
             //提交
             string userName = DataManager.GetInstance.user.username;
             settingCtrl.SubmitPassword(userName, psw, () =>
            {
                //modify success
                Debug.Log("modify password success");
            });
             childPanel.gameObject.SetActive(false);
             parentPanel.gameObject.SetActive(true);
         });
    }
    void SelectMointorSettings(Transform _systemSettingPanel)
    {
        Transform parentPanel = _systemSettingPanel.Find("ParentPanel");
        parentPanel.gameObject.SetActive(true);
        Transform childPanel = _systemSettingPanel.Find("ChildPanel");
        childPanel.gameObject.SetActive(false);
        IniteChildBtns(parentPanel, childPanel, 1, "MonitorDetail");
        Text warningText = childPanel.Find("Detail/MonitorDetail/WarningText").GetComponent<Text>();
        warningText.gameObject.SetActive(false);
        //复选设置
        //监控数量
        int monitorCount = 6;
        Transform toggleGroup = childPanel.Find("Detail/MonitorDetail/ToggleGroup");
        GameObject toggleItemPrefab = Resources.Load("Prefabs/Toggle") as GameObject;
        for (int i = 0; i < monitorCount; i++)
        {
            GameObject go = Instantiate(toggleItemPrefab, toggleGroup);
            go.transform.Find("Label").GetComponent<Text>().text = monitorNames[i];
        }
        //控制选择数量为2
        toggleGroup.gameObject.AddComponent<ToggleGroupPro>();
        toggleGroup.GetComponent<ToggleGroupPro>().OptionsNumber = 2;
        //读取并解析历史选项
        if (EvConst.monitorSequence != string.Empty)
        {
            string tempStr = EvConst.monitorSequence;
            string[] textstr = tempStr.Split('_');
            for (int i = 0; i < textstr.Length; i++)
            {
                toggleGroup.GetChild(i).GetComponent<Toggle>().isOn = true;
            }
        }
        //提交
        SetChildPanelSubmission(_systemSettingPanel, 1, () =>
        {
            //存储选中的信息，用于提交
            List<int> selectedList = new List<int>();
            int index = 0;
            foreach (Transform item in toggleGroup)
            {
                if (item.GetComponent<Toggle>().isOn)
                {
                    selectedList.Add(index);
                }
                index++;
            }
            //少于2个选项警告
            if(selectedList.Count<2){
                warningText.gameObject.SetActive(true);
                return;
            }
            //根据选择修改media
            settingCtrl.ModifyHomeMonitorSequences(selectedList);
            //submit
            //Test
            string monitorSequence = string.Empty;
            for (int i = 0; i < selectedList.Count; i++)
            {
                if (i < selectedList.Count - 1)
                {
                    monitorSequence += selectedList[i] + "_";
                }
                else
                {
                    monitorSequence += selectedList[i];
                }
            }
            Debug.Log("submit monitor select " + monitorSequence);
            //submit
            settingCtrl.UpdateMonitorSettings(monitorSequence, () =>
            {
                Debug.Log("submit monitor selectstr success");
            });
            warningText.gameObject.SetActive(false);
            childPanel.gameObject.SetActive(false);
            parentPanel.gameObject.SetActive(true);
        });
    }

    //定位查找
    void SetSeekInfo(Transform _seekPanel, MyType _seekType)
    {
        Transform parentPanel = _seekPanel.Find("ParentPanel");
        parentPanel.gameObject.SetActive(true);
        Transform childPanel = _seekPanel.Find("ChildPanel");
        childPanel.gameObject.SetActive(false);
        string targetDetailName = string.Format("{0}SeekDetail", _seekType);
        //IniteChildBtns(parentPanel, childPanel, 0, targetDetailName);
        int index = (int)_seekType;
        IniteChildBtns(parentPanel, childPanel, index, targetDetailName);
        //无效输入提示
        //Text warningText = childPanel.Find("Detail/WorkerSeekDetail/WarningText").GetComponent<Text>();
        Text warningText = childPanel.Find(string.Format("Detail/{0}/WarningText", targetDetailName)).GetComponent<Text>();
        warningText.gameObject.SetActive(false);
        string inputId = string.Empty;
        //InputField workerIdInput = childPanel.Find("Detail/WorkerSeekDetail/WorkerIdInput").GetComponent<InputField>();
        InputField idInput = childPanel.Find(string.Format("Detail/{0}/IdInput", targetDetailName)).GetComponent<InputField>();
        idInput.onEndEdit.RemoveAllListeners();
        idInput.onEndEdit.AddListener(str =>
        {
            inputId = str;
        });
        SetChildPanelSubmission(_seekPanel, (int)_seekType, () =>
          {
             //submit
             Debug.Log(string.Format("submit-{0}", _seekType));
              Transform spawner = GameObject.Find("Spawners").transform.Find(string.Format("{0}Spawner", _seekType));
              if(inputId==""){
                      warningText.gameObject.SetActive(true);
                      return;
                  }
              if (_seekType == MyType.Worker)
              {
                  //本地查询
                  Transform tragetTran = spawner.Find(inputId);
                  if (tragetTran == null){
                      Debug.Log("所查Id不存在,请检查后再试!");
                      warningText.gameObject.SetActive(true);
                  }else{
                      warningText.gameObject.SetActive(false);
                      //显示
                      tragetTran.parent.gameObject.SetActive(true);
                      foreach (Transform item in tragetTran.parent)
                      {
                          item.gameObject.SetActive(false);
                      }
                      tragetTran.gameObject.SetActive(true);
                      uiManager.HideOtherPanels(canvas.Find("UIPanels/MainPanels/WorkerPanel"));
                      GameManager.GetInstance.SetViewState(2);
                      //聚焦效果
                      ShowItemDetails(spawner,inputId);
                    //   GameManager.GetInstance.currentState= GameManager.ViewState.Worker;
                  }
                  //   settingCtrl.GetWorkerInfoById(inputId,workerInfo=>{
                  //       ShowItemDetails(spawner,inputId);
                  //   });
              }
              if (_seekType == MyType.Device)
              {
                  //本地查询
                  Transform tragetTran = spawner.Find(inputId);
                  if (tragetTran == null){
                      Debug.Log("所查Id不存在,请检查后再试!");
                      warningText.gameObject.SetActive(true);
                  }else{
                      warningText.gameObject.SetActive(false);
                      //显示
                      tragetTran.parent.gameObject.SetActive(true);
                      foreach (Transform item in tragetTran.parent)
                      {
                          item.gameObject.SetActive(false);
                      }
                      tragetTran.gameObject.SetActive(true);
                      uiManager.HideOtherPanels(canvas.Find("UIPanels/MainPanels/VehiclePanel"));
                      GameManager.GetInstance.SetViewState(3);
                      //聚焦效果
                      ShowItemDetails(spawner,inputId);
                    //   GameManager.GetInstance.currentState= GameManager.ViewState.Vehicle;
                  }
                //   settingCtrl.GetDeviceInfoById(inputId,deviceInfo=>{
                //       ShowItemDetails(spawner,inputId);
                //   });
              }
          });
    }
    private void ShowItemDetails(Transform spawner,string inputId)
    {
        spawner.gameObject.SetActive(true);
        Transform tran = spawner.Find(inputId);
        if (tran == null)
        {
            Debug.Log("not find");
            return;
        }
        else
        {
            //使用imge特殊标记
            // tran.gameObject.SetActive(true);
            // GameObject go = Resources.Load("Prefabs/MonitorIcon") as GameObject;
            // GameObject img = Instantiate(go, canvas);
            // img.SetActive(true);
            // img.GetComponent<RectTransform>().anchoredPosition = EvConst.GetScreenPosition(tran, canvas);
            // //相机跟随
            // EvConst.MoveCamera(Camera.main.transform, tran, 1);
            //高亮显示
            // StartCoroutine(DrawObjBoards(tran));
            DrawObjBoards(tran);
        }
    }
    // IEnumerator DrawObjBoards(Transform tran)
    // {
    //     tran.GetComponent<Renderer>().material.color=Color.red;
    //     yield return new WaitForSeconds(0.8f);
    //     tran.GetComponent<Renderer>().material.color=Color.green;
    //     yield return new WaitForSeconds(0.8f);
    //     tran.GetComponent<Renderer>().material.color=Color.blue;
    //     yield return null;
    // }
    void DrawObjBoards(Transform  tran){
        tran.GetComponent<Renderer>().material.color=Color.green;
    }
    enum InfoType
    {
        Danger = 0,
        Alert = 1
    }
    //信息查询
    void SetSearchInfoPanel(Transform _infoPanel, InfoType _infoType)
    {
        Transform parentPanel = _infoPanel.Find("ParentPanel");
        parentPanel.gameObject.SetActive(true);
        Transform childPanel = _infoPanel.Find("ChildPanel");
        childPanel.gameObject.SetActive(false);
        string targetDetailName = string.Format("{0}Detail", _infoType);
        int index = (int)_infoType;
        IniteChildBtns(parentPanel, childPanel, index, targetDetailName);
        //无效输入提示
        // Text warningText = childPanel.Find(string.Format("Detail/{0}/WarningText",targetDetailName)).GetComponent<Text>();
        // warningText.gameObject.SetActive(false);
        // string inputId = string.Empty;
        // InputField idInput = childPanel.Find(string.Format("Detail/{0}/IdInput",targetDetailName)).GetComponent<InputField>();
        // idInput.onEndEdit.RemoveAllListeners();
        if (_infoType == InfoType.Danger)
        {
            Transform queryInfoPanel = canvas.Find("UIPanels/MessagePanels/AreaInfo");
            Transform dangerDetail=childPanel.Find("Detail/DangerDetail");
            Button queryPanelCancel=queryInfoPanel.Find("CancelImg").GetComponent<Button>();
            queryInfoPanel.gameObject.SetActive(false);
            queryPanelCancel.onClick.AddListener(() =>
            {
                _infoPanel.gameObject.SetActive(true);
                queryInfoPanel.gameObject.SetActive(false);
            });
            // 选取方式未定，Dropdown用于测试
            string selectedArea = string.Empty;
            Dropdown dropdown = dangerDetail.Find("Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions();
            List<OptionData> options = new List<OptionData>();
            for (int i = 0; i < 14; i++){
                options.Add(new OptionData("Lines" + (i+1)));
            }
            // options.Add(new OptionData("A"));
            // options.Add(new OptionData("B"));
            // options.Add(new OptionData("C"));
            // options.Add(new OptionData("D"));
            // options.Add(new OptionData("E"));
            // options.Add(new OptionData("F"));
            // options.Add(new OptionData("G"));
            dropdown.AddOptions(options);
            DatePicker datePicker = dangerDetail.Find("DatePicker").GetComponent<DatePicker>();
            datePicker.SelectedDate = DateTime.Today;
            string inputDate="";
            SetChildPanelSubmission(_infoPanel, index, () =>
            {
                DateTime date = datePicker.SelectedDate;
                inputDate=date.ToString("yyyy-MM-dd");
                selectedArea=dropdown.captionText.text;
                Debug.Log("dangersubmit" + selectedArea + "----" + inputDate);
                //清空列表
                settingCtrl.GetWorkerInTargetArea(selectedArea, inputDate, workerInAreaInfos =>
                {
                    LoadDangerousAreaContent(selectedArea, workerInAreaInfos);
                });
                _infoPanel.gameObject.SetActive(false);
                queryInfoPanel.gameObject.SetActive(true);
            });
        }
        if (_infoType == InfoType.Alert)
        {
            // 按类别和时间查询警报信息
            int page = 1;
            //back or next page
            Transform queryInfoPanel = canvas.Find("UIPanels/MessagePanels/QueryInfo");
            Transform alertDetail=childPanel.Find("Detail/AlertDetail");
            //show panel
            queryInfoPanel.gameObject.SetActive(false);
            string username = DataManager.GetInstance.user.username;
            string submitType = string.Empty;
            //input date
            DatePicker datePicker = alertDetail.Find("DatePicker").GetComponent<DatePicker>();
            string inputDate="";
            // DateTime date = datePicker.SelectedDate;
            datePicker.SelectedDate = DateTime.Today;
            //input type
            Dropdown dropdown = alertDetail.Find("TypeDropDown").GetComponent<Dropdown>();
            dropdown.ClearOptions();
            List<OptionData> options = new List<OptionData>();
            options.Add(new OptionData("所有"));
            options.Add(new OptionData("火灾"));
            options.Add(new OptionData("未戴安全帽"));
            options.Add(new OptionData("危险区域"));
            dropdown.AddOptions(options);
            // //warningText
            // Text warningText=alertDetail.Find("WarningText").GetComponent<Text>();
            // warningText.gameObject.SetActive(false);
            //pagetext
            Text pageText=queryInfoPanel.Find("CurrentPage").GetComponent<Text>();
            //cancelBtn
            Button queryPanelCancel=queryInfoPanel.Find("CancelImg").GetComponent<Button>();
            queryPanelCancel.onClick.AddListener(()=>{
                _infoPanel.gameObject.SetActive(true);
                Transform resultPanel = queryInfoPanel.Find("ResultPanel");
                foreach (Transform item in resultPanel)
                {
                    Destroy(item.gameObject);
                }
                queryInfoPanel.gameObject.SetActive(false);
            });
            //back&next page
            Button nextBtn = queryInfoPanel.Find("nextBtn").GetComponent<Button>();
            nextBtn.onClick.AddListener(() =>
            {
                submitType=dropdown.value.ToString();
                settingCtrl.QueryHistoryAlertInfo(submitType, inputDate, username, page.ToString(), historyAlertInfos =>
                {
                    LoadQueryContent(historyAlertInfos);
                });
                settingCtrl.QueryCurrentPageState(submitType, inputDate, username, page.ToString(), flag =>
                {
                    Debug.Log("current page is"+flag);
                    if (!flag)
                    {
                        ++page;
                        pageText.text = page.ToString();
                    }
                });
            });
            Button backBtn = queryInfoPanel.Find("backBtn").GetComponent<Button>();
            backBtn.onClick.AddListener(() =>
            {
                if (page > 2)
                {
                    settingCtrl.QueryHistoryAlertInfo(submitType, inputDate, username, page.ToString(), historyAlertInfos =>
                    {
                        LoadQueryContent(historyAlertInfos);
                    });
                    --page;
                }
            });
            SetChildPanelSubmission(_infoPanel, index, () =>
            {
                if (dropdown.value == 0)
                {
                    //全体
                    submitType = "";
                }
                else
                {
                    submitType = dropdown.value.ToString();
                }
                //单次提交
                DateTime date = datePicker.SelectedDate;
                inputDate = date.ToString();
                Debug.Log("submitdate:"+inputDate);
                settingCtrl.QueryHistoryAlertInfo(submitType, inputDate, username, page.ToString(), historyAlertInfos =>
                {
                    // Debug.Log("settingload----");
                    // foreach (var item in historyAlertInfos)
                    // {
                    //     Debug.Log(item.state);
                    // }
                    LoadQueryContent(historyAlertInfos);
                });
                _infoPanel.gameObject.SetActive(false);
                queryInfoPanel.gameObject.SetActive(true);
            });
        }
    }
    //实例化危险区查询列表内容
    private void LoadDangerousAreaContent(string selectArea,List<WorkerInAreaInfo> workerInAreaInfos)
    {
        Transform queryInfoPanel = canvas.Find("UIPanels/MessagePanels/AreaInfo");
        Transform resultPanel = queryInfoPanel.Find("ResultPanel");
        queryInfoPanel.gameObject.SetActive(true);
        Transform contentPanel=resultPanel.Find("Viewport/Content");
        Transform warningText = queryInfoPanel.Find("WarningText");
        if (workerInAreaInfos.Count == 0)
        {
            foreach (Transform item in contentPanel)
            {
                Destroy(item.gameObject);
            }
            warningText.gameObject.SetActive(true);
            return;
        }else{
            warningText.gameObject.SetActive(false);
        }
        //数量更新则刷新
        if (contentPanel.childCount != workerInAreaInfos.Count)
        {
            foreach (Transform item in contentPanel)
            {
                Destroy(item.gameObject);
            }
            GameObject go = Resources.Load("Prefabs/QueryAreaInfoItem") as GameObject;
            for (int i = 0; i < workerInAreaInfos.Count; i++)
            {
                GameObject tempItem = Instantiate(go, contentPanel);
                ModifyAreaInfo(selectArea,tempItem.transform, i, workerInAreaInfos);
            }
        }
        else
        //内容替换
        {
            int childIndex = 0;
            foreach (Transform item in contentPanel)
            {
                ModifyAreaInfo(selectArea,item, childIndex, workerInAreaInfos);
                childIndex++;
            }
        }
    }
    //补充信息
    private void ModifyAreaInfo(string selectArea,Transform item,int index,List<WorkerInAreaInfo> workerInAreaInfos){
        item.Find("Index").GetComponent<Text>().text=workerInAreaInfos[index].id;
        item.Find("Date").GetComponent<Text>().text=workerInAreaInfos[index].date;
        item.Find("Type").GetComponent<Text>().text=workerInAreaInfos[index].type;
        item.Find("Name").GetComponent<Text>().text=workerInAreaInfos[index].name;
        item.Find("WorkerId").GetComponent<Text>().text=workerInAreaInfos[index].workerId;
        item.Find("Direction").GetComponent<Text>().text=selectArea;
        // item.Find("Direction").GetComponent<Text>().text=ChangeAreaStr(workerInAreaInfos[index].direction);
    }
    //更改dir0-A等
    private string ChangeAreaStr(string num){
        switch(num){
            case "0": return "A";
            case "1": return "B";
            case "2": return "C";
            case "3": return "D";
            case "4": return "E";
            case "5": return "F";
            case "6": return "G";
            default: return "";
        }
    }
    //更改处理状态等
    private string ChangedealState(string num){
        switch(num){
            case "0": return "未处理";
            default: return "已处理";
        }
    }
    //更改类别
    private string ChangeTypeStr(string num){
        switch(num){
            case "1": return "火灾";
            case "2": return "未戴安全帽";
            default: return "危险区域";
        }
    }
    //实例化报警信息查询列表内容
    private void LoadQueryContent(List<HistoryAlertInfo> historyAlertInfos)
    {
        Debug.Log("load");
        //load alert list
        Transform queryInfoPanel = canvas.Find("UIPanels/MessagePanels/QueryInfo");
        //go parent
        Transform resultPanel = queryInfoPanel.Find("ResultPanel");
        queryInfoPanel.gameObject.SetActive(true);
        //需要重新实例化
        Transform warningText=queryInfoPanel.Find("WarningText");
        if (historyAlertInfos.Count == 0)
        {
            foreach (Transform item in resultPanel)
            {
                Destroy(item.gameObject);
            }
            warningText.gameObject.SetActive(true);
            return;
        }
        else
        {
            warningText.gameObject.SetActive(false);
        }
        if (resultPanel.childCount != historyAlertInfos.Count)
        {
            foreach (Transform item in resultPanel)
            {
                Destroy(item.gameObject);
            }
            GameObject go = Resources.Load("Prefabs/QueryInfoContenItem") as GameObject;
            Debug.Log(go.name);
            for (int i = 0; i < historyAlertInfos.Count; i++)
            {
                GameObject tempItem = Instantiate(go, resultPanel);
                FillInfos(tempItem.transform, i, historyAlertInfos);
            }
        }
        else
        {
            Debug.Log("替换");
            //可直接替换内容部分
            int childIndex = 0;
            foreach (Transform item in resultPanel)
            {
                FillInfos(item, childIndex, historyAlertInfos);
                childIndex++;
                Debug.Log(childIndex);
            }
        }
    }
    //
    private void FillInfos(Transform item,int i,List<HistoryAlertInfo> historyAlertInfos)
    {
        item.Find("Index").GetComponent<Text>().text = historyAlertInfos[i].id.ToString();
        item.Find("Title").GetComponent<Text>().text = historyAlertInfos[i].title;
        item.Find("Type").GetComponent<Text>().text = ChangeTypeStr(historyAlertInfos[i].type);
        item.Find("State").GetComponent<Text>().text = ChangedealState(historyAlertInfos[i].state);
        item.Find("Detail").GetComponent<Text>().text = historyAlertInfos[i].detail;
        item.Find("Handler").GetComponent<Text>().text = historyAlertInfos[i].username;
    }
}
