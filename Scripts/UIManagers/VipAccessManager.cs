using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class VipAccessManager : UdonSharpBehaviour
{
    [Header("1. VIP List")]
    [Tooltip("URL naar een simpele tekstlijst met VRChat display names. Een naam per regel.")]
    public VRCUrl vipListUrl;

    [Header("2. Tablet / Tab")]
    [Tooltip("Je PaperTabletTabManager.")]
    public PaperTabletTabManager tabletTabManager;

    [Tooltip("Welke tabIndex is de VIP-tab?")]
    public int vipTabIndex = 6;

    [Header("3. VIP Tab Button")]
    [Tooltip("De zichtbare VIP-tab button.")]
    public Button vipTabButton;

    [Header("4. VIP Panel Content")]
    [Tooltip("Echte VIP-inhoud. Alleen zichtbaar voor spelers op de VIP-lijst wanneer de VIP-tab open is.")]
    public GameObject vipContentRoot;

    [Tooltip("Niet meer hoofdlogica. Mag leeg blijven als je nu alleen TabletLockedRoot gebruikt.")]
    public GameObject nonVipDummyRoot;

    [Header("5. Tablet Locked Overlay")]
    [Tooltip("Algemeen locked-scherm dat niet-VIPs zien wanneer Tablet Lock aan staat, of wanneer zij de VIP-tab openen.")]
    public GameObject tabletLockedRoot;

    [Header("6. Restricted Objects On VIP Tab")]
    [Tooltip("Extra objecten die zeker uit moeten voor niet-VIPs wanneer TabletLockedRoot wordt getoond. Bijvoorbeeld losse PlayerControls als die niet goed via TabState worden uitgezet.")]
    public GameObject[] restrictedVipTabObjects;

    [Header("7. Refresh")]
    [Tooltip("Knop onder VipContentRoot waarmee VIPs/admins de lijst opnieuw laten laden voor iedereen.")]
    public Button refreshVipListButton;

    [Tooltip("TextMeshPro tekstveld om voor VIPs te tonen of refresh gelukt is.")]
    public TextMeshProUGUI refreshStatusText;

    [Header("8. Optional Local VIP Objects")]
    [Tooltip("Deze objecten gaan lokaal AAN als de speler VIP is, ongeacht welke tab open is.")]
    public GameObject[] enableWhenVip;

    [Tooltip("Deze objecten gaan lokaal UIT als de speler VIP is, en AAN als de speler geen VIP is.")]
    public GameObject[] disableWhenVip;

    [Header("9. Tablet Lock")]
    [Tooltip("Gesyncte tablet lock status. Als deze aan staat, mogen niet-VIPs geen tabs bedienen en zien ze alleen TabletLockedRoot.")]
    [UdonSynced] public bool tabletLockEnabled = false;

    [Tooltip("Knop waarmee VIPs/admins de tablet lock aan/uit zetten.")]
    public Button tabletLockButton;

    [Tooltip("Los TextMeshPro veld dat Tablet Locked / Tablet Unlocked toont.")]
    public TextMeshProUGUI tabletLockStatusText;

    [Tooltip("Image component van de tablet lock knop waarvan de sprite gewisseld moet worden.")]
    public Image tabletLockButtonImage;

    [Tooltip("Sprite wanneer de tablet locked is.")]
    public Sprite tabletLockOnSprite;

    [Tooltip("Sprite wanneer de tablet unlocked is.")]
    public Sprite tabletLockOffSprite;

    [Header("10. Stage Blocker")]
    [Tooltip("Gesyncte status. Als deze aan staat, krijgen niet-VIPs lokaal de stage blockers aan. VIPs blijven erdoorheen kunnen.")]
    [UdonSynced] public bool stageBlockerEnabled = true;

    [Tooltip("Deze objecten gaan lokaal AAN voor niet-VIPs wanneer Stage Blocker aan staat. Voor VIPs blijven ze UIT.")]
    public GameObject[] stageBlockerObjects;

    [Tooltip("Optionele TextMeshPro tekst om de stage blocker status te tonen.")]
    public TextMeshProUGUI stageBlockerStatusText;

    [Header("10B. Stage Blocker Button Visual")]
    [Tooltip("Image component van de stage blocker knop waarvan de sprite gewisseld moet worden.")]
    public Image stageBlockerButtonImage;

    [Tooltip("Sprite wanneer de stage blocker AAN staat.")]
    public Sprite stageBlockerOnSprite;

    [Tooltip("Sprite wanneer de stage blocker UIT staat.")]
    public Sprite stageBlockerOffSprite;

    [Header("11. Optional Debug")]
    [Tooltip("Alleen handig tijdens testen. Zet uit voor release.")]
    public bool debugLogs = false;

    [UdonSynced] private int refreshVersion = 0;

    private bool isVip = false;
    private bool hasLoadedList = false;

    private int localKnownRefreshVersion = -1;
    private bool localKnownTabletLockEnabled = false;
    private bool localKnownStageBlockerEnabled = true;

    private string lastLoadedListText = "";
    private bool hasPreviousListText = false;
    private bool currentLoadWasRefresh = false;

    private bool pendingSerialization = false;
    private int serializationTries = 0;

    private void Start()
    {
        localKnownTabletLockEnabled = tabletLockEnabled;
        localKnownStageBlockerEnabled = stageBlockerEnabled;

        ApplyAccessBase(false);
        ApplyTabletLockVisual();
        ApplyTabletLockedOverlay();
        ApplyStageBlockerAccess();
        UpdateStageBlockerStatusAndVisual();

        SetStatus("Loading VIP list...");

        localKnownRefreshVersion = refreshVersion;

        LoadVipList(false);
    }

    public override void OnDeserialization()
    {
        bool tabletLockChanged = localKnownTabletLockEnabled != tabletLockEnabled;
        bool stageBlockerChanged = localKnownStageBlockerEnabled != stageBlockerEnabled;

        localKnownTabletLockEnabled = tabletLockEnabled;
        localKnownStageBlockerEnabled = stageBlockerEnabled;

        ApplyTabletLockVisual();
        ApplyStageBlockerAccess();
        UpdateStageBlockerStatusAndVisual();

        if (tabletLockChanged || stageBlockerChanged)
        {
            if (tabletTabManager != null)
            {
                tabletTabManager.RefreshAll();
            }
            else
            {
                RefreshCurrentTabFilter();
            }
        }

        if (localKnownRefreshVersion == refreshVersion)
        {
            return;
        }

        localKnownRefreshVersion = refreshVersion;

        if (debugLogs)
        {
            Debug.Log("[VipAccessManager] Refresh signal received. Version: " + refreshVersion);
        }

        LoadVipList(true);
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        hasLoadedList = true;

        string newListText = result.Result;
        string localName = "";

        if (Networking.LocalPlayer != null)
        {
            localName = Networking.LocalPlayer.displayName;
        }

        bool previousVipState = isVip;

        string addedNames = "";
        string removedNames = "";
        bool listChanged = false;

        if (hasPreviousListText)
        {
            listChanged = lastLoadedListText != newListText;

            if (listChanged)
            {
                addedNames = GetAddedNames(lastLoadedListText, newListText);
                removedNames = GetRemovedNames(lastLoadedListText, newListText);
            }
        }

        lastLoadedListText = newListText;
        hasPreviousListText = true;

        isVip = IsNameInList(localName, newListText);

        ApplyAccessBase(isVip);

        if (tabletTabManager != null)
        {
            tabletTabManager.RefreshAll();
        }
        else
        {
            RefreshCurrentTabFilter();
        }

        ApplyTabletLockVisual();
        ApplyTabletLockedOverlay();
        ApplyStageBlockerAccess();
        UpdateStageBlockerStatusAndVisual();

        if (debugLogs)
        {
            Debug.Log("[VipAccessManager] Local player: " + localName + " | Is VIP: " + isVip + " | List changed: " + listChanged);
        }

        if (currentLoadWasRefresh)
        {
            if (listChanged)
            {
                string message = "VIP list updated.";

                if (!string.IsNullOrEmpty(addedNames))
                {
                    message += "\nAdded: " + LimitText(addedNames, 160);
                }

                if (!string.IsNullOrEmpty(removedNames))
                {
                    message += "\nRemoved: " + LimitText(removedNames, 160);
                }

                if (string.IsNullOrEmpty(addedNames) && string.IsNullOrEmpty(removedNames))
                {
                    message += "\nOnly comments/spacing changed.";
                }

                if (previousVipState != isVip)
                {
                    message += isVip ? "\nYou are now VIP." : "\nYou are no longer VIP.";
                }
                else
                {
                    message += isVip ? "\nYour access: VIP." : "\nYour access: not VIP.";
                }

                SetStatus(message);
            }
            else
            {
                SetStatus(isVip
                    ? "VIP list reloaded.\nNo name changes found.\nYour access: VIP."
                    : "VIP list reloaded.\nNo name changes found.\nYour access: not VIP.");
            }
        }
        else
        {
            SetStatus(isVip
                ? "VIP access loaded.\nYour access: VIP."
                : "VIP access loaded.\nYour access: not VIP.");
        }

        currentLoadWasRefresh = false;
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        hasLoadedList = false;
        isVip = false;
        currentLoadWasRefresh = false;

        if (debugLogs)
        {
            Debug.Log("[VipAccessManager] Could not load VIP list. Error: " + result.ErrorCode + " - " + result.Error);
        }

        ApplyAccessBase(false);

        if (tabletTabManager != null)
        {
            tabletTabManager.RefreshAll();
        }
        else
        {
            RefreshCurrentTabFilter();
        }

        ApplyTabletLockVisual();
        ApplyTabletLockedOverlay();
        ApplyStageBlockerAccess();
        UpdateStageBlockerStatusAndVisual();

        SetStatus("Could not load VIP list.");
    }

    public void RequestVipListRefreshForEveryone()
    {
        if (!isVip)
        {
            SetStatus("Only VIPs can refresh the VIP list for everyone.");
            return;
        }

        TakeOwnershipIfNeeded();
        if (!Networking.IsOwner(gameObject)) return;

        refreshVersion++;
        localKnownRefreshVersion = refreshVersion;

        RequestSerializationSafe();

        if (debugLogs)
        {
            Debug.Log("[VipAccessManager] Refresh requested for everyone. Version: " + refreshVersion);
        }

        SetStatus("Refreshing VIP list for everyone...");
        LoadVipList(true);
    }

    public void RefreshVipListLocalOnly()
    {
        SetStatus("Refreshing VIP list locally...");
        LoadVipList(true);
    }

    public void ToggleTabletLockForEveryone()
    {
        if (!isVip)
        {
            SetStatus("Only VIPs can lock or unlock the tablet.");
            return;
        }

        TakeOwnershipIfNeeded();
        if (!Networking.IsOwner(gameObject)) return;

        tabletLockEnabled = !tabletLockEnabled;
        localKnownTabletLockEnabled = tabletLockEnabled;

        RequestSerializationSafe();

        if (tabletTabManager != null)
        {
            tabletTabManager.RefreshAll();
        }
        else
        {
            RefreshCurrentTabFilter();
        }

        ApplyTabletLockVisual();
        ApplyTabletLockedOverlay();

        if (debugLogs)
        {
            Debug.Log("[VipAccessManager] Tablet lock toggled. Locked: " + tabletLockEnabled);
        }
    }

    public void ToggleStageBlockerForEveryone()
    {
        if (!isVip)
        {
            SetStatus("Only VIPs can change the stage blocker.");
            return;
        }

        TakeOwnershipIfNeeded();
        if (!Networking.IsOwner(gameObject)) return;

        stageBlockerEnabled = !stageBlockerEnabled;
        localKnownStageBlockerEnabled = stageBlockerEnabled;

        RequestSerializationSafe();

        ApplyStageBlockerAccess();
        UpdateStageBlockerStatusAndVisual();

        if (debugLogs)
        {
            Debug.Log("[VipAccessManager] Stage blocker toggled. Enabled: " + stageBlockerEnabled);
        }
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

    public bool IsLocalPlayerVip()
    {
        return isVip;
    }

    public bool HasLoadedList()
    {
        return hasLoadedList;
    }

    public bool IsTabletLocked()
    {
        return tabletLockEnabled;
    }

    public bool IsStageBlockerEnabled()
    {
        return stageBlockerEnabled;
    }

    public bool ShouldHideTabletContentForLocalPlayer(int currentTabIndex)
    {
        if (isVip)
        {
            return false;
        }

        if (tabletLockEnabled)
        {
            return true;
        }

        if (currentTabIndex == vipTabIndex)
        {
            return true;
        }

        return false;
    }

    public bool CanLocalPlayerSelectTab(int requestedTabIndex, int currentTabIndex)
    {
        if (isVip)
        {
            return true;
        }

        if (tabletLockEnabled)
        {
            return false;
        }

        return true;
    }

    public void ApplyAccessForCurrentTab(int currentTabIndex)
    {
        bool showOnlyLockedRoot = ShouldHideTabletContentForLocalPlayer(currentTabIndex);

        if (vipTabButton != null)
        {
            vipTabButton.interactable = true;
        }

        if (tabletLockButton != null)
        {
            tabletLockButton.interactable = isVip;
        }

        if (refreshVipListButton != null)
        {
            refreshVipListButton.interactable = isVip;
        }

        SetObjectsActive(enableWhenVip, isVip);
        SetObjectsActive(disableWhenVip, !isVip);

        ApplyTabletLockVisual();
        ApplyTabletLockedOverlay();
        ApplyStageBlockerAccess();
        UpdateStageBlockerStatusAndVisual();

        if (showOnlyLockedRoot)
        {
            SetActiveIfNeeded(vipContentRoot, false);
            SetActiveIfNeeded(nonVipDummyRoot, false);
            SetObjectsActive(restrictedVipTabObjects, false);
            return;
        }

        if (currentTabIndex != vipTabIndex)
        {
            SetActiveIfNeeded(vipContentRoot, false);
            SetActiveIfNeeded(nonVipDummyRoot, false);
            return;
        }

        SetActiveIfNeeded(vipContentRoot, isVip);
        SetActiveIfNeeded(nonVipDummyRoot, !isVip);
        SetObjectsActive(restrictedVipTabObjects, isVip);
    }

    private void RefreshCurrentTabFilter()
    {
        if (tabletTabManager != null)
        {
            ApplyAccessForCurrentTab(tabletTabManager.GetSelectedTabIndex());
        }
        else
        {
            ApplyAccessForCurrentTab(-1);
        }
    }

    private void LoadVipList(bool isRefresh)
    {
        currentLoadWasRefresh = isRefresh;

        if (vipListUrl == null || string.IsNullOrEmpty(vipListUrl.ToString()))
        {
            ApplyAccessBase(false);

            if (tabletTabManager != null)
            {
                tabletTabManager.RefreshAll();
            }
            else
            {
                RefreshCurrentTabFilter();
            }

            ApplyTabletLockVisual();
            ApplyTabletLockedOverlay();
            ApplyStageBlockerAccess();
            UpdateStageBlockerStatusAndVisual();

            SetStatus("No VIP list URL set.");
            return;
        }

        VRCStringDownloader.LoadUrl(vipListUrl, (IUdonEventReceiver)this);
    }

    private void ApplyAccessBase(bool allowVipAccess)
    {
        if (refreshVipListButton != null)
        {
            refreshVipListButton.interactable = allowVipAccess;
        }

        if (tabletLockButton != null)
        {
            tabletLockButton.interactable = allowVipAccess;
        }

        SetObjectsActive(enableWhenVip, allowVipAccess);
        SetObjectsActive(disableWhenVip, !allowVipAccess);
    }

    private void ApplyTabletLockVisual()
    {
        if (tabletLockStatusText != null)
        {
            tabletLockStatusText.text = tabletLockEnabled ? "Tablet Locked" : "Tablet Unlocked";
        }

        if (tabletLockButtonImage != null)
        {
            if (tabletLockEnabled)
            {
                if (tabletLockOnSprite != null)
                {
                    tabletLockButtonImage.sprite = tabletLockOnSprite;
                }
            }
            else
            {
                if (tabletLockOffSprite != null)
                {
                    tabletLockButtonImage.sprite = tabletLockOffSprite;
                }
            }
        }
    }

    private void ApplyTabletLockedOverlay()
    {
        int currentTabIndex = -1;

        if (tabletTabManager != null)
        {
            currentTabIndex = tabletTabManager.GetSelectedTabIndex();
        }

        SetActiveIfNeeded(tabletLockedRoot, ShouldHideTabletContentForLocalPlayer(currentTabIndex));
    }

    private void ApplyStageBlockerAccess()
    {
        bool shouldBlockThisPlayer = stageBlockerEnabled && !isVip;

        SetObjectsActive(stageBlockerObjects, shouldBlockThisPlayer);
    }

    private void UpdateStageBlockerStatusAndVisual()
    {
        if (stageBlockerStatusText != null)
        {
            stageBlockerStatusText.text = stageBlockerEnabled
                ? "Stage blocker: ON for non-VIPs"
                : "Stage blocker: OFF";
        }

        if (stageBlockerButtonImage != null)
        {
            if (stageBlockerEnabled)
            {
                if (stageBlockerOnSprite != null)
                {
                    stageBlockerButtonImage.sprite = stageBlockerOnSprite;
                }
            }
            else
            {
                if (stageBlockerOffSprite != null)
                {
                    stageBlockerButtonImage.sprite = stageBlockerOffSprite;
                }
            }
        }
    }

    private bool IsNameInList(string playerName, string listText)
    {
        if (string.IsNullOrEmpty(playerName)) return false;
        if (string.IsNullOrEmpty(listText)) return false;

        string cleanPlayerName = NormalizeName(playerName);

        string[] lines = listText.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = NormalizeName(lines[i]);

            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("#")) continue;

            if (line == cleanPlayerName)
            {
                return true;
            }
        }

        return false;
    }

    private string GetAddedNames(string oldText, string newText)
    {
        string result = "";

        string[] newLines = newText.Split('\n');

        for (int i = 0; i < newLines.Length; i++)
        {
            string newName = CleanListName(newLines[i]);

            if (string.IsNullOrEmpty(newName)) continue;

            if (!IsNameInList(newName, oldText))
            {
                result = AddNameToResult(result, newName);
            }
        }

        return result;
    }

    private string GetRemovedNames(string oldText, string newText)
    {
        string result = "";

        string[] oldLines = oldText.Split('\n');

        for (int i = 0; i < oldLines.Length; i++)
        {
            string oldName = CleanListName(oldLines[i]);

            if (string.IsNullOrEmpty(oldName)) continue;

            if (!IsNameInList(oldName, newText))
            {
                result = AddNameToResult(result, oldName);
            }
        }

        return result;
    }

    private string CleanListName(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        string clean = value.Replace("\r", "").Trim();

        if (string.IsNullOrEmpty(clean)) return "";
        if (clean.StartsWith("#")) return "";

        return clean;
    }

    private string AddNameToResult(string currentResult, string nameToAdd)
    {
        if (string.IsNullOrEmpty(currentResult))
        {
            return nameToAdd;
        }

        return currentResult + ", " + nameToAdd;
    }

    private string NormalizeName(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";

        return value.Replace("\r", "").Trim().ToLower();
    }

    private string LimitText(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return "";

        if (value.Length <= maxLength)
        {
            return value;
        }

        return value.Substring(0, maxLength) + "...";
    }

    private void SetStatus(string message)
    {
        if (refreshStatusText != null)
        {
            refreshStatusText.text = message;
        }
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null) return;

        for (int i = 0; i < objects.Length; i++)
        {
            SetActiveIfNeeded(objects[i], active);
        }
    }

    private void SetActiveIfNeeded(GameObject target, bool active)
    {
        if (target == null) return;

        if (target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }
}