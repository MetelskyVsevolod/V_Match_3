using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PromptWindow : MonoBehaviour
{
    public Text promptText, noText, yesText;

    public Button yesButton, noButton;

    [HideInInspector]
    public CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void Show(string promptString, string yesString, string noString, Action yesAction)
    {
        promptText.text = promptString;
        yesText.text    = yesString;
        noText.text     = noString;

        yesButton.onClick.AddListener(delegate { Close(); yesAction.Invoke(); });
        noButton.onClick.AddListener(delegate { Close(); });

        cg.Show();
    }

    public void Close()
    {
        cg.Hide();
    }

    public bool IsShown
    {
        get { return cg.alpha == 1f; }
    }
}
