using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings : MonoBehaviour
{
    public int minCombo = 3, minBonusCombo = 5;

    public int threeComboPoints = 100, fourComboPoint = 300, fiveComboPoint = 1000, moreThanFiveBonus = 500;

    public enum ItemTypeEnum
    {
        Black,
        Pink,
        Purple,
        Red,
        White
    }

    [SerializeField]
    List<ItemTypeSprite> typesSprites = new List<ItemTypeSprite>();

    public List<ItemTypeSprite> TypesSprites
    {
        get { return typesSprites; }
        set
        {
            typesSprites = value;
        }
    }

    [SerializeField]
    public GameObject itemPrefab, playboard;

    [SerializeField]
    public Vector3 firstItemCoordinates = Vector3.zero;

    [SerializeField]
    public float xDistance = 1f, yDistance = 1f;

    [SerializeField]
    public int rows = 8, columns = 8;

    [SerializeField]
    List<Item> items = new List<Item>();

    public List<Item> Items
    {
        get { return items; }
        set
        {
            items = value;
        }
    }

    MatchController matchController;

    public MatchController MatchController
    {
        get
        {
            if (Application.isPlaying)
            {
                return matchController;
            }
            else
            {
                return FindObjectOfType<MatchController>();
            }
        }
    }

    void spawnItems()
    {
        while (items.Count < rows * columns)
        {
            GameObject newItemGO = Instantiate(itemPrefab);
            items.Add(newItemGO.GetComponent<Item>());

            newItemGO.transform.parent = playboard.transform;
        }

        refreshItemsNames();
    }

    public void refreshItemsNames()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].gameObject.name = "item_" + i;
        }
    }

    public void SortItemPoses()
    {
        spawnItems();

        int index = 0;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                float   xPos = firstItemCoordinates.x + xDistance * j,
                        yPos = firstItemCoordinates.y + yDistance * i;

                items[index].gameObject.transform.position = new Vector3(xPos, yPos, -1f);

                index++;
            }
        }
    }

    public void randomizeItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            int randIndex = Random.Range(0, TypesSprites.Count);
            items[i].ItemType = TypesSprites[randIndex].type;
        }
    }

    public Sprite getSpriteForType(ItemTypeEnum type)
    {
        for (int i = 0; i < typesSprites.Count; i++)
        {
            if (typesSprites[i].type == type)
            {
                return typesSprites[i].sprite;
            }
        }

        return null;
    }

    public Item doesIndexExist(int index)
    {
        if (index >= items.Count)
            return null;

        if (index < 0)
            return null;

        return items[index];
    }

    void addPoints(int combo)
    {
        int addPoints = 0;

        if (combo == 3)
        {
            addPoints = threeComboPoints;
        }
        else if (combo == 4)
        {
            addPoints = fourComboPoint;
        }
        else if (combo >= 5)
        {
            addPoints = 1000;

            for (int i = combo; i > 5; i++)
            {
                addPoints += moreThanFiveBonus;
            }
        }
        else return;

        matchController.Score += addPoints;
    }

    void Awake()
    {
        matchController = FindObjectOfType<MatchController>();
    }
}
