using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBags : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.DropText(true);
        }
        else if (other.CompareTag("GoldBag"))
        {
            Destroy(other.gameObject, 2f);
            gameManager.AddMoney(100000);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.DropText(false);
        }
    }
}
