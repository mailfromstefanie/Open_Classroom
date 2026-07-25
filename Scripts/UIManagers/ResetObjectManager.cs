using UdonSharp;
using UnityEngine;

namespace StefanieInVR.PaperTablet 
{

public class ResetObjectManager : UdonSharpBehaviour
{
    [Header("Objects To Reset")]
    [Tooltip("Drag all objects here that have a ResettableObject script.")]
    public ResettableObject[] resettableObjects;

    public void ResetEverything()
    {
        if (resettableObjects == null)
        {
            return;
        }

        for (int i = 0; i < resettableObjects.Length; i++)
        {
            ResettableObject resetObject = resettableObjects[i];

            if (resetObject != null)
            {
                resetObject.ResetObject();
            }
        }
    }

    public void ResetGroup0()
    {
        ResetGroup(0);
    }

    public void ResetGroup1()
    {
        ResetGroup(1);
    }

    public void ResetGroup2()
    {
        ResetGroup(2);
    }

    public void ResetGroup3()
    {
        ResetGroup(3);
    }

    public void ResetGroup4()
    {
        ResetGroup(4);
    }

    public void ResetGroup5()
    {
        ResetGroup(5);
    }

    public void ResetGroup(int groupNumber)
    {
        if (resettableObjects == null)
        {
            return;
        }

        for (int i = 0; i < resettableObjects.Length; i++)
        {
            ResettableObject resetObject = resettableObjects[i];

            if (resetObject != null && resetObject.resetGroup == groupNumber)
            {
                resetObject.ResetObject();
            }
        }
    }
}
}