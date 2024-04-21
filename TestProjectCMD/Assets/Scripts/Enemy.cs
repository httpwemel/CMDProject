using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Enemy : MonoBehaviour, IInteractable
{    
    internal Collider2D _collider;
    SpriteRenderer spriteRenderer;

    public Bounds Bounds => _collider.bounds;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnInteract(PlayerCharacter playerCharacter)
    {
        Debug.Log(playerCharacter.health.Value - 1);
        playerCharacter.health.SetValueAndForceNotify(playerCharacter.health.Value - 1);
        if (playerCharacter.health.Value < 1)
        {
            playerCharacter.status.SetValueAndForceNotify("Player Died");
            playerCharacter.move.Disable();
            playerCharacter.jump.Disable();
            playerCharacter.interact.Disable();
        }
        this.gameObject.SetActive(false);
    }

}