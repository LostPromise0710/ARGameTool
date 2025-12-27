using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaymodeManager : MonoBehaviour
{
    public static PlaymodeManager instance;

    private void Awake()
    {
        instance = this;
    }
    public List<GameObject> PlayObjects;
}
