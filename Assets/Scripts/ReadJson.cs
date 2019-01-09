using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using System.IO;

using LitJson;

using UnityEngine.UI;

public class ReadJson : MonoBehaviour {


    public static ReadJson instance;

  //  public  Ntext ntext;

    private JsonData itemDate;

    private string jsonString;

 


    public IEnumerator initialization() {
        if (instance == null)
        {

            instance = this;

        }

     yield return   StartCoroutine(readJson());
    }

    IEnumerator readJson() {
        string spath = Application.streamingAssetsPath + "/information.json";

        Debug.Log(spath);

        WWW www = new WWW(spath);

        yield return www;

        jsonString = System.Text.Encoding.UTF8.GetString(www.bytes);

        JsonMapper.ToObject(www.text);

       itemDate = JsonMapper.ToObject(jsonString.ToString());


        for (int i = 0; i < itemDate["Setup"].Count; i++)
        {
            string name = itemDate["Setup"][i]["videoName"].ToString();
            string UDP = itemDate["Setup"][i]["UDP"].ToString();

        ValueSheet.videoName.Add  (name);
         ValueSheet.VideoUDP.Add(UDP);
         ValueSheet.UDP_Video_keyValuePairs.Add(UDP, name);
        }

        ValueSheet.TurnOnLight = itemDate["Commond"]["turnOnLight"].ToString();
        ValueSheet.TurnOffLight = itemDate["Commond"]["turnOffLight"].ToString();
        ValueSheet.backUDP = itemDate["Commond"]["back"].ToString();

        ValueSheet.ServerIP = itemDate["Commond"]["ServerIP"].ToString();
        ValueSheet.ServerPort = int.Parse( itemDate["Commond"]["ServerPort"].ToString());

        ValueSheet.IsVideoBG = itemDate["Commond"]["IsVideoBG"].ToString();

        ValueSheet.BgVideoPath = itemDate["Commond"]["BGVideo"].ToString();

    }


}
