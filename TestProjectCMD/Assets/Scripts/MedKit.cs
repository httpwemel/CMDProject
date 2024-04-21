using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class MedKit : MonoBehaviour, IInteractable
{
    internal bool collected = false;
    public void OnInteract(PlayerCharacter player)
    {
        Debug.Log("MedKit: Player Detected");
        if (collected)
        {
            return;
        }
        player.health.SetValueAndForceNotify(player.maxHealth);
        this.gameObject.SetActive(false);
    }
}
