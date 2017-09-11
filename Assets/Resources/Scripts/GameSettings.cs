using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings : MonoBehaviour
{
    public int  minCombo            = 3, 
                minBonusCombo       = 5, 
                threeComboPoints    = 100, 
                fourComboPoint      = 300, 
                fiveComboPoint      = 1000, 
                moreThanFiveBonus   = 500;

    public float moveItemsSpeed = 0.8f, hintDuration = 1.5f;

    public enum ItemTypeEnum
    {
        Blue,
        Red,
        Green,
        Yellow,
        Orange
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
    public int rows = 8;

    [SerializeField]
    List<Item> items = new List<Item>();

    [SerializeField]
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

    public void randomizeItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].ItemType = getRandomItemType();
        }

        for (int i = 0; i < items.Count; i++)
        {
            bool outcome = MatchController.checkItemForMatches(items[i], false);

            if (outcome)
            {
                List<Item> neighboursTypes = items[i].getAllNeighbours();

                ItemTypeEnum newType = getRandomItemType();

                bool typeExist = doesTypeExistAmongItems(neighboursTypes, newType);

                while (typeExist)
                {
                    newType = getRandomItemType();
                    typeExist = doesTypeExistAmongItems(neighboursTypes, newType);
                }

                items[i].ItemType = newType;
            }
        }
    }

    public List<Item> getItemsToReplace()
    {
        List<Item> result = new List<Item>();

        for (int i = 0; i < Items.Count; i++)
        {
            if (items[i].shouldBeReplaced)
            {
                result.Add(items[i]);
            }
        }

        return result;
    }

    bool doesTypeExistAmongItems(List<Item> testItems, ItemTypeEnum type)
    {
        for (int i = 0; i < testItems.Count; i++)
        {
            if (testItems[i].ItemType == type)
            {
                return true;
            }
        }

        return false;
    }

    public ItemTypeEnum getRandomItemType()
    {
        int randIndex = Random.Range(0, TypesSprites.Count);
        return TypesSprites[randIndex].type;
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

    public void addPoints(int combo)
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
