﻿using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GasInventory : MonoBehaviour
{
    [SerializeField] private int gasCylinderMax = 10;
    [SerializeField] private int gasCylinderCount = 10;
    [SerializeField] private BlockyBar gasCylinderBar;
    
    public void AddCompressedSteamCylinder(int count)
    {
        gasCylinderCount = Mathf.Min(gasCylinderCount + count, gasCylinderMax);
        if (gasCylinderBar.IsUnityNull() && gasCylinderBar.enabled) 
            gasCylinderBar.Set(gasCylinderCount);
        PlayerPrefs.SetInt("CompressedGas", gasCylinderCount);
    }

    public bool IsEmpty()
    {
        return gasCylinderCount <= 0;
    }

    public void Edit(int value)
    {
        gasCylinderCount--;
        if (!gasCylinderBar.IsUnityNull() && gasCylinderBar.enabled) 
            gasCylinderBar.Set(gasCylinderCount);
        PlayerPrefs.SetInt("CompressedGas", gasCylinderCount);
    }
    public void Start()
    {
        if (PlayerPrefs.HasKey("CompressedGas"))
            gasCylinderCount = PlayerPrefs.GetInt("CompressedGas");
        gasCylinderBar.Initialize(
            gasCylinderMax,
            new Vector2(0, 0),
            new Vector2Int(128, 72),
            new Vector2Int(10, -40),
            1);
        gasCylinderBar.Set(gasCylinderCount);
        DisableBar();
    }

    public void EnableBar()
    {
        if (gasCylinderBar == null) return;
        gasCylinderBar.gameObject.SetActive(true);
        gasCylinderBar.Set(gasCylinderCount);
    }

    public void DisableBar()
    {
        if (gasCylinderBar == null) return;
        gasCylinderBar.gameObject.SetActive(false);
    }
}