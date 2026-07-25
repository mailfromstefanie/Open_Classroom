using UdonSharp;
using UnityEngine;

namespace StefanieInVR.PaperTablet
{

public class PaperTabletToggleButton : UdonSharpBehaviour
{
    [Header("Manager")]
    public PaperTabletToggleManager toggleManager;

    [Header("Welke toggle bestuur ik?")]
    public int toggleIndex = 0;

    public void OnButtonPressed()
    {
        if (toggleManager == null) return;

        toggleManager.ToggleFromButton(toggleIndex);
    }
}
}