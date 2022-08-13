using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class Friend : MonoBehaviour, IDamageable
{
    public void OnHit()
    {
        LevelManager.instance.UndoAction(new InputAction.CallbackContext());
        StartCoroutine(TakeDamage());
        SoundManager.instance.PlayBark();
    }

    private IEnumerator TakeDamage()
    {
        var renderer = GetComponentInChildren<SpriteRenderer>();
        var color = renderer.color;
        yield return new WaitForSeconds(0.2f);
        renderer.color = color;
    }
}
