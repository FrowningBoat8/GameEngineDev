using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMobileControls : MonoBehaviour
{
    [SerializeField] GameObject[] mobileControls;
    [HideInInspector] public bool isMobileControls;

    public void toggleOn()
    {
        isMobileControls = !isMobileControls;
        foreach(GameObject control in mobileControls)
        {
            control.SetActive(isMobileControls);
        }
    }
}
