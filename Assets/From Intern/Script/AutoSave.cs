using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Android;

public class AutoSave : MonoBehaviour
{
    public string fileName = "saveData.txt";
    private string fileContent;


    // Start is called before the first frame update
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        Debug.Log("start: " + SaveFile.isToggle);
        Debug.Log(Application.persistentDataPath);
    }

    private void Update()
    {
        if(SaveFile.isToggle == false)
        {
            SaveFile.isToggle = true; // Set the flag here to prevent further loading
            Debug.Log("changed");
            LoadFile();
            if (fileContent.Length != 0)
            {
                SceneManager.LoadScene(fileContent);
            }
        }
        CreateAndWriteTextFile();
    }

    public void CreateAndWriteTextFile()
    {
        string fileContent = SceneManager.GetActiveScene().name;
        string filePath = Path.Combine("/sdcard/Documents", fileName);

        // Create a StreamWriter to write to the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the content to the file
            writer.Write(fileContent);
        }
        Debug.Log("Text file created at: " + filePath);
    }

    public void LoadFile()
    {
        string filePath = Path.Combine("/sdcard/Documents", fileName);

        if (File.Exists(filePath))
        {
            try
            {
                fileContent = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
        }
    }
}