using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    [SerializeField]
    private bool randomLevelRotation = false;

    [SerializeField]
    private MenuLevel menuLevel;

    [SerializeField]
    private List<PlayableLevel> playableLevels;

    [SerializeField]
    private Level currentLevel;

    private int playableLevelIndex;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playableLevels is null || playableLevels.Count == 0)
        {
            Debug.LogError("No playable levels found");
        }
        
        if (currentLevel is MenuLevel) 
        {
            playableLevelIndex = -1;
        } else 
        {
            foreach (var level in playableLevels)
            {
                if (level.GetLevelName() == currentLevel.GetLevelName())
                {
                    playableLevelIndex = playableLevels.IndexOf(level);
                    break;
                }
            }
        }
    }

    public void LoadLevel(Level level)
    {
        string levelName = level.GetLevelName();
        Debug.Log("Loading level: " + levelName);
        currentLevel = level;
        SceneManager.LoadScene(levelName);
    }

    public void LoadMenuLevel()
    {
        playableLevelIndex = -1;
        LoadLevel(menuLevel);
    }

    public void LoadNextPlayableLevel()
    {
        if (!randomLevelRotation)
        {
            playableLevelIndex++;
            Debug.Log(playableLevelIndex);
            if (playableLevelIndex >= playableLevels.Count)
            {
                Debug.Log("triggered");
                playableLevelIndex = 0;
            }
        }
        else
        {
            int newPlayableLevelIndex = Random.Range(0, playableLevels.Count);
            while (newPlayableLevelIndex == playableLevelIndex)
            {
                newPlayableLevelIndex = Random.Range(0, playableLevels.Count);
            }
            playableLevelIndex = newPlayableLevelIndex;
        }
        Level nextLevel = playableLevels[playableLevelIndex];
        LoadLevel(nextLevel);
    }
    
    public MenuLevel GetMenuLevel()
    {
        return menuLevel;
    }

    public Level GetCurrentLevel()
    {
        return currentLevel;
    }
}
