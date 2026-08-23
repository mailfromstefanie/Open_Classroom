using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using Texel;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TXLPlaylistPrivacyFilter : UdonSharpBehaviour
{
    [Header("VideoTXL")]
    public VideoSourceUI videoSourceUI;
    public GameObject playlistContentRoot;

    [Header("VIP access")]
    public VipAccessManager vipAccessManager;
    public Playlist[] vipOnlyPlaylists;

    [Header("Only me")]
    public string ownerDisplayName;
    public Playlist[] ownerOnlyPlaylists;
    public GameObject[] ownerOnlyButtons;

    private SourceManager sourceManager;

    private void Start()
    {
        ApplyOwnerOnlyButtons();

        // Give VideoTXL time to bind its Source Manager first.
        SendCustomEventDelayedFrames(nameof(InitializePrivacyFilter), 2);
    }

    public void InitializePrivacyFilter()
    {
        if (videoSourceUI == null || playlistContentRoot == null)
            return;

        sourceManager = videoSourceUI.SourceManager;

        if (sourceManager == null)
            return;

        sourceManager._Register(
            SourceManager.EVENT_URL_READY,
            this,
            nameof(OnVideoSourceReady)
        );

        RefreshPrivacy();
    }

    public void OnVideoSourceReady()
    {
        if (sourceManager == null)
            return;

        ApplyForSource(sourceManager.ReadySource);
    }

    public void RefreshPrivacy()
    {
        if (sourceManager == null ||
            sourceManager.VideoPlayer == null)
        {
            return;
        }

        ApplyForSource(sourceManager.VideoPlayer.currentUrlSource);
    }

    private void ApplyForSource(VideoUrlSource source)
    {
        if (playlistContentRoot == null)
            return;

        bool maySeePlaylist = true;

        if (IsOwnerOnlyPlaylist(source))
        {
            maySeePlaylist = IsOwner();
        }
        else if (IsVipOnlyPlaylist(source))
        {
            maySeePlaylist =
                vipAccessManager != null &&
                vipAccessManager.IsLocalPlayerVip();
        }

        playlistContentRoot.SetActive(maySeePlaylist);
    }

    private bool IsVipOnlyPlaylist(VideoUrlSource source)
    {
        if (source == null || vipOnlyPlaylists == null)
            return false;

        for (int i = 0; i < vipOnlyPlaylists.Length; i++)
        {
            if (vipOnlyPlaylists[i] == source)
                return true;
        }

        return false;
    }

    private bool IsOwnerOnlyPlaylist(VideoUrlSource source)
    {
        if (source == null || ownerOnlyPlaylists == null)
            return false;

        for (int i = 0; i < ownerOnlyPlaylists.Length; i++)
        {
            if (ownerOnlyPlaylists[i] == source)
                return true;
        }

        return false;
    }

    private bool IsOwner()
    {
        if (Networking.LocalPlayer == null)
            return false;

        if (string.IsNullOrEmpty(ownerDisplayName))
            return false;

        return Networking.LocalPlayer.displayName == ownerDisplayName;
    }

    private void ApplyOwnerOnlyButtons()
    {
        if (ownerOnlyButtons == null)
            return;

        bool show = IsOwner();

        for (int i = 0; i < ownerOnlyButtons.Length; i++)
        {
            if (ownerOnlyButtons[i] != null)
                ownerOnlyButtons[i].SetActive(show);
        }
    }
}
