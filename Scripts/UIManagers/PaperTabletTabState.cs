using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PaperTabletTabState : UdonSharpBehaviour
{
    [Header("1. Tab")]
    [Tooltip("Bij welke tab hoort deze lijst? 0 = Video, 3 = Playlists, 6 = VIP.")]
    public int tabIndex = 0;

    [Header("2. Objects Visible On This Tab")]
    [Tooltip("Sleep hier alle extra objecten in die zichtbaar moeten zijn wanneer deze tab actief is.")]
    public GameObject[] objectsVisibleOnThisTab;
}