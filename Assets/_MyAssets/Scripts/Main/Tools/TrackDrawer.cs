using Mathd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackDrawer : MonoBehaviour
{
    //轨迹的父对象
    Transform trackLineParent;
    LocationSpawner spawner;
    private void Awake()
    {
        //trackLineParent = transform.Find("TrackLines");
        spawner = FindObjectOfType<LocationSpawner>();
    }
    //轨迹类型
    public enum TrackType
    {
        close,
        open
    }
    public void DrawVehicleTracks(List<Vector3d> historyPosList,Transform _parent,TrackType trackType,Color col)
    {
        //历史点集转化为模型中虚拟点集
        List<Vector3> virtualPosList = new List<Vector3>();
        virtualPosList.Clear();
        foreach (var item in historyPosList)
        {
            virtualPosList.Add(spawner.TranformRealPointToVirtual(item));
        }
        GameObject go = new GameObject();
        LineRenderer trackLine = go.AddComponent<LineRenderer>();
        trackLine.useWorldSpace = true;
        trackLine.transform.SetParent(_parent);
        trackLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        trackLine.alignment = LineAlignment.TransformZ;
        trackLine.startWidth = 30;
        trackLine.endWidth = 30;
        trackLine.startColor = col;
        trackLine.endColor = col;
        Vector3[] arr;
        if (trackType == TrackType.close)
        {
             arr= new Vector3[virtualPosList.Count + 1];
            for (int i = 0; i < arr.Length - 1; i++)
            {
                arr[i] = virtualPosList[i];
            }
            arr[arr.Length - 1] = arr[0];
            arr[virtualPosList.Count] = virtualPosList[0];
            //隐藏
        }
        else
        {
            arr = virtualPosList.ToArray();
        }
        trackLine.positionCount = arr.Length;
        trackLine.SetPositions(arr);
        Debug.Log(string.Format("绘制点的数量为{0}----", trackLine.positionCount));
        //trackLine.SetPositions(virtualPosList.ToArray());
        //int index = 0;
        //Debug.Log(virtualPosList.Count + "---");
        //for (int i = 0; i < virtualPosList.Count - 1; i++)
        //{
        //    //绘制路径
        //    index = i;

        //    trackLine.SetPosition(index, virtualPosList[index]);
        //    trackLine.SetPosition(index + 1, virtualPosList[index + 1]);
        //    Debug.Log(index + "test----");
        //}
    }
}
