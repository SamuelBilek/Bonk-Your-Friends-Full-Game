using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchPanel : MonoBehaviour
{
    [SerializeField]
    private Button startMatchButton;

    [SerializeField]
    private TextMeshProUGUI roundCounterDisplay;

    // Update is called once per frame
    void Update()
    {
        startMatchButton.interactable = PlayerConfigurationManager.Instance.CanMatchStart();
        roundCounterDisplay.text = LevelManager.Instance.GetMenuLevel().GetNumberOfRounds().ToString();
    }

    public void StartMatchButton_OnClick()
    {
        GameManager.Instance.StartMatch(LevelManager.Instance.GetMenuLevel().GetNumberOfRounds());
    }

    public void IncrementRoundButton_OnClick()
    {
        LevelManager.Instance.GetMenuLevel().IncrementNumberOfRounds();
    }

    public void DecrementRoundButton_OnClick()
    {
        LevelManager.Instance.GetMenuLevel().DecrementNumberOfRounds();
    }
}
