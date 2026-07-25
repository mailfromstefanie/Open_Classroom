using UdonSharp;
using UnityEngine;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class GlobalMaterialSwitchInput : UdonSharpBehaviour
    {
        [Header("Required")]
        public GlobalMaterialSwitchManager manager;

        [Tooltip("Which material switch group this button controls. First switch = 0.")]
        public int groupIndex = 0;

        [Tooltip("How many material options this group has. Example: 2 for normal/special.")]
        public int materialCount = 2;

        [Header("Button Action")]
        [Tooltip("If true, this button goes to the next material index.")]
        public bool nextMaterial = true;

        [Tooltip("If true, this button goes to the previous material index.")]
        public bool previousMaterial = false;

        [Tooltip("If true, this button sets a fixed material index.")]
        public bool setFixedIndex = false;

        [Tooltip("Only used when Set Fixed Index is true.")]
        public int fixedMaterialIndex = 0;

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
                Debug.LogWarning("[GlobalMaterialSwitchInput] No manager assigned.");
                return;
            }

            if (materialCount <= 0)
            {
                Debug.LogWarning("[GlobalMaterialSwitchInput] Material count must be higher than zero.");
                return;
            }

            if (setFixedIndex)
            {
                manager.SetMaterialIndex(groupIndex, fixedMaterialIndex);
            }
            else if (previousMaterial)
            {
                manager.PreviousMaterial(groupIndex, materialCount);
            }
            else
            {
                manager.NextMaterial(groupIndex, materialCount);
            }

            if (debugLogs)
            {
                Debug.Log("[GlobalMaterialSwitchInput] Button pressed. Group: " + groupIndex);
            }
        }
    }
}