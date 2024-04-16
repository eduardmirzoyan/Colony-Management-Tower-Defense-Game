using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI stateLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;

    private void Start()
    {
        GameEvents.instance.OnStateExpand += EventExpand;
        GameEvents.instance.OnStatePrepare += EventPrepare;
        GameEvents.instance.OnStateDefend += EventDefend;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnStateExpand -= EventExpand;
        GameEvents.instance.OnStatePrepare -= EventPrepare;
        GameEvents.instance.OnStateDefend -= EventDefend;
    }

    private void EventDefend()
    {
        stateLabel.text = "Defend";
        descriptionLabel.text = "Defend your base from the incoming wave of enemies!";
    }

    private void EventPrepare(WaveData _)
    {
        stateLabel.text = "Prepare";
        descriptionLabel.text = "Prepare your troops for the incoming enemy wave!";
    }

    private void EventExpand()
    {
        stateLabel.text = "Expand";
        descriptionLabel.text = "Expand your nation's borders by exploring an adjacent location!";
    }
}
