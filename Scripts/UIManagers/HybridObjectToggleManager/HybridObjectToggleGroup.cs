using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HybridObjectToggleGroup : UdonSharpBehaviour
    {
        [Header("Required")]
        public HybridObjectToggleManager manager;

        [Tooltip("Index of this object group. Bird = 0.")]
        public int toggleIndex = 0;

        [Header("Controlled Objects")]
        [Tooltip("Objects that are active when the final/effective state is ON.")]
        public GameObject[] objectsOnWhenEnabled;

        [Tooltip("Objects that are active when the final/effective state is OFF. Usually leave empty.")]
        public GameObject[] objectsOnWhenDisabled;

        [Header("Global Toggle Sprite")]
        [Tooltip("Image on the GLOBAL button that should swap between ON and OFF sprite.")]
        public Image globalToggleImage;

        [Header("Local Toggle Sprite")]
        [Tooltip("Image on the LOCAL button that should swap between ON and OFF sprite.")]
        public Image localToggleImage;

        [Header("Sprites")]
        [Tooltip("Sprite shown when the state is ON.")]
        public Sprite onSprite;

        [Tooltip("Sprite shown when the state is OFF.")]
        public Sprite offSprite;

        [Header("Optional Local Override Indicator")]
        [Tooltip("Optional object that becomes active when this player has a local override.")]
        public GameObject localOverrideIndicator;

        [Header("Options")]
        public bool refreshOnStart = true;

        [Header("Debug")]
        public bool debugLogs = false;

        private void Start()
        {
            if (refreshOnStart)
            {
                SendCustomEventDelayedFrames(nameof(RefreshFromManager), 1);
            }
        }

        public void RefreshFromManager()
        {
            if (manager == null)
            {
                Debug.LogWarning("[HybridObjectToggleGroup] No manager assigned.");
                return;
            }

            bool globalState = manager.GetGlobalState(toggleIndex);
            bool effectiveState = manager.GetEffectiveState(toggleIndex);
            bool hasLocalOverride = manager.HasLocalOverride(toggleIndex);

            ApplyControlledObjects(effectiveState);
            ApplyGlobalSprite(globalState);
            ApplyLocalSprite(effectiveState);
            ApplyLocalOverrideIndicator(hasLocalOverride);

            if (debugLogs)
            {
                Debug.Log("[HybridObjectToggleGroup] Refreshed index: " + toggleIndex + " Global: " + globalState + " Effective: " + effectiveState + " Local override: " + hasLocalOverride);
            }
        }

        private void ApplyControlledObjects(bool effectiveState)
        {
            SetGameObjects(objectsOnWhenEnabled, effectiveState);
            SetGameObjects(objectsOnWhenDisabled, !effectiveState);
        }

        private void ApplyGlobalSprite(bool globalState)
        {
            if (globalToggleImage == null)
            {
                return;
            }

            Sprite targetSprite = globalState ? onSprite : offSprite;

            if (targetSprite == null)
            {
                return;
            }

            if (globalToggleImage.sprite != targetSprite)
            {
                globalToggleImage.sprite = targetSprite;
            }
        }

        private void ApplyLocalSprite(bool effectiveState)
        {
            if (localToggleImage == null)
            {
                return;
            }

            Sprite targetSprite = effectiveState ? onSprite : offSprite;

            if (targetSprite == null)
            {
                return;
            }

            if (localToggleImage.sprite != targetSprite)
            {
                localToggleImage.sprite = targetSprite;
            }
        }

        private void ApplyLocalOverrideIndicator(bool hasLocalOverride)
        {
            if (localOverrideIndicator == null)
            {
                return;
            }

            if (localOverrideIndicator.activeSelf != hasLocalOverride)
            {
                localOverrideIndicator.SetActive(hasLocalOverride);
            }
        }

        private void SetGameObjects(GameObject[] objects, bool state)
        {
            if (objects == null)
            {
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    if (objects[i].activeSelf != state)
                    {
                        objects[i].SetActive(state);
                    }
                }
            }
        }
    }
}