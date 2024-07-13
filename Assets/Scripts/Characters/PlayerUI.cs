using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{   
    [SerializeField]
    private TextMeshProUGUI playerName;
    
    [SerializeField]
    private ReadyLabelBillboard readyLabel;

    [SerializeField]
    private Image attackVisualizerImage;

    private int playerScore;

    public void SetReady(bool ready)
    {
        readyLabel.SetReady(ready);
    }

    public void MenuUISetEnabled(bool enabled)
    {
        playerName.gameObject.SetActive(enabled);
        readyLabel.gameObject.SetActive(enabled);
    }

    public void SetCamera(Camera camera)
    {
        playerName.GetComponent<Billboard>().SetCamera(camera);
        readyLabel.SetCamera(camera);
    }

    public void SetPlayerColor(Color color)
    {
        playerName.color = color;
        readyLabel.SetReadyColor(color);
        attackVisualizerImage.color = new Color(color.r, color.g, color.b, 0.5f);
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public string GetPlayerName()
    {
        return playerName.text;
    }

    public Color GetPlayerColor()
    {
        return playerName.color;
    }

    public void AddScore(int value)
    {
        playerScore += value;
    }

    public void ResetScore()
    {
        playerScore = 0;
    }

    public int GetScore()
    {
        return playerScore;
    }
}
