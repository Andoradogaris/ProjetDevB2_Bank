using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int money;

    [Header("UI")]
    [SerializeField] private TMP_Text moneyAmount;
    [SerializeField] private TMP_Text dropText;

    private void Start()
    {
        moneyAmount.text = "Carry more moneyBags";
        dropText.enabled = false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyAmount.text = "moneyAmount : " + amount.ToString() + " $";
    }

    public void DropText(bool isActive)
    {
        if (isActive)
        {
            dropText.enabled = true;
        }
        else
        {
            dropText.enabled = false;
        }
    }
}
