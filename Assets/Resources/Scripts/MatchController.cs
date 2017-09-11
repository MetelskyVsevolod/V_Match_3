using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class MatchController : MonoBehaviour
{
    public PromptWindow promptWindow;

    Item selectedItem = null;

    public Item SelectedItem
    {
        get { return selectedItem; }
        set
        {
            if (value == null)
            { 
                if(selectedItem != null)
                { 
                    selectedItem.IsSelected = false;
                }
            }

            selectedItem = value;
        }
    }

    bool CanSelectItem
    {
        get { return movingItems.Count < 1 && !promptWindow.IsShown; }
    }

    public void setSelectedItem(Item selectedItem)
    {
        if (!CanSelectItem) return;

        if (this.selectedItem == selectedItem)
        {
            SelectedItem = null;
            return;
        }

        if (this.selectedItem != null)
        {
            rePoseItems(this.selectedItem.Index, selectedItem.Index, true);

            SelectedItem = null;

            selectedItem.IsSelected = false;

            return;
        }

        this.selectedItem = selectedItem;
        selectedItem.IsSelected = true;
    }

    int score = 0, highScore = 0;

    [SerializeField]
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
                HighScore = score;
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

    GameSettings gameSettings;

    GameSettings GameSettings
    {
        get {return gameSettings; }
    }

    bool reviewBoard = false, shouldDropdownItems = false;

    Action dropDownItemsAction;

    Item firstItemSwapped, secondItemSwapped;

    void swapItems(Item item1, Item item2)
    {
        int index1 = item1.Index;
        int index2 = item2.Index;

        GameSettings.Items[index1] = item2;
        GameSettings.Items[index2] = item1;
    }

    void rePoseItems(int item1index, int item2index, bool review)
    {
        firstItemSwapped    = GameSettings.Items[item1index];
        secondItemSwapped   = GameSettings.Items[item2index];

        swapItems(GameSettings.Items[item1index], GameSettings.Items[item2index]);

        GameSettings.Items[item1index].MoveToItem(GameSettings.Items[item2index].transform.position);

        GameSettings.Items[item2index].MoveToItem(GameSettings.Items[item1index].transform.position);

        reviewMove = review;
    }

    bool reviewMove = false;

    public List<Item> movingItems = new List<Item>();

    public void removeMovingItems(Item itemToRemove)
    {
        movingItems.Remove(itemToRemove);

        if (movingItems.Count == 0)
        {
            if (reviewMove)
            {
                reviewMove = false;

                bool    firstItemMatched = checkItemForMatches(firstItemSwapped, false),
                        secondItemMatched = false;

                if (!firstItemMatched)
                {
                    secondItemMatched = checkItemForMatches(secondItemSwapped, false);
                }

                if (!firstItemMatched && !secondItemMatched)
                {
                    rePoseItems(firstItemSwapped.Index, secondItemSwapped.Index, false);
                }
                else
                {
                    firstItemSwapped = null; secondItemSwapped = null;
                    reviewBoard = true;
                }
            }

            if (reviewBoard)
            {
                reviewBoard = false;

                searchMatchesInBoard();

                if (shouldDropdownItems)
                {
                    dropDownItemsAction.Invoke();
                    dropDownItemsAction = null;
                    shouldDropdownItems = false;
                }
            }

            while (searchMoves() == -1)
            {
                GameSettings.randomizeItems();
            }
        }
    }

    public bool checkItemForMatches(Item item, bool executeIfFound)
    {
        List<Item> horizontalMatchedItems = new List<Item>();

        Item leftNeighbour = item.getLeftNeighbour();

        while (leftNeighbour != null)
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

        if (horizontalMatchedItems.Count < GameSettings.minCombo && verticalMatchedItems.Count < GameSettings.minCombo)
        {
            return false;
        }

        if (executeIfFound)
        {
            if (horizontalMatchedItems.Count >= verticalMatchedItems.Count)
            {
                matchItems(horizontalMatchedItems, true);
            }
            else
            {
                matchItems(verticalMatchedItems, false);
            }
        }

        return true;
    }

    void matchItems(List<Item> itemsToMatch, bool matchHorizontal)
    {
        for (int i = 0; i < itemsToMatch.Count; i++)
        {
            itemsToMatch[i].shouldBeReplaced = true;
        }

        if (matchHorizontal)
        {
            for (int i = 0; i < itemsToMatch.Count; i++)
            {
                List<Item> affectedItems = itemsToMatch[i].getAboveItems();

                affectedItems.Add(itemsToMatch[i]);

                dropDownItemsAction += delegate { dropDownItems(affectedItems); };
            }
        }
        else
        {
            Item lowestItem = getLowestItem(itemsToMatch);
            List<Item> affectedItems = lowestItem.getAboveItems();

            affectedItems.Add(lowestItem);

            dropDownItemsAction += delegate { dropDownItems(affectedItems); };
        }

        shouldDropdownItems = true;
        GameSettings.addPoints(itemsToMatch.Count);
    }

    Item getLowestItem(List<Item> itemsToMatch)
    {
        Item lowestItem = itemsToMatch[0];

        for (int i = 1; i < itemsToMatch.Count; i++)
        {
            if (itemsToMatch[i].transform.position.y < lowestItem.transform.position.y)
            {
                lowestItem = itemsToMatch[i];
            }
        }

        return lowestItem;
    }

    void dropDownItems(List<Item> affectedItems)
    {
        List<Item> matchedItems = affectedItems.Where(itm => itm.shouldBeReplaced).OrderBy(item => item.Index).ToList();

        float newY = affectedItems.OrderByDescending(item => item.Index).First().transform.position.y;

        for (int i = 0; i < matchedItems.Count; i++)
        {
            Vector3 curPos = matchedItems[i].transform.position;

            matchedItems[i].transform.position = new Vector3(curPos.x, newY + (i + 1) * GameSettings.yDistance, curPos.z);

            matchedItems[i].ItemType = GameSettings.getRandomItemType();
        }

        affectedItems = affectedItems.OrderByDescending(item => item.transform.position.y).ToList();

        List<int> indexes = new List<int>();

        for (int i = 0; i < affectedItems.Count; i++)
        {
            indexes.Add(affectedItems[i].Index);
        }

        indexes = indexes.OrderByDescending(index => index).ToList();

        Vector3 targetOffset = new Vector3(0f, matchedItems.Count * GameSettings.yDistance, 0f);

        for (int i = 0; i < affectedItems.Count; i++)
        {
            Vector3 targetPos = affectedItems[i].transform.position - targetOffset;

            GameSettings.Items[indexes[i]] = affectedItems[i];

            affectedItems[i].gameObject.name = "item_" + indexes[i];

            affectedItems[i].MoveToItem(targetPos);

            affectedItems[i].shouldBeReplaced = false;
        }

        reviewBoard = true;
    }

    public void showHint()
    {
        if (!CanSelectItem) return;

        int indexOfMatchItem = searchMoves();

        if (indexOfMatchItem > 0)
            GameSettings.Items[indexOfMatchItem].showAsHint();
    }

    int searchMoves()
    {
        for (int i = 0; i < GameSettings.Items.Count; i++)
        {
            Item curItem = GameSettings.Items[i];

            List<Item> neighbours = curItem.getAllNeighbours();

            for (int j = 0; j < neighbours.Count; j++)
            {
                int index1 = curItem.Index,
                    index2 = neighbours[j].Index;

                swapItems(GameSettings.Items[index1], GameSettings.Items[index2]);

                bool foundMove = checkItemForMatches(curItem, false);

                swapItems(GameSettings.Items[index1], GameSettings.Items[index2]);

                if (foundMove)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    int searchMatchesInBoard()
    {
        bool outcome = false;

        for (int i = 0; i < GameSettings.Items.Count; i++)
        {
            outcome = checkItemForMatches(GameSettings.Items[i], true);

            if (outcome)
            {
                return i;
            }
        }

        return -1;
    }

    void Awake()
    {
        gameSettings = FindObjectOfType<GameSettings>();

        SaveLoad.GetInstance().loadPersistentData();
        Score = 0;
    }

    public void showNewGamePrompt()
    {
        promptWindow.Show(GlobalVars.GL_NewGamePrompt, GlobalVars.GL_YesPrompt, GlobalVars.GL_NoPrompt, newGame);
    }

    void Start()
    {
        newGame();
    }

    void newGame()
    {
        SelectedItem = null;

        for (int i = 0; i < GameSettings.Items.Count; i++)
        {
            GameSettings.Items[i].IsSelected = false;
            GameSettings.Items[i].IsAHint = false;
            GameSettings.Items[i].shouldBeReplaced = false;
        }

        Score = 0;

        GameSettings.randomizeItems();

        while (searchMoves() == -1)
        {
            GameSettings.randomizeItems();
        }
    }

    public void showQuitGamePrompt()
    {
        promptWindow.Show(GlobalVars.GL_ExitGamePrompt, GlobalVars.GL_YesPrompt, GlobalVars.GL_NoPrompt, Quit);
    }

    void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveLoad.GetInstance().savePersistentData(HighScore);
    }

    #region Borders
    public int getMinRowBorderOfItem(Item item)
    {
        int indexOfItem = GameSettings.Items.IndexOf(item);

        int minBorder = indexOfItem;

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
    #endregion
}
