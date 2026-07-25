using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StefanieInVR.PaperTablet 
{

public class PaperTabletToggleVisual : UdonSharpBehaviour
{
    [Header("Manager + index")]
    public PaperTabletToggleManager toggleManager;
    public int toggleIndex = 0;

    [Header("Objecten die aan/uit moeten")]
    public GameObject[] targetObjects;

    [Header("Button - optioneel")]
    public Button targetButton;

    [Header("Sprites - OFF")]
    public Sprite spriteOff;

    [Header("Sprites - OFF Hover/Highlight")]
    public Sprite spriteOffHigh;

    [Header("Sprites - ON")]
    public Sprite spriteOn;

    [Header("Sprites - ON Hover/Highlight")]
    public Sprite spriteOnHigh;

    [Header("Tekst UI - optioneel")]
    public TextMeshProUGUI[] labelTextsUI;

    [Header("Tekst World - optioneel")]
    public TextMeshPro[] labelTextsWorld;

    [Header("Tekst")]
    public string textOff = "Off";
    public string textOn = "On";

    [Header("Tekstkleur - optioneel")]
    public bool useTextColors = false;
    public Color textColorOff = Color.white;
    public Color textColorOn = Color.green;

    private void Start()
    {
        RefreshVisual();
    }

    public void RefreshVisual()
    {
        if (toggleManager == null) return;

        bool isOn = toggleManager.GetToggleState(toggleIndex);

        RefreshTargetObjects(isOn);
        RefreshButtonSprites(isOn);
        RefreshTexts(isOn);
    }

    private void RefreshTargetObjects(bool isOn)
    {
        if (targetObjects == null) return;

        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] != null)
            {
                targetObjects[i].SetActive(isOn);
            }
        }
    }

    private void RefreshButtonSprites(bool isOn)
    {
        if (targetButton == null) return;
        if (targetButton.image == null) return;

        Sprite normalSprite = isOn ? spriteOn : spriteOff;
        Sprite highSprite = isOn ? spriteOnHigh : spriteOffHigh;

        if (normalSprite != null)
        {
            targetButton.image.sprite = normalSprite;
        }

        SpriteState spriteState = targetButton.spriteState;

        if (highSprite != null)
        {
            spriteState.highlightedSprite = highSprite;
            spriteState.pressedSprite = highSprite;
            spriteState.selectedSprite = highSprite;
        }

        targetButton.spriteState = spriteState;
    }

    private void RefreshTexts(bool isOn)
    {
        if (labelTextsUI != null)
        {
            for (int i = 0; i < labelTextsUI.Length; i++)
            {
                if (labelTextsUI[i] != null)
                {
                    labelTextsUI[i].text = isOn ? textOn : textOff;

                    if (useTextColors)
                    {
                        labelTextsUI[i].color = isOn ? textColorOn : textColorOff;
                    }
                }
            }
        }

        if (labelTextsWorld != null)
        {
            for (int i = 0; i < labelTextsWorld.Length; i++)
            {
                if (labelTextsWorld[i] != null)
                {
                    labelTextsWorld[i].text = isOn ? textOn : textOff;

                    if (useTextColors)
                    {
                        labelTextsWorld[i].color = isOn ? textColorOn : textColorOff;
                    }
                }
            }
        }
    }
}
}