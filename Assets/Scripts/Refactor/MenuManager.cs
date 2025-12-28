using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    private Canvas MenuCanvas;

    public GameObject ButtonBack;
    public GameObject ObjectMenuAnimator;
    public List<GameObject> ToShow = new List<GameObject>();
    public List<GameObject> ToHide = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        MenuCanvas = GetComponent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickButtonNewProject()
    {
        ObjectSpawner.instance.stopSpawn = false;
        ToggleMenuCanvasState(false);
        MenuShowGameObjects();
    }

    public void OnCLickButtonBack()
    {
        ObjectSpawner.instance.stopSpawn = true;
        JsonArraySaver.instance.SaveandClearobjectsInObjectSpawner();
        ToggleMenuCanvasState(true);
        MenuHideGameObjects();
    }

    private void MenuShowGameObjects()
    {
        foreach (GameObject go in ToShow)
        {
            go?.SetActive(true);
        }
    }

    private void MenuHideGameObjects()
    {
        foreach (GameObject go in ToHide)
        {
            go?.SetActive(false);
        }
    }

    private void ToggleMenuCanvasState(bool CanvasState)
    {
        if (MenuCanvas != null)
        {
            if (CanvasState)
            {
                MenuCanvas.enabled = true;
            }
            else
            {
                MenuCanvas.enabled = false;
            }

        }
    }
}
