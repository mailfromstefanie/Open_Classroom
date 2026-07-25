using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace StefanieInVR.PaperTablet { }

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TXLScreenAutoVisibility : UdonSharpBehaviour
{
    [Header("Required")]
    [Tooltip("Your TXL SyncPlayer component.")]
    public Texel.SyncPlayer txlPlayer;

    [Tooltip("The TXL screen plane object with MeshRenderer.")]
    public GameObject txlScreenObject;

    [Tooltip("The projector screen cloth object with SkinnedMeshRenderer and blendshape.")]
    public GameObject projectorScreenObject;

    [Header("Projector Gate")]
    [Tooltip("Same blendshape index as the projector toggle script.")]
    public int blendShapeIndex = 0;

    [Tooltip("The projector screen counts as open above this blendshape value.")]
    [Range(0f, 100f)]
    public float openThreshold = 99.5f;

    [Header("TXL Gate")]
    [Tooltip("Show the TXL screen while video is loading, but only when the projector screen is open.")]
    public bool showWhenLoading = true;

    [Tooltip("Keep the TXL screen visible while paused.")]
    public bool showWhenPaused = true;

    [Header("Collider Control")]
    [Tooltip("Enable/disable colliders on the TXL screen object together with the renderer.")]
    public bool controlColliders = true;

    [Tooltip("Also include child colliders.")]
    public bool includeChildColliders = false;

    [Header("Performance")]
    [Tooltip("How often visibility is checked. 0.25 is usually light and fast enough.")]
    [Range(0.05f, 1f)]
    public float pollInterval = 0.25f;

    private Renderer screenRenderer;
    private SkinnedMeshRenderer projectorRenderer;
    private Collider[] screenColliders;

    private float nextPollTime;
    private bool lastVisible;

    private void Start()
    {
        CacheComponents();
        ApplyVisibility(true);
    }

    private void Update()
    {
        if (Time.time < nextPollTime)
        {
            return;
        }

        nextPollTime = Time.time + pollInterval;
        ApplyVisibility(false);
    }

    private void CacheComponents()
    {
        if (txlScreenObject != null)
        {
            screenRenderer = txlScreenObject.GetComponent<Renderer>();

            if (controlColliders)
            {
                if (includeChildColliders)
                {
                    screenColliders = txlScreenObject.GetComponentsInChildren<Collider>(true);
                }
                else
                {
                    screenColliders = txlScreenObject.GetComponents<Collider>();
                }
            }
        }

        if (projectorScreenObject != null)
        {
            projectorRenderer = projectorScreenObject.GetComponent<SkinnedMeshRenderer>();
        }
    }

    private void ApplyVisibility(bool force)
    {
        if (!Utilities.IsValid(txlPlayer))
        {
            return;
        }

        if (screenRenderer == null || projectorRenderer == null)
        {
            return;
        }

        float weight = projectorRenderer.GetBlendShapeWeight(blendShapeIndex);
        bool projectorOpen = weight >= openThreshold;

        int state = txlPlayer.playerState;
        bool txlShouldBeVisible = false;

        if (state == Texel.TXLVideoPlayer.VIDEO_STATE_LOADING)
        {
            txlShouldBeVisible = showWhenLoading;
        }
        else if (state == Texel.TXLVideoPlayer.VIDEO_STATE_PLAYING)
        {
            txlShouldBeVisible = txlPlayer.paused ? showWhenPaused : true;
        }
        else
        {
            txlShouldBeVisible = false;
        }

        bool visible = projectorOpen && txlShouldBeVisible;

        if (!force && visible == lastVisible)
        {
            return;
        }

        screenRenderer.enabled = visible;

        if (controlColliders && screenColliders != null)
        {
            for (int i = 0; i < screenColliders.Length; i++)
            {
                if (screenColliders[i] != null)
                {
                    screenColliders[i].enabled = visible;
                }
            }
        }

        lastVisible = visible;
    }
}