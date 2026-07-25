using UdonSharp;
using UnityEngine;

public class PaperTabletTabButton : UdonSharpBehaviour
{
    [Header("Manager")]
    public PaperTabletTabManager tabManager;

    [Header("Welke tab opent deze knop?")]
    public int tabIndex = 0;

    public void OnButtonPressed()
    {
        if (tabManager == null) return;

        tabManager.SelectTab(tabIndex);
    }
}