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
        //GoalManager.instance.CompleteCoaching();
        ObjectSpawner.instance.stopSpawn = false;
        ARTemplateMenuManager.Instance.createButton?.gameObject.SetActive(true);
        if(MenuCanvas!= null)
        {
            MenuCanvas.enabled = false;
        }
        ButtonBack?.gameObject.SetActive(true);
        ObjectMenuAnimator?.gameObject.SetActive(true);
        if(ARTemplateMenuManager.Instance!= null)
        {
            ARTemplateMenuManager.Instance.enabled = true;
        }

    }
}
