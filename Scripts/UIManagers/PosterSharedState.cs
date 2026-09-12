using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PosterSharedState : UdonSharpBehaviour
{
    public VRCUrlInputField urlInput;
    public PosterImageLoaderLocal imageLoader;

    [UdonSynced]
    private VRCUrl sharedUrl;

    [UdonSynced]
    private bool sharedHasPoster = false;

    private VRCUrl pendingUrl;
    private bool pendingHasPoster = false;

    private bool publishPending = false;
    private int ownershipRetryCount = 0;

    public void PublishUrl()
    {
        VRCUrl newUrl = urlInput.GetUrl();

        if (VRCUrl.IsNullOrEmpty(newUrl))
        {
            Debug.LogWarning(
                "[Poster Shared] Cannot publish an empty URL."
            );
            return;
        }

        BeginChange(newUrl, true);
    }

    public void UnloadPoster()
    {
        BeginChange(VRCUrl.Empty, false);
    }

    private void BeginChange(VRCUrl url, bool hasPoster)
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (!Utilities.IsValid(localPlayer))
        {
            Debug.LogWarning(
                "[Poster Shared] Local player is not ready."
            );
            return;
        }

        pendingUrl = url;
        pendingHasPoster = hasPoster;

        publishPending = true;
        ownershipRetryCount = 0;

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(
                localPlayer,
                gameObject
            );
        }

        SendCustomEventDelayedFrames(
            nameof(CompletePublish),
            1
        );
    }

    public void CompletePublish()
    {
        if (!publishPending)
            return;

        if (!Networking.IsOwner(gameObject))
        {
            ownershipRetryCount++;

            if (ownershipRetryCount < 20)
            {
                SendCustomEventDelayedFrames(
                    nameof(CompletePublish),
                    1
                );
                return;
            }

            publishPending = false;

            Debug.LogError(
                "[Poster Shared] Could not obtain ownership."
            );
            return;
        }

        sharedUrl = pendingUrl;
        sharedHasPoster = pendingHasPoster;
        publishPending = false;

        ApplySharedState();
        RequestSerialization();

        Debug.Log(
            sharedHasPoster
                ? "[Poster Shared] Poster URL serialized."
                : "[Poster Shared] Poster unload serialized."
        );
    }

    public override void OnDeserialization()
    {
        ApplySharedState();
    }

    private void ApplySharedState()
    {
        if (imageLoader == null)
            return;

        if (
            sharedHasPoster &&
            !VRCUrl.IsNullOrEmpty(sharedUrl)
        )
        {
            Debug.Log(
                "[Poster Shared] Shared poster URL received."
            );

            imageLoader.LoadUrl(sharedUrl);
        }
        else
        {
            Debug.Log(
                "[Poster Shared] Shared poster unload received."
            );

            imageLoader.UnloadImage();
        }
    }
}
