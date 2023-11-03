using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class AutoSave2 : MonoBehaviour
{
    public string fileName = "saveData.txt";
    // Start is called before the first frame update
    private void OnEnable()
    {
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
}
