using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ToggleAudioUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image iconImage;

    [Header("Data")]
    [SerializeField] private string parameterName;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private bool isOn;

    private void Awake()
    {
        audioMixer.GetFloat(parameterName, out float value);
        if (value < 0)
        {
            iconImage.sprite = offSprite;
            isOn = false;
        }
        else
        {
            iconImage.sprite = onSprite;
            isOn = true;
        }
    }

    public void Toggle()
    {
        if (isOn)
        {
            audioMixer.SetFloat(parameterName, -80f);
            iconImage.sprite = offSprite;
            isOn = false;
        }
        else
        {
            audioMixer.SetFloat(parameterName, 0f);
            iconImage.sprite = onSprite;
            isOn = true;
        }
    }
}
