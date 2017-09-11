using UnityEngine;

public static class Extended
{
    public static void Show(this CanvasGroup cg)
    {
        cg.alpha = 1f;
        cg.blocksRaycasts = cg.interactable = true;
    }

    public static void Hide(this CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.blocksRaycasts = cg.interactable = false;
    }
}
