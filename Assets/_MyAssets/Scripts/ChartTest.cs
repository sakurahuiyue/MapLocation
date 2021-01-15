using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCharts;

public class ChartTest : MonoBehaviour
{
    private GameObject canvas;
    public Transform pieChart;
    public Transform barChart;
    private void Start()
    {
        PieChart pc = pieChart.GetComponent<PieChart>();
        Dictionary<string, int> testDic = new Dictionary<string, int>();
        testDic.Add("a", 1);
        testDic.Add("b", 5);
        testDic.Add("c", 20);
        testDic.Add("d", 10);
        List<string> keysList = testDic.Keys.ToList();
        pc.ClearData();
        for (int i = 0; i < testDic.Count; i++)
        {
            pc.AddData(0, testDic[keysList[i]], keysList[i]);
        }
        BarChart bc = barChart.GetComponent<BarChart>();
        bc.ClearData();
        for (int i = 0; i < testDic.Count; i++)
        {
            bc.AddData(0, testDic[keysList[i]], keysList[i]);
        }
    }
}
