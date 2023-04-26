using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int health;
    [SerializeField] private int maxHealth;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        ClampHealth();
    }

    private void ClampHealth()
    {
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        else if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
