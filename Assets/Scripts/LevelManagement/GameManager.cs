using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;
using UnityEngine.InputSystem;

using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Playing,
    }

    public static GameManager Instance { get; private set; }

    private GameState gameState;
    private int roundsLeft = int.MaxValue;
    private bool isPaused;

    private float volume = 1f;

    public float Volume { 
        get {
            return volume;
        } 
        set { 
            volume = Mathf.Clamp(value, 0f, 1f);
        } 
    }

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private List<PlayerScoreCounter> playerScoreCounters;

    private List<PlayerInput> activePlayers;

    public ReadOnlyCollection<PlayerInput> GetActivePlayers() {
        return activePlayers.AsReadOnly();
    }

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

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
        activePlayers = new List<PlayerInput>();
        MenuLevel.MatchStart += MenuLevel_OnMatchStart;
        PlayableLevel.LevelFinished += PlayableLevel_OnLevelFinished;
        Level.LevelLoaded += Level_OnLevelLoaded;

        if (LevelManager.Instance.GetCurrentLevel() is MenuLevel)
        {
            gameState = GameState.Menu;
        }
        else
        {
            gameState = GameState.Playing;
        }
    }

    private void MenuLevel_OnMatchStart(object sender, EventArgs e)
    {
        StartMatch(LevelManager.Instance.GetMenuLevel().GetNumberOfRounds());
    }

    private void PlayableLevel_OnLevelFinished(object sender, System.EventArgs e)
    {
        roundsLeft--;
        if (roundsLeft <= 0)
        {
            roundsLeft = int.MaxValue;
            ReturnToMenu();
            return;
        }
        LevelManager.Instance.LoadNextPlayableLevel();
    }

    private void Level_OnLevelLoaded(object sender, System.EventArgs e)
    {
        if (LevelManager.Instance.GetCurrentLevel() is MenuLevel)
        {
            gameState = GameState.Menu;
            PlayerConfigurationManager.Instance.ReturnActivePlayersToMenu(activePlayers);
            activePlayers.Clear();
            ResumeGame();
        }
        else
        {
            if (gameState == GameState.Menu)
            {
                gameState = GameState.Playing;
                PlayerConfigurationManager.Instance.ExtractActivePlayersForMatch();
            }
            else
            {
                PlayerSpawner.SpawnPlayers(LevelManager.Instance.GetCurrentLevel(), activePlayers, false);
            }
        }
    }

    public int GetActivePlayerCount()
    {
        if (activePlayers == null)
        {
            return 0;
        }
        return activePlayers.Count;
    }

    public List<PlayerInput> GetActiveAlivePlayers()
    {
        List<PlayerInput> alivePlayers = new List<PlayerInput>();
        if (activePlayers == null)
        {
            return alivePlayers;
        }
        foreach (PlayerInput player in activePlayers)
        {
            if (!player.GetComponent<PlayableLevelPlayerController>().IsDead())
            {
                alivePlayers.Add(player);
            }
        }
        return alivePlayers;
    }

    public Tuple<Vector3, float> GetCenterAndMaxDistanceBetweenPlayers()
    {
        List<PlayerInput> alivePlayers = GetActiveAlivePlayers();
        if (alivePlayers.Count == 0)
        {
            return new Tuple<Vector3, float>(Vector3.zero, 0);
        }
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.zero;   
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            PlayerInput player = alivePlayers[i];
            if (player != null)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (player.transform.position[j] < min[j])
                    {
                        min[j] = player.transform.position[j];
                    }
                    if (player.transform.position[j] > max[j])
                    {
                        max[j] = player.transform.position[j];
                    }
                }
            }
        }
        return new Tuple<Vector3, float>(Vector3.Lerp(min, max, 0.5f), (max - min).magnitude);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void StartMatch(int rounds)
    {
        canvas.SetActive(true);
        Debug.Log("Starting match...");
        PlayerConfigurationManager.Instance.GetComponent<PlayerInputManager>().DisableJoining();
        roundsLeft = rounds;
        LevelManager.Instance.LoadNextPlayableLevel();
    }

    public void ReturnToMenu()
    {
        canvas.SetActive(false);
        for (int i = 0; i < activePlayers.Count; i++)
        {
            PlayerScoreCounter playerScoreCounter = playerScoreCounters[i];
            playerScoreCounter.ResetScore();
            activePlayers[i].GetComponent<PlayerUI>().ResetScore();
        }
        Debug.Log("Returning to menu...");
        LevelManager.Instance.LoadMenuLevel();
        PlayerConfigurationManager.Instance.GetComponent<PlayerInputManager>().EnableJoining();
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        Time.timeScale = 1;
        isPaused = false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddPlayerToMatch(PlayerInput player)
    {
        activePlayers.Add(player);
        player.transform.SetParent(Instance.transform);
        player.GetComponent<PlayableLevelPlayerController>().SetPlayableLevelActionMap();
        PlayerSpawner.SpawnPlayer(
            LevelManager.Instance.GetCurrentLevel().GetSpawnPoint(activePlayers.Count - 1), player, false, activePlayers.Count - 1);
    }

    public PlayerScoreCounter GetPlayerScoreCounter(int playerIndex)
    {
        return playerScoreCounters[playerIndex];
    }

    public void IncrementPlayerScore(PlayerInput player)
    {
        PlayerScoreCounter playerScoreCounter = playerScoreCounters[player.playerIndex];
        playerScoreCounter.AddScore(1);
        PlayerUI playerUI = player.GetComponent<PlayerUI>();
        playerUI.AddScore(1);
    }

    public bool IsLastRound()
    {
        return roundsLeft == 1;
    }

    public List<PlayerInput> GetWinningPlayers()
    {
        int maxScore = activePlayers.Max(player => player.GetComponent<PlayerUI>().GetScore());
        return activePlayers.Where(player => player.GetComponent<PlayerUI>().GetScore() == maxScore).ToList();
    }
}
