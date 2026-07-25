using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TableButton : UdonSharpBehaviour
{
    [Header("Manager")]
    public TableScreenManager manager;

    [Header("Table index")]
    public int tableIndex;

    [Header("Auto-found UI refs")]
    public Button uiButton;
    public Image targetImage;

    void Start()
    {
        if (uiButton == null)
        {
            uiButton = (Button)GetComponent(typeof(Button));
        }

        if (targetImage == null)
        {
            targetImage = (Image)GetComponent(typeof(Image));
        }
    }

    public void Press()
    {
        if (manager == null) return;
        manager.OnTableButtonPressed(tableIndex);
    }

    public void UpdateVisual(bool isActive, Sprite offSprite, Sprite onSprite, Sprite highlightSprite)
    {
        if (targetImage != null)
        {
            targetImage.sprite = isActive ? onSprite : offSprite;
        }

        if (uiButton != null)
        {
            uiButton.transition = Selectable.Transition.SpriteSwap;

            SpriteState state = uiButton.spriteState;
            state.highlightedSprite = highlightSprite;
            state.pressedSprite = highlightSprite;
            state.selectedSprite = highlightSprite;
            state.disabledSprite = isActive ? onSprite : offSprite;
            uiButton.spriteState = state;
        }
    }
}