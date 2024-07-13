using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static event EventHandler LevelLoaded;
    
    [SerializeField]
    private string levelName;

    [SerializeField]
    private List<Transform> spawnPoints;

    public string GetLevelName() {
        return levelName;
    }

    public Transform GetSpawnPoint(int index) {
        return spawnPoints[index];
    }

    public List<Transform> GetSpawnPoints() {
        return spawnPoints;
    }

    public void OnLoaded()
    {
        Debug.Log(this.name + " loaded!");
        LevelLoaded?.Invoke(this, EventArgs.Empty);
    }
}
