using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    GameSettings.ItemTypeEnum itemtype;

    public GameSettings.ItemTypeEnum ItemType
    {
        get { return itemtype; }
        set
        {
            itemtype = value;

            Sprite = GameSettings.getSpriteForType(itemtype);
        }
    }

    public Sprite Sprite
    {
        get
        {
            return GetComponent<SpriteRenderer>().sprite;
        }
        set
        {
            GetComponent<SpriteRenderer>().sprite = value;
        }
    }

    GameSettings gameSettings;

    public GameSettings GameSettings
    {
        get
        {
            if (Application.isPlaying)
                return gameSettings;

            return FindObjectOfType<GameSettings>();
        }
    }

    MatchController matchController;

    public MatchController MatchController
    {
        get
        {
            if (Application.isPlaying)
                return matchController;

            return FindObjectOfType<MatchController>();
        }
    }

    bool isSelected = false;

    public bool IsSelected
    {
        get { return isSelected; }

        set
        {
            isSelected = value;

            Outline.SetActive(isSelected);
        }
    }

    GameObject outline, hintOutline;

    public GameObject Outline
    {
        get
        {
            if (Application.isPlaying)
                return outline;

            return transform.GetChild(0).gameObject;
        }
        set { outline = value; }
    }

    public int Index
    {
        get { return GameSettings.Items.IndexOf(this); }
    }

    public bool shouldBeReplaced = false;

    bool isAHint = false;

    public bool IsAHint
    {
        get { return isAHint; }
        set { isAHint = value; hintOutline.gameObject.SetActive(isAHint); }
    }

    void Awake()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        outline = transform.GetChild(0).gameObject;
        hintOutline = transform.GetChild(1).gameObject;
        matchController = FindObjectOfType<MatchController>();
    }

    void OnMouseDown()
    {
        MatchController.setSelectedItem(this);
    }

    public void MoveToItem(Vector3 targetPos)
    {
        MatchController.movingItems.Add(this);

        StartCoroutine(IMoveToPose(targetPos));
    }

    IEnumerator IMoveToPose(Vector3 targetPos)
    {
        float timeToShow = GameSettings.moveItemsSpeed;

        Vector3 startPos = transform.position;

        for (float t = 0f; t < timeToShow; t += Time.deltaTime/timeToShow)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t/timeToShow);
            yield return null;
        }

        transform.position = targetPos;

        MatchController.removeMovingItems(this);
    }

    public List<Item> getAllNeighbours()
    {
        List<Item> result = new List<Item>();

        Item    leftNeighbour   = getLeftNeighbour(),
                rightNeighbour  = getRightNeighbour(),
                topNeighbour    = getTopNeighbour(),
                bottomNeighbour = getBottomNeighbour();

        if (leftNeighbour != null)      result.Add(leftNeighbour);
        if (rightNeighbour != null)     result.Add(rightNeighbour);
        if (topNeighbour != null)       result.Add(topNeighbour);
        if (bottomNeighbour != null)    result.Add(bottomNeighbour);

        return result;
    }

    public Item getLeftNeighbour()
    {
        int index = Index;

        int leftIndex = index - 1;

        int minBorder = MatchController.getMinRowBorderOfItem(this);

        if (leftIndex >= minBorder)
        { 
            if (leftIndex > -1 && leftIndex < GameSettings.Items.Count)
            {
                return GameSettings.Items[leftIndex];
            }
        }

        return null;
    }

    public Item getRightNeighbour()
    {
        int index = Index;

        int rightIndex = index + 1;

        int maxBorder = MatchController.getMaxRowBorderOfItem(this);

        if (rightIndex <= maxBorder)
        {
            if (rightIndex > -1 && rightIndex < GameSettings.Items.Count)
            {
                return GameSettings.Items[rightIndex];
            }
        }

        return null;
    }

    public Item getTopNeighbour()
    {
        int index = Index;

        int topIndex = index + GameSettings.rows;

        int maxBorder = MatchController.getMaxColumnBorderOfItem(this);

        if (topIndex <= maxBorder)
        {
            if (topIndex > -1 && topIndex < GameSettings.Items.Count)
            {
                return GameSettings.Items[topIndex];
            }
        }

        return null;
    }

    public Item getBottomNeighbour()
    {
        int index = Index;

        int bottomIndex = index - GameSettings.rows;

        int minBorder = MatchController.getMinColumnBorderOfItem(this);

        if (bottomIndex >= minBorder)
        {
            if (bottomIndex > -1 && bottomIndex < GameSettings.Items.Count)
            {
                return GameSettings.Items[bottomIndex];
            }
        }

        return null;
    }

    public List<Item> getAboveItems()
    {
        List<Item> aboveItems = new List<Item>();

        for (int i = Index + GameSettings.rows; i < GameSettings.Items.Count; i += GameSettings.rows)
        {
            aboveItems.Add(GameSettings.Items[i]);
        }

        return aboveItems;
    }

    public void showAsHint()
    {
        StartCoroutine(IShowAsHint());
    }

    IEnumerator IShowAsHint()
    {
        IsAHint = true;

        yield return new WaitForSeconds(GameSettings.hintDuration);

        IsAHint = false;
    }
}
