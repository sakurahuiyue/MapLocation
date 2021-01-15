using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoverView : MonoBehaviour
{
    CoverManager coverManager;

    Transform coverLoginPanel;
    Transform titleBasePanel;

    private void Awake()
    {
        coverManager = FindObjectOfType<CoverManager>();

        coverLoginPanel = GameObject.Find("UIPanels").transform.Find("MainPanels/CoverPanel/LoginPanel");
        titleBasePanel = GameObject.Find("UIPanels").transform.Find("TitleBase");
    }

    private void Start()
    {
        SetCoverLoginPanel();
        CancelProgram();
    }
    //退出程序
    void CancelProgram()
    {
        Button cancelBtn = titleBasePanel.Find("QuitBtn").GetComponent<Button>();
        cancelBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
    //设置登陆界面
    void SetCoverLoginPanel()
    {
        string username = string.Empty;
        string password = string.Empty;
        InputField userNameInput = coverLoginPanel.Find("InputUserNameField").GetComponent<InputField>();
        userNameInput.onEndEdit.AddListener(inputStr =>
        {
            username = inputStr;
        });
        InputField passwordInput = coverLoginPanel.Find("InputUserPasswordField").GetComponent<InputField>();
        passwordInput.onEndEdit.AddListener(inputStr =>
        {
            password = inputStr;
        });
        Button confirmBtn = coverLoginPanel.Find("Confirm").GetComponent<Button>();
        //警告文字，默认隐藏
        Text warningText = coverLoginPanel.Find("Warning").GetComponent<Text>();
        warningText.gameObject.SetActive(false);
        //提交
        confirmBtn.onClick.AddListener(() =>
        {
            StartCoroutine(DataManager.GetInstance.Login(username, password, requestText =>
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
                if (!((IDictionary)jsonData).Contains("code"))
                    return;
                int code=(int)jsonData["code"];
                if (!200.Equals(code))
                {
                    // string warningStr = (string)jsonData["message"];
                    // warningText.text = warningStr;
                    warningText.text = "访问指定资源内容失败!";
                    warningText.gameObject.SetActive(true);
                    return;
                }
                DataManager.GetInstance.user.username = username;
                DataManager.GetInstance.user.password = password;
                //载入下一个场景
                int sceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(sceneIndex + 1);
            }));
            // DataManager.GetInstance.user.username = username;
            // DataManager.GetInstance.user.password = password;
            // //载入下一个场景
            // int testsceneIndex = SceneManager.GetActiveScene().buildIndex;
            // SceneManager.LoadScene(testsceneIndex + 1);
        });
    }

    //设置常用按钮监听事件
    void SetCommonBtns()
    {
        Button quitBtn = titleBasePanel.Find("QuitBtn").GetComponent<Button>();
        quitBtn.onClick.AddListener(() =>
        {
            //退出当前程序
            Application.Quit();
        });
    }
}
