using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.XR.ARSubsystems;
using System.IO;
using Unity.Barracuda;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Events;
using TMPro;

public class DetectLocal : MonoBehaviour
{
    public string INPUT_NAME;
    public string OUTPUT_NAME;
    UnityAction<byte[]> callback;
    public RawImage rawImage;
    const int IMAGE_SIZE = 224;
    Vector2 offset = Vector2.zero;
    Vector2 scale = new Vector2(1, 1);
    string[] labels;
    public TextAsset labelAsset;
    public GameObject text;

    public string labelMappingFilePath = "labels_map"; // Adjust the path as needed
    public NNModel squeezeNetModel; // Assign your SqueezeNet ONNX model in the Unity Inspector

    private List<string> _labels = new List<string>();
    private float[] resultProbabilities;

    private Model model;
    private IWorker worker;

    private bool m_Enter = false;

    private const int REQUEST_CODE_CAPTURE_PHOTO = 2;

    private AndroidJavaObject currentActivity;
    private AndroidJavaObject intentObject;

    public static DetectObject Instance;

    string deviceName;
    WebCamTexture webCam;
    public RawImage videoPreview;
    public GameObject videoObject;
    public GameObject handGestureObject;
    Texture2D sourceTex2D;
    RenderTexture renderTexture;
    private float elapseTime;
    public int FPS = 30;
    public GameObject objectParent;

    private bool startRecord = true;

    public GameObject images;

    private static float probabilityThreshold = 0.6f;
    Camera MRTKCam;
    RaycastHit rayCastHit;
    private static int _meshPhysicsLayer = 0;

    [SerializeField]
    private GameObject objectPrefab;

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

    private void Start()
    {
        //InitVideo(1280, 720, 30);
        //ToggleGes("true");

        //var model = ModelLoader.Load(squeezeNetModel);
        //worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        //LoadLabels();
        model = ModelLoader.Load(squeezeNetModel);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);

        TextAsset labelMappingAsset = Resources.Load<TextAsset>(labelMappingFilePath);

