using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintDisplayer : MonoBehaviour
{
    [SerializeField] GameEvent TextToDisplayEvent;
    [SerializeField] GameEvent TextToUndisplayEvent;

    [SerializeField] TMP_Text m_Text;

    private void Awake()
    {
        m_Text = GetComponentInChildren<TMP_Text>();
    }
    public void OnTextToDisplay(Component sender, object data)
    {
        string text = data as string;

        m_Text.text = text;
    }

    public void OnTextToUndisplay(Component sender, object data)
    {
        m_Text.text = "";
    }
}
