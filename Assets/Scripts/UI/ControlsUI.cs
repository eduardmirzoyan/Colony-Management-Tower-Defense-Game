using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup holdGroup;
    [SerializeField] private TextMeshProUGUI holdLabel;

    [SerializeField] private CanvasGroup pressGroup;
    [SerializeField] private TextMeshProUGUI pressLabel;

    private void Start()
    {
        EventExitFollower(null);

        GameEvents.instance.OnEnterFollower += EventEnterFollower;
        GameEvents.instance.OnExitFollower += EventExitFollower;
        GameEvents.instance.OnRallyStart += EventRallyStart;
        GameEvents.instance.OnRallyEnd += EventRallyEnd;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterFollower -= EventEnterFollower;
        GameEvents.instance.OnExitFollower -= EventExitFollower;
        GameEvents.instance.OnRallyStart -= EventRallyStart;
        GameEvents.instance.OnRallyEnd -= EventRallyEnd;
    }

    private void EventEnterFollower(IFollower follower)
    {
        pressGroup.alpha = 1f;
        pressLabel.text = "Recruit";
    }

    private void EventExitFollower(IFollower follower)
    {
        pressGroup.alpha = 0f;
        pressLabel.text = "--";
    }

    private void EventRallyStart()
    {
        holdGroup.alpha = 0.66f;
        holdLabel.text = "Stop";
    }

    private void EventRallyEnd()
    {
        holdGroup.alpha = 1f;
        holdLabel.text = "Rally";
    }
}
