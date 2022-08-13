using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public void OnHit()
    {
        LevelManager.instance.RemoveEnemy(this);
        Destroy(gameObject, 0.1f);
        SoundManager.instance.PlayBurn();
    }
}
