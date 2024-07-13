using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class PauseGamePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseGamePanel;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseGamePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.Instance.IsPaused()) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        GameManager.Instance.PauseGame();
        pauseGamePanel.SetActive(true);
    }

    private void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        pauseGamePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        ResumeGame();
        GameManager.Instance.ReturnToMenu();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
