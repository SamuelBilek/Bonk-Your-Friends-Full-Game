using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayableLevel : Level
{
    [SerializeField]
    private LevelTransitionPanel transitionPanel;
    private bool isFinished = false;
    public static event EventHandler LevelFinished;
    private void Start()
    {
        LevelTransitionPanel.FadeInComplete += TransitionPanel_OnLoaded;
        isFinished = false;
        OnLoaded();
        StartCoroutine(transitionPanel.FadeOutImage());
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinished)
        {
            return;
        }
        if (GameManager.Instance.GetActivePlayerCount() >= 2 && GameManager.Instance.GetActiveAlivePlayers().Count <= 1)
        {
            foreach (PlayerInput player in GameManager.Instance.GetActiveAlivePlayers())
            {
                GameManager.Instance.IncrementPlayerScore(player);
            }
            isFinished = true;
            StartCoroutine(transitionPanel.FadeInImage());
            return;
        }
        #if UNITY_EDITOR
        // TODO: Add custom condition to finish the level   
        if (Input.GetKeyDown(KeyCode.F)) {
            isFinished = true;
            StartCoroutine(transitionPanel.FadeInImage());
        }
        #endif
    }

    private void TransitionPanel_OnLoaded(object sender, EventArgs e)
    {
        // WTF Why does this trigger even after being destroyed??
        if (this != null)
        {
            LevelFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
