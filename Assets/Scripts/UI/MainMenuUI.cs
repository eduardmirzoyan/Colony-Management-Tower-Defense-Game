using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        TransitionManager.instance.Initialize();
        TransitionManager.instance.OpenScene();
    }

    public void StartGame()
    {
        // Start next level
        TransitionManager.instance.LoadNextScene();
    }
}
