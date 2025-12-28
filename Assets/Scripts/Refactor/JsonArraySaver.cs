using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

[System.Serializable]
public class ObjectSaveData
{
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class SceneSaveData
{
    public List<ObjectSaveData> objects;
}

public class JsonArraySaver : MonoBehaviour
{
    public static JsonArraySaver instance;
    string path;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        path = Application.persistentDataPath + "/array.json";
    }

    public void SaveObjects(List<GameObject> targets)
    {
        SceneSaveData saveData = new SceneSaveData();
        saveData.objects = new List<ObjectSaveData>();

        foreach (GameObject go in targets)
        {
            if(go!= null)
            {
                ObjectSaveData data = new ObjectSaveData
                {
                    tag = go.tag,
                    position = go.transform.position,
                    rotation = go.transform.rotation
                };

                saveData.objects.Add(data);
            }

        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);

        Debug.Log("Saved: " + path);
    }

    public void SaveobjectsInObjectSpawner()
    {
        SaveObjects(ObjectSpawner.instance.objectToAdded);
    }
    public SceneSaveData Load()
    {
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SceneSaveData>(json);
    }

    public void Restore()
    {
        SceneSaveData data = Load();
        if (data == null) return;
        //删除场景中存留信息和物体，防止重复生成
        ClearGameObjects(ObjectSpawner.instance.gameObject);
        ObjectSpawner.instance.objectToAdded.Clear();

        foreach (ObjectSaveData obj in data.objects)
        {
            GameObject gameObject = null;
            switch (obj.tag)
            {
                case "Cube":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[0], obj.position, obj.rotation, ObjectSpawner.instance.gameObject.transform);
                    break;
                case "Chest":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[1], obj.position, obj.rotation, ObjectSpawner.instance.gameObject.transform);
                    break;
                case "Princess":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[2], obj.position, obj.rotation, ObjectSpawner.instance.gameObject.transform);
                    break;
                case "Goblin":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[3], obj.position, obj.rotation, ObjectSpawner.instance.gameObject.transform);
                    break;
                case "Dialog":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[4], obj.position, obj.rotation, ObjectSpawner.instance.gameObject.transform);
                    break;
                default:
                    Debug.Log("No Tag!!!");
                    continue;
            }
            ObjectSpawner.instance.objectToAdded.Add(gameObject);
        }
    }

    private void ClearGameObjects(GameObject parent)
    {
        // 先缓存所有子物体，避免 foreach 直接修改 Transform 抛异常
        Transform[] children = new Transform[parent.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = parent.transform.GetChild(i);
        }

        // 删除
        foreach (Transform child in children)
        {
            Destroy(child.gameObject); // 延迟销毁，等到这一帧结束
        }
    }

}
