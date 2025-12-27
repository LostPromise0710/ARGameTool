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

        foreach (ObjectSaveData obj in data.objects)
        {
            switch (obj.tag)
            {
                case "Cube":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[0], obj.position, obj.rotation);
                    break;
                case "Chest":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[1], obj.position, obj.rotation);
                    break;
                case "Princess":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[2], obj.position, obj.rotation);
                    break;
                case "Goblin":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[3], obj.position, obj.rotation);
                    break;
                case "Dialog":
                    Instantiate(ObjectSpawner.instance.objectPrefabs[4], obj.position, obj.rotation);
                    break;
            }

        }
    }

}
