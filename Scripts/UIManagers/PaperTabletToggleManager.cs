using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace StefanieInVR.PaperTablet
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PaperTabletToggleManager : UdonSharpBehaviour
    {
        [Header("Alle visuals die moeten verversen")]
        public PaperTabletToggleVisual[] visuals;

        [Header("Startstatus per toggle - max 32")]
        public bool[] startStates;

        [Header("Sync per toggle - true = global, false = local")]
        [Tooltip("True = gesynct voor iedereen. False = alleen lokaal voor deze speler. Als deze lijst te kort is, wordt die toggle automatisch als global behandeld.")]
        public bool[] syncToggles;

        [UdonSynced] private int stateBits = 0;

        private int localStateBits = 0;
        private bool hasStarted = false;

        private bool pendingSerialization = false;
        private int serializationTries = 0;

        private void Start()
        {
            InitializeLocalStates();

            if (Networking.IsOwner(gameObject))
            {
                InitializeGlobalStates();
                RequestSerialization();
            }

            hasStarted = true;
            RefreshAllVisuals();
        }

        public override void OnDeserialization()
        {
            if (!hasStarted) return;

            RefreshAllVisuals();
        }

        private void InitializeGlobalStates()
        {
            stateBits = 0;

            if (startStates == null) return;

            int count = startStates.Length;

            if (count > 32)
            {
                count = 32;
            }

            for (int i = 0; i < count; i++)
            {
                if (!IsToggleGlobal(i)) continue;

                if (startStates[i])
                {
                    stateBits |= (1 << i);
                }
            }
        }

        private void InitializeLocalStates()
        {
            localStateBits = 0;

            if (startStates == null) return;

            int count = startStates.Length;

            if (count > 32)
            {
                count = 32;
            }

            for (int i = 0; i < count; i++)
            {
                if (IsToggleGlobal(i)) continue;

                if (startStates[i])
                {
                    localStateBits |= (1 << i);
                }
            }
        }

        public bool GetToggleState(int toggleIndex)
        {
            if (toggleIndex < 0 || toggleIndex > 31)
            {
                return false;
            }

            if (IsToggleGlobal(toggleIndex))
            {
                return (stateBits & (1 << toggleIndex)) != 0;
            }

            return (localStateBits & (1 << toggleIndex)) != 0;
        }

        public bool IsToggleGlobal(int toggleIndex)
        {
            if (toggleIndex < 0 || toggleIndex > 31)
            {
                return true;
            }

            if (syncToggles == null)
            {
                return true;
            }

            if (toggleIndex >= syncToggles.Length)
            {
                return true;
            }

            return syncToggles[toggleIndex];
        }

        public void ToggleFromButton(int toggleIndex)
        {
            if (toggleIndex < 0 || toggleIndex > 31)
            {
                return;
            }

            if (IsToggleGlobal(toggleIndex))
            {
                ToggleGlobal(toggleIndex);
            }
            else
            {
                ToggleLocal(toggleIndex);
            }
        }

        private void ToggleGlobal(int toggleIndex)
        {
            TakeOwnershipIfNeeded();

            if (!Networking.IsOwner(gameObject)) return;

            stateBits ^= (1 << toggleIndex);

            RefreshAllVisuals();
            RequestSerializationSafe();
        }

        private void ToggleLocal(int toggleIndex)
        {
            localStateBits ^= (1 << toggleIndex);

            RefreshAllVisuals();
        }

        public void SetToggleState(int toggleIndex, bool state)
        {
            if (toggleIndex < 0 || toggleIndex > 31)
            {
                return;
            }

            if (IsToggleGlobal(toggleIndex))
            {
                SetGlobalToggleState(toggleIndex, state);
            }
            else
            {
                SetLocalToggleState(toggleIndex, state);
            }
        }

        private void SetGlobalToggleState(int toggleIndex, bool state)
        {
            TakeOwnershipIfNeeded();

            if (!Networking.IsOwner(gameObject)) return;

            bool oldState = GetToggleState(toggleIndex);

            if (oldState == state)
            {
                RefreshAllVisuals();
                return;
            }

            if (state)
            {
                stateBits |= (1 << toggleIndex);
            }
            else
            {
                stateBits &= ~(1 << toggleIndex);
            }

            RefreshAllVisuals();
            RequestSerializationSafe();
        }

        private void SetLocalToggleState(int toggleIndex, bool state)
        {
            bool oldState = GetToggleState(toggleIndex);

            if (oldState == state)
            {
                RefreshAllVisuals();
                return;
            }

            if (state)
            {
                localStateBits |= (1 << toggleIndex);
            }
            else
            {
                localStateBits &= ~(1 << toggleIndex);
            }

            RefreshAllVisuals();
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

        public void RefreshAllVisuals()
        {
            if (visuals == null) return;

            for (int i = 0; i < visuals.Length; i++)
            {
                if (visuals[i] != null)
                {
                    visuals[i].RefreshVisual();
                }
            }
        }
    }
}