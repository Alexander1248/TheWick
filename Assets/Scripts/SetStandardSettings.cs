using System;
using Unity.VisualScripting;
using UnityEngine;


public class SetStandardSettings : MonoBehaviour
{
    private void Awake()
    {
        PlayerPrefs.SetString("Attack", KeyCode.Mouse0.ToString());
    }
}