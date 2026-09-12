using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PosterScaleManager : UdonSharpBehaviour
{
    [Header("Scale Target")]
    [Tooltip("Only the child Scale_Root. Do NOT assign the pickup root here.")]
    public Transform scaleRoot;

    [Header("Scale UI")]
    public Slider scaleSlider;

    [Header("Existing Poster ObjectSync")]
    [Tooltip("Assign the VRCObjectSync that already exists on Persistent_Poster.")]
    public VRCObjectSync posterObjectSync;

    [Header("Scale Settings")]
    public float minScale = 1f;
    public float maxScale = 5f;
    public float defaultScale = 1f;

    [Header("Network Settings")]
    [Tooltip("Wait after the slider stops moving before syncing.")]
    public float syncDelay = 0.35f;

    [UdonSynced]
    private float syncedScale = 1f;

    private Vector3 _baseScale;

    private bool _ignoreSliderCallbacks;
    private bool _syncDirty;
    private float _nextSyncTime;
    private float _pendingScale = 1f;

    private bool _scaleCommitInProgress;
    private int _scaleOwnershipRetryCount;

    private bool _positionResetPending;
    private int _positionOwnershipRetryCount;

    private void Start()
    {
        if (scaleRoot != null)
        {
            _baseScale = scaleRoot.localScale;
        }

        _pendingScale = syncedScale;
        ApplyScale(syncedScale, true);
    }

    private void Update()
    {
        if (!_syncDirty)
            return;

        if (_scaleCommitInProgress)
            return;

        if (Time.time < _nextSyncTime)
            return;

        _scaleCommitInProgress = true;
        _scaleOwnershipRetryCount = 0;

        TryCommitScale();
    }

    public void OnScaleSliderChanged()
    {
        if (_ignoreSliderCallbacks)
            return;

        if (scaleSlider == null)
            return;

        float value = Mathf.Clamp(
            scaleSlider.value,
            minScale,
            maxScale
        );

        ApplyScale(value, false);

        _pendingScale = value;
        _syncDirty = true;
        _nextSyncTime = Time.time + syncDelay;
    }

    public void TryCommitScale()
    {
        if (!_syncDirty)
        {
            _scaleCommitInProgress = false;
            return;
        }

        if (Time.time < _nextSyncTime)
        {
            _scaleCommitInProgress = false;
            return;
        }

        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (!Utilities.IsValid(localPlayer))
        {
            _scaleCommitInProgress = false;
            return;
        }

        if (!Networking.IsOwner(gameObject))
        {
            if (_scaleOwnershipRetryCount == 0)
            {
                Networking.SetOwner(localPlayer, gameObject);
            }

            _scaleOwnershipRetryCount++;

            if (_scaleOwnershipRetryCount < 20)
            {
                SendCustomEventDelayedFrames(
                    nameof(TryCommitScale),
                    1
                );
                return;
            }

            _scaleCommitInProgress = false;
            return;
        }

        syncedScale = _pendingScale;

        _syncDirty = false;
        _scaleCommitInProgress = false;
        _scaleOwnershipRetryCount = 0;

        RequestSerialization();
    }

    public void ResetScale()
    {
        float value = Mathf.Clamp(
            defaultScale,
            minScale,
            maxScale
        );

        _pendingScale = value;
        ApplyScale(value, true);

        _syncDirty = true;
        _nextSyncTime = Time.time;

        if (!_scaleCommitInProgress)
        {
            _scaleCommitInProgress = true;
            _scaleOwnershipRetryCount = 0;
            TryCommitScale();
        }
    }

    public void ResetPosition()
    {
        if (posterObjectSync == null)
            return;

        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (!Utilities.IsValid(localPlayer))
            return;

        _positionResetPending = true;
        _positionOwnershipRetryCount = 0;

        TryResetPosition();
    }

    public void TryResetPosition()
    {
        if (!_positionResetPending)
            return;

        if (posterObjectSync == null)
        {
            _positionResetPending = false;
            return;
        }

        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        if (!Utilities.IsValid(localPlayer))
        {
            _positionResetPending = false;
            return;
        }

        GameObject posterRoot = posterObjectSync.gameObject;

        if (!Networking.IsOwner(posterRoot))
        {
            if (_positionOwnershipRetryCount == 0)
            {
                Networking.SetOwner(localPlayer, posterRoot);
            }

            _positionOwnershipRetryCount++;

            if (_positionOwnershipRetryCount < 20)
            {
                SendCustomEventDelayedFrames(
                    nameof(TryResetPosition),
                    1
                );
                return;
            }

            _positionResetPending = false;
            return;
        }

        _positionResetPending = false;
        _positionOwnershipRetryCount = 0;

        posterObjectSync.Respawn();
    }

    public override void OnDeserialization()
    {
        _pendingScale = syncedScale;
        ApplyScale(syncedScale, true);
    }

    private void ApplyScale(float scaleValue, bool updateSlider)
    {
        scaleValue = Mathf.Clamp(
            scaleValue,
            minScale,
            maxScale
        );

        if (scaleRoot != null)
        {
            scaleRoot.localScale =
                _baseScale * scaleValue;
        }

        if (updateSlider && scaleSlider != null)
        {
            _ignoreSliderCallbacks = true;
            scaleSlider.value = scaleValue;
            _ignoreSliderCallbacks = false;
        }
    }
}
