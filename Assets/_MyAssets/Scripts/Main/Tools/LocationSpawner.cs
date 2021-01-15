using Mathd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LocationSpawner : MonoBehaviour
{
    public GameObject workerPrefab;
    public Transform frame;
    float frameWidth;
    float frameHeight;
    List<Vector3d> realPointList;
    List<Vector3d> virtualPointList;
    int divideRatioX = 3;
    int divideRatioY = 7;

    public const float normalHeight = 313;
    private void Awake()
    {
        frameHeight = frame.GetComponent<Renderer>().bounds.size.z;
        frameWidth = frame.GetComponent<Renderer>().bounds.size.x;
        //Debug.Log(frameWidth + "____________________________" + frameHeight);
        //读取实际点的数据，即固定的分割点数据
        realPointList = new List<Vector3d>();
        realPointList = GetLocationInfo("RealLocationData.txt");
        //realPointList = GetLocationInfo("20200717.txt");
        //读取UI点的数据
        virtualPointList = new List<Vector3d>();
        virtualPointList = SetVirtualLocation(divideRatioX, divideRatioY);


        //测试数据点集
        //所求点集
        List<Vector3d> taPointsList = GetLocationInfo("points.txt");
        //List<Vector3d> taPointsList = GetLocationInfo("Test.txt");

        //Test
        ////实例化工人
        //for (int i = 0; i < taPointsList.Count; i++)
        //{
        //    ManufactureWorker(taPointsList[i], transform);
        //}
    }

    //向外暴露实例化点的方法，供使用
    /// <summary>
    /// 绘制点集
    /// </summary>
    /// <param name="_locationPosList"></param>
    /// <param name="_parent"></param>
    public void DrawLocationMaps(List<Vector3d> _locationPosList, Transform _parent)
    {
        if (_locationPosList == null)
        {
            Debug.Log("待实例化的点集有误----");
            return;
        }
        for (int i = 0; i < _locationPosList.Count; i++)
        {
            ManufactureWorker(_locationPosList[i], _parent);
        }
    }

    /// <summary>
    /// 将实际经纬度转化为虚拟坐标
    /// </summary>
    /// <param name="_inputPos"></param>
    /// <returns></returns>
    public Vector3d TranformRealPointToVirtual(Vector3d _inputPos)
    {
        Vector3d targetPos = new Vector3d(_inputPos.x, _inputPos.y, 0);
        int index = GetVectorOrginalPoint(targetPos, realPointList);
        ///Test
        //
        //Debug.Log(string.Format("---距离求解---:{0}", index));

        //int testindex = GetSurroundPoint(realPointList, targetPos);
        //Debug.Log(string.Format("---分治求解---:{0}", testindex));
        //
        Vector3d OPoint = realPointList[index];
        Vector3d virtualOrginPos = virtualPointList[index];
        List<Vector3d> xyAxisPos = GetSurroundParts(index, OPoint, realPointList);
        Vector3d resultPosition = CalculatePosition(OPoint, virtualOrginPos, xyAxisPos[0], xyAxisPos[1], targetPos);
        return new Vector3d(resultPosition.x, normalHeight, resultPosition.y);
    }

    private List<Vector3d> SetVirtualLocation(int _divideRatioX, int _divideRatioY)
    {
        int num = (_divideRatioX + 2) * (_divideRatioY + 2);
        Vector3d[] _arr = new Vector3d[num];
        for (int i = 0; i < num; i++)
        {
            double _y = frameHeight / (_divideRatioY + 1) * Math.Floor((double)(i / (divideRatioX + 2)));
            double _x = frameWidth / (divideRatioX + 1) * (i % (divideRatioX + 2));
            _arr[i] = new Vector3d(_x, _y, 0);
        }
        //绘制
        //横向
        List<Vector3d> startArr = new List<Vector3d>();
        List<Vector3d> endArr = new List<Vector3d>();
        for (int i = 0; i < _arr.Length; i++)
        {
            if (i % (_divideRatioX + 2) == 0)
            {
                startArr.Add(_arr[i]);
            }
            if (i % (_divideRatioX + 2) == divideRatioX + 1)
            {
                endArr.Add(_arr[i]);
            }
        }
        for (int i = 0; i < startArr.Count; i++)
        {
            Debug.DrawLine(new Vector3((float)startArr[i].x, normalHeight, (float)startArr[i].y), new Vector3((float)endArr[i].x, normalHeight, (float)endArr[i].y), Color.red, 100000f);
        }
        //纵向
        startArr = new List<Vector3d>();
        endArr = new List<Vector3d>();
        for (int i = 0; i <= divideRatioX + 1; i++)
        {
            startArr.Add(_arr[i]);
        }
        for (int i = _arr.Length - 1 - (divideRatioX + 1); i < _arr.Length; i++)
        {
            endArr.Add(_arr[i]);
        }
        for (int i = 0; i < startArr.Count; i++)
        {
            Debug.DrawLine(new Vector3((float)startArr[i].x, normalHeight, (float)startArr[i].y), new Vector3((float)endArr[i].x, normalHeight, (float)endArr[i].y), Color.red, 100000f);
        }
        return _arr.ToList();
    }
    /// <summary>
    /// 读取本地真实点集
    /// </summary>
    /// <param name="_fileName"></param>
    /// <returns></returns>
    public List<Vector3d> GetLocationInfo(string _fileName)
    {
        var fileAddress = Path.Combine(Application.streamingAssetsPath, _fileName);
        FileInfo fInfo0 = new FileInfo(fileAddress);
        string s = string.Empty;
        if (fInfo0.Exists)
        {
            StreamReader r = new StreamReader(fileAddress);
            //StreamReader默认的是UTF8的不需要转格式了，因为有些中文字符的需要有些是要转的，下面是转成String代码
            //byte[] data = new byte[1024];
            // data = Encoding.UTF8.GetBytes(r.ReadToEnd());
            // s = Encoding.UTF8.GetString(data, 0, data.Length);
            s = r.ReadToEnd();
            //Debug.Log(s);
            string[] strText = s.Split('|');
            //List<string> strList = strText.ToList();
            //foreach (var item in strText)
            //{
            //    Debug.Log(item);
            //}
            List<Vector3d> pointList = new List<Vector3d>();
            foreach (var item in strText)
            {
                pointList.Add(Vector3d.Parse(item));
                //Debug.Log("----------------------"+item);
            }
            return pointList;
        }
        else
            return null;
    }
    /// <summary>
    /// 根据人的经纬度，实例化人物
    /// 初始坐标(经,纬) A(120.285461,30.308242) B(120.306783,30.308281) C(120.306711,30.359481) D(120.285375,30.359453)
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    public GameObject ManufactureWorker(Vector3d _inputPos, Transform _parent)
    {

        Vector3d targetPos = new Vector3d(_inputPos.x, _inputPos.y, 0);
        int index = GetVectorOrginalPoint(targetPos, realPointList);
        ///Test
        //
        //Debug.Log(string.Format("---距离求解---:{0}", index));

        //int testindex = GetSurroundPoint(realPointList, targetPos);
        //Debug.Log(string.Format("---分治求解---:{0}", testindex));
        //
        Vector3d OPoint = realPointList[index];
        Vector3d virtualOrginPos = virtualPointList[index];
        List<Vector3d> xyAxisPos = GetSurroundParts(index, OPoint, realPointList);
        Vector3d resultPosition = CalculatePosition(OPoint, virtualOrginPos, xyAxisPos[0], xyAxisPos[1], targetPos);

        //Vector3d resultPosition = CalculatePosition(OPoint, virtualOrginPos, GetSurroundParts(index, OPoint, realPointList)[0], GetSurroundParts(index, OPoint, realPointList)[1], targetPos);
        //Debug.Log(string.Format("O:{0},X:{1},Y:{2}", virtualPointList.IndexOf(OPoint), virtualPointList.IndexOf(xyAxisPos[0]), virtualPointList.IndexOf(xyAxisPos[1])));
        GameObject _worker = Instantiate(workerPrefab, new Vector3d(resultPosition.x, normalHeight, resultPosition.y), Quaternion.identity, _parent);
        return _worker;
    }


    //计算p与所有已知顶点的距离中最近的点 的数组下标
    private int GetVectorOrginalPoint(Vector3d _targetPos, List<Vector3d> _realPointList)
    {
        //Vector3d _targetPos = Vector3d.zero;
        int _index = 0;
        //Debug.Log(_targetPos - _realPointList[0]);
        double _minDistence = (_targetPos - _realPointList[0]).magnitude;
        //Debug.Log(string.Format("Test NullException{0}--{1}", _targetPos, realPointList[0]));

        for (int i = 0; i < _realPointList.Count; ++i)
        {
            double _tempDis = (_targetPos - _realPointList[i]).magnitude;
            if (_tempDis < _minDistence)
            {
                _minDistence = _tempDis;
                _index = i;
            }
        }
        //Debug.Log(string.Format("起点O的坐标为:{0},序号为{1}", _realPointList[_index], _index));
        //Debug.Log(string.Format("list长度{0}", _realPointList.Count));
        return _index;
        //return _targetPos;
    }

    /// <summary>
    /// 计算在已知的最近点周围 x最近和y最近 构建二位坐标系
    /// </summary>
    /// <param name="_orginPos"></param>坐标系起始点O
    /// <param name="_vecList"></param>已知的点集
    /// <returns></returns>返回x方向和y方向的向量组
    private List<Vector3d> GetSurroundParts(int _index, Vector3d _orginPos, List<Vector3d> _vecList)
    {
        //Debug.Log("getsurroundParts------------------");
        List<Vector3d> _surroundPart = new List<Vector3d>();
        _surroundPart.Add(Vector3d.zero);
        _surroundPart.Add(Vector3d.zero);
        double _xMinDis = 100;
        double _yMinDis = 100;
        double _tempxDis = 0;
        double _tempyDis = 0;
        List<Vector3d> testPointList = new List<Vector3d>();
        //一般情况
        //左侧
        if (_index - 1 >= 0)
        {
            testPointList.Add(_vecList[_index - 1]);
        }
        //右侧
        if (_index + 1 <= _vecList.Count)
        {
            testPointList.Add(_vecList[_index + 1]);
        }
        //上
        if (_index + divideRatioX + 2 < _vecList.Count)
        {
            testPointList.Add(_vecList[_index + divideRatioX + 2]);
        }
        //下
        if (_index - divideRatioX - 2 >= 0)
        {
            testPointList.Add(_vecList[_index - divideRatioX - 2]);
        }
        for (int i = 0; i < testPointList.Count; i++)
        {
            //foreach (var item in testPointList)
            //{
            //    Debug.Log(string.Format("---------------------test surroundpos{0}", item));
            //}
            if (testPointList[i] == Vector3d.zero)
                continue;
            _tempxDis = Math.Abs((testPointList[i].x - _orginPos.x));
            if (_tempxDis != 0 && _tempxDis < _xMinDis)
            {
                _xMinDis = _tempxDis;
                _surroundPart[1] = testPointList[i];
            }
            _tempyDis = Math.Abs((testPointList[i].y - _orginPos.y));
            if (_tempyDis != 0 && _tempyDis < _yMinDis)
            {
                _yMinDis = _tempyDis;
                _surroundPart[0] = testPointList[i];
            }
        }
        //Debug.Log("x方向的点：" + _surroundPart[0] + ",y方向的点：" + _surroundPart[1]);
        return _surroundPart;
    }
    /// <summary>
    /// 获得定位点
    /// </summary>
    /// <param name="_orginPos"></param>起始点O
    /// <param name="_xAxisPos"></param>X轴方向
    /// <param name="_yAxisPos"></param>Y轴方向
    /// <param name="_targetPos"></param>待定位点P
    public Vector3d CalculatePosition(Vector3d _orginPos, Vector3d _virtualOrginPos, Vector3d _xAxisPos, Vector3d _yAxisPos, Vector3d _targetPos)
    {
        //Debug.Log("待确认点P" + _targetPos);
        //Debug.Log(string.Format("输入坐标P:{0},X:{1},Y:{2},O:{3}", _targetPos, _xAxisPos, _yAxisPos, _orginPos));
        Vector3d vecOX = _xAxisPos - _orginPos;
        Vector3d vecOY = _yAxisPos - _orginPos;

        Vector3d _vecOP = _targetPos - _orginPos;
        //Debug.Log(string.Format("OP-{0},OA-{1},OB-{2}", vecOX, vecOY, _vecOP));
        Vector3d virtualPosition = Vector3d.zero;
        virtualPosition.x = _virtualOrginPos.x + _vecOP.x * frameWidth / (divideRatioX + 1) / vecOX.magnitude;
        virtualPosition.y = _virtualOrginPos.y + _vecOP.y * frameHeight / (divideRatioY + 1) / vecOY.magnitude;
        virtualPosition.z = transform.position.z;
        //Debug.LogFormat(string.Format("所求坐标为{0}", virtualPosition));
        //Debug.LogFormat(string.Format("O点坐标{0}", _virtualOrginPos));
        //Debug.LogFormat(string.Format("向量OX为{0}", vecOX));
        //Debug.LogFormat(string.Format("向量OY为{0}", vecOY));
        return virtualPosition;
    }
}