        using (var streamReader = new StringReader(labelMappingAsset.text))
        {
            Debug.Log(3);
            string line = "";
            char[] charToTrim = { '\"', ' ' };
            Debug.Log(4);
            while (streamReader.Peek() >= 0)
            {
                line = streamReader.ReadLine();
                line.Trim(charToTrim);
                var indexAndLabel = line.Split(':');
                if (indexAndLabel.Count() == 2)
                {
                    _labels.Add(indexAndLabel[1]);
                }
            }
        }
    }

    void OnDestroy()
    {
        worker.Dispose();
    }

    private void Update()
    {

        //if (startRecord)
        //{
        //    elapseTime += Time.deltaTime;
        //    if (elapseTime > 1.0f / FPS)
        //    {
        //        elapseTime = 0;
        //        sourceTex2D = TextureToTexture2D(webCam);
        //        if (sourceTex2D == null)
        //        {
        //            Debug.Log("SourceTex2D is Null !!!");
        //        }
        //        else
        //        {
        //            Debug.Log("Enter Data to Buffer");
        //            UXRGesCamera.Instance.EnterVideoFrameBuffer(new UXRGesCamera.VideoFrame()
        //            {
        //                frameData = sourceTex2D.GetRawTextureData()
        //            });
        //        }
        //    }
        //}
        //}
    }
    //try to startactivityresult in unity and onactivityresult from java file
    private void OnEnable()
    {
        MRTKCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void CapturePhoto()
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        intentObject = new AndroidJavaObject("android.content.Intent", "android.media.action.IMAGE_CAPTURE");

        currentActivity.Call("startActivityForResult", intentObject, REQUEST_CODE_CAPTURE_PHOTO);
    }

    //void LoadLabels()
    //{
    //    //get only items in quotes
    //    var stringArray = labelAsset.text.Split('"').Where((item, index) => index % 2 != 0);
    //    //get every other item
    //    labels = stringArray.Where((x, i) => i % 2 != 0).ToArray();
    //}

    //public static byte[] ScaleAndCropImage(byte[] imageData, int originalWidth, int originalHeight, int desiredSize)
    //{
    //    Debug.Log(1);
    //    Texture2D originalTexture = new Texture2D(originalWidth, originalHeight);
    //    Debug.Log(2);
    //    originalTexture.LoadImage(imageData); // Load the image data into a Texture2D.

    //    Debug.Log(3);
    //    float aspectRatio = (float)originalWidth / originalHeight;
    //    Debug.Log(4);
    //    int newWidth, newHeight;

    //    Debug.Log(5);
    //    if (originalWidth > originalHeight)
    //    {
    //        Debug.Log(6);
    //        newWidth = desiredSize;
    //        Debug.Log(7);
    //        newHeight = Mathf.RoundToInt(desiredSize / aspectRatio);
    //    }
    //    else
    //    {
    //        Debug.Log(8);
    //        newWidth = Mathf.RoundToInt(desiredSize * aspectRatio);
    //        Debug.Log(9);
    //        newHeight = desiredSize;
    //    }

    //    Debug.Log(10);
    //    Texture2D scaledTexture = ScaleTexture(originalTexture, newWidth, newHeight);

    //    // Now, you can save the scaled and cropped image as a byte array or in your desired format.
    //    Debug.Log(11);
    //    byte[] processedImageData = scaledTexture.EncodeToPNG();
    //    Debug.Log(12);
    //    return processedImageData;
    //}

    //private static Texture2D ScaleTexture(Texture2D source, int newWidth, int newHeight)
    //{
    //    RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
    //    RenderTexture.active = rt;

    //    Texture2D scaledTexture = new Texture2D(newWidth, newHeight);
    //    Graphics.Blit(source, rt);
    //    scaledTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
    //    scaledTexture.Apply();

    //    RenderTexture.active = null;
    //    RenderTexture.ReleaseTemporary(rt);

    //    return scaledTexture;
    //}

    //void RunModel(byte[] pixels)
    //{
    //    Texture2D tex = new Texture2D(1, 1);
    //    tex.LoadImage(pixels); // Load image from byte array
    //    Texture2D inputTex = LoadImage(tex.EncodeToPNG());
    //    Texture2D resizedAndCroppedTex = ResizeAndCrop(inputTex);
    //    byte[] byteArray = resizedAndCroppedTex.EncodeToPNG();

    //    Debug.Log(13);
    //    StartCoroutine(RunModelRoutine(byteArray));
    //}

    //IEnumerator RunModelRoutine(byte[] pixels)
    //{
    //    Debug.Log(14);
    //    Tensor tensor = TransformInput(pixels);

    //    Debug.Log(15);
    //    var inputs = new Dictionary<string, Tensor> {
    //        { INPUT_NAME, tensor }
    //    };

    //    Debug.Log(16);
    //    worker.Execute(inputs);
    //    Debug.Log(17);
    //    Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);

    //    // Get the top N predictions
    //    Debug.Log(18);
    //    int topN = 3;
    //    Debug.Log(19);
    //    List<int> topIndices = GetTopIndices(outputTensor, topN);

    //    // Display the top N labels and their percentages
    //    Debug.Log(20);
    //    string resultText = "Top " + topN + " Predictions:\n";
    //    Debug.Log(21);
    //    for (int i = 0; i < topN; i++)
    //    {
    //        int index = topIndices[i];
    //        float probability = outputTensor[index];
    //        string label = labels[index];
    //        resultText += $"{label}: {probability:P2}\n";
    //    }

    //    // Set UI text
    //    Debug.Log(22);
    //    text.GetComponent<TMP_Text>().text = resultText;
    //    Debug.Log("new: " + resultText);

    //    // Dispose tensors
    //    tensor.Dispose();
    //    Debug.Log(23);
    //    outputTensor.Dispose();
    //    Debug.Log(24);
    //    yield return null;
    //}

    //List<int> GetTopIndices(Tensor tensor, int topN)
    //{
    //    List<float> temp = tensor.ToReadOnlyArray().ToList();
    //    List<int> topIndices = temp.Select((value, index) => new { Value = value, Index = index })
    //        .OrderByDescending(item => item.Value)
    //        .Take(topN)
    //        .Select(item => item.Index)
    //        .ToList();
    //    return topIndices;
    //}

    //Tensor TransformInput(byte[] pixels)
    //{
    //    Debug.Log(25);
    //    float[] transformedPixels = new float[pixels.Length];
    //    Debug.Log(26);
    //    for (int i = 0; i < pixels.Length; i++)
    //    {

    //        transformedPixels[i] = (pixels[i] - 127f) / 128f;
    //    }
    //    Debug.Log(28);
    //    return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels, "test");
    //}

    public async void DetectObjects(string photoData)
    {
        Debug.Log("detect local");
        byte[] photoBytes = System.Convert.FromBase64String(photoData);

        //RunModel(ScaleAndCropImage(photoBytes, 2, 2, IMAGE_SIZE));
        //RunModel(photoBytes);
        ComputeBuffer computeBuffer = PreprocessImage(photoBytes);
        Postprocess(computeBuffer, 3);
    }

    public void onClickButton()
    {
        //webCam.Stop(); // To Test
        //videoObject.SetActive(false);
        //startRecord = false;

        Debug.Log("2");
        CapturePhoto();
        Debug.Log("1");
    }

    private Texture2D LoadImage(byte[] photoBytes)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(photoBytes); // Load image from byte array
        return tex;
    }

    private Texture2D ResizeAndCrop(Texture2D inputTex)
    {
        int targetSize = 224;
        Texture2D resizedAndCropped = new Texture2D(targetSize, targetSize);

        // Resize
        RenderTexture rt = new RenderTexture(targetSize, targetSize, 24);
        RenderTexture.active = rt;
        Graphics.Blit(inputTex, rt);
        resizedAndCropped.ReadPixels(new Rect(0, 0, targetSize, targetSize), 0, 0);
        resizedAndCropped.Apply();

        return resizedAndCropped;
    }

    private float[] Normalize(Texture2D inputTex)
    {
        float meanR = 0.485f;
        float meanG = 0.456f;
        float meanB = 0.406f;
        float stdR = 0.229f;
        float stdG = 0.224f;
        float stdB = 0.225f;

        Color[] pixels = inputTex.GetPixels();

        float[] normalizedPixels = pixels.Select(p =>
        {
            float r = (p.r - meanR) / stdR;
            float g = (p.g - meanG) / stdG;
            float b = (p.b - meanB) / stdB;
            return new float[] { r, g, b };
        }).SelectMany(x => x).ToArray();

        return normalizedPixels;
    }

    public ComputeBuffer PreprocessImage(byte[] photoBytes)
    {
        Texture2D inputTex = LoadImage(photoBytes);
        Texture2D resizedAndCroppedTex = ResizeAndCrop(inputTex);
        float[] normalizedPixels = Normalize(resizedAndCroppedTex);

        ComputeBuffer inputBuffer = new ComputeBuffer(normalizedPixels.Length, sizeof(float));
        inputBuffer.SetData(normalizedPixels);

        return inputBuffer;
    }

    public List<Dictionary<string, object>> Postprocess(ComputeBuffer outputBuffer, int numClasses)
    {
        float[] outputData = new float[numClasses];
        outputBuffer.GetData(outputData);

        float[] outputData2 = new float[1000];
        outputBuffer.GetData(outputData2);

        // Convert the output data to probabilities using softmax
        float[] probabilities = Softmax(outputData);
        resultProbabilities = Softmax(outputData2);

        // Create a list of dictionaries containing percentage and item (class label)
        List<Dictionary<string, object>> sortedItems = new List<Dictionary<string, object>>();
        for (int i = 0; i < numClasses; i++)
        {
            // Replace 'i' with the appropriate class label based on your label mapping
            string classLabel = GetClassLabel(i); // You need to implement this method

            Dictionary<string, object> item = new Dictionary<string, object>
        {
            { "percentage", probabilities[i] },
            { "item", classLabel }
        };
            sortedItems.Add(item);//add dictionary into list, each dictionary is different percentage + detected item
        }

        // Sort the list by percentage in descending order
        sortedItems.Sort((a, b) => -((float)a["percentage"]).CompareTo((float)b["percentage"]));

        string results = "";
        // Debug log the sorted items
        foreach (var item in sortedItems)
        {
            Debug.Log("Percentage: " + item["percentage"] + ", Item: " + item["item"]);
            results += "Percentage: " + item["percentage"] + ", Item: " + item["item"];
        }
        text.GetComponent<TMP_Text>().text = results;
        // Return the sorted items
        return sortedItems;
    }

    public string GetClassLabel(int index)//this return the label of the detected object
    {
        try
        {
            // Parse labels from label file
            //algorithm for getting true label
            int trueIndex = 0;
            //
            var topProbabilities = new float[3];
            var topProbabilityLabelIndexes = new int[3];
            for (int i = 0; i < resultProbabilities.Count(); i++)//resultprobabilities contain only probability element and maybe up to how many json file said, aka 1000
            {
                for (int j = 0; j < 3; j++)
                {
                    if (resultProbabilities[i] > topProbabilities[j])//check to see if the current probability is the biggest in topprobability
                    {
                        topProbabilityLabelIndexes[j] = i;
                        trueIndex = i;
                        topProbabilities[j] = resultProbabilities[i];
                        break;//there is break, so the nested for loop is for the purpose of sorting order of percentage, bind with the index of detected index by different list with same index
                    }
                }
            }
            //
            return _labels[topProbabilityLabelIndexes[index]];
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading label mapping: " + e.Message);
        }
        // If the label mapping doesn't exist or there's an error, return "Unknown"
        return "Unknown";
    }


    private float[] Softmax(float[] input)
    {
        float maxVal = input.Max();
        float sum = 0.0f;
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = Mathf.Exp(input[i] - maxVal);
            sum += input[i];
        }
        for (int i = 0; i < input.Length; i++)
        {
            input[i] /= sum;
        }
        return input;
    }

}

