using UnityEngine;

public static class ErrorMessages
{
    public static void ShowWrongItemTypeError(string wrongType)
    {
        writeError("Item type <b>" + wrongType + " </b> does not exist.");
    }

    public static void ShowSpriteForTypeError(string typeWithoutSprite)
    {
        writeError("Item type <b>" + typeWithoutSprite + " </b> does not have a valid sprite.");
    }

    public static void ShowNoItemTypesError()
    {
        writeError("GamesSettings script has no available item types!");
    }

    static void writeError(string errorString)
    {
        Debug.Log("<color=red>" + errorString + "</color>");
    }
}
