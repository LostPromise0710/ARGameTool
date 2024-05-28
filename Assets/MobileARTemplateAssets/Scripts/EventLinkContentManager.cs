using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using TMPro;
using UnityEngine.UI;

public class EventLinkContentManager : MonoBehaviour
{

    public GameObject content;

    public ObjectSpawner ObjectSpawner;

    public EventLink eventLink = new EventLink();

    //具体事件按钮的List
    public List<GameObject> eventButtonList = new List<GameObject>();

    //具体事件按钮(预制体)
    public GameObject eventButton;

    //添加事件按钮
    public GameObject addButton;

    //添加物体按钮
    public GameObject addObjectButton;

    //添加交互按钮
    public GameObject addInteractionButton;

    //删除按钮
    public GameObject deleteButton;

    //编辑按钮
    public GameObject editButton;

    // 保存按钮
    public GameObject saveButton;

    public GameObject createButton;

    //动画管理器
    public GameObject animationManager;

    //UI
    public GameObject UI;

    public List<GameObject> objectToAdded;

    // 事件个数(1...n)，不是事件index
    public int eventCount;

    //当前选中事件
    public int focusedEventIndex = -1;

    public void Start()
    {
        if (eventLink == null) eventLink = new EventLink();
        eventCount = 0;
    }

    public void showCreateButton()
    {
        if (eventLink.link[focusedEventIndex].objectType <= 1)
            createButton.SetActive(true);
    }

    /// <summary>
    /// 显示编辑、删除按钮
    /// TODO 保存
    /// </summary>
    public void showButtons()
    {
        deleteButton.SetActive(true);
        editButton.SetActive(true);
    }

    /// <summary>
    /// 隐藏编辑、删除按钮
    /// TODO 保存
    /// </summary>
    public void hideButtons()
    {
        deleteButton.SetActive(false);
        editButton.SetActive(false);
        createButton.SetActive(false);
    }


    /// <summary>
    /// 点击某个事件按钮
    /// </summary>
    /// <param name="index">
    /// index = -1  : 点击了newEvent按钮
    /// index = k   ：点击了第k (0...n-1) 个事件
    /// </param>
    public void Click(int index)
    {
        for (int i = 0; i < eventCount; ++i)
        {
            if (i == index)
            {
                // 把第index个按钮高亮，并取消添加事件按钮的高亮
                eventButtonList[i].transform.Find("SelectionBox").gameObject.SetActive(true);
                addButton.transform.Find("SelectionBox").gameObject.SetActive(false);
            }
            else
            {
                // 取消其他事件按钮的高亮
                eventButtonList[i].transform.Find("SelectionBox").gameObject.SetActive(false);
            }
        }
        focusedEventIndex = index;  // 设置当前选中事件

        // 隐藏或者显示编辑、删除按钮等
        if (index != -1) showButtons();
        else hideButtons();
    }

    public void addObject(GameObject newObject)
    {
        eventLink.link[focusedEventIndex].addObject(newObject);
    }

    /// <summary>
    /// 新建事件
    /// </summary>
    /// <param name="type">
    ///     0: 点击添加物体按钮
    ///     1: 点击添加交互按钮
    ///     2: 点击添加动画按钮
    /// </param>
    public void newEvent(int type)
    {
        GameObject newEvent = Instantiate(eventButton);         // 复制一个事件按钮
        Debug.Log(newEvent);
        eventButtonList.Add(newEvent);
        ++eventCount;
        newEvent.transform.parent = content.transform;          // 把新事件放到content下
        newEvent.transform.localScale = new Vector3(1, 1, 1);
        addButton.transform.SetSiblingIndex(eventCount + 1);    // 第一个固定是template，有eventCount个事件，所以是eventCount + 1
        if (type == 0)
        {
            TextMeshProUGUI text = newEvent.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();   //设置按钮内容
            text.SetText("Object");

            eventLink.addEvent(0);  // type 0 是物体
            //TODO 这里未将对话框和物体区分，统一是0
        }
        else if (type == 1)
        {
            TextMeshProUGUI text = newEvent.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();   //设置按钮内容
            text.SetText("Interaction");

            eventLink.addEvent(2);  // type 2 是交互
        }
        else if (type == 2)
        {
            TextMeshProUGUI text = newEvent.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();   //设置按钮内容
            text.SetText("Animation");

            eventLink.addEvent(3);  // type 3 是动画
        }
        newEvent.SetActive(true);
        addButton.transform.Find("SelectionBox").gameObject.SetActive(false);

        
    }

