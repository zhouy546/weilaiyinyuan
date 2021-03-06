﻿using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCtr : MonoBehaviour {

    public static Action<string> PlayerVideo;
    public static Action StopVideo;
    public MediaPlayer mediaPlayer;
    public Animator animator;
    // Use this for initialization
	public void initialization() {
        stop();
	}

    private void OnEnable()
    {
        PlayerVideo += play;
        StopVideo += stop;
    }

    private void OnDisable()
    {
        PlayerVideo -= play;
        StopVideo -= stop;
    }

    public static void playVideo(string str) {
        PlayerVideo?.Invoke(str);
    }

    public static void stopVideo() {
        StopVideo?.Invoke();
    }


    private void play(string str)
    {
        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, str, true);
        animator.SetBool("Show", true);
     
        //Debug.Log(ValueSheet.TurnOffLight);

        if (str != ValueSheet.BgVideoPath) {
            SendUPDData.instance.udp_Send(ValueSheet.TurnOffLight);//关灯
            mediaPlayer.Control.SetLooping(false);
        }
    }

    private void stop() {

        if (ValueSheet.IsVideoBG == "true") {
            play(ValueSheet.BgVideoPath);
            mediaPlayer.Control.SetLooping(true);

            //animator.SetBool("Show", false);
            //Debug.Log(ValueSheet.TurnOnLight);
            SendUPDData.instance.udp_Send(ValueSheet.TurnOnLight);//开灯
        }
        else if(ValueSheet.IsVideoBG == "false")
        {
            mediaPlayer.Stop();
            animator.SetBool("Show", false);
            //Debug.Log(ValueSheet.TurnOnLight);
            SendUPDData.instance.udp_Send(ValueSheet.TurnOnLight);//开灯

        }



    }

    public void onVideoFinished() {
        stop();


    }

}
