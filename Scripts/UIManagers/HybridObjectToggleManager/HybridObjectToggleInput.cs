using UdonSharp;
using UnityEngine;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HybridObjectToggleInput : UdonSharpBehaviour
    {
        [Header("Required")]
        public HybridObjectToggleManager manager;

        [Tooltip("Index of the object group. Bird = 0.")]
        public int toggleIndex = 0;

        [Header("Button Type")]
        [Tooltip("ON = this button changes the synced global state. OFF = this button changes only the local player view.")]
        public bool isGlobalButton = false;

        [Header("Optional Fixed Action")]
        [Tooltip("If true, this button always sets ON instead of toggling.")]
        public bool forceOn = false;

        [Tooltip("If true, this button always sets OFF instead of toggling.")]
        public bool forceOff = false;

        [Header("Local Only Option")]
        [Tooltip("Only for local buttons. If true, this clears the local override and returns to the global state.")]
        public bool clearLocalOverrideInstead = false;

        [Header("Debug")]
        public bool debugLogs = false;

        public override void Interact()
        {
            Press();
        }

        public void Press()
        {
            if (manager == null)
            {
                Debug.LogWarning("[HybridObjectToggleInput] No manager assigned.");
                return;
            }

            if (toggleIndex < 0 || toggleIndex > 30)
            {
                Debug.LogWarning("[HybridObjectToggleInput] Invalid toggleIndex: " + toggleIndex);
                return;
            }

            if (isGlobalButton)
            {
                PressGlobal();
            }
            else
            {
                PressLocal();
            }
        }

        public void PressGlobal()
        {
            if (manager == null)
            {
                Debug.LogWarning("[HybridObjectToggleInput] PressGlobal failed. No manager assigned.");
                return;
            }

            if (forceOn)
            {
                manager.SetGlobalOn(toggleIndex);
            }
            else if (forceOff)
            {
                manager.SetGlobalOff(toggleIndex);
            }
            else
            {
                manager.ToggleGlobal(toggleIndex);
            }

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleInput] Global button pressed. Index: " + toggleIndex);
            }
        }

        public void PressLocal()
        {
            if (manager == null)
            {
                Debug.LogWarning("[HybridObjectToggleInput] PressLocal failed. No manager assigned.");
                return;
            }

            if (clearLocalOverrideInstead)
            {
                manager.ClearLocalOverride(toggleIndex);
            }
            else
            {
                manager.ToggleLocal(toggleIndex);
            }

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleInput] Local button pressed. Index: " + toggleIndex);
            }
        }
    }
}