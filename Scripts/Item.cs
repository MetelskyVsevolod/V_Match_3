using System;
using System.Collections;
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

    GameObject outline;

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

    void Awake()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        outline = transform.GetChild(0).gameObject;
        matchController = FindObjectOfType<MatchController>();
    }

    void OnMouseDown()
    {
        matchController.setSelectedItem(this);
    }

    Coroutine movePoseCoroutine;

    Action invokeOnStoppedMoving;

    int itemToReplace = -1;

    public int ItemToReplace
    {
        get{ return itemToReplace;}
        set
        {
            itemToReplace = value;
            //string deb = value.ToString();

            //if (value != -1)
            //    deb += " " + GameSettings.Items[value].name;

            //Debug.Log("\\\\\\"+ deb);
        }
    }

    public void MoveToItem(Vector3 targetPos, Action invokeOnStoppedMoving)
    {
        //matchController.AddMovingItem(this);

        this.invokeOnStoppedMoving = invokeOnStoppedMoving;

        if (movePoseCoroutine != null)
        {
            StopCoroutine(movePoseCoroutine);
        }

        movePoseCoroutine = StartCoroutine(IMoveToPose(targetPos));
    }

    IEnumerator IMoveToPose(Vector3 targetPos)
    {
        while(Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*5f);
            yield return null;
        }

        transform.position = targetPos;

       // Debug.Log("END: " + name);

        invokeOnStoppedMoving.Invoke();
    }



    public Item getLeftNeighbour()
    {
        int index = GameSettings.Items.IndexOf(this);

        int leftIndex = index - 1;

        int minBorder = matchController.getMinRowBorderOfItem(this);

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
        int index = GameSettings.Items.IndexOf(this);

        int rightIndex = index + 1;

        int maxBorder = matchController.getMaxRowBorderOfItem(this);

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
        int index = GameSettings.Items.IndexOf(this);

        int topIndex = index + GameSettings.rows;

        int maxBorder = matchController.getMaxColumnBorderOfItem(this);

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
        int index = GameSettings.Items.IndexOf(this);

        int bottomIndex = index - GameSettings.rows;

        int minBorder = matchController.getMinColumnBorderOfItem(this);

        if (bottomIndex >= minBorder)
        {
            if (bottomIndex > -1 && bottomIndex < GameSettings.Items.Count)
            {
                return GameSettings.Items[bottomIndex];
            }
        }

        return null;
    }
}
