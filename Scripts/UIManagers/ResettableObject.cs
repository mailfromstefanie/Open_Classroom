using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

namespace StefanieInVR.PaperTablet
{
public class ResettableObject : UdonSharpBehaviour
{
    [Header("Basic Setup")]
    [Tooltip("The empty GameObject that marks where this object should return to.")]
    public Transform homeSpot;

    [Tooltip("Group number. Buttons reset objects by group number.")]
    public int resetGroup = 0;

    [Tooltip("OFF = local reset only. ON = reset should be visible for everyone. Use this with VRC Object Sync.")]
    public bool resetForEveryone = false;

    [Header("What Should Reset?")]
    [Tooltip("Reset the world position to the Home Spot.")]
    public bool resetPosition = true;

    [Tooltip("Reset the world rotation to the Home Spot.")]
    public bool resetRotation = true;

    [Tooltip("Reset the local scale to the Home Spot local scale. Usually leave this OFF.")]
    public bool resetScale = false;

    [Header("Pickup / Sync Options")]
    [Tooltip("If this object is a VRC Pickup, drop it before resetting.")]
    public bool dropPickupBeforeReset = true;

    [Tooltip("Optional. Fill this if VRC Pickup is not on the same GameObject as this script.")]
    public VRC_Pickup pickupObject;

    [Tooltip("Optional. Fill this if VRC Object Sync is not on the same GameObject as this script.")]
    public VRCObjectSync objectSync;

    private void Start()
    {
        AutoFindMissingReferences();
    }

    private void AutoFindMissingReferences()
    {
        if (pickupObject == null)
        {
            pickupObject = (VRC_Pickup)GetComponent(typeof(VRC_Pickup));
        }

        if (objectSync == null)
        {
            objectSync = (VRCObjectSync)GetComponent(typeof(VRCObjectSync));
        }
    }

    public void ResetObject()
    {
        if (homeSpot == null)
        {
            Debug.LogWarning("[ResettableObject] No Home Spot assigned on: " + gameObject.name);
            return;
        }

        AutoFindMissingReferences();

        if (resetForEveryone)
        {
            ResetForEveryone();
        }
        else
        {
            ResetLocalOnly();
        }
    }

    private void ResetLocalOnly()
    {
        DropPickupIfNeeded();
        MoveToHomeSpot();
    }

    private void ResetForEveryone()
    {
        if (Networking.LocalPlayer != null)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (pickupObject != null)
            {
                Networking.SetOwner(Networking.LocalPlayer, pickupObject.gameObject);
            }

            if (objectSync != null)
            {
                Networking.SetOwner(Networking.LocalPlayer, objectSync.gameObject);
                objectSync.FlagDiscontinuity();
            }
        }

        DropPickupIfNeeded();
        MoveToHomeSpot();

        if (objectSync != null)
        {
            objectSync.FlagDiscontinuity();
        }
    }

    private void DropPickupIfNeeded()
    {
        if (!dropPickupBeforeReset)
        {
            return;
        }

        if (pickupObject == null)
        {
            return;
        }

        pickupObject.Drop();
    }

    private void MoveToHomeSpot()
    {
        if (resetPosition)
        {
            transform.position = homeSpot.position;
        }

        if (resetRotation)
        {
            transform.rotation = homeSpot.rotation;
        }

        if (resetScale)
        {
            transform.localScale = homeSpot.localScale;
        }
    }
}
}