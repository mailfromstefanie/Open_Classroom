using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class GlobalMaterialSwitchTarget : UdonSharpBehaviour
    {
        [Header("Required")]
        public GlobalMaterialSwitchManager manager;

        [Tooltip("Which material switch group this target listens to. First switch = 0.")]
        public int groupIndex = 0;

        [Header("Renderer Material Switch")]
        [Tooltip("Renderer that should receive the selected material.")]
        public Renderer targetRenderer;

        [Tooltip("Material slot on the renderer. Usually 0.")]
        public int materialSlot = 0;

        [Tooltip("Materials to choose from. Index 0 = default, Index 1 = second material, etc.")]
        public Material[] materials;

        [Header("Button / UI Sprite Switch")]
        [Tooltip("Optional UI Image that should change sprite when the material index changes.")]
        public Image spriteImage;

        [Tooltip("Sprites to choose from. Index 0 belongs to material index 0, index 1 belongs to material index 1, etc.")]
        public Sprite[] sprites;

        [Header("Extra Objects ON At Selected Index")]
        [Tooltip("These objects turn ON only when the current material index equals Extra Objects ON Index.")]
        public GameObject[] extraObjectsOnAtIndex;

        [Tooltip("Material index where the extra objects should be ON. Example: 1.")]
        public int extraObjectsOnIndex = 1;

        [Header("Extra Objects OFF At Selected Index")]
        [Tooltip("These objects turn OFF only when the current material index equals Extra Objects OFF Index.")]
        public GameObject[] extraObjectsOffAtIndex;

        [Tooltip("Material index where the extra objects should be OFF. Example: 1.")]
        public int extraObjectsOffIndex = 1;

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
                Debug.LogWarning("[GlobalMaterialSwitchTarget] No manager assigned.");
                return;
            }

            int index = manager.GetMaterialIndex(groupIndex);

            ApplyMaterial(index);
            ApplySprite(index);
            ApplyExtraObjects(index);

            if (debugLogs)
            {
                Debug.Log("[GlobalMaterialSwitchTarget] Refreshed. Group: " + groupIndex + " Index: " + index);
            }
        }

        private void ApplyMaterial(int index)
        {
            if (targetRenderer == null)
            {
                return;
            }

            if (materials == null)
            {
                return;
            }

            if (materials.Length == 0)
            {
                return;
            }

            if (index < 0)
            {
                index = 0;
            }

            if (index >= materials.Length)
            {
                index = materials.Length - 1;
            }

            Material selectedMaterial = materials[index];

            if (selectedMaterial == null)
            {
                return;
            }

            Material[] rendererMaterials = targetRenderer.sharedMaterials;

            if (rendererMaterials == null)
            {
                return;
            }

            if (materialSlot < 0)
            {
                materialSlot = 0;
            }

            if (materialSlot >= rendererMaterials.Length)
            {
                Debug.LogWarning("[GlobalMaterialSwitchTarget] Material slot is outside renderer material array.");
                return;
            }

            if (rendererMaterials[materialSlot] == selectedMaterial)
            {
                return;
            }

            rendererMaterials[materialSlot] = selectedMaterial;
            targetRenderer.sharedMaterials = rendererMaterials;
        }

        private void ApplySprite(int index)
        {
            if (spriteImage == null)
            {
                return;
            }

            if (sprites == null)
            {
                return;
            }

            if (sprites.Length == 0)
            {
                return;
            }

            if (index < 0)
            {
                index = 0;
            }

            if (index >= sprites.Length)
            {
                index = sprites.Length - 1;
            }

            Sprite selectedSprite = sprites[index];

            if (selectedSprite == null)
            {
                return;
            }

            if (spriteImage.sprite != selectedSprite)
            {
                spriteImage.sprite = selectedSprite;
            }
        }

        private void ApplyExtraObjects(int index)
        {
            bool extraOnState = index == extraObjectsOnIndex;
            SetGameObjects(extraObjectsOnAtIndex, extraOnState);

            bool extraOffState = index != extraObjectsOffIndex;
            SetGameObjects(extraObjectsOffAtIndex, extraOffState);
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