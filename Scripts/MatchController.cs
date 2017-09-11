using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    Item selectedItem = null;

    public Item SelectedItem
    {
        get { return selectedItem; }
        set
        {
            if (value == null)
                selectedItem.IsSelected = false;

            selectedItem = value;
        }
    }

    bool canSelectItem = true;

    public void setSelectedItem(Item selectedItem)
    {
        if (!canSelectItem) return;

        if (this.selectedItem == selectedItem)
        {
            SelectedItem = null;
            return;
        }

        if (this.selectedItem != null)
        {
            swapItems(ref this.selectedItem, ref selectedItem);
            rePoseItems(this.selectedItem, selectedItem);

            SelectedItem = null;

            selectedItem.IsSelected = false;

            return;
        }

        this.selectedItem = selectedItem;
        selectedItem.IsSelected = true;
    }

    int score = 0, highScore = 0;

    public Text scoreText, highScoreText;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            scoreText.text = GlobalVars.GL_ScoreText + score;

            if (score > highScore)
            {
                highScore = score;
            }
        }
    }

    public int HighScore
    {
        get { return highScore; }
        set
        {
            highScore = value;
            highScoreText.text = GlobalVars.GL_HighscoreText + highScore;
        }
    }

    GameSettings GameSettings
    {
        get { return FindObjectOfType<GameSettings>(); }
    }

    void swapItems(ref Item item1, ref Item item2)
    {
        Item temp = item1;
        item1 = item2;
        item2 = temp;

        string tempName = item1.name;
        item1.name = item2.name;
        item2.name = tempName;
    }

    void rePoseItems(Item item1, Item item2)
    {
        item1.MoveToItem(item2.transform.position, delegate { removeMovingItems(item1); });
        item2.MoveToItem(item1.transform.position, delegate { removeMovingItems(item2); });
    }

    List<Item> movingItems = new List<Item>();

    void removeMovingItems(Item itemToRemove)
    {
        movingItems.Remove(itemToRemove);

        if (movingItems.Count == 0)
        {
            canSelectItem = true;
            checkItemForMatches(ref itemToRemove);
        }
    }

    void checkItemForMatches(ref Item item)
    {
        //====
        //Debug.Log("============");
        //Debug.Log(item.name);
        //Debug.Log("============");
        List<Item> horizontalMatchedItems = new List<Item>();

        Item leftNeighbour = item.getLeftNeighbour();

        while(leftNeighbour != null)
        {
            if (leftNeighbour.ItemType != item.ItemType)
                break;

            horizontalMatchedItems.Add(leftNeighbour);
            leftNeighbour = leftNeighbour.getLeftNeighbour();
        }

        Item rightNeighbour = item.getRightNeighbour();

        while (rightNeighbour != null)
        {
            if (rightNeighbour.ItemType != item.ItemType)
                break;

            horizontalMatchedItems.Add(rightNeighbour);
            rightNeighbour = rightNeighbour.getRightNeighbour();
        }

        horizontalMatchedItems.Add(item);
        //Debug.Log("============");
        //====

        List<Item> verticalMatchedItems = new List<Item>();

        Item topNeighbour = item.getTopNeighbour();

        while (topNeighbour != null)
        {
            if (topNeighbour.ItemType != item.ItemType)
                break;

            verticalMatchedItems.Add(topNeighbour);
            topNeighbour = topNeighbour.getTopNeighbour();
        }

        Item bottomNeighbour = item.getBottomNeighbour();

        while (bottomNeighbour != null)
        {
            if (bottomNeighbour.ItemType != item.ItemType)
                break;

            verticalMatchedItems.Add(bottomNeighbour);
            bottomNeighbour = bottomNeighbour.getBottomNeighbour();
        }

        verticalMatchedItems.Add(item);

        if (horizontalMatchedItems.Count >= verticalMatchedItems.Count)
        {
            for (int i = 0; i < horizontalMatchedItems.Count; i++)
            {
                // Debug.Log(matchedItems[i]);
                horizontalMatchedItems[i].GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
        else
        {
            for (int i = 0; i < verticalMatchedItems.Count; i++)
            {
                // Debug.Log(matchedItems[i]);
                verticalMatchedItems[i].GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }


    int testCounter = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < GameSettings.Items.Count; i++)
            {
                GameSettings.Items[i].GetComponent<SpriteRenderer>().color = Color.white;
            }

            Item itm = GameSettings.Items[testCounter];

            checkItemForMatches(ref itm);
            testCounter++;
        }
    }

    bool areNeighbours(Item item1, Item item2)
    {
        int curIndex = item1.GameSettings.Items.IndexOf(item1);

        Item    left    = item1.GameSettings.doesIndexExist(curIndex - 1),
                right   = item1.GameSettings.doesIndexExist(curIndex + 1),
                top     = item1.GameSettings.doesIndexExist(curIndex - item1.GameSettings.rows),
                bottom  = item1.GameSettings.doesIndexExist(curIndex + item1.GameSettings.rows);

        return (left == item2 || right == item2 || top == item2 || bottom == item2);
    }


    public int getMinRowBorderOfItem(Item item)
    {
        int indexOfItem = GameSettings.Items.IndexOf(item);

        int minBorder = indexOfItem;

        //for (int i = indexOfItem; i % GameSettings.rows != 0; i--)
        //{
        //    minBorder = i;
        //}

        if (indexOfItem > 0)
        {
            while (true)
            {
                if (minBorder % GameSettings.rows == 0)
                {
                    break;
                }
                else
                {
                    minBorder--;
                }
            }
        }

        return minBorder;
    }

    public int getMaxRowBorderOfItem(Item item)
    {
        int indexOfItem = GameSettings.Items.IndexOf(item);

        int maxBorder = indexOfItem;

        //for (int i = indexOfItem; i % GameSettings.rows != 0; i++)
        //{
        //    maxBorder = i;
        //}

        for (int i = GameSettings.Items.Count; i > indexOfItem; i -= GameSettings.rows)
        {
            maxBorder = i;
        }

        maxBorder--;

        return maxBorder;
    }

    public int getMinColumnBorderOfItem(Item item)
    {
        int indexOfItem = GameSettings.Items.IndexOf(item);

        int minBorder = indexOfItem - GameSettings.rows;

        while(minBorder > 0)
        {
            minBorder -= GameSettings.rows;
        }

        return minBorder;
    }

    public int getMaxColumnBorderOfItem(Item item)
    {
        int indexOfItem = GameSettings.Items.IndexOf(item);

        int maxBorder = indexOfItem + GameSettings.rows;

        while (maxBorder < GameSettings.rows)
        {
            maxBorder += GameSettings.rows;
        }

        return maxBorder;
    }
}
