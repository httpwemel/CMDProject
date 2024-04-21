using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Coin : MonoBehaviour, IInteractable
{
    public void OnInteract(PlayerCharacter player)
    {
        Debug.Log("Coin: Player Detected");
        player.coins.SetValueAndForceNotify(player.coins.Value + 1);
        this.gameObject.SetActive(false);
    }
}
