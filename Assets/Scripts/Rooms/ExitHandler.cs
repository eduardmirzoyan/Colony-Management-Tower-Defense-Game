using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHandler : MonoBehaviour, ISpawner
{
    public void Spawn(UnitData _)
    {
        GameManager.instance.GameWin();
    }
}
