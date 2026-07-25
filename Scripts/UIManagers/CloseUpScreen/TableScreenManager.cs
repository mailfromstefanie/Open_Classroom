using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TableScreenManager : UdonSharpBehaviour
{
    [Header("Screen root")]
    public GameObject closeUpScreen;

    [Header("Scalable root")]
    public Transform scalableRoot;

    [Header("Slider")]
    public Slider scaleSlider;

    [Header("Spawnpoints")]
    public Transform[] tableSpawnPoints;

    [Header("Buttons")]
    public TableButton[] tableButtons;

    [Header("Sprites")]
    public Sprite spriteOff;
    public Sprite spriteOn;
    public Sprite spriteHighlight;

    [Header("Scale")]
    public float minScale = 0.2f;
    public float maxScale = 1.2f;
    public float defaultScale = 0.4f;

    [Header("VR Thumbstick")]
    public float thumbstickSpeed = 0.5f;
    public string thumbstickAxisName = "Oculus_CrossPlatform_SecondaryThumbstickVertical";

    private int activeTableIndex = -1;

    void Start()
    {
        if (closeUpScreen != null)
        {
            closeUpScreen.SetActive(false);
        }

        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = defaultScale;
        }

        ApplyScale(defaultScale);
        RefreshAllButtons();
    }

    public void OnSliderChanged()
    {
        if (scaleSlider == null) return;
        ApplyScale(scaleSlider.value);
    }

    public void OnTableButtonPressed(int tableIndex)
    {
        if (tableSpawnPoints == null) return;
        if (tableIndex < 0 || tableIndex >= tableSpawnPoints.Length) return;

        if (activeTableIndex == tableIndex)
        {
            DeactivateScreen();
            return;
        }

        ActivateTable(tableIndex);
    }

    private void ActivateTable(int tableIndex)
    {
        activeTableIndex = tableIndex;

        if (closeUpScreen != null && tableSpawnPoints[tableIndex] != null)
        {
            closeUpScreen.transform.position = tableSpawnPoints[tableIndex].position;
            closeUpScreen.transform.rotation = tableSpawnPoints[tableIndex].rotation;
            closeUpScreen.SetActive(true);
        }

        RefreshAllButtons();
    }

    private void DeactivateScreen()
    {
        activeTableIndex = -1;

        if (closeUpScreen != null)
        {
            closeUpScreen.SetActive(false);
        }

        RefreshAllButtons();
    }

    private void RefreshAllButtons()
    {
        if (tableButtons == null) return;

        for (int i = 0; i < tableButtons.Length; i++)
        {
            if (tableButtons[i] != null)
            {
                bool isActive = (tableButtons[i].tableIndex == activeTableIndex);
                tableButtons[i].UpdateVisual(isActive, spriteOff, spriteOn, spriteHighlight);
            }
        }
    }

    private void ApplyScale(float s)
    {
        if (scalableRoot == null) return;
        scalableRoot.localScale = new Vector3(s, s, s);
    }

    void Update()
    {
        if (activeTableIndex < 0) return;
        if (scaleSlider == null) return;
        if (!Networking.LocalPlayer.IsUserInVR()) return;

        float joystick = Input.GetAxis(thumbstickAxisName);

        if (Mathf.Abs(joystick) > 0.1f)
        {
            float newValue = Mathf.Clamp(
                scaleSlider.value + joystick * thumbstickSpeed * Time.deltaTime,
                minScale,
                maxScale
            );

            if (Mathf.Abs(newValue - scaleSlider.value) > 0.0001f)
            {
                scaleSlider.value = newValue;
                ApplyScale(newValue);
            }
        }
    }
}