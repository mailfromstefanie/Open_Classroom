using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace StefanieInVR.PaperTablet
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ProjectorScreenToggle : UdonSharpBehaviour
    {
        [Header("Screen Blendshape")]
        [Tooltip("The Skinned Mesh Renderer that has the projector screen blendshape.")]
        public SkinnedMeshRenderer targetRenderer;

        [Tooltip("Blendshape index. Usually 0 if this mesh has only one blendshape.")]
        public int blendShapeIndex = 0;

        [Header("Open / Closed Values")]
        [Range(0f, 100f)]
        public float closedWeight = 0f;

        [Range(0f, 100f)]
        public float openWeight = 100f;

        [Header("Movement")]
        public bool smooth = true;

        [Tooltip("How fast the screen moves. 50 is calm. 100 is faster.")]
        public float smoothSpeed = 50f;

        [Header("Start State")]
        public bool startOpen = true;

        [Header("Optional Button Visuals")]
        public Image buttonImage;
        public Button uiButton;

        [Header("Normal Sprites")]
        public Sprite normalClosed;
        public Sprite normalOpen;

        [Header("Highlighted Sprites")]
        public Sprite highlightedClosed;
        public Sprite highlightedOpen;

        [Header("Pressed Sprites")]
        public Sprite pressedClosed;
        public Sprite pressedOpen;

        [Header("Optional Owner Lock")]
        public bool lockButtonToOwner = false;

        [UdonSynced]
        private bool isOpen;

        private float currentWeight;
        private float targetWeight;
        private bool isAnimating;

        private bool pendingSerialization;
        private int serializationTries;
        private bool hasStarted;

        private void Start()
        {
            if (Networking.IsOwner(gameObject))
            {
                isOpen = startOpen;
                RequestSerialization();
            }

            hasStarted = true;
            ApplyStateImmediate();
            UpdateButtonVisual();
            UpdateButtonLock();
        }

        private void Update()
        {
            if (!isAnimating)
            {
                return;
            }

            if (targetRenderer == null)
            {
                isAnimating = false;
                return;
            }

            currentWeight = Mathf.MoveTowards(
                currentWeight,
                targetWeight,
                smoothSpeed * Time.deltaTime
            );

            targetRenderer.SetBlendShapeWeight(blendShapeIndex, currentWeight);

            if (Mathf.Approximately(currentWeight, targetWeight))
            {
                isAnimating = false;
            }
        }

        public void ToggleScreen()
        {
            SetScreenOpen(!isOpen);
        }

        public void OpenScreen()
        {
            SetScreenOpen(true);
        }

        public void CloseScreen()
        {
            SetScreenOpen(false);
        }

        public void SetScreenOpen(bool open)
        {
            TakeOwnershipIfNeeded();

            isOpen = open;

            ApplyTargetFromState();
            UpdateButtonVisual();
            UpdateButtonLock();

            RequestSerializationSafe();
        }

        public bool IsOpen()
        {
            return isOpen;
        }

        public override void OnDeserialization()
        {
            if (!hasStarted)
            {
                return;
            }

            ApplyTargetFromState();
            UpdateButtonVisual();
            UpdateButtonLock();
        }

        public override void Interact()
        {
            ToggleScreen();
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            UpdateButtonLock();

            if (pendingSerialization && Networking.IsOwner(gameObject))
            {
                SendCustomEventDelayedFrames(nameof(DelayedRequestSerialization), 1);
            }
        }

        private void ApplyStateImmediate()
        {
            targetWeight = isOpen ? openWeight : closedWeight;
            ApplyWeightImmediate(targetWeight);
        }

        private void ApplyTargetFromState()
        {
            targetWeight = isOpen ? openWeight : closedWeight;

            if (smooth)
            {
                if (targetRenderer != null)
                {
                    currentWeight = targetRenderer.GetBlendShapeWeight(blendShapeIndex);
                }

                isAnimating = true;
            }
            else
            {
                ApplyWeightImmediate(targetWeight);
            }
        }

        private void ApplyWeightImmediate(float weight)
        {
            if (targetRenderer == null)
            {
                return;
            }

            currentWeight = weight;
            targetWeight = weight;
            isAnimating = false;

            targetRenderer.SetBlendShapeWeight(blendShapeIndex, weight);
        }

        private void UpdateButtonVisual()
        {
            Sprite normalSprite = isOpen ? normalOpen : normalClosed;
            Sprite highlightedSprite = isOpen ? highlightedOpen : highlightedClosed;
            Sprite pressedSprite = isOpen ? pressedOpen : pressedClosed;

            if (highlightedSprite == null)
            {
                highlightedSprite = normalSprite;
            }

            if (pressedSprite == null)
            {
                pressedSprite = highlightedSprite;
            }

            if (buttonImage != null && normalSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }

            if (uiButton != null)
            {
                if (buttonImage != null && uiButton.targetGraphic != buttonImage)
                {
                    uiButton.targetGraphic = buttonImage;
                }

                SpriteState state = uiButton.spriteState;

                state.highlightedSprite = highlightedSprite;
                state.pressedSprite = pressedSprite;
                state.selectedSprite = null;
                state.disabledSprite = normalSprite;

                uiButton.spriteState = state;
            }
        }

        private void UpdateButtonLock()
        {
            if (!lockButtonToOwner)
            {
                return;
            }

            if (uiButton == null)
            {
                return;
            }

            uiButton.interactable = Networking.IsOwner(gameObject);
        }

        private void TakeOwnershipIfNeeded()
        {
            if (Networking.IsOwner(gameObject))
            {
                return;
            }

            if (Networking.LocalPlayer == null)
            {
                return;
            }

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
            if (!pendingSerialization)
            {
                return;
            }

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
    }
}