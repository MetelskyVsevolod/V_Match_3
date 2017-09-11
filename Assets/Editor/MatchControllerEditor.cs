using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(MatchController))]
public class MatchControllerEditr : Editor
{
    MatchController matchController;

    [SerializeField]
    bool showMovingItems = true;

    public override void OnInspectorGUI()
    {
        matchController = (MatchController)target;

        matchController.promptWindow = (PromptWindow)EditorGUILayout.ObjectField("Prompt window:", matchController.promptWindow, typeof(PromptWindow), true);
        matchController.scoreText = (Text)EditorGUILayout.ObjectField("Score Text:", matchController.scoreText, typeof(Text), true);
        matchController.highScoreText = (Text)EditorGUILayout.ObjectField("Highscore Text:", matchController.highScoreText, typeof(Text), true);

        showMovingItems = EditorGUILayout.Foldout(showMovingItems, "Moving Items: [" + matchController.movingItems.Count + "]:");

        if (showMovingItems)
            for (int i = 0; i < matchController.movingItems.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                matchController.movingItems[i] = (Item)EditorGUILayout.ObjectField("[" + i + "]", matchController.movingItems[i], typeof(Item), true);

                GUILayout.EndHorizontal();
            }

        EditorUtility.SetDirty(target);
    }
}
