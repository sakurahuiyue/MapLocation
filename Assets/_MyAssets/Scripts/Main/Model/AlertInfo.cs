using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
[Serializable]
public class AlertInfo
{
    public int id { get; set; }
    public string title { get; set; }
    public string image { get; set; }
    //1、2需要Ip转换,3直接显示
    public string type { get; set; }
    //0、1转换是否处理
    public string state { get; set; }
    public string date { get; set; }
    public string detail { get; set; }
    public string remark { get; set; }
    public string username { get; set; }
    // public string page { get; set; }
}
