using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogContentAffordance : MonoBehaviour
{

    public List<string> m_DialogContent = new List<string>();

    /// <summary>
    /// 存储对话内容
    /// </summary>
    public List<string> dialogContent
    {
        get => m_DialogContent;
        set => m_DialogContent = value;
    }

    public int m_DialogCount = -1;

    /// <summary>
    /// 对话数量
    /// </summary>
    public int dialogCount
    {
        get => m_DialogCount;
        set => m_DialogCount = value;
    }

    public int m_CurrentDialogIndex = -1;

    /// <summary>
    /// 当前对话
    /// </summary>
    //public int currentDialogIndex
    //{
    //    get => m_CurrentDialogIndex;
    //    set => m_CurrentDialogIndex = value;
    //}

    [SerializeField]
    TMP_Text m_TextSource;

    /// <summary>
    /// 对话文本框
    /// </summary>
    public TMP_Text textSource
    {
        get => m_TextSource;
        set => m_TextSource = value;
    }

    /// <summary>
    /// 修改当前对话框的内容
    /// </summary>
    public void editDiologContent(string content)
    {
        m_DialogContent[m_CurrentDialogIndex] = content;
        updateDialogContent(m_DialogContent[m_CurrentDialogIndex]);
    }

    /// <summary>
    /// 跳转到后一个对话框并更新内容
    /// </summary>
    public void nextDialog()
    {
        if (m_CurrentDialogIndex == m_DialogCount)
            return ;
        else
        {
            ++m_CurrentDialogIndex;
            updateDialogContent(m_DialogContent[m_CurrentDialogIndex]);
        }
        
    }

    /// <summary>
    /// 跳转到前一个对话框并更新内容
    /// </summary>
    public void lastDialog()
    {
        if (m_CurrentDialogIndex == 0)
            return;
        else
        {
            --m_CurrentDialogIndex;
            updateDialogContent(m_DialogContent[m_CurrentDialogIndex]);
        }
    }

    /// <summary>
    /// 在当前对话框之后新加一个空的对话框并跳转
    /// </summary>
    public void addDialog()
    {
        ++m_DialogCount;
        ++m_CurrentDialogIndex;
        m_DialogContent.Insert(m_CurrentDialogIndex, "");
        updateDialogContent("");
    }

    /// <summary>
    /// 删除当前对话框并跳到后一个对话框，如果是最后一个就跳到前一个，如果只有一个就清空内容
    /// </summary>
    public void deleteDialog()
    {
        if (m_DialogCount == 0)
        {
            m_DialogContent[m_CurrentDialogIndex] = "";
            updateDialogContent(m_DialogContent[m_CurrentDialogIndex]);
        }
        else
        {
            m_DialogContent.RemoveAt(m_CurrentDialogIndex);
            if (m_CurrentDialogIndex == m_DialogCount)
            {
                --m_CurrentDialogIndex;
            }
            --m_DialogCount;
            updateDialogContent(m_DialogContent[m_CurrentDialogIndex]);
        }
        
    }

    private float delayTime = 2;

    /// <summary>
    /// 
    /// </summary>
    public void play()
    {
        if(Interval.Instance!= null)
        {
            StartCoroutine(DialogCoroutine(Interval.Instance.IntervalSlider.value));
        }
    }

    IEnumerator DialogCoroutine(float seconds)
    {
        foreach (string content in m_DialogContent)
        {
            updateDialogContent(content);
            yield return new WaitForSeconds(seconds);
        }
    }
    /// <summary>
    /// 把当前文本框的内容更新成content
    /// </summary>
    /// <param name="content"></param>

    public void updateDialogContent(string content)
    {
        m_TextSource.text = content;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        m_DialogCount = 0;
        m_CurrentDialogIndex = 0;
        m_DialogContent.Add("");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
