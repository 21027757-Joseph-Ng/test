using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickedQRScanners : MonoBehaviour
{
    private bool m_Enter = false;
    private const int REQUEST_CODE_SCAN_INFO = 6;
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
            Activate();
        }
    }
    public void Activate()
    {
        SceneManager.LoadScene("QRScanner");
    }
}
