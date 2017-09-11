using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    GameSettings gameSettings;

    [SerializeField]
    bool showTypes = true, showBoardSetting = true, showItems = false;

    public override void OnInspectorGUI()
    {
        gameSettings = (GameSettings)target;

        showBoardSetting = EditorGUILayout.Foldout(showBoardSetting, "Board Settings:");

        EditorGUIUtility.labelWidth = 100f;
        EditorGUIUtility.fieldWidth = 30f;

        if (showBoardSetting)
        { 
            GUILayout.BeginVertical(EditorStyles.helpBox);

            gameSettings.firstItemCoordinates = EditorGUILayout.Vector2Field("First Item Coords:", (Vector2)gameSettings.firstItemCoordinates);

            GUILayout.BeginHorizontal();
            gameSettings.xDistance = EditorGUILayout.FloatField("Step x:", gameSettings.xDistance);
            gameSettings.yDistance = EditorGUILayout.FloatField("Step y:", gameSettings.yDistance);
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();
            gameSettings.rows = EditorGUILayout.IntField("Rows:", gameSettings.rows);
            GUILayout.EndHorizontal();

            gameSettings.playboard = (GameObject)EditorGUILayout.ObjectField("Playboard:", gameSettings.playboard, typeof(GameObject), true);
            gameSettings.itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab:", gameSettings.itemPrefab, typeof(GameObject), false);

            GUILayout.EndVertical();
        }

        if (GUILayout.Button("Refresh board items"))
        {
            spawnItems();
            SortItemPoses();
        }

        if (GUILayout.Button("Add new type"))
        {
            gameSettings.TypesSprites.Add(new ItemTypeSprite());
        }

        showTypes = EditorGUILayout.Foldout(showTypes, "Item Types: [" + gameSettings.TypesSprites.Count + "]:");

        EditorGUIUtility.fieldWidth = 60f;
        EditorGUIUtility.labelWidth = 30f;

        if (showTypes)
            for (int i = 0; i < gameSettings.TypesSprites.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                GUI.color = Color.red;

                if (GUILayout.Button("Delete"))
                {
                    gameSettings.TypesSprites.Remove(gameSettings.TypesSprites[i]);
                }

                GUI.color = Color.white;
                
                gameSettings.TypesSprites[i].type = (GameSettings.ItemTypeEnum)EditorGUILayout.EnumPopup("["+i+"]", gameSettings.TypesSprites[i].type);
                gameSettings.TypesSprites[i].sprite = (Sprite)EditorGUILayout.ObjectField(string.Empty, gameSettings.TypesSprites[i].sprite, typeof(Sprite), true);

                GUILayout.EndHorizontal();
            }

        if (GUILayout.Button("Randomize items"))
        {
            if (gameSettings.TypesSprites.Count < 1)
            {
                ErrorMessages.ShowNoItemTypesError();
            }
            else
            { 
                gameSettings.randomizeItems();
            }
        }

        showItems = EditorGUILayout.Foldout(showItems, "Items: [" + gameSettings.Items.Count + "]:");

        for (int i = 0; i < gameSettings.Items.Count; i++)
        {
            if (gameSettings.Items[i] == null)
            {
                gameSettings.Items.Remove(gameSettings.Items[i]);
            }
        }

        if (showItems)
            for (int i = 0; i < gameSettings.Items.Count; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField("Type: ", gameSettings.Items[i].ItemType.ToString());

                EditorGUILayout.LabelField("Pos: ", gameSettings.Items[i].transform.position.ToString());

                gameSettings.Items[i] = (Item)EditorGUILayout.ObjectField("["+i+"]", gameSettings.Items[i], typeof(Item), true);

                GUILayout.EndHorizontal();
            }

        EditorUtility.SetDirty(target);
    }


    void spawnItems()
    {
        while (gameSettings.Items.Count < gameSettings.rows * gameSettings.rows)
        {
            GameObject newItemGO = PrefabUtility.InstantiatePrefab(gameSettings.itemPrefab) as GameObject;
            gameSettings.Items.Add(newItemGO.GetComponent<Item>());

            newItemGO.transform.parent = gameSettings.playboard.transform;
        }

        for (int i = 0; i < gameSettings.Items.Count; i++)
        {
            gameSettings.Items[i].gameObject.name = "item_" + i;
        }
    }

    public void SortItemPoses()
    {
        spawnItems();

        int index = 0;

        for (int i = 0; i < gameSettings.rows; i++)
        {
            for (int j = 0; j < gameSettings.rows; j++)
            {
                float xPos = gameSettings.firstItemCoordinates.x + gameSettings.xDistance * j,
                        yPos = gameSettings.firstItemCoordinates.y + gameSettings.yDistance * i;

                gameSettings.Items[index].gameObject.transform.position = new Vector3(xPos, yPos, -1f);

                index++;
            }
        }
    }
}
