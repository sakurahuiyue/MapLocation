using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
    //private void Start()
    //{
    //    DataManager.GetInstance.CreateUserInfo();
    //}
    public void CheckLoginInfo(string _userName,string _password,Action<int> _action)
    {
        DataManager.GetInstance.Login(_userName, _password,requestText=>
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
            int code = (int)jsonData["code"];
            if (!((IDictionary)jsonData).Contains("data"))
                return;
            if (((IDictionary)jsonData["data"]).Contains("monitorInfo"))
            {
                string msg = (string)jsonData["data"]["monitorInfo"];
                if(msg!=string.Empty)
                    EvConst.monitorSequence = msg;
            }

            _action(code);
        });
    }
}
