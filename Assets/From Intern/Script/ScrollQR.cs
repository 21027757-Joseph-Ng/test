using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollQR : MonoBehaviour
{
    public GameObject relaunch;
    public GameObject yes;
    public GameObject mainMenu;

    private List<GameObject> list = new List<GameObject>();
    private int buttonIndex = 0;
    private AndroidJavaObject currentActivity;
    private const int REQUEST_CODE_SCAN_INFO = 7;
    // Start is called before the first frame update
    void Start()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        list.Add(relaunch);
        list.Add(yes);
        list.Add(mainMenu);

        GetChild(buttonIndex);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))//forward
        {
            Debug.Log("forward");

            if (buttonIndex == -1)
            {
                buttonIndex++;
                GetChild(buttonIndex);
            }


            else if (buttonIndex == 2)
            {
                buttonIndex++;
                DeactivateChild(buttonIndex - 1);
            }

            else if (buttonIndex != 3)// -1 0 1 2 3
            {
                buttonIndex++;
                GetChild(buttonIndex);
                DeactivateChild(buttonIndex - 1);
                Debug.Log(buttonIndex);
            }
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))//backward
        {
            Debug.Log("backward");
            if (buttonIndex == 0)
            {
                buttonIndex--;
                DeactivateChild(buttonIndex + 1);
            }

            else if (buttonIndex == 3)//biggest number? -1 0 1 2 3
            {
                buttonIndex--;
                GetChild(buttonIndex);
            }

            else if (buttonIndex != -1)//smallest number?
            {
                buttonIndex--;
                GetChild(buttonIndex);
                DeactivateChild(buttonIndex + 1);
                Debug.Log(buttonIndex);
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
        {
            if (buttonIndex != 3 && buttonIndex != -1)
            {
                buttonFunction(buttonIndex);
            }
        }
    }

    private void GetChild(int index)
    {


        GameObject child = list[index].gameObject;
        child.transform.Find("BackPlate").gameObject.SetActive(true);
        Debug.Log("get: " + child);
    }

    private void DeactivateChild(int index)
    {

        GameObject child = list[index].gameObject;
        child.transform.Find("BackPlate").gameObject.SetActive(false);
        Debug.Log("deactivate: " + child);
    }

    private void buttonFunction(int index)
    {
        switch (index)
        {
            case 0:
                LaunchQRScanner();
                break;
            case 1://
                string QRCode = yes.GetComponent<ClickedYes>().QRCode;
                currentActivity.Call("QRPopUp", QRCode);
                Debug.Log("button1");
                break;
            case 2://
                SceneManager.LoadScene("MainMenu");
                Debug.Log("button2");
                break;
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
