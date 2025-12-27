using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceInteractionManager : MonoBehaviour
{
    public VoiceInteractionManager instance;

    private void Awake()
    {
        instance = this;
    }

    public TextMeshProUGUI inputText;

    public void OnSubmit()
    {
        EventLinkContentManager.Instance.eventLink.link[EventLinkContentManager.Instance.focusedEventIndex].voiceInteractionSentence = inputText.text;
    }
}
