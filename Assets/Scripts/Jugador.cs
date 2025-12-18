using System;
using Unity.Cinemachine;
using UnityEngine;
using Transform = UnityEngine.Transform;

public class Jugador : VIVO
{
    #region CORE
    [SerializeField] private CinemachineCamera camara;


    //Este metodo se ejecuta cada frame
    // por ejemplo: Si el juego nos va a 60 FPS...
    //Se ejecutara 60 veces por segundo

    private void Update()
    {
        Update_Input();
    }

    protected override void EnTriggerEnter(GameObject go)
    {
        switch (go.layer)
        {
            //Layer Moneda
            case 9:
                //Aumentamos el contador de monedas
                GameManager.Monedas++;
                //Desttruimos la moneda
                Destroy(go);
                break;

            //Layer10: Checkpoint
            case 10: Trigger_checkpoint(go); break;

            //Layer Default:
            default:
                if (go.CompareTag("Victoria"))
                    GameManager.Victoria();//Funcion
                break;
        }
    }
    #endregion CORE

    #region INPUT

    private void Update_Input()
    {
        //Registramos el axis del WASD / Flechas / Joystick
        axis.x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) Saltar();

        if (Input.GetKeyDown(KeyCode.K)) Atacar();
    }
    #endregion INPUT


    #region VIDA
    public override void Morir()
    {
        base.Morir();

        //Nos va a dejar de seguir la camara
        camara.Follow = null;

        //Despues de 2s de morir, revive el jugador
        Invoke(methodName: "Revivir", time: 2);
    }
    

    private void Revivir()
    {
        //Lo aparece en el ultimo checkpoint activado
        transform.position = checkpoint.position;

        //Si tenia una velocidad al morir (por ejemplo de caida
        //Restauramos la velocidad a cero para evitar BUGS
        rb.linearVelocity = Vector2.zero;

        //Hacemos que la camara vuelva a seguir al jugador
        camara.Follow = transform;

        //Desbloqieamos el movimiento
        bloquearMovimiento = false;

        //Volvemos a mostrar la barra de vida
        canvas.enabled = true;

        //Restauramos la vida
        Vida = _vidaMax; 

        //Animacion
        animator.SetBool(name:"muerto", value:false);

    }
    #endregion VIDA

    #region CHECKPOINT
    [SerializeField] private Transform checkpoint;

    private void Trigger_checkpoint(GameObject go)
    {
        //obtenemos la referencia de la bandera
        GameObject bandera = go.transform.GetChild(0).gameObject;
        
        //Si no habia activado el Checkpoint ...
        if (!bandera.activeSelf)
        {
            //Activamos la bandera
            bandera.SetActive(true);

            //Guardamos el Checkpoint
            checkpoint = go.transform;
        }

    }




    #endregion

}
