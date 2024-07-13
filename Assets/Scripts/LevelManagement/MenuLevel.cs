using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuLevel : Level
{
    public static event EventHandler MatchStart;

    [SerializeField]
    private PlayerInputManager playerJoinManager;

    [SerializeField]
    private int maxRounds = 99;

    [SerializeField]
    private int minRounds = 1;

    [SerializeField]
    private int numberOfRounds = 3;

    [SerializeField]
    private GameObject mainPanel;

    [SerializeField]
    private GameObject newGamePanel;

    [SerializeField]
    private GameObject optionsPanel;
    
    [SerializeField]
    private GameObject controlsPanel;

    [SerializeField]
    private GameObject creditsPanel;

    private List<GameObject> uiPanels;
    
    private void Start() 
    {
        uiPanels = new List<GameObject> { mainPanel, newGamePanel, optionsPanel, controlsPanel, creditsPanel };
        ShowMainUI();
        OnLoaded();
    }
    
    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Return)) {
        //     MatchStart?.Invoke(this, EventArgs.Empty);
        // }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ShowMainUI();
        }
    }

    public void ShowOptions()
    {
        playerJoinManager.DisableJoining();
        DisableAllPanels();
        optionsPanel.SetActive(true);
    }

    public void ShowControls()
    {
        playerJoinManager.DisableJoining();
        DisableAllPanels();
        controlsPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        playerJoinManager.DisableJoining();
        DisableAllPanels();
        creditsPanel.SetActive(true);
    }

    public void ShowMainUI()
    {
        playerJoinManager.DisableJoining();
        DisableAllPanels();
        mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowNewGameUI()
    {
        playerJoinManager.EnableJoining();
        DisableAllPanels();
        newGamePanel.SetActive(true);
    }

    private void DisableAllPanels()
    {
        uiPanels.ForEach(p => p.SetActive(false));
    }

    public int GetNumberOfRounds()
    {
        return numberOfRounds;
    }

    public void IncrementNumberOfRounds()
    {
        if (numberOfRounds < maxRounds)
        {
            numberOfRounds++;
        }
    }

    public void DecrementNumberOfRounds()
    {
        if (numberOfRounds > minRounds)
        {
            numberOfRounds--;
        }
    }
}
