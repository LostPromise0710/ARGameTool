using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveInFile : MonoBehaviour
{

    
    public GameObject Cube;
    public GameObject Chest;
    public GameObject Princess;
    public GameObject Dialog;
    public GameObject Goblin;

    private EventLinkContentManager eventLinkContentManager;
    private EventLink eventLink;

    private Save CreateSave()
    {
        Save save = new Save();

        eventLinkContentManager = FindObjectOfType<EventLinkContentManager>();
        eventLink = eventLinkContentManager.eventLink;

        save.eventLink = eventLink.eventCount+1;
        int count = 0;
        foreach(EventUnit eventUnit in eventLink.link)
        {
            save.eventType.Add(eventUnit.objectType);
            foreach(GameObject gameObject in eventUnit.objectList)
            {
                int ID = gameObject.GetHashCode();
                if (save.objectID.Contains(ID))
                {
                    save.eventCount[save.objectID.IndexOf(ID)].Add(count);
                    continue;
                }
                save.objectID.Add(ID);
                save.position.Add(new SerVector3(gameObject.transform.position));
                save.scale.Add(new SerVector3(gameObject.transform.localScale));
                save.rotation.Add(new SerQuaternion(gameObject.transform.rotation));
                save.name.Add(gameObject.name);
                List<int> goEventCount =new List<int>();
                goEventCount.Add(count);
                save.eventCount.Add(goEventCount);

                
            }

                if (eventUnit.objectType == 0 || eventUnit.objectType == 1)
                {
                    save.interactionType.Add(0);
                    save.animationType.Add(0);
                }
                else if (eventUnit.objectType == 2)
                {
                    save.interactionType.Add(0);//加交互类型
                    save.animationType.Add(0);
                }
                else if (eventUnit.objectType == 3)
                {
                    save.interactionType.Add(0);
                    save.animationType.Add(eventUnit.animationType);
                }


            count++;
        }
        /*
        GameObject spawner = GameObject.Find("Object Spawner");
        foreach (Transform child in spawner.transform)
        {
            save.position.Add(new SerVector3(child.position));
            save.scale.Add(new SerVector3(child.localScale));
            save.rotation.Add(new SerQuaternion(child.rotation));
            save.name.Add(child.name);
        }
        */
        return save;
    }
    
    private void LoadSave(Save save)
    {
        GameObject spawner = GameObject.Find("Object Spawner");
        foreach (Transform child in spawner.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Clear");

        Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
        dictionary.Add("ChestVariant(Clone)", Chest);
        dictionary.Add("CubeVariant(Clone)", Cube);
        dictionary.Add("PrincessVariant(Clone)", Princess);
        dictionary.Add("GoblinVariant(Clone)", Goblin);
        dictionary.Add("DialogVariant(Clone)", Dialog);

        List<EventUnit> eventLink = new List<EventUnit>();

        
        for (int i = 0; i < save.eventLink;i++)
        {
            EventUnit eventUnit = new EventUnit();
            eventUnit.objectType = save.eventType[i];
            if (save.interactionType[i] != 0)
            {
                        //添加交互
            }
            if (save.animationType[i] != 0)
            {
                eventUnit.animationType = save.animationType[i];
            }
            eventLink.Add(eventUnit);
        }

        Debug.Log(save.eventLink);
        Debug.Log(eventLink.Count);

        for(int j = 0; j < save.eventCount.Count; j++)
        {
            GameObject prefab = dictionary[save.name[j]];
            prefab.transform.localScale = save.scale[j].GetVector3();
            GameObject gameObject = Instantiate(prefab, save.position[j].GetVector3(), save.rotation[j].GetQuaternion(), spawner.transform);
            
            for(int k = 0; k < save.eventCount[j].Count; k++) 
            {
                Debug.Log(save.eventCount[j][k]);
                eventLink[save.eventCount[j][k]].objectList.Add(gameObject);

            }
                          
        }

        EventLink link = new EventLink();
        link.link = eventLink;
        link.eventCount = save.eventLink;
        eventLinkContentManager = FindObjectOfType<EventLinkContentManager>();
        eventLinkContentManager.eventLink = link;


    }

    public void SaveGame()
    {
        Save save = CreateSave();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/gamesave.txt");
        binaryFormatter.Serialize(fileStream, save);
        fileStream.Close();
        Debug.Log(Application.persistentDataPath);
        Debug.Log("Saved");

    }

    public void LoadGame()
    {

        //GameObject spawner = GameObject.Find("Object Spawner");
        if (File.Exists(Application.persistentDataPath + "/gamesave.txt"))
        {
            /*
            foreach (Transform child in spawner.transform)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("Clear");
            */
        }
        else
        {
            Debug.Log("NoSave");
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(Application.persistentDataPath + "/gamesave.txt", FileMode.Open);

        Save save = (Save)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();

        LoadSave(save);
        /*
        Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
        dictionary.Add("ChestVariant(Clone)", Chest);
        dictionary.Add("CubeVariant(Clone)", Cube);
        dictionary.Add("PrincessVariant(Clone)", Princess);
        dictionary.Add("GoblinVariant(Clone)", Goblin);
        dictionary.Add("DialogVariant(Clone)", Dialog);
        


        for (int i = 0; i < save.position.Count; i++)
        {
            GameObject prefab = dictionary[save.name[i]];
            prefab.transform.localScale = save.scale[i].GetVector3();
            Instantiate(prefab, save.position[i].GetVector3(), save.rotation[i].GetQuaternion(), spawner.transform);
        }
        */
        Debug.Log("Load");
        
    }
}

