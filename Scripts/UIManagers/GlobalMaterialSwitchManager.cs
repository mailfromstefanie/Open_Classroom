using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GlobalMaterialSwitchManager : UdonSharpBehaviour
    {
        [Header("Default Indexes")]
        [Tooltip("One default material index per switch group. Group 0 = first material switch.")]
        public int[] defaultIndexes;

        [Header("Targets")]
        [Tooltip("All material switch targets that should refresh when a global material index changes.")]
        public GlobalMaterialSwitchTarget[] targets;

        [Header("Debug")]
        public bool debugLogs = false;

        [UdonSynced]
        private int packedIndexes = 0;

        [UdonSynced]
        private bool hasInitializedNetworkState = false;

        private const int BitsPerGroup = 4;
        private const int MaxIndexValue = 15;
        private const int MaxGroups = 7;

        private void Start()
        {
            if (!hasInitializedNetworkState)
            {
                packedIndexes = BuildDefaultPackedIndexes();

                if (Networking.LocalPlayer != null && Networking.IsOwner(gameObject))
                {
                    hasInitializedNetworkState = true;
                    RequestSerialization();

                    if (debugLogs)
                    {
                        Debug.Log("[GlobalMaterialSwitchManager] Initialized default indexes.");
                    }
                }
            }

            SendCustomEventDelayedFrames(nameof(RefreshAll), 1);
        }

        public override void OnDeserialization()
        {
            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[GlobalMaterialSwitchManager] Deserialized and refreshed.");
            }
        }

        public void NextMaterial(int groupIndex, int materialCount)
        {
            if (!IsValidGroupIndex(groupIndex))
            {
                Debug.LogWarning("[GlobalMaterialSwitchManager] NextMaterial failed. Invalid group index: " + groupIndex);
                return;
            }

            if (materialCount <= 0)
            {
                Debug.LogWarning("[GlobalMaterialSwitchManager] NextMaterial failed. Material count is zero.");
                return;
            }

            if (materialCount > MaxIndexValue + 1)
            {
                materialCount = MaxIndexValue + 1;
            }

            int currentIndex = GetMaterialIndex(groupIndex);
            int nextIndex = currentIndex + 1;

            if (nextIndex >= materialCount)
            {
                nextIndex = 0;
            }

            SetMaterialIndex(groupIndex, nextIndex);
        }

        public void PreviousMaterial(int groupIndex, int materialCount)
        {
            if (!IsValidGroupIndex(groupIndex))
            {
                Debug.LogWarning("[GlobalMaterialSwitchManager] PreviousMaterial failed. Invalid group index: " + groupIndex);
                return;
            }

            if (materialCount <= 0)
            {
                Debug.LogWarning("[GlobalMaterialSwitchManager] PreviousMaterial failed. Material count is zero.");
                return;
            }

            if (materialCount > MaxIndexValue + 1)
            {
                materialCount = MaxIndexValue + 1;
            }

            int currentIndex = GetMaterialIndex(groupIndex);
            int previousIndex = currentIndex - 1;

            if (previousIndex < 0)
            {
                previousIndex = materialCount - 1;
            }

            SetMaterialIndex(groupIndex, previousIndex);
        }

        public void SetMaterialIndex(int groupIndex, int materialIndex)
        {
            if (!IsValidGroupIndex(groupIndex))
            {
                Debug.LogWarning("[GlobalMaterialSwitchManager] SetMaterialIndex failed. Invalid group index: " + groupIndex);
                return;
            }

            if (materialIndex < 0)
            {
                materialIndex = 0;
            }

            if (materialIndex > MaxIndexValue)
            {
                materialIndex = MaxIndexValue;
            }

            TakeOwnershipIfNeeded();

            packedIndexes = SetPackedIndex(packedIndexes, groupIndex, materialIndex);

            hasInitializedNetworkState = true;

            RequestSerialization();
            RefreshAll();

            if (debugLogs)
            {
                Debug.Log("[GlobalMaterialSwitchManager] Set material group " + groupIndex + " to index " + materialIndex);
            }
        }

        public int GetMaterialIndex(int groupIndex)
        {
            if (!IsValidGroupIndex(groupIndex))
            {
                return 0;
            }

            return GetPackedIndex(packedIndexes, groupIndex);
        }

        public void RefreshAll()
        {
            if (targets == null)
            {
                return;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    targets[i].RefreshFromManager();
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

        private int BuildDefaultPackedIndexes()
        {
            int result = 0;

            if (defaultIndexes == null)
            {
                return result;
            }

            int max = defaultIndexes.Length;

            if (max > MaxGroups)
            {
                max = MaxGroups;
            }

            for (int i = 0; i < max; i++)
            {
                int value = defaultIndexes[i];

                if (value < 0)
                {
                    value = 0;
                }

                if (value > MaxIndexValue)
                {
                    value = MaxIndexValue;
                }

                result = SetPackedIndex(result, i, value);
            }

            return result;
        }

        private bool IsValidGroupIndex(int groupIndex)
        {
            if (groupIndex < 0)
            {
                return false;
            }

            if (groupIndex >= MaxGroups)
            {
                return false;
            }

            return true;
        }

        private int GetPackedIndex(int packedValue, int groupIndex)
        {
            int shift = groupIndex * BitsPerGroup;
            int mask = MaxIndexValue << shift;
            return (packedValue & mask) >> shift;
        }

        private int SetPackedIndex(int packedValue, int groupIndex, int materialIndex)
        {
            int shift = groupIndex * BitsPerGroup;
            int clearMask = ~(MaxIndexValue << shift);
            int cleanedValue = packedValue & clearMask;
            int newBits = materialIndex << shift;

            return cleanedValue | newBits;
        }
    }
}