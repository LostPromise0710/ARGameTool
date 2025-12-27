using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using System.IO;


public class GestureRecognition : MonoBehaviour
{
    public static GestureRecognition Instance;

    private void Awake()
    {
        Instance = this;
    }
    public string apiKey = "WceuaOmtBTLYDNg4lXaDuR0b";
    public string secretKey = "dvRVsbKVrqzAIPWBzEvxz3Ps0oUQOCoR";
    private string accessToken;
    private WebCamTexture webCamTexture;

    // 当前工作以手势识别为主体，后续扩展方向为当手势识别为空，即不是手势时，转而进行广义通用的图像识别，以植物、建筑为主要分析对象，将识别结果返回至大语言模型，进行检索和生成

    // 手势字典
    public Dictionary<string, string> recognizedGestures = new Dictionary<string, string>
    {
        { "1", "One" },
        { "2", "Five" },
        { "3", "Fist" },
        { "4", "Ok" },
        { "5", "Prayer" },
        { "6", "Congratuation" },
        { "7", "Honour" },
        { "8", "Heart_single" },
        { "9", "Thumb_up" },
        { "10", "Thumb_down" },
        { "11", "ILY" },
        { "12", "Palm_up" },
        { "13", "Heart_1" },
        { "14", "Heart_2" },
        { "15", "Heart_3" },
        { "16", "Two" },
        { "17", "Three" },
        { "18", "Four" },
        { "19", "Six" },
        { "20", "Seven" },
        { "21", "Eight" },
        { "22", "Nine" },
        { "23", "Rock" },
        { "24", "Insult" }
    };

    // 用来保存协程的引用
    private Coroutine captureCoroutine;

    /*
    // 点击事件
    public Button EditButton;
    public Button FinishButton;
    */

    // 摄像头实时情况
    public Image Image;

    // 音乐控制类
    public MusicPlayer musicPlayer;

    // 存储识别到的手势序列
    private List<string> gestureSequence = new List<string>();

    // 目标手势序列来播放音乐
    private string targetOpenSequence;
    // 目标手势序列来关闭音乐
    private string targetStopSequence;
    // 目标手势序列来切换音乐
    private string targetNextSongSequence;
    // 目标手势序列最大长度
    private int sequenceMaxLength = 10;
    // 启动手势列表(将启动手势的设定交换给用户，即在UI界面编辑时，从用户选择中添加到队列中) 
    private List<string> startGestures = new List<string>();

    /*
    // 目前实现以文本输入，之后将迭代为选择按钮或者图片
    public TMP_InputField openMusicInput;
    public TMP_InputField stopMusicInput;
    public TMP_InputField nextSongInput;
    */
    // 虚拟手部模型的Transform组件
    public Transform handModelTransform;

    // 临时虚拟物体
    public Transform TestModleTransform;

    // 保存上一次碰撞的虚拟物体
    private GameObject lastCollidedObject;

    void Start()
    {
        StartCoroutine(GetAccessToken());

        // 初始化WebCamTexture并开始捕获
        webCamTexture = new WebCamTexture();
        // 将摄像头的输出设置为UI Image的纹理
        Image.material.mainTexture = webCamTexture;
        webCamTexture.Play();

        StartGestureRecognition();
        /*
        // 绑定按钮点击事件
        EditButton.onClick.AddListener(StartGestureRecognition);
        FinishButton.onClick.AddListener(StopRecognitionCoroutine);
        */
    }

    IEnumerator GetAccessToken()
    {
        string authUrl = $"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={apiKey}&client_secret={secretKey}";
        using (UnityWebRequest www = UnityWebRequest.Get(authUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                accessToken = JsonUtility.FromJson<AccessTokenResponse>(www.downloadHandler.text).access_token;
            }
        }
    }

