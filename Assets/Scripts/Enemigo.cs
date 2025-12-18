using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Enemigo : VIVO
{
    #region CORE

    private void Start()
    {
        StartCoroutine(routine: CrPatrullaje());
    }

    private void Update()
    {
        if (muerto) return;
        Update_Rayo();
        Update_DirJugador();
    }

    protected override void EnTriggerEnter(GameObject go)
    {

    }
    #endregion
    #region MOVIMIENTO
    private float destinoX;

    //Si llego a su destino

    private bool DestinoAlcanzado()
    {
        //Posicion X del enemigo
        float x = transform.position.x;

        //Esta en la tolerancia del destino

        return (x >= destinoX - 0.1f && x <= destinoX + 0.1f);
    }

    #endregion

    #region PATRULLAJE
    [SerializeField] private float p1x;
    [SerializeField] private float p2x;
    private bool patrullajeIzquierda;

    public IEnumerator CrPatrullaje()
    {
    Inicio:
    // - Al principio determinamois hacia donde se va a mover
    if(patrullajeIzquierda) //Derecha a izquierda
        {
            axis = Vector3.left;
            destinoX = p1x;
        }
    else //Izquierda a derecha
        {
            axis = Vector3.right;
            destinoX = p2x;
        }
    //Va a estar en un bucle revisando si llego al destino
    //Se termina el bucle cuando haya llegado al destino

    RevisarDestinoAlcanzado:
        yield return  new WaitForFixedUpdate();
        if(!DestinoAlcanzado()) goto RevisarDestinoAlcanzado;

        // Si ya llego al destino, se para por (2s-4s) segundos
        axis = Vector3.zero; //Se para
        float tiempoEspera = Random.Range(0.5f, 4f); //Se obtiene el T random
        yield return new WaitForSeconds(tiempoEspera);

        //Despues del tiempo de espera, va a alternar el sentido
        patrullajeIzquierda = !patrullajeIzquierda;
        goto Inicio;
    }

    private void OnDrawGizmosSelected()
    {
        float y = transform.position.y;
        Vector2 p1 = new Vector2(p1x, y + 0.1f);
        Vector2 p2 = new Vector2(p2x, y + 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawIcon(p1, "P1");
        Gizmos.DrawIcon(p2, "P2");
        Gizmos.DrawLine(p1,p2);
    }
    #endregion PATRULLAJE

    #region ATAQUE

    private float distanciaAtaque = 0.6f;
    private float distanciaVision = 2;
    private bool persiguiendo = false;

    private void Update_DirJugador()
    {
        //Return: Si no esta persiguiendo al jugador
        if (!persiguiendo) return;

        //obtenemos la direccion del jugador
        Vector3 miPosicion = new Vector3(transform.position.x, 0, 0);
        Vector3 jugadorPosicion = new Vector3(GameManager.jugador.transform.position.x, 0, 0);
        Vector3 dir = jugadorPosicion - miPosicion; //Direccion izquierda o derecha

        //Hacemos que se mueva hacia donde esta el jugador
        axis = dir.normalized;
    }

    private void Update_Rayo()
    {
        if(atacando) return;


        //El origen, va a ser en el centro de la capsula collider
        Vector3 origen = transform.position + new Vector3(0, collider.offset.y, 0);

        //El rayo siempre va a apuntar hacia el "frente" del personaje, osea a la derecha
        Vector3 dir = transform.right;

        //A la distancia de ataque, le sumamos la mitad del ancho de la capsula collider
        float distancia = (collider.size.x / 2) + distanciaAtaque;

        
        LayerMask capa = LayerMask.GetMask("Jugador");

        //Lanzamos el rayo
        RaycastHit2D hit = Physics2D.Raycast(origin: origen, direction: dir, distancia, capa);

        //Si impacto con el jugador
        if (hit) Atacar();

        //Si aun no estamos persiguiendo al jugador
        if(!persiguiendo)
        {
            distancia = (collider.size.x / 2) + distanciaVision;
            hit = Physics2D.Raycast(origin: origen, direction: dir, distancia, capa);

            if(hit)
            {
                //Detenemos la corrutina de Patrullaje
                StopAllCoroutines();

                //Cambiamos el estado para que empiece a perseguir al jugador
                persiguiendo = true;
            }
        }

    }

    #endregion ATAQUE
    #region MUERTE
    [Header("Muerte")]
    [SerializeField] private float tiempoDesaparecer = 1.0f;
    private bool muerto;

   
    public override void Morir()
    {
        if (muerto) return;
        muerto = true;

        base.Morir();                 
        StopAllCoroutines();          // detiene patrullaje

        collider.enabled = false;     // ya no colisiona 
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;         

        Destroy(gameObject, tiempoDesaparecer);
    }
    #endregion MUERTE
}
