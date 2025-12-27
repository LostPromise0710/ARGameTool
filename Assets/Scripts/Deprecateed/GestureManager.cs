using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    public GameObject[] toHide;
    public GameObject[] toShow;
    public void OnChangeUnitGestureInteractionIndex(int index)
    {
        EventLinkContentManager.Instance.eventLink.link[EventLinkContentManager.Instance.focusedEventIndex].gestureInteractionIndex = index;
    }
    public void Show()
    {
        foreach (GameObject go in toShow) { go.SetActive(true); }
    }

    public void Hide()
    {
        foreach (GameObject go in toHide) { go.SetActive(false); }
    }
}
