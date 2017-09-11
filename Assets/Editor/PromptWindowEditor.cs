using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PromptWindow))]
public class PromptWindowEditor : Editor
{
    PromptWindow promptWindow;

    public override void OnInspectorGUI()
    {
        promptWindow = (PromptWindow)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Show"))
        {
            if(promptWindow.cg == null)
                promptWindow.cg = promptWindow.GetComponent<CanvasGroup>();

            promptWindow.cg.Show();
        }

        if (GUILayout.Button("Hide"))
        {
            if (promptWindow.cg == null)
                promptWindow.cg = promptWindow.GetComponent<CanvasGroup>();

            promptWindow.cg.Hide();
        }
    }
}
