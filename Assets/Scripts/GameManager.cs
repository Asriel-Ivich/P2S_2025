using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    #region CORE
    private static GameManager self;
    private static Jugador _Jugador;
    public static Jugador jugador => _Jugador;


    private void Awake()
    {
        //Se autoreferencia el GameManager
        self = this;
        _Jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();
        Awake_UI();
    }
    private void Update()
    {
        Update_UI();
    }
    #endregion CORE

    #region UI

    [Header("UI Monedas")]
    [SerializeField] private TMP_Text contadorMonedas;//Using TMpro;

    [Header("UI Pausa")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private Button botonReanudar;//using UnityEngine.UI;
    [SerializeField] private Button botonSalirPausa;

    [Header("UI Victoria")]
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private Button botonSalirVictoria;

    private void Awake_UI()
    {
        //Monedas
        contadorMonedas.text = "0";

        //Pausa
        panelPausa.SetActive(false);
        botonReanudar.onClick.AddListener(call: () => { EnPausa = false; });
        botonSalirPausa.onClick.AddListener(call: () => { Application.Quit(); });

        //Victoria
        panelVictoria.SetActive(false);
        botonSalirVictoria.onClick.AddListener(call: () => { Application.Quit(); });
    }


    private void Update_UI()
    {
        //Cuando presionamos ESC si invierte la Pausa
        if (Input.GetKeyDown(KeyCode.Escape)) EnPausa = !EnPausa;
    }
    //Funcion para cuando el jugador gane 

    public static void Victoria()
    {
        self.panelVictoria.SetActive(true);
    }

    private static int _monedas;
    public static int Monedas
    {
        get => _monedas;
        set
        {
            _monedas = value;
            // Lo mostramos en pantalla
            self.contadorMonedas.text = _monedas.ToString();
        }
    }

    private bool _enPausa;

    public bool EnPausa
    {
        get => _enPausa;
        set
        {
            _enPausa = value;

            //Mostramos el Menu
            panelPausa.SetActive(value);

            //Paramos o reanudamos el tiempo
            Time.timeScale = value ? 0 : 1;
        }
    }
}
    #endregion