using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PolygonCollider2D polygonCollider2d;
    [SerializeField] private Light2D light2d;
    [SerializeField] private BarrierHandler[] barrierHandlers;

    [Header("Data")]
    [SerializeField] private RoomData roomData;

    [Header("Settings")]
    [SerializeField] private float dimIntensity;
    [SerializeField] private float lowIntensity;
    [SerializeField] private float brightIntensity;
    [SerializeField] private float lightDuration;

    private void Start()
    {
        GameEvents.instance.OnEnterRoom += EventEnterRoom;
        GameEvents.instance.OnExitRoom += EventExitRoom;
        GameEvents.instance.OnDiscoverRoom += EventDiscoverRoom;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEnterRoom -= EventEnterRoom;
        GameEvents.instance.OnExitRoom -= EventExitRoom;
        GameEvents.instance.OnDiscoverRoom -= EventDiscoverRoom;
    }

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;

        // Set position
        transform.position = roomData.worldPosition;

        // Set boundary
        Vector2[] path = new Vector2[4]
        {
            new (-roomData.size / 2, - roomData.size / 2),
            new (roomData.size / 2, -roomData.size / 2),
            new (roomData.size / 2, roomData.size / 2),
            new (-roomData.size / 2, roomData.size / 2),
        };
        polygonCollider2d.SetPath(0, path);

        // Set light
        light2d.intensity = dimIntensity;

        InitializeBarriers();

        // Set name
        gameObject.name = $"{roomData}";
    }

    public void Enter()
    {
        GameManager.instance.EnterRoom(roomData);
    }

    private void EventEnterRoom(RoomData roomData)
    {
        if (this.roomData != roomData) return;

        // Move camera to here
        CameraManager.instance.SetTarget(transform);

        // Light up room depending on state
        StopAllCoroutines();
        if (roomData.isDiscovered)
            StartCoroutine(ChangeLightOverTime(lowIntensity, brightIntensity, lightDuration));
        else
            StartCoroutine(ChangeLightOverTime(dimIntensity, brightIntensity, lightDuration));

    }

    private void EventExitRoom(RoomData roomData)
    {
        if (this.roomData != roomData) return;

        // Dim room depending on state
        StopAllCoroutines();
        if (roomData.isDiscovered)
            StartCoroutine(ChangeLightOverTime(brightIntensity, lowIntensity, lightDuration));
        else
            StartCoroutine(ChangeLightOverTime(brightIntensity, dimIntensity, lightDuration));
    }

    private void EventDiscoverRoom(RoomData roomData)
    {
        if (this.roomData != roomData) return;

        // Delete all barriers
        foreach (var barrier in barrierHandlers)
        {
            Destroy(barrier.gameObject);
        }
        barrierHandlers = null;
    }

    private IEnumerator ChangeLightOverTime(float startIntensity, float endIntensity, float duration)
    {
        light2d.intensity = startIntensity;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            light2d.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        light2d.intensity = endIntensity;
    }

    #region Helpers

    private void InitializeBarriers()
    {
        barrierHandlers[0].Initialize(roomData, Vector2.up);
        barrierHandlers[1].Initialize(roomData, Vector2.down);
        barrierHandlers[2].Initialize(roomData, Vector2.left);
        barrierHandlers[3].Initialize(roomData, Vector2.right);
    }

    #endregion
}
