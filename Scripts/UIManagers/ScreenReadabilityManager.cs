using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ScreenReadabilityManager : UdonSharpBehaviour
    {
        [Header("Screen Materials")]
        [Tooltip("All materials that should receive brightness/contrast changes. Usually your shared video screen material.")]
        public Material[] targetMaterials;

        [Header("Shader Property Names")]
        [Tooltip("Brightness float property on the screen shader.")]
        public string brightnessProperty = "_Brightness";

        [Tooltip("Contrast float property on the screen shader.")]
        public string contrastProperty = "_Contrast";

        [Header("Default Values")]
        [Range(0f, 2f)]
        public float defaultBrightness = 1f;

        [Range(0f, 2f)]
        public float defaultContrast = 1f;

        [Header("Limits")]
        [Range(0f, 2f)]
        public float minBrightness = 0f;

        [Range(0f, 2f)]
        public float maxBrightness = 2f;

        [Range(0f, 2f)]
        public float minContrast = 0f;

        [Range(0f, 2f)]
        public float maxContrast = 2f;

        [Header("Global Synced Values")]
        [UdonSynced]
        private float globalBrightness = 1f;

        [UdonSynced]
        private float globalContrast = 1f;

        [Header("Linked UI Inputs")]
        [Tooltip("All ScreenReadabilityInput scripts that should visually update when values change.")]
        public ScreenReadabilityInput[] linkedInputs;

        [Header("Network Sync")]
        [Tooltip("Delay before sending global slider changes over the network. Prevents network spam while dragging.")]
        public float syncDelay = 0.35f;

        [Header("Debug")]
        public bool debugLogs = false;

        private float localBrightness = 1f;
        private float localContrast = 1f;

        private bool waitingForOwnership;
        private int ownershipRetryCount;
        private const int MaxOwnershipRetries = 10;

        private bool delayedSyncScheduled;
        private float lastGlobalChangeTime;

        private void Start()
        {
            globalBrightness = ClampBrightness(globalBrightness);
            globalContrast = ClampContrast(globalContrast);

            localBrightness = globalBrightness;
            localContrast = globalContrast;

            ApplyValues();
            RefreshLinkedInputs();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Started.");
            }
        }

        public override void OnDeserialization()
        {
            globalBrightness = ClampBrightness(globalBrightness);
            globalContrast = ClampContrast(globalContrast);

            // A global change overrules local values on every client.
            localBrightness = globalBrightness;
            localContrast = globalContrast;

            ApplyValues();
            RefreshLinkedInputs();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Received global values from network.");
            }
        }

        public void SetGlobalBrightness01(float normalizedValue)
        {
            SetGlobalBrightness(Convert01ToBrightness(normalizedValue));
        }

        public void SetGlobalContrast01(float normalizedValue)
        {
            SetGlobalContrast(Convert01ToContrast(normalizedValue));
        }

        public void SetLocalBrightness01(float normalizedValue)
        {
            SetLocalBrightness(Convert01ToBrightness(normalizedValue));
        }

        public void SetLocalContrast01(float normalizedValue)
        {
            SetLocalContrast(Convert01ToContrast(normalizedValue));
        }

        public void SetGlobalBrightness(float value)
        {
            globalBrightness = ClampBrightness(value);

            // Global overrules local immediately on this client too.
            localBrightness = globalBrightness;

            ApplyValues();
            RefreshLinkedInputs();
            ScheduleDelayedGlobalSync();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Global brightness changed locally, delayed sync scheduled.");
            }
        }

        public void SetGlobalContrast(float value)
        {
            globalContrast = ClampContrast(value);

            // Global overrules local immediately on this client too.
            localContrast = globalContrast;

            ApplyValues();
            RefreshLinkedInputs();
            ScheduleDelayedGlobalSync();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Global contrast changed locally, delayed sync scheduled.");
            }
        }

        public void SetLocalBrightness(float value)
        {
            localBrightness = ClampBrightness(value);

            ApplyValues();
            RefreshLinkedInputs();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Local brightness changed.");
            }
        }

        public void SetLocalContrast(float value)
        {
            localContrast = ClampContrast(value);

            ApplyValues();
            RefreshLinkedInputs();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Local contrast changed.");
            }
        }

        public void ResetGlobal()
        {
            globalBrightness = ClampBrightness(defaultBrightness);
            globalContrast = ClampContrast(defaultContrast);

            // Global reset overrules local values.
            localBrightness = globalBrightness;
            localContrast = globalContrast;

            ApplyValues();
            RefreshLinkedInputs();

            // Reset is a button press, so sync immediately.
            RequestSafeSerialization();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Global reset.");
            }
        }

        public void ResetLocal()
        {
            localBrightness = ClampBrightness(defaultBrightness);
            localContrast = ClampContrast(defaultContrast);

            ApplyValues();
            RefreshLinkedInputs();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Local reset.");
            }
        }

        public float GetGlobalBrightness()
        {
            return globalBrightness;
        }

        public float GetGlobalContrast()
        {
            return globalContrast;
        }

        public float GetLocalBrightness()
        {
            return localBrightness;
        }

        public float GetLocalContrast()
        {
            return localContrast;
        }

        public float GetBrightness01(bool useGlobal)
        {
            float value = useGlobal ? globalBrightness : localBrightness;
            return ConvertBrightnessTo01(value);
        }

        public float GetContrast01(bool useGlobal)
        {
            float value = useGlobal ? globalContrast : localContrast;
            return ConvertContrastTo01(value);
        }

        public void RefreshLinkedInputs()
        {
            if (linkedInputs == null)
            {
                return;
            }

            for (int i = 0; i < linkedInputs.Length; i++)
            {
                ScreenReadabilityInput input = linkedInputs[i];

                if (input == null)
                {
                    continue;
                }

                input.RefreshFromManager();
            }
        }

        private void ApplyValues()
        {
            if (targetMaterials == null)
            {
                return;
            }

            for (int i = 0; i < targetMaterials.Length; i++)
            {
                Material mat = targetMaterials[i];

                if (mat == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(brightnessProperty) && mat.HasProperty(brightnessProperty))
                {
                    mat.SetFloat(brightnessProperty, localBrightness);
                }

                if (!string.IsNullOrEmpty(contrastProperty) && mat.HasProperty(contrastProperty))
                {
                    mat.SetFloat(contrastProperty, localContrast);
                }
            }
        }

        private float ClampBrightness(float value)
        {
            return Mathf.Clamp(value, minBrightness, maxBrightness);
        }

        private float ClampContrast(float value)
        {
            return Mathf.Clamp(value, minContrast, maxContrast);
        }

        private float Convert01ToBrightness(float normalizedValue)
        {
            return Mathf.Lerp(minBrightness, maxBrightness, Mathf.Clamp01(normalizedValue));
        }

        private float Convert01ToContrast(float normalizedValue)
        {
            return Mathf.Lerp(minContrast, maxContrast, Mathf.Clamp01(normalizedValue));
        }

        private float ConvertBrightnessTo01(float value)
        {
            if (Mathf.Approximately(maxBrightness, minBrightness))
            {
                return 0f;
            }

            return Mathf.InverseLerp(minBrightness, maxBrightness, ClampBrightness(value));
        }

        private float ConvertContrastTo01(float value)
        {
            if (Mathf.Approximately(maxContrast, minContrast))
            {
                return 0f;
            }

            return Mathf.InverseLerp(minContrast, maxContrast, ClampContrast(value));
        }

        private void ScheduleDelayedGlobalSync()
        {
            lastGlobalChangeTime = Time.time;

            if (delayedSyncScheduled)
            {
                return;
            }

            delayedSyncScheduled = true;
            SendCustomEventDelayedSeconds(nameof(DelayedGlobalSyncCheck), syncDelay);
        }

        public void DelayedGlobalSyncCheck()
        {
            float timeSinceLastChange = Time.time - lastGlobalChangeTime;

            if (timeSinceLastChange < syncDelay)
            {
                SendCustomEventDelayedSeconds(nameof(DelayedGlobalSyncCheck), syncDelay);
                return;
            }

            delayedSyncScheduled = false;
            RequestSafeSerialization();

            if (debugLogs)
            {
                Debug.Log("[ScreenReadabilityManager] Delayed global sync requested.");
            }
        }

        private void RequestSafeSerialization()
        {
            if (Networking.LocalPlayer == null)
            {
                RequestSerialization();
                return;
            }

            if (Networking.IsOwner(gameObject))
            {
                RequestSerialization();
                return;
            }

            waitingForOwnership = true;
            ownershipRetryCount = 0;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomEventDelayedFrames(nameof(DelayedRequestSerialization), 1);
        }

        public void DelayedRequestSerialization()
        {
            if (!waitingForOwnership)
            {
                return;
            }

            if (Networking.LocalPlayer == null)
            {
                waitingForOwnership = false;
                RequestSerialization();
                return;
            }

            if (Networking.IsOwner(gameObject))
            {
                waitingForOwnership = false;
                RequestSerialization();

                if (debugLogs)
                {
                    Debug.Log("[ScreenReadabilityManager] Serialization requested after ownership transfer.");
                }

                return;
            }

            ownershipRetryCount++;

            if (ownershipRetryCount >= MaxOwnershipRetries)
            {
                waitingForOwnership = false;

                if (debugLogs)
                {
                    Debug.LogWarning("[ScreenReadabilityManager] Could not get ownership in time.");
                }

                return;
            }

            SendCustomEventDelayedFrames(nameof(DelayedRequestSerialization), 1);
        }
    }
}