using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueSheet : MonoBehaviour {
    public static int width;

    public static int height;

    public static List<string> videoName = new List<string>();
    public static List<string> VideoUDP = new List<string>();

    public static Dictionary<string, string> UDP_Video_keyValuePairs = new Dictionary<string, string>();

    public static string TurnOnLight;
    public static string TurnOffLight;
    public static string backUDP;

    public static List<Sprite> BGsprite = new List<Sprite>();

    public static string ServerIP;

    public static int ServerPort;

    public static string IsVideoBG;

    public static string BgVideoPath;
}
