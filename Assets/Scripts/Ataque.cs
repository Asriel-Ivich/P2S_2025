using System;
using UnityEngine;

public class Ataque : MonoBehaviour
{
    [SerializeField] private int daño = 10;
    [SerializeField] private Objetivo objetivo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Si colisiona con el jugador y el objetivo es el jugador
        //...o colisiona con el enemigo y el objetivo es el enemigo
        if ((other.gameObject.layer == 6 && objetivo == Objetivo.Jugador)
            || other.gameObject.layer == 7 && objetivo == Objetivo.Enemigo)
        {
            VIVO vivo = other.gameObject.GetComponent<VIVO>();
            vivo.Vida -= daño;
        }
    }
}

//Creamo un Enum, para designar el objetivo del daño

public enum Objetivo
{
    Jugador,
    Enemigo
}
