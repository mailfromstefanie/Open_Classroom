using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GlobeTransformManager : UdonSharpBehaviour
{
    [Header("Targets")]
    [Tooltip("The root pickup object. This usually has VRC_Pickup, Rigidbody, Collider and VRCObjectSync.")]
    public Transform pickupRoot;

    [Tooltip("The child object that will be scaled by the scale slider.")]
    public Transform scaleRoot;

    [Tooltip("The child pivot that will be rotated by the rotate slider.")]
    public Transform yawPivot;

    [Header("UI Sliders")]
    public Slider rotateSlider;
    public Slider scaleSlider;

    [Header("Yaw Settings")]
    public float minYaw = -180f;
    public float maxYaw = 180f;
    public Vector3 localYawAxis = Vector3.up;

    [Header("Scale Settings")]
    [Tooltip("Normal start scale. For your globe this should usually be 1.")]
    public float minScale = 1f;

    [Tooltip("Maximum enlarged scale.")]
    public float maxScale = 5f;

    [Header("Default Slider Values")]
    [Tooltip("0.5 means the rotate slider starts in the middle.")]
    [Range(0f, 1f)]
    public float defaultYaw01 = 0.5f;

    [Tooltip("0 means normal size when Min Scale is 1.")]
    [Range(0f, 1f)]
    public float defaultScale01 = 0f;

    [Header("Sync Settings")]
    public bool synced = true;

    [Tooltip("Delay after slider movement before sending network sync. Keeps it Quest-friendly.")]
    public float syncDelay = 0.35f;

    [UdonSynced] private float syncedYaw01 = 0.5f;
    [UdonSynced] private float syncedScale01 = 0f;

    private bool _ignoreSliderCallbacks;
    private bool _syncDirty;
    private float _nextSyncTime;

    private Quaternion _baseYawLocalRotation;
    private Vector3 _baseScaleRootLocalScale;

    private void Start()
    {
        if (yawPivot != null)
        {
            _baseYawLocalRotation = yawPivot.localRotation;
        }

        if (scaleRoot != null)
        {
            _baseScaleRootLocalScale = scaleRoot.localScale;
        }

        ApplyAll(true);
    }

    private void Update()
    {
        if (!synced) return;
        if (!_syncDirty) return;

        if (Time.time >= _nextSyncTime)
        {
            CommitSyncedValues();
        }
    }

    public void OnRotateSliderChanged()
    {
        if (_ignoreSliderCallbacks) return;
        if (rotateSlider == null) return;

        float value = Mathf.Clamp01(rotateSlider.value);

        ApplyYaw(value, false);

        if (synced)
        {
            EnsureOwnership();
            syncedYaw01 = value;
            MarkDirtyForDelayedSync();
        }
    }

    public void OnScaleSliderChanged()
    {
        if (_ignoreSliderCallbacks) return;
        if (scaleSlider == null) return;

        float value = Mathf.Clamp01(scaleSlider.value);

        ApplyScale(value, false);

        if (synced)
        {
            EnsureOwnership();
            syncedScale01 = value;
            MarkDirtyForDelayedSync();
        }
    }

    public void ResetGlobeControls()
    {
        EnsureOwnership();

        syncedYaw01 = Mathf.Clamp01(defaultYaw01);
        syncedScale01 = Mathf.Clamp01(defaultScale01);

        _syncDirty = false;

        ApplyAll(true);

        if (synced)
        {
            RequestSerialization();
        }
    }

    public void RefreshFromManager()
    {
        ApplyAll(true);
    }

    public override void OnDeserialization()
    {
        if (!synced) return;

        ApplyAll(true);
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        ApplyAll(true);
    }

    private void ApplyAll(bool updateSliderUI)
    {
        ApplyYaw(syncedYaw01, updateSliderUI);
        ApplyScale(syncedScale01, updateSliderUI);
    }

    private void ApplyYaw(float value01, bool updateSliderUI)
    {
        value01 = Mathf.Clamp01(value01);

        if (yawPivot != null)
        {
            Vector3 axis = localYawAxis;

            if (axis == Vector3.zero)
            {
                axis = Vector3.up;
            }

            axis.Normalize();

            float angle = Mathf.Lerp(minYaw, maxYaw, value01);
            Quaternion rotationOffset = Quaternion.AngleAxis(angle, axis);

            yawPivot.localRotation = _baseYawLocalRotation * rotationOffset;
        }

        if (updateSliderUI && rotateSlider != null)
        {
            _ignoreSliderCallbacks = true;
            rotateSlider.value = value01;
            _ignoreSliderCallbacks = false;
        }
    }

    private void ApplyScale(float value01, bool updateSliderUI)
    {
        value01 = Mathf.Clamp01(value01);

        if (scaleRoot != null)
        {
            float scaleValue = Mathf.Lerp(minScale, maxScale, value01);
            scaleRoot.localScale = _baseScaleRootLocalScale * scaleValue;
        }

        if (updateSliderUI && scaleSlider != null)
        {
            _ignoreSliderCallbacks = true;
            scaleSlider.value = value01;
            _ignoreSliderCallbacks = false;
        }
    }

    private void MarkDirtyForDelayedSync()
    {
        _syncDirty = true;
        _nextSyncTime = Time.time + syncDelay;
    }

    private void CommitSyncedValues()
    {
        if (!synced) return;

        EnsureOwnership();

        _syncDirty = false;
        RequestSerialization();
    }

    private void EnsureOwnership()
    {
        if (!synced) return;
        if (Networking.LocalPlayer == null) return;

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }
}