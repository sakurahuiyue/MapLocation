using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetData());
        //StartCoroutine(GetWeatherData());
    }

    private IEnumerator GetData()
    {
        string weaid = "zhenjiang";
        string appkey = "53807";
        string sign = "da758d582b3cc05f58233c3b41ab92ca";
        string type = "json";
        string url = string.Format("http://api.k780.com/?app=weather.today&weaid={0}&appkey={1}&sign={2}&format={3}", weaid, appkey, sign, type);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
    }

    public IEnumerator GetWeatherData()
    {
        //http://api.k780.com/?app=weather.today&weaid=beijing&appkey=53807&sign=da758d582b3cc05f58233c3b41ab92ca&format=json
        string url = "https://sapi.k780.com";
        WWWForm form = new WWWForm();
        form.AddField("weaid", "beijing");
        form.AddField("appkey", "53807");
        form.AddField("sign", "da758d582b3cc05f58233c3b41ab92ca");
        //form.AddField("format", "json");
        //Debug.Log(string.Format("上传的值为{0}", _inputDate));
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("GetWeatherData" + webRequest.downloadHandler.text);
        }
    }
}
