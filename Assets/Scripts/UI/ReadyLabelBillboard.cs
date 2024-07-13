using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class ReadyLabelBillboard : Billboard
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    [SerializeField]
    private Button label;

    [SerializeField]
    private Color readyColor;

    [SerializeField]
    private Color notReadyColor;

    [SerializeField]
    private string readyText;

    [SerializeField]
    private string notReadyText;

    public void SetReady(bool ready)
    {
        if (ready)
        {
            SetColor(readyColor);
            SetText(readyText);
        }
        else
        {
            SetColor(notReadyColor);
            SetText(notReadyText);
        }
    }

    private void SetText(string text)
    {
        textMesh.text = text;
    }

    private void SetColor(Color color)
    {
        var cb = label.colors;
        cb.normalColor = color;
        label.colors = cb;
    }

    public void SetReadyColor(Color color)
    {
        readyColor = color;
    }
}
