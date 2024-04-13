using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandler : MonoBehaviour, ISpawner
{
    public void Spawn()
    {
        // Start wave
        GameManager.instance.StartWave();
    }
}
