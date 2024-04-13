using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawner
{
    public abstract void Spawn(UnitData unitData);
}
