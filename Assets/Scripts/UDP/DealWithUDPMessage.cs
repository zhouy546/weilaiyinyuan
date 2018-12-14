//*********************❤*********************
// 
// 文件名（File Name）：	DealWithUDPMessage.cs
// 
// 作者（Author）：			LoveNeon
// 
// 创建时间（CreateTime）：	Don't Care
// 
// 说明（Description）：	接受到消息之后会传给我，然后我进行处理
// 
//*********************❤*********************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class DealWithUDPMessage : MonoBehaviour {
    public static DealWithUDPMessage instance;

    private string dataTest;
  
    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="_data"></param>
    public void MessageManage(string _data)
    {

        if (_data != "")
        {
            dataTest = _data;

            Debug.Log(dataTest);

            if (ValueSheet.UDP_Video_keyValuePairs.ContainsKey(dataTest))//播放片子1
            {
                VideoCtr.playVideo(ValueSheet.UDP_Video_keyValuePairs[dataTest]);
            }
            else if (dataTest == ValueSheet.backUDP)//返回
            {
                VideoCtr.stopVideo();
            }
        }
       
    }

    

    public void Start()
    {
        if (instance == null) {
            instance = this;
        }
    }


    private void Update()
    {

    }

}
