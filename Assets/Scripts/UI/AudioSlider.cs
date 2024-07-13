using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] Slider slider;

    private void Start()
    {
        slider.value = GameManager.Instance.Volume;
    }

    public void SetVolume()
    {
        GameManager.Instance.Volume = slider.value;
    }
}
