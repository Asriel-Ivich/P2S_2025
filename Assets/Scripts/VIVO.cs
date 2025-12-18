using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class VIVO : MonoBehaviour
{
    #region CORE
    //Este metodo se ejecuta una sola vez cuando se instancia el objeto
    private void Awake()
    {
        Awake_Componentes();
    }

    //Este metodo se ejecuta 50 veces por segundo
    private void FixedUpdate()
    {
        FixedUpdate_Movimiento();
    }
    protected abstract void EnTriggerEnter(GameObject go);

    //Este metodo se ejecuta cuando nuestro collider toca un trigger 
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Si toca un Trigger con el tag "Muerte" Se muere
        if (other.CompareTag("Muerte"))
        {
            Morir();
        }
        //Layer 8: Daño
        else if (other.gameObject.layer ==8)
        {
            //Obtenemos el componente daño del Trigger
            Daño daño = other.GetComponent<Daño>();

            //Le restamos da;o a nuestra vida
            Vida -= daño.Valor;
        }

            //Llamar al metodo abstracto
            EnTriggerEnter(other.gameObject);
    }
    #endregion CORE


    #region COMPONENTES
    protected Rigidbody2D rb;
    protected CapsuleCollider2D collider;

    protected SpriteRenderer sprite;
    protected Animator animator;

    private void Awake_Componentes()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }
    #endregion COMPONENTES

    #region MOVIMIENTO
    //Parea que un atriburo se pueda ver en el inspecto de Unity
    //Hay que agregar la tiqueta de: [SerializeField]

    [SerializeField] protected float velocidad = 5;
    [SerializeField] protected float fuerzaSalto = 15;
    protected Vector3 axis = Vector3.zero;
    protected Vector3 movimiento = Vector3.zero;
    protected bool bloquearMovimiento = false;

    //Salto
    private bool enDelaySalto = false;

  


    private void FixedUpdate_Movimiento()
    {
        //Revisamos si esta tocando el piso o en el aire
        SensorPiso();

        //Revisa si esta cayendo el personaje
        SensorCaida();

        //RETURN: si esta bloqueado el movimiento
        if (bloquearMovimiento) return;

        //Si estamos moviendo al personaje
        if (axis.x !=0)
        {
            //Enviamos al parametro al animator
            animator.SetBool(name: "corriendo", true);

            //Rotacion Izquierda / Derecha
            Quaternion rotacion;
            if (axis.x > 0) rotacion = Quaternion.Euler(new Vector3(0, 0, 0));
            else rotacion = Quaternion.Euler(new Vector3(0, 180, 0));
            transform.rotation = rotacion;

            //Para que la rotacion de la barra de vida dse mantenga igual
            canvas.transform.rotation = Quaternion.identity;

        }
        //Si no estamos en moviendo al personaje
        else
        {
            //Enviamos el parametro al Animator
            animator.SetBool(name:"corriendo", false);
        }

            //Calculamos el movimiento en X
            movimiento.x = axis.x * velocidad;

        //Ajustamos el movimiento al delta
        movimiento *= Time.fixedDeltaTime; //0.02

        //Aplicamos el movimiento
        transform.position += movimiento;    
    }
    protected void Saltar()
    {

        //Si no esta tocando el piso, aqui terminamos el metodo y no continua
        if (!tocandoPiso) return; //Es lo mismo que : if (tocandoPiso == false) return

        //Creamos la fuerza en Vector 2 que tendra el salto
        Vector2 fuerza = new Vector2(0, fuerzaSalto);

        //Aplicamops la fuerza de impulso al Rigidbody
        rb.AddForce(fuerza, ForceMode2D.Impulse);

        //Ejecute la animacion
        animator.SetBool(name: "saltando", value: true);
        //Damos un pequeñoDelay:
        //para no cortar la animacion de salto con el sensor piso
        enDelaySalto = true;
        Invoke(methodName: "DelaySalto", time: 0.2f);
    }

    private void DelaySalto()
    {
        enDelaySalto = false;   
    }

    #endregion MOVIMIENTO

#region SENSORES
    private bool tocandoPiso = false;
    private Vector2 tamañoSensorPiso = new Vector2(0.25f, 0.05f);

    private void SensorPiso()
    {
        //Return: Si empezo el salto
        if(enDelaySalto) return;

        //El centro del rectangulo
        Vector2 centro = transform.position; // El piso del jugador

        //Angulo de rotacion
        float angulo = 0;

        //Filtro de capa
        LayerMask capa = LayerMask.GetMask("Default");

        //Proyectamos al Rectangulo imaginatrio, para saber que Collider hay dentro
        Collider2D[] arreglo = Physics2D.OverlapBoxAll(point: centro, size: tamañoSensorPiso, angulo, capa);

        //Numero de colliders dentro 
        int collidersDentro = arreglo.Length;

        //Si hay 1 o mas colliders, significa que esta tocando el piso de lo contrario esta en el aire
        tocandoPiso = collidersDentro > 0;

        //Si estaba en la animacion de salto o caida, las quita
        if (tocandoPiso)
        {
            animator.SetBool(name: "saltando", false);
            animator.SetBool(name:"cayendo",false);
        }

    }

    private void SensorCaida()
    {
        //Return: Si esta en el piso
        if(tocandoPiso) return;

        //si tiene velocidad decente en Y
        if(rb.linearVelocity.y<-1)
        {
            print(message: "Esta cayendo");
            animator.SetBool(name:"Saltando", false);
            animator.SetBool(name: "cayendo", true);
        }
    }



    #endregion SENSORES

    #region ATAQUE

    

    [Header("Ataque")]
    [SerializeField] private AnimationClip clipAtaque;
    protected bool atacando = false;

    protected void Atacar ()
    {
        //Return: Si ya esta atacando
        if (atacando) return;

        //Cambiamos el estado a que esta atacando 
        atacando = true;

        //bloqueamos el movimiento mientras realiza el ataque
        bloquearMovimiento = true;

        //Ejecuta la animacion
        animator.SetTrigger(name: "Ataque1");

        //cuando termine la animacion, se cambia el estado de atacando a false
        Invoke(methodName: "TerminarAtaque", clipAtaque.length);
    }
    private void TerminarAtaque()
    {
        atacando = false;
        bloquearMovimiento = false;
    }

    #endregion ATAQUE

    #region VIDA
    //Para usar la image, importar la libreria: Usibns UnityEngine.UI;
    [SerializeField] protected Canvas canvas;
    [SerializeField] private Image barraVida;
    private int _vida = 100;
    protected int _vidaMax = 100;
    private bool muerte = false;

    public int Vida
    {
        get => _vida;
        set
        {
            //Si recibo da;o
            if (value < _vida) StartCoroutine(routine: CrDaño());

            //Cambio de valor
            if (value <= 0) Morir(); //Muerte
            else if (value > _vidaMax) _vida = _vidaMax; //Evitamos Sobrewcuracion
            else _vida = value; //Rango normal 1/100

            //actualizamos la barra de vida
            barraVida.fillAmount = (float) _vida/ _vidaMax;
        }
    }

    //Los metodos virtuales, son los que podemos sobreescribir en las clases hijas

    public virtual void Morir()
    {
        muerte = true;

        //bloqueamos el movimiento
        bloquearMovimiento = true;

        //Ocultamos la barra de vida
        canvas.enabled = false;

        //Animacion
        animator.SetBool(name: "muerto", value: true);
        
       
    }

    public IEnumerator CrDaño()
    {
        //Cambia el color a rojo por 0.1s
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }
    #endregion VIDA
}
