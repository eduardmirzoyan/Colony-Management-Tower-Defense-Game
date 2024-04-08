using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas playerScreen;
    [SerializeField] private Image blackScreen;

    [Header("Settings")]
    [SerializeField] private float transitionDuration;

    private Vector2 _targetCanvasPos;

    private Coroutine coroutine;
    private static readonly int RADIUS = Shader.PropertyToID("_Radius");
    private static readonly int CENTER_X = Shader.PropertyToID("_CenterX");
    private static readonly int CENTER_Y = Shader.PropertyToID("_CenterY");

    public void Reset()
    {
        var mat = blackScreen.material;
        mat.SetFloat(RADIUS, 0f);
        mat.SetFloat(CENTER_X, 0.5f);
        mat.SetFloat(CENTER_Y, 0.5f);
    }

    public void OpenBlackScreen()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(TransitionOverTime(transitionDuration, 0, 1));
    }

    public void CloseBlackScreen()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(TransitionOverTime(transitionDuration, 1, 0));
    }

    private IEnumerator TransitionOverTime(float duration, float beginRadius, float endRadius)
    {
        var mat = blackScreen.material;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            var t = elapsed / duration;
            var radius = Mathf.Lerp(beginRadius, endRadius, t);
            mat.SetFloat(RADIUS, radius);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat(RADIUS, endRadius);
    }
}