using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class LevelTransitionPanel : MonoBehaviour
{
    [SerializeField]
    private Image transitionImage;
    
    [SerializeField]
    private TextMeshProUGUI transitionCounter;

    public static event EventHandler FadeInComplete;

    public IEnumerator FadeOutImage()
    {
        gameObject.SetActive(true);
        transitionCounter.gameObject.SetActive(true);
        transitionCounter.color = Color.white;
        GameManager.Instance.PauseGame();
        StartCoroutine(UpdateCounter(3));
        yield return StartCoroutine(ChangeImageAlpha(0f, 3f));
        
        gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public IEnumerator FadeInImage()
    {
        gameObject.SetActive(true);
        transitionCounter.gameObject.SetActive(false);
        GameManager.Instance.PauseGame();
        float timer = 0.5f;
        if (GameManager.Instance.IsLastRound())
        {
            timer = 3f;
            List<PlayerInput> winners = GameManager.Instance.GetWinningPlayers();
            if (winners.Count == 1)
            {
                StartCoroutine(DisplayWinner(winners[0], (int)timer));
            }
            else
            {
                StartCoroutine(DisplayWinner(null, (int)timer));
            }
        }
        
        yield return StartCoroutine(ChangeImageAlpha(1f, timer));

        FadeInComplete?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration)
    {
        // Ensure the image has a reference
        if (transitionImage == null)
        {
            Debug.LogError("TransitionImage is not set.");
            yield break;
        }

        Color initialColor = transitionImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(initialColor.a, targetAlpha, elapsedTime / duration);
            transitionImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);

            yield return null;
        }

        // Ensure the target alpha is set at the end
        transitionImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
    }

    public IEnumerator UpdateCounter(int seconds)
    {
        if (transitionCounter == null)
        {
            Debug.LogError("TransitionCounter is not set.");
            yield break;
        }

        transitionCounter.gameObject.SetActive(true);

        for (int i = seconds; i > 0; i--)
        {
            if (i > 0)
            {
                transitionCounter.text = i.ToString();
            }
            yield return new WaitForSecondsRealtime(1f);
        }

        transitionCounter.gameObject.SetActive(false);
    }

    public IEnumerator DisplayWinner(PlayerInput player, int duration)
    {
        if (transitionCounter == null)
        {
            Debug.LogError("TransitionCounter is not set.");
            yield break;
        }

        transitionCounter.gameObject.SetActive(true);

        if (player == null)
        {
            transitionCounter.text = "It's a tie!";
            transitionCounter.color = Color.white;
        }
        else
        {
            PlayerUI playerUI = player.GetComponent<PlayerUI>();
            transitionCounter.text = playerUI.GetPlayerName() + " wins!";
            transitionCounter.color = playerUI.GetPlayerColor();
        }

        yield return new WaitForSecondsRealtime(duration);
        transitionCounter.gameObject.SetActive(false);
    }
}
