using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HybridObjectToggleManager : UdonSharpBehaviour
    {
        [Header("Default Global States")]
        [Tooltip("Index 0 = Bird. True = starts ON, False = starts OFF.")]
        public bool[] defaultGlobalStates;

        [Header("Toggle Groups")]
        [Tooltip("One group per object toggle. Example: Bird group = index 0.")]
        public HybridObjectToggleGroup[] groups;

        [Header("Debug")]
        public bool debugLogs = false;

        [UdonSynced]
        private int globalStateBits = 0;

        [UdonSynced]
        private int globalRevision = 0;

        [UdonSynced]
        private bool hasInitializedNetworkState = false;

        private int localOverrideBits = 0;
        private int localValueBits = 0;
        private int lastAppliedGlobalRevision = -1;

        private bool hasStarted = false;

        private void Start()
        {
            hasStarted = true;

            if (!hasInitializedNetworkState)
            {
                globalStateBits = BuildDefaultBits();

                if (Networking.LocalPlayer != null && Networking.IsOwner(gameObject))
                {
                    hasInitializedNetworkState = true;
                    RequestSerialization();

                    if (debugLogs)
                    {
                        Debug.Log("[HybridObjectToggleManager] Initialized default global states.");
                    }
                }
            }

            lastAppliedGlobalRevision = globalRevision;

            SendCustomEventDelayedFrames(nameof(RefreshAll), 1);
        }

        public override void OnDeserialization()
        {
            ApplyGlobalRevisionIfNeeded();
            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleManager] Deserialized. Revision: " + globalRevision);
            }
        }

        public void ToggleGlobal(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                Debug.LogWarning("[HybridObjectToggleManager] ToggleGlobal failed. Invalid index: " + toggleIndex);
                return;
            }

            TakeOwnershipIfNeeded();

            bool currentState = GetGlobalState(toggleIndex);
            SetGlobalStateInternal(toggleIndex, !currentState);
        }

        public void SetGlobalOn(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                Debug.LogWarning("[HybridObjectToggleManager] SetGlobalOn failed. Invalid index: " + toggleIndex);
                return;
            }

            TakeOwnershipIfNeeded();
            SetGlobalStateInternal(toggleIndex, true);
        }

        public void SetGlobalOff(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                Debug.LogWarning("[HybridObjectToggleManager] SetGlobalOff failed. Invalid index: " + toggleIndex);
                return;
            }

            TakeOwnershipIfNeeded();
            SetGlobalStateInternal(toggleIndex, false);
        }

        public void ToggleLocal(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                Debug.LogWarning("[HybridObjectToggleManager] ToggleLocal failed. Invalid index: " + toggleIndex);
                return;
            }

            ApplyGlobalRevisionIfNeeded();

            bool currentEffectiveState = GetEffectiveState(toggleIndex);
            bool newLocalState = !currentEffectiveState;

            localOverrideBits = SetBit(localOverrideBits, toggleIndex, true);
            localValueBits = SetBit(localValueBits, toggleIndex, newLocalState);

            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleManager] Local toggle changed. Index: " + toggleIndex + " State: " + newLocalState);
            }
        }

        public void ClearLocalOverride(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                Debug.LogWarning("[HybridObjectToggleManager] ClearLocalOverride failed. Invalid index: " + toggleIndex);
                return;
            }

            localOverrideBits = SetBit(localOverrideBits, toggleIndex, false);
            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleManager] Local override cleared. Index: " + toggleIndex);
            }
        }

        public void ClearAllLocalOverrides()
        {
            localOverrideBits = 0;
            localValueBits = 0;

            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleManager] All local overrides cleared.");
            }
        }

        public bool GetGlobalState(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                return false;
            }

            return GetBit(globalStateBits, toggleIndex);
        }

        public bool GetEffectiveState(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                return false;
            }

            ApplyGlobalRevisionIfNeeded();

            bool hasLocalOverride = GetBit(localOverrideBits, toggleIndex);

            if (hasLocalOverride)
            {
                return GetBit(localValueBits, toggleIndex);
            }

            return GetGlobalState(toggleIndex);
        }

        public bool HasLocalOverride(int toggleIndex)
        {
            if (!IsValidIndex(toggleIndex))
            {
                return false;
            }

            ApplyGlobalRevisionIfNeeded();
            return GetBit(localOverrideBits, toggleIndex);
        }

        public int GetGlobalRevision()
        {
            return globalRevision;
        }

        public void RefreshAll()
        {
            ApplyGlobalRevisionIfNeeded();

            if (groups == null)
            {
                return;
            }

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] != null)
                {
                    groups[i].RefreshFromManager();
                }
            }
        }

        private void SetGlobalStateInternal(int toggleIndex, bool newState)
        {
            globalStateBits = SetBit(globalStateBits, toggleIndex, newState);

            globalRevision++;

            hasInitializedNetworkState = true;

            localOverrideBits = 0;
            localValueBits = 0;
            lastAppliedGlobalRevision = globalRevision;

            RequestSerialization();
            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleManager] Global state changed. Index: " + toggleIndex + " State: " + newState + " Revision: " + globalRevision);
            }
        }

        private void ApplyGlobalRevisionIfNeeded()
        {
            if (lastAppliedGlobalRevision != globalRevision)
            {
                localOverrideBits = 0;
                localValueBits = 0;
                lastAppliedGlobalRevision = globalRevision;

                if (debugLogs && hasStarted)
                {
                    Debug.Log("[HybridObjectToggleManager] Global revision changed. Local overrides cleared.");
                }
            }
        }

        private void TakeOwnershipIfNeeded()
        {
            if (Networking.LocalPlayer == null)
            {
                return;
            }

            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
        }

        private int BuildDefaultBits()
        {
            int bits = 0;

            if (defaultGlobalStates == null)
            {
                return bits;
            }

            int max = defaultGlobalStates.Length;

            if (max > 31)
            {
                max = 31;
            }

            for (int i = 0; i < max; i++)
            {
                bits = SetBit(bits, i, defaultGlobalStates[i]);
            }

            return bits;
        }

        private bool IsValidIndex(int toggleIndex)
        {
            if (toggleIndex < 0)
            {
                return false;
            }

            if (toggleIndex > 30)
            {
                return false;
            }

            return true;
        }

        private bool GetBit(int bits, int index)
        {
            int mask = 1 << index;
            return (bits & mask) != 0;
        }

        private int SetBit(int bits, int index, bool value)
        {
            int mask = 1 << index;

            if (value)
            {
                return bits | mask;
            }

            return bits & ~mask;
        }
    }
}