using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PaperTabletTabManager : UdonSharpBehaviour
{
    [Header("1. Tab Panels")]
    [Tooltip("Zelfde volgorde als je tabs. Element 0 = tab 0, element 1 = tab 1, enz.")]
    public GameObject[] tabPanels;

    [Header("2. Tab Buttons")]
    [Tooltip("Zelfde volgorde als je tabs.")]
    public Button[] tabButtons;

    [Header("3. Button Sprites")]
    public Sprite[] tabOffSprites;
    public Sprite[] tabOffHighSprites;
    public Sprite[] tabOnSprites;
    public Sprite[] tabOnHighSprites;

    [Header("4. Tab States")]
    [Tooltip("Sleep hier de PaperTabletTabState scripts in. Elke TabState zegt welke extra objecten zichtbaar zijn op die tab.")]
    public PaperTabletTabState[] tabStates;

    [Header("5. VIP Access Filter")]
    [Tooltip("Sleep hier je VipAccessManager in. Die bepaalt lokaal of deze speler tabs mag bedienen en filtert VIP/locked inhoud.")]
    public VipAccessManager vipAccessManager;

    [Header("6. Start Tab")]
    public int defaultTabIndex = 0;

    [UdonSynced] private int selectedTabIndex = 0;

    private bool hasStarted = false;

    private bool pendingSerialization = false;
    private int serializationTries = 0;

    private void Start()
    {
        if (Networking.IsOwner(gameObject))
        {
            selectedTabIndex = ClampTabIndex(defaultTabIndex);
            RequestSerialization();
        }

        hasStarted = true;
        RefreshAll();
    }

    public override void OnDeserialization()
    {
        if (!hasStarted) return;

        RefreshAll();
    }

    public void SelectTab(int tabIndex)
    {
        tabIndex = ClampTabIndex(tabIndex);

        if (vipAccessManager != null)
        {
            if (!vipAccessManager.CanLocalPlayerSelectTab(tabIndex, selectedTabIndex))
            {
                return;
            }
        }

        if (selectedTabIndex == tabIndex)
        {
            RefreshAll();
            return;
        }

        int previousTabIndex = selectedTabIndex;

        TakeOwnershipIfNeeded();

        selectedTabIndex = tabIndex;

        RefreshAllForTabChange(previousTabIndex, selectedTabIndex);

        RequestSerializationSafe();
    }

    private void TakeOwnershipIfNeeded()
    {
        if (Networking.IsOwner(gameObject)) return;
        if (Networking.LocalPlayer == null) return;

        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    private void RequestSerializationSafe()
    {
        pendingSerialization = true;
        serializationTries = 0;
        SendCustomEventDelayedFrames(nameof(DelayedRequestSerialization), 1);
    }

    public void DelayedRequestSerialization()
    {
        if (!pendingSerialization) return;

        if (!Networking.IsOwner(gameObject))
        {
            serializationTries++;

            if (serializationTries < 10)
            {
                SendCustomEventDelayedFrames(nameof(DelayedRequestSerialization), 1);
            }

            return;
        }

        pendingSerialization = false;
        RequestSerialization();
    }

    public int GetSelectedTabIndex()
    {
        return selectedTabIndex;
    }

    public bool IsTabSelected(int tabIndex)
    {
        return selectedTabIndex == tabIndex;
    }

    public void RefreshAll()
    {
        if (vipAccessManager != null && vipAccessManager.ShouldHideTabletContentForLocalPlayer(selectedTabIndex))
        {
            HideAllPanelsAndTabStateObjects();
            RefreshButtonVisuals();
            ApplyVipAccessFilter();
            return;
        }

        RefreshPanels();
        RefreshAllExtraObjectsForCurrentTab();
        RefreshButtonVisuals();
        ApplyVipAccessFilter();
    }

    private void RefreshAllForTabChange(int oldTabIndex, int newTabIndex)
    {
        if (vipAccessManager != null && vipAccessManager.ShouldHideTabletContentForLocalPlayer(newTabIndex))
        {
            HideAllPanelsAndTabStateObjects();
            RefreshButtonVisuals();
            ApplyVipAccessFilter();
            return;
        }

        RefreshPanels();
        RefreshExtraObjectsTransition(oldTabIndex, newTabIndex);
        RefreshButtonVisuals();
        ApplyVipAccessFilter();
    }

    public void HideAllPanelsAndTabStateObjects()
    {
        if (tabPanels != null)
        {
            for (int i = 0; i < tabPanels.Length; i++)
            {
                SetActiveIfNeeded(tabPanels[i], false);
            }
        }

        if (tabStates != null)
        {
            for (int s = 0; s < tabStates.Length; s++)
            {
                PaperTabletTabState state = tabStates[s];

                if (state == null) continue;
                if (state.objectsVisibleOnThisTab == null) continue;

                for (int i = 0; i < state.objectsVisibleOnThisTab.Length; i++)
                {
                    SetActiveIfNeeded(state.objectsVisibleOnThisTab[i], false);
                }
            }
        }
    }

    private void RefreshPanels()
    {
        if (tabPanels == null) return;

        for (int i = 0; i < tabPanels.Length; i++)
        {
            SetActiveIfNeeded(tabPanels[i], i == selectedTabIndex);
        }
    }

    private void RefreshExtraObjectsTransition(int oldTabIndex, int newTabIndex)
    {
        if (tabStates == null) return;

        for (int s = 0; s < tabStates.Length; s++)
        {
            PaperTabletTabState state = tabStates[s];

            if (state == null) continue;
            if (state.tabIndex != oldTabIndex) continue;
            if (state.objectsVisibleOnThisTab == null) continue;

            for (int i = 0; i < state.objectsVisibleOnThisTab.Length; i++)
            {
                GameObject obj = state.objectsVisibleOnThisTab[i];

                if (obj == null) continue;

                if (!ShouldObjectBeVisibleOnTab(obj, newTabIndex))
                {
                    SetActiveIfNeeded(obj, false);
                }
            }
        }

        for (int s = 0; s < tabStates.Length; s++)
        {
            PaperTabletTabState state = tabStates[s];

            if (state == null) continue;
            if (state.tabIndex != newTabIndex) continue;
            if (state.objectsVisibleOnThisTab == null) continue;

            for (int i = 0; i < state.objectsVisibleOnThisTab.Length; i++)
            {
                SetActiveIfNeeded(state.objectsVisibleOnThisTab[i], true);
            }
        }
    }

    private void RefreshAllExtraObjectsForCurrentTab()
    {
        if (tabStates == null) return;

        for (int s = 0; s < tabStates.Length; s++)
        {
            PaperTabletTabState state = tabStates[s];

            if (state == null) continue;
            if (state.objectsVisibleOnThisTab == null) continue;

            for (int i = 0; i < state.objectsVisibleOnThisTab.Length; i++)
            {
                GameObject obj = state.objectsVisibleOnThisTab[i];

                if (obj == null) continue;

                bool shouldBeVisible = ShouldObjectBeVisibleOnTab(obj, selectedTabIndex);
                SetActiveIfNeeded(obj, shouldBeVisible);
            }
        }
    }

    private bool ShouldObjectBeVisibleOnTab(GameObject obj, int tabIndex)
    {
        if (obj == null) return false;
        if (tabStates == null) return false;

        for (int s = 0; s < tabStates.Length; s++)
        {
            PaperTabletTabState state = tabStates[s];

            if (state == null) continue;
            if (state.tabIndex != tabIndex) continue;
            if (state.objectsVisibleOnThisTab == null) continue;

            for (int i = 0; i < state.objectsVisibleOnThisTab.Length; i++)
            {
                if (state.objectsVisibleOnThisTab[i] == obj)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ApplyVipAccessFilter()
    {
        if (vipAccessManager != null)
        {
            vipAccessManager.ApplyAccessForCurrentTab(selectedTabIndex);
        }
    }

    private void RefreshButtonVisuals()
    {
        if (tabButtons == null) return;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            Button button = tabButtons[i];

            if (button == null) continue;
            if (button.image == null) continue;

            bool isActive = i == selectedTabIndex;

            Sprite normalSprite = GetSprite(isActive ? tabOnSprites : tabOffSprites, i);
            Sprite highSprite = GetSprite(isActive ? tabOnHighSprites : tabOffHighSprites, i);

            if (normalSprite != null)
            {
                button.image.sprite = normalSprite;
            }

            SpriteState spriteState = button.spriteState;

            if (highSprite != null)
            {
                spriteState.highlightedSprite = highSprite;
                spriteState.pressedSprite = highSprite;
                spriteState.selectedSprite = highSprite;
            }

            button.spriteState = spriteState;
        }
    }

    private Sprite GetSprite(Sprite[] sprites, int index)
    {
        if (sprites == null) return null;
        if (index < 0 || index >= sprites.Length) return null;

        return sprites[index];
    }

    private void SetActiveIfNeeded(GameObject target, bool active)
    {
        if (target == null) return;

        if (target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }

    private int ClampTabIndex(int tabIndex)
    {
        if (tabPanels == null || tabPanels.Length == 0)
        {
            return 0;
        }

        if (tabIndex < 0) return 0;
        if (tabIndex >= tabPanels.Length) return tabPanels.Length - 1;

        return tabIndex;
    }
}