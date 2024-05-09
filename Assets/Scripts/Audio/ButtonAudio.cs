using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [Header("References")]
    [SerializeField] private Button button;

    private const string BUTTON_CLICK = "button_click";
    private const string BUTTON_HOVER = "button_hover";

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickAudio);
    }

    private void PlayClickAudio()
    {
        AudioManager.instance.PlaySFX(BUTTON_CLICK);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            AudioManager.instance.PlaySFX(BUTTON_HOVER);
    }
}