    IEnumerator GestureRecognize(string base64Image)
    {
        string gestureUrl = "https://aip.baidubce.com/rest/2.0/image-classify/v1/gesture";
        string urlWithAccessToken = $"{gestureUrl}?access_token={accessToken}";

        WWWForm form = new WWWForm();
        form.AddField("image", base64Image);

        using (UnityWebRequest www = UnityWebRequest.Post(urlWithAccessToken, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // 解析返回的JSON，提取手势识别结果
                Debug.Log(www.downloadHandler.text);
                // 假设API返回包含手势编号的JSON对象
                GestureResponse response = JsonUtility.FromJson<GestureResponse>(www.downloadHandler.text);
                if (response.result != null && response.result.Length > 0)
                {
                    string gestureName = response.result[0].classname;
                    ProcessGesture(gestureName);
                    GestureResult hand = response.result[0];
                    UpdateVirtualHand(hand);
                }
                else
                {
                    Debug.Log("No gesture result found in response.");
                }
            }
        }
    }

    // 检查虚拟物体是不是在摄像机中心视野附近
    public bool IsCameraPointingAtObject(GameObject obj)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen && (screenPoint - new Vector3(0.5f, 0.5f, 0)).magnitude < 0.1;
    }


    void UpdateVirtualHand(GestureResult hand)
    {
        // 计算虚拟手部的位置和旋转
        float screenX = hand.left + hand.width / 2f;
        float screenY = hand.top + hand.height / 2f;

        // 转换为世界坐标
        // 10f是z轴距离摄像头的距离
        Vector3 worldPos = new Vector3(screenX / Screen.width, 1f - screenY / Screen.height, 10f);
        Vector3 handPosition = Camera.main.ViewportToWorldPoint(worldPos);

        // 更新虚拟手部的位置
        handModelTransform.position = handPosition;

        // 获取手的世界坐标
        Vector3 handWorldPosition = handModelTransform.position;

        Vector3 Testposition = TestModleTransform.position;

        // 输出虚拟物体的位置信息
        Debug.Log("Virtual Object Name: " + TestModleTransform.name + ", Position: " + Testposition);

        // 定义一个球形区域，用于检测碰撞
        float interactionRange = 150f; // 碰撞检测范围

        // 获取位于球形区域内的所有碰撞器
        Collider[] hitColliders = Physics.OverlapSphere(handWorldPosition, interactionRange);

        // 遍历检测到的碰撞器，查找与虚拟物体的碰撞
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("VirtualObject"))
            {
                // 如果与上一次碰撞的虚拟物体相同，则不重新播放音效
                if (lastCollidedObject != hitCollider.gameObject)
                {
                    // 执行交互操作
                    hitCollider.GetComponent<Renderer>().material.color = Color.red;
                    Debug.Log("Successful Touch!!!");

                    // 输出被检测到的虚拟物体的名称和位置信息
                    Debug.Log("Detected Virtual Object: " + hitCollider.gameObject.name);
                    Debug.Log("Virtual Object Position: " + hitCollider.transform.position);

                    // 播放碰撞音效
                    musicPlayer.PlayCollisionSound();

                    // 将当前碰撞的虚拟物体记录为上一次碰撞的物体
                    lastCollidedObject = hitCollider.gameObject;
                }
            }
        }

        // 根据百度AI返回的数据设置虚拟手部的旋转
        // ...

        Debug.Log($"Updated virtual hand position to {handPosition}");
    }

    void CaptureAndRecognizeCamera()
    {
        if (captureCoroutine == null)
        {
            captureCoroutine = StartCoroutine(CaptureAndRecognizeRoutine());
            Debug.Log("Recognition Coroutine Started.");
        }
    }

    void StopRecognitionCoroutine()
    {
        if (captureCoroutine != null)
        {
            StopCoroutine(captureCoroutine); // 使用保存的引用来停止协程
            captureCoroutine = null; // 清除引用
            Debug.Log("Recognition Coroutine Stopped.");
        }
    }

    [System.Serializable]
    public class GestureResponse
    {
        public GestureResult[] result;
    }

    [System.Serializable]
    public class GestureResult
    {
        public string classname;
        public int top;
        public int left;
        public float probability;
        public int width;
        public int height;
    }



    // 检查序列是否包含特定手势组合
    bool CheckGestureSequence(string targetSequence)
    {
        string currentSequence = string.Join("-", gestureSequence);
        return currentSequence.Contains(targetSequence);
    }

    void ProcessGestureSequence(List<string> gestureSequence)
    {
        AudioSource musicPlayerAudioSource = musicPlayer.GetAudioSource();

        string sequence = string.Join("-", gestureSequence);

        // 检查是否识别到了目标开启序列
        if (CheckGestureSequence(targetOpenSequence))
        {
            // 如果识别到序列，则播放音乐
            if (musicPlayerAudioSource != null && !musicPlayerAudioSource.isPlaying)
            {
                Debug.Log("Open Match!");
                musicPlayer.PlaySong();
            }
            gestureSequence.Clear(); // 清空序列，避免重复播放
        }

        // 检查是否识别到了目标关闭序列
        if (CheckGestureSequence(targetStopSequence))
        {
            // 如果识别到序列，则停止播放音乐
            if (musicPlayerAudioSource != null && musicPlayerAudioSource.isPlaying)
            {
                Debug.Log("Close Match!");
                musicPlayer.StopSong();
            }
            gestureSequence.Clear(); // 清空序列，避免重复关闭
        }

        // 检查序列是否包含切换歌曲的手势组合
        if (CheckGestureSequence(targetNextSongSequence))
        {
            // 如果序列匹配
            if (musicPlayerAudioSource != null && musicPlayerAudioSource.isPlaying)
            {
                Debug.Log("Switching song!");
                musicPlayer.NextSong();
            }
            gestureSequence.Clear(); // 清空序列
        }
    }

    void ProcessGesture(string gestureName)
    {
        // 先检查手势是否为启动手势列表中的一个
        if (startGestures.Contains(gestureName))
        {
            // 清空当前序列
            gestureSequence.Clear();
            // 添加新的启动手势到序列中
            gestureSequence.Add(gestureName);
            // 打印启动新序列的信息
            Debug.Log("Started new gesture sequence with: " + gestureName);
        }
        else if (gestureName != null && gestureName != "Face")
        {
            // 如果不是启动手势且有效，添加到序列中
            gestureSequence.Add(gestureName);
            // 检查序列长度是否达到上限，如果达到，也清空重新开始
            if (gestureSequence.Count >= sequenceMaxLength)
            {
                Debug.Log("Gesture sequence reached max length. Starting new sequence.");
                gestureSequence.Clear();
                gestureSequence.Add(gestureName);
            }
        }

        // 打印当前手势序列
        Debug.Log("Current gesture sequence: " + string.Join("-", gestureSequence));

        ProcessGestureSequence(gestureSequence);
    }

    void StartGestureRecognition()
    {
        // 设置目标手势序列
        targetOpenSequence = "Six-Five";
        targetStopSequence = "Ok-Two";
        targetNextSongSequence = "Seven-Four";

        Debug.Log("targetOpenSequence: " + targetOpenSequence);
        Debug.Log("targetStopSequence: " + targetStopSequence);
        Debug.Log("targetNextSongSequence: " + targetNextSongSequence);

        // 记录启动手势
        startGestures.Add(targetOpenSequence.Split("-")[0]);
        startGestures.Add(targetStopSequence.Split("-")[0]);
        startGestures.Add(targetNextSongSequence.Split("-")[0]);

        Debug.Log("Gesture recognition started.");
        CaptureAndRecognizeCamera(); // 开始捕获和识别手势
    }

    // 创建公共方法来允许外部调用
    public void BeginGestureRecognition()
    {
        StartGestureRecognition();
    }

    IEnumerator CaptureAndRecognizeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            Texture2D capturedTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
            capturedTexture.SetPixels(webCamTexture.GetPixels());
            capturedTexture.Apply();

            byte[] bytes = capturedTexture.EncodeToJPG();
            string base64Image = System.Convert.ToBase64String(bytes);

            // 如果成功获取到Base64编码，则识别手势
            if (!string.IsNullOrEmpty(base64Image))
            {
                StartCoroutine(GestureRecognize(base64Image));
            }
            else
            {
                Debug.LogError("Failed to capture image from camera.");
            }

            // 释放Texture2D对象，避免内存泄露
            Destroy(capturedTexture);
        }
    }

    [System.Serializable]
    public class AccessTokenResponse
    {
        public string access_token;
        public string expires_in;
    }

}
