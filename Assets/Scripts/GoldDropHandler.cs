using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDropHandler : MonoBehaviour
{
    [SerializeField] private int goldValue;

    public void Initialize(int goldValue)
    {
        this.goldValue = goldValue;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerHandler playerHandler))
        {
            print($"{playerHandler.UnitData} gained {goldValue} gold!");

            // Add gold to player
            playerHandler.UnitData.goldHeld += goldValue;

            // Event
            GameEvents.instance.TriggerOnGoldGain();

            // Destroy this
            Destroy(gameObject);
        }
    }
}
