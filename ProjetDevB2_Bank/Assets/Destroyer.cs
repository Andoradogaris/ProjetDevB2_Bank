using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private float tempoTime;

    private void Awake()
    {
        Destroy(gameObject, tempoTime);
    }
}
