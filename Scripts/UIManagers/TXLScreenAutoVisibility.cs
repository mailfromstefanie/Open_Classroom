using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using StefanieInVR.Presentation;

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

    [Tooltip("Optional Presentation controller. While Presentation Mode is active, the physical screen stays visible even though VideoTXL is locally suspended.")]
    public PresentationController presentationController;

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
    [Tooltip("How often visibility is checked.")]
    [Range(0.05f, 1f)]
    public float pollInterval = 0.25f;

    private Renderer screenRenderer;
    private SkinnedMeshRenderer projectorRenderer;
    private Collider[] screenColliders;

    // Runtime UdonBehaviour belonging to VideoTXL.
    // We only READ variables from it.
    private UdonBehaviour txlUdonBehaviour;

    private float nextPollTime;
    private bool lastVisible;

    private const string PlayerStateVariable = "playerState";
    private const string PausedVariable = "paused";

    private void Start()
    {
        CacheComponents();
        TryCacheTxlUdonBehaviour();
        ApplyVisibility(true);
    }

    private void Update()
    {
        if (Time.time < nextPollTime)
        {
            return;
        }

        nextPollTime = Time.time + pollInterval;

        // If VideoTXL was not ready during Start, try again later.
        if (!Utilities.IsValid(txlUdonBehaviour))
        {
            TryCacheTxlUdonBehaviour();
        }

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
                    screenColliders =
                        txlScreenObject.GetComponentsInChildren<Collider>(true);
                }
                else
                {
                    screenColliders =
                        txlScreenObject.GetComponents<Collider>();
                }
            }
        }

        if (projectorScreenObject != null)
        {
            projectorRenderer =
                projectorScreenObject.GetComponent<SkinnedMeshRenderer>();
        }
    }

    private void TryCacheTxlUdonBehaviour()
    {
        if (!Utilities.IsValid(txlPlayer))
        {
            return;
        }

        UdonBehaviour[] behaviours =
            txlPlayer.gameObject.GetComponents<UdonBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            UdonBehaviour candidate = behaviours[i];

            if (!Utilities.IsValid(candidate))
            {
                continue;
            }

            // Do NOT use txlPlayer.playerState directly.
            // Look for the Udon program that contains VideoTXL's runtime state.
            object stateValue =
                candidate.GetProgramVariable(PlayerStateVariable);

            if (stateValue != null)
            {
                txlUdonBehaviour = candidate;
                return;
            }
        }
    }

    private void ApplyVisibility(bool force)
    {
        if (screenRenderer == null || projectorRenderer == null)
        {
            return;
        }

        float weight =
            projectorRenderer.GetBlendShapeWeight(blendShapeIndex);

        bool projectorOpen =
            weight >= openThreshold;

        bool txlShouldBeVisible = false;

        // Read VideoTXL state only when its UdonBehaviour is available.
        if (Utilities.IsValid(txlUdonBehaviour))
        {
            object stateValue =
                txlUdonBehaviour.GetProgramVariable(PlayerStateVariable);

            object pausedValue =
                txlUdonBehaviour.GetProgramVariable(PausedVariable);

            if (stateValue != null)
            {
                int state = (int)stateValue;

                bool isPaused = false;

                if (pausedValue != null)
                {
                    isPaused = (bool)pausedValue;
                }

                if (state == Texel.TXLVideoPlayer.VIDEO_STATE_LOADING)
                {
                    txlShouldBeVisible = showWhenLoading;
                }
                else if (state == Texel.TXLVideoPlayer.VIDEO_STATE_PLAYING)
                {
                    txlShouldBeVisible =
                        isPaused ? showWhenPaused : true;
                }
            }
        }

        // Presentation Mode is independent from VideoTXL playback.
        bool presentationShouldBeVisible =
            Utilities.IsValid(presentationController) &&
            presentationController.modeActive;

        bool visible =
            projectorOpen &&
            (txlShouldBeVisible || presentationShouldBeVisible);

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
