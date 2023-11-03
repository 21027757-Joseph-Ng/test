using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoiceQRScanner : MonoBehaviour
{
    public GameObject clickedYes;
    private AndroidJavaObject currentActivity;
    private void Start()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
    }

    private void Awake()
    {
        RegisterCommand();
    }

    private void OnDestroy()
    {
        UnRegisterCommand();
    }

    private void RegisterCommand()
    {
        VoiceCommandLogic.Instance.AddInstrucEntity(1, "main menu", true, true, true, this.gameObject.name, "ColorRes", "main menu");
        VoiceCommandLogic.Instance.AddInstrucEntity(1, "open", true, true, true, this.gameObject.name, "ColorRes", "open");
        VoiceCommandLogic.Instance.AddInstrucEntity(1, "relaunch", true, true, true, this.gameObject.name, "ColorRes", "relaunch");
    }

    private void UnRegisterCommand()
    {
        VoiceCommandLogic.Instance.RemoveInstruct(1, "main menu");
        VoiceCommandLogic.Instance.RemoveInstruct(1, "open");
        VoiceCommandLogic.Instance.RemoveInstruct(1, "relaunch");
    }


    private void ColorRes(string msg)
    {
        Debug.LogError("ColorRes");

        if (string.Equals("main menu", msg))
        {
            Debug.LogError("Main menu");
            SceneManager.LoadScene("MainMenu");
        }
        else if (string.Equals("open", msg))
        {
            Debug.LogError("open");
            string QRCode = clickedYes.GetComponent<ClickedYes>().QRCode;
            currentActivity.Call("QRPopUp", QRCode);
        }
        else if (string.Equals("relaunch", msg))
        {
            Debug.LogError("relaunch");
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
