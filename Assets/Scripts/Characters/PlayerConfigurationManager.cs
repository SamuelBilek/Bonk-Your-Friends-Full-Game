using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurationManager : MonoBehaviour
{    
    public static PlayerConfigurationManager Instance { get; private set; }
    
    private List<PlayerConfiguration> playerConfigs;

    [SerializeField]
    private List<Color> playerColors;

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
        playerConfigs = new List<PlayerConfiguration>();
    }

    public void ReadyPlayer(PlayerInput input) {
        PlayerConfiguration playerConfig = playerConfigs.Where(p => p.Input == input).FirstOrDefault();
        if (playerConfig != null) {
            playerConfig.IsReady = true;
            input.GetComponent<PlayerUI>().SetReady(true);
            Debug.Log("Player " + (input.playerIndex + 1) + " is ready");
        } else {
            Debug.LogError("Player not found in playerConfigs");
        }
    }

    public void UnreadyPlayer(PlayerInput input) {
        PlayerConfiguration playerConfig = playerConfigs.Where(p => p.Input == input).FirstOrDefault();
        if (playerConfig != null) {
            playerConfig.IsReady = false;
            input.GetComponent<PlayerUI>().SetReady(false);
            Debug.Log("Player " + (input.playerIndex + 1) + " is not ready");
        } else {
            Debug.LogError("Player not found in playerConfigs");
        }
    }

    public bool IsPlayerReady(PlayerInput input) {
        PlayerConfiguration playerConfig = playerConfigs.Where(p => p.Input == input).FirstOrDefault();
        if (playerConfig != null) {
            return playerConfig.IsReady;
        } else {
            Debug.LogError("Player not found in playerConfigs");
            return false;
        }
    }

    public bool CanMatchStart() {
        if (playerConfigs == null) {
            return false;
        }
        return playerConfigs.Where(p => p.IsReady).Count() > 1;
    }

    public void HandlePlayerJoin(PlayerInput pi) {
        #if UNITY_EDITOR
        if (GameManager.Instance.GetGameState() == GameManager.GameState.Playing) {
            // Check if the player is already in the game
            if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex)) {
                playerConfigs.Add(new PlayerConfiguration(pi));
                PlayerUI playerUI = pi.GetComponent<PlayerUI>();
                playerUI.SetPlayerColor(playerColors[pi.playerIndex]);
                playerUI.SetPlayerName("Player " + (pi.playerIndex + 1).ToString());
                playerUI.SetPlayerColor(playerColors[pi.playerIndex]);
                playerUI.ResetScore();
                PlayerScoreCounter scoreCounter = GameManager.Instance.GetPlayerScoreCounter(pi.playerIndex);
                scoreCounter.gameObject.SetActive(true);
                scoreCounter.ResetScore();
                scoreCounter.SetColor(playerColors[pi.playerIndex]);
                UnreadyPlayer(pi);
                GameManager.Instance.AddPlayerToMatch(pi);
                return;
            }
        }
        #endif
        
        if (GameManager.Instance.GetGameState() != GameManager.GameState.Menu) {
            Debug.LogError("HandlePlayerJoin called outside of menu level");
        }
        
        Debug.Log("Player " + (pi.playerIndex + 1)  + " has joined the game");

        // Check if the player is already in the game
        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex)) {
            pi.GetComponent<MenuLevelPlayerController>().SetMenuLevelActionMap();
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(pi));
            PlayerUI playerUI = pi.GetComponent<PlayerUI>();
            playerUI.SetPlayerColor(playerColors[pi.playerIndex]);
            playerUI.SetPlayerName("Player " + (pi.playerIndex + 1).ToString());
            playerUI.SetPlayerColor(playerColors[pi.playerIndex]);
            playerUI.ResetScore();
            PlayerScoreCounter scoreCounter = GameManager.Instance.GetPlayerScoreCounter(pi.playerIndex);
            scoreCounter.gameObject.SetActive(true);
            scoreCounter.ResetScore();
            scoreCounter.SetColor(playerColors[pi.playerIndex]);
            UnreadyPlayer(pi);
            MenuLevel menuLevel = LevelManager.Instance.GetMenuLevel();
            Transform spawnPoint = menuLevel.GetSpawnPoint(playerConfigs.Count - 1);
            PlayerSpawner.SpawnPlayer(spawnPoint, pi, true, pi.playerIndex);
        }
    }

    public void ExtractActivePlayersForMatch() {     
        var toRemove = new List<PlayerConfiguration>();
        foreach (var player in playerConfigs) {
            if (!player.IsReady) {
                toRemove.Add(player);
            } else {
                GameManager.Instance.AddPlayerToMatch(player.Input);
            }
        }
        foreach (var player in toRemove) {
            playerConfigs.Remove(player);
            Destroy(player.PlayerTransform.gameObject);
        }
        playerConfigs.Clear();
    }

    public void ReturnActivePlayersToMenu(List<PlayerInput> players) {
        if (GameManager.Instance.GetGameState() != GameManager.GameState.Menu) {
            Debug.LogError("ReturnActivePlayersToMenu called outside of menu level");
        }
        
        playerConfigs?.Clear();
        foreach (var player in players) {
            HandlePlayerJoin(player);
        }
        
    }
}

public class PlayerConfiguration {
    public PlayerInput Input { get; set; }
    public Transform PlayerTransform { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }

    public PlayerConfiguration(PlayerInput input) {
        PlayerIndex = input.playerIndex;
        PlayerTransform = input.transform; 
        Input = input;
    }
}
