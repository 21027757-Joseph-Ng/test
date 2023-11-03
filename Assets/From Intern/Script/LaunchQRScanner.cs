using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchQRScanner : MonoBehaviour
{
    private AndroidJavaObject currentActivity;

    // Start is called before the first frame update
    private void OnEnable()
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
