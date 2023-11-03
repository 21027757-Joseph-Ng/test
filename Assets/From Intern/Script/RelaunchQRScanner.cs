using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaunchQRScanner : MonoBehaviour
{
    private bool m_Enter = false;
    private AndroidJavaObject currentActivity;

    public void OnPointerEnter()
    {
        m_Enter = true;
        VoiceCommandLogic.Instance.AddInstrucEntityZH("打开", "da kai", true, true, true, this.gameObject.name, "Open", "打开");
    }

    public void OnPointerExit()
    {
        m_Enter = false;
        VoiceCommandLogic.Instance.RemoveInstructZH("打开");
    }

    private void Update()
    {
        if (!m_Enter) return;

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("1");
            LaunchQRScanner();
        }
    }

    public void LaunchQRScanner()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        AndroidJavaObject comp = new AndroidJavaObject("android.content.ComponentName",
            "com.rokid.glass.scan2",
            "com.rokid.glass.scan2.activity.QrCodeActivity");
        intent.Call<AndroidJavaObject>("setComponent", comp);
        currentActivity.Call("startActivityForResult", intent, 6);
    }
}
