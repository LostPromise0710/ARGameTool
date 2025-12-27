using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusManager : MonoBehaviour
{
    public static GameStatusManager instance;
    public bool isEditing;
    public TextMeshProUGUI TextStatus;
    public Button ButtonNewProject;
    public Button ButtonEditProject;
    public Button ButtonPlay;
    private void Awake()
    {
        instance = this;
        isEditing = true;
    }
    public void ChangetoEditMode()
    {
        isEditing = true;
        RenewTextStatus();
        RenewLayoutStatus();
    }

    public void ChangetoPlayMode()
    {
        isEditing = false;
        RenewTextStatus();
        RenewLayoutStatus();
    }

    public void RenewTextStatus(){
        if (isEditing)
        {
            TextStatus.text = "Edit";
        }
        else
        {
            TextStatus.text = "Play";
        }
    }

    public void RenewLayoutStatus()
    {
        ButtonNewProject.gameObject.SetActive(isEditing);
        ButtonEditProject.gameObject.SetActive(isEditing);
        ButtonPlay.gameObject.SetActive(!isEditing);
    }

}
