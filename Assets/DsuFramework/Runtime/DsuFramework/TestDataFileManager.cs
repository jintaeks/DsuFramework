using System;
using UnityEngine;
using Dsu.Framework;

[Serializable]
public struct PlayerDataTest
{
    public string playerName;
    public int level;
    public float health;
    public Vector3 position;
    public int[] inventoryItemIds;
}

[Serializable]
public struct GameSettingsTest
{
    public bool isSoundOn;
    public float volume;
    public int resolutionIndex;
}

public class TestDataFileManager : MonoBehaviour
{
    void Start()
    {
        // Save
        PlayerDataTest pd = new PlayerDataTest
        {
            playerName = "UnityGuy",
            level = 10,
            health = 88.5f,
            position = new Vector3(1, 2, 3),
            inventoryItemIds = new[] { 101, 102 }
        };
        DataFileManager.Save(pd, "PlayerDataTest.json");

        // Load
        if (DataFileManager.Load("PlayerDataTest.json", out PlayerDataTest loaded)) {
            Debug.Log($"Loaded Player: {loaded.playerName}, Level: {loaded.level}");
        }

        // 다른 타입도 저장/로드 가능
        GameSettingsTest settings = new GameSettingsTest
        {
            isSoundOn = true,
            volume = 0.8f,
            resolutionIndex = 2
        };
        DataFileManager.Save(settings, "settings.json");

        if (DataFileManager.Load("settings.json", out GameSettingsTest loadedSettings)) {
            Debug.Log($"Sound: {loadedSettings.isSoundOn}, Volume: {loadedSettings.volume}");
        }
    }
}
