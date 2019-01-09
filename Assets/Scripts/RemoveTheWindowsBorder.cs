using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class RemoveTheWindowsBorder : MonoBehaviour
{

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;  //边框用的
    const int WS_BORDER = 1;
    const int WS_POPUP = 0x800000;

    int _posX = 0;
    int _posY = 0;
    //int _Txtwith = 4352;
   // int _Txtheight = 5248;
    public IEnumerator initialization()
    {
        Screen.SetResolution(ValueSheet.width, ValueSheet.height, false);
        yield return StartCoroutine("Setposition");
        //StartCoroutine("ReSetposition");
    }


    private void Update()
    {
        
    }

    IEnumerator Setposition()
    {
        yield return new WaitForSeconds(0.1f);		//不知道为什么发布于行后，设置位置的不会生效，我延迟0.1秒就可以
        SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);      //无边框
        bool result = SetWindowPos(GetForegroundWindow(), 0, _posX, _posY, ValueSheet.width, ValueSheet.height, SWP_SHOWWINDOW);       //设置屏幕大小和位置
    }

    //IEnumerator ReSetposition()
    //{
    //    yield return new WaitForSeconds(1f);		//不知道为什么发布于行后，设置位置的不会生效，我延迟0.1秒就可以
    //    SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);      //无边框
    //    bool result = SetWindowPos(GetForegroundWindow(), 0, _posX, _posY, _Txtwith, _Txtheight, SWP_SHOWWINDOW);       //设置屏幕大小和位置
    //}
}
