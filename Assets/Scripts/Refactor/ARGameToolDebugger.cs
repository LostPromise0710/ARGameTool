using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ARGameToolDebugger : MonoBehaviour
{
    public static ARGameToolDebugger Instance;

    
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickButtonStore()
    {
        Debug.Log("Start Printing!");
        foreach (var obj in ObjectSpawner.instance.objectToAdded)
        {
            if(obj!= null)
            {
                Debug.Log(obj.tag);
                Debug.Log(obj.gameObject.transform.position);
            }
        }
        Debug.Log("End Printing!");
    }

    public void OnClickButtonResotre()
    {
        Debug.Log("Restoring Scene!");
    }
}
