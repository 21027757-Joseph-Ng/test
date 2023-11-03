using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickedYes : MonoBehaviour//save the qr code data here when scanned, click to open it up in browser
{

    public string QRCode;
    public GameObject URL;

    private bool m_Enter = false;
    private const int REQUEST_CODE_SCAN_INFO = 7;
    private AndroidJavaObject currentActivity;
    // Start is called before the first frame update
    private void Start()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_Enter) return;
        //if (Application.platform == RuntimePlatform.Android)
        //{
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
        { 
            Debug.Log("url2: " + QRCode);

            currentActivity.Call("QRPopUp", QRCode);
        }
        //}
    }

    public void QRCodes(string qrcode)
    {
        QRCode = qrcode;
        Debug.Log("url: " + QRCode);
        URL.GetComponent<TextMeshPro>().SetText("URL: " + QRCode);
    }
}