    /// <summary>
    /// 点击删除按钮删除事件
    /// </summary>
    public void deleteEvent()
    {
        for (int i = focusedEventIndex + 1; i < eventCount; ++i)
        {
            eventButtonList[i].GetComponent<EventButtonIndexController>().setIndex(i - 1);  //把当前事件之后的所有事件修改序号
        }
        eventLink.deleteEvent(focusedEventIndex);                                           //在事件链中删除当前选中事件
        GameObject target = eventButtonList[focusedEventIndex].transform.gameObject;        
        eventButtonList.RemoveAt(focusedEventIndex);                                        //在事件按钮中删除当前选中事件按钮
        Destroy(target);                                                                    //销毁对应事件按钮
        --eventCount;
        focusedEventIndex = -1;
    }


    /// <summary>
    /// 点击编辑按钮对事件编辑
    /// TODO 需要在eventLink中获取当前eventUnit的type，进行对应UI的展示，type = 0/1 需要展示添加物体的UI，type = 2 需要展示添加交互的UI, type = 3展示动画的UI
    /// </summary>
    public void editEvent()
    {
        int eventType = eventLink.link[focusedEventIndex].objectType;
        if (eventType == 0)
        {
            ObjectSpawner.stopSpawn = false;
            UI.GetComponent<GoalManager>().StartCoaching();
            UI.transform.Find("Object Menu Animator").gameObject.SetActive(true);
            eventLink.editEvent(focusedEventIndex);
        }
        else if (eventType == 1)
        {
            // TODO 展示对话框相关UI
        }
        else if (eventType == 2)
        {
            // TODO 展示交互相关UI
        }
        else if (eventType == 3)
        {
            animationManager.SetActive(true);
            animationManager.transform.Find("Button (Choose Object)").gameObject.SetActive(true);
            animationManager.transform.Find("Button (Choose Animation)").gameObject.SetActive(true);
        }
    }



    /// <summary>
    /// 保存当前事件
    /// </summary>
    public void saveEvent()
    {
        int eventType = eventLink.link[focusedEventIndex].objectType;
        if (eventType == 0)
        {
            foreach (GameObject gameObject in ObjectSpawner.objectToAdded) objectToAdded.Add(gameObject);
            //objectToAdded = ObjectSpawner.objectToAdded;
            ObjectSpawner.objectToAdded.Clear();

            Debug.Log("开始保存");
            Debug.Log(objectToAdded);
            eventLink.saveEvent(focusedEventIndex, objectToAdded);
            UI.transform.Find("Create Button").gameObject.SetActive(false) ;
            ObjectSpawner.stopSpawn = true;
        }
        else if (eventType == 1)
        {
            //TODO 隐藏对话框相关UI
        }
        else if (eventType == 2)
        {
            //TODO 隐藏隐藏交互相关UI
        }
        else if (eventType == 3)
        {
            eventLink.saveEvent(focusedEventIndex, null);
            //animationManager.SetActive(false);
            animationManager.transform.Find("Button (Choose Object)").gameObject.SetActive(false);
            animationManager.transform.Find("Button (Choose Animation)").gameObject.SetActive(false);
            Debug.Log(eventLink.link[focusedEventIndex].objectList[0]);
            Debug.Log("Dammmmmmmmmmmmmmn");
            Debug.Log(eventLink.link[focusedEventIndex].animationType);
            AnimationManager.instance.playAnimation(eventLink.link[focusedEventIndex].objectList[0], eventLink.link[focusedEventIndex].animationType);
        }
    }

    public void nextEvent()
    {
        eventButton.transform.Find("SelectionBox").gameObject.SetActive(false);
        ++focusedEventIndex;
        if (focusedEventIndex == eventCount) return;
        eventButton = eventButtonList[focusedEventIndex];
        eventButton.transform.Find("SelectionBox").gameObject.SetActive(true);
        eventLink.play(focusedEventIndex);
    }
    

    public void playEvent()
    {
        foreach (GameObject button in eventButtonList)
        {
            button.GetComponent<Button>().enabled = false;
        }

        focusedEventIndex = 0;
        eventButton = eventButtonList[focusedEventIndex];
        eventButton.transform.Find("SelectionBox").gameObject.SetActive(true);
        eventLink.play(focusedEventIndex);
        
    }
}
