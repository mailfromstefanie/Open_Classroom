using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace StefanieInVR
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ScreenReadabilityInput : UdonSharpBehaviour
    {
        [Header("Manager")]
        public ScreenReadabilityManager manager;

        [Header("Input Type")]
        [Tooltip("Use this object as a slider input.")]
        public bool isSlider = true;

        [Tooltip("Use this object as a reset button input.")]
        public bool isResetButton = false;

        [Header("Control")]
        [Tooltip("0 = Brightness, 1 = Contrast")]
        public int controlIndex = 0;

        [Tooltip("If true, this input controls the global synced value. If false, it controls only the local player value.")]
        public bool global = false;

        [Header("UI References")]
        [Tooltip("Optional. If empty, the script tries to find a Slider on this GameObject.")]
        public Slider slider;

        [Header("Safety")]
        [Tooltip("Prevents RefreshFromManager from triggering slider logic again.")]
        public bool ignoreRefreshCallbacks = true;

        private bool isRefreshing;

        private void Start()
        {
            if (slider == null)
            {
                slider = (Slider)GetComponent(typeof(Slider));
            }

            RefreshFromManager();
        }

        public void OnSliderChanged()
        {
            if (!isSlider)
            {
                return;
            }

            if (isRefreshing && ignoreRefreshCallbacks)
            {
                return;
            }

            if (manager == null || slider == null)
            {
                return;
            }

            float value01 = slider.value;

            if (controlIndex == 0)
            {
                if (global)
                {
                    manager.SetGlobalBrightness01(value01);
                }
                else
                {
                    manager.SetLocalBrightness01(value01);
                }

                return;
            }

            if (controlIndex == 1)
            {
                if (global)
                {
                    manager.SetGlobalContrast01(value01);
                }
                else
                {
                    manager.SetLocalContrast01(value01);
                }

                return;
            }
        }

        public void OnResetPressed()
        {
            if (!isResetButton)
            {
                return;
            }

            if (manager == null)
            {
                return;
            }

            if (global)
            {
                manager.ResetGlobal();
            }
            else
            {
                manager.ResetLocal();
            }
        }

        public void RefreshFromManager()
        {
            if (manager == null)
            {
                return;
            }

            if (!isSlider)
            {
                return;
            }

            if (slider == null)
            {
                slider = (Slider)GetComponent(typeof(Slider));
            }

            if (slider == null)
            {
                return;
            }

            isRefreshing = true;

            if (controlIndex == 0)
            {
                slider.value = manager.GetBrightness01(global);
            }
            else if (controlIndex == 1)
            {
                slider.value = manager.GetContrast01(global);
            }

            isRefreshing = false;
        }
    }
}