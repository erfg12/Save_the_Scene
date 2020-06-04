using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controlador : MonoBehaviour {

    public static Controlador instancia;

    public GameObject FadeInCuadro;
    public Text puntuacionText;
    public Text conteoRegresivo;
    public GameObject[] MisBombas = new GameObject[4];
    public Sprite BombaRoja;
    private GameObject Bomba = null;
    public GameObject Final1;
    public GameObject Final2;
    public GameObject Final3;
    public GameObject Final4;

    public static int TouchedBtn = -1;

    public GameObject FXBomba;
    private GameObject FX;

    private int Conteo = 3;
    private int Puntuacion = 0;
    private int PuntuacionMax = 0;
    private string GameName = "0";
    private int BN = -1;
    public bool SoltarBomba = true;
    public bool Fallo = false;
    public float Gravedad = 0.15f;
    
    public bool GameOver = true;
    public bool Reinicar = false;
    private bool Continuar = false;

    void Awake()
    {
        FadeInCuadro.SetActive(true);
        Controlador.instancia = this;
    }
        
	void Start ()
    {
        PuntuacionMax = PlayerPrefs.GetInt("Puntuacion", 0);
        ActualizarPuntuacion();

        StartCoroutine(ConteoRegresivo());
	}

    public void ActualizarPuntuacion()
    {
        if (Convert.ToInt32(GameName) != Puntuacion)
        {
            GameOver = true;
            puntuacionText.text = "    You are a cheater, press X to try again";
            Reinicar = true;
        }
        else
        {
            if (Puntuacion > 100)
                puntuacionText.text = "    Score: " + Puntuacion + " \n    High Score: " + PuntuacionMax;
            else
            {
                puntuacionText.text = "    Bombs Left: " + (100 - Puntuacion).ToString() + " to kex\n    High Score: " + PuntuacionMax;

                if (100 - Puntuacion == 0)
                {
                    // ganaste...
                    GameOver = true;
                    puntuacionText.text = "    Score: " + Puntuacion + " \n    High Score: " + PuntuacionMax;
                    SistemaSonido.instancia.PlayGanaste();
                    Final1.SetActive(true);

                    StartCoroutine(MostrarFinal());
                }
            }
        }
    }

    private IEnumerator MostrarFinal()
    {
        yield return new WaitForSeconds(5f);

        Final1.SetActive(false);
        Final2.SetActive(true);

        yield return new WaitForSeconds(5f);
        Final2.SetActive(false);
        Final3.SetActive(true);

        yield return new WaitForSeconds(5f);
        Final3.SetActive(false);
        Final4.SetActive(true);

        Continuar = true;
    }

    private IEnumerator ConteoRegresivo()
    {
        yield return new WaitForSeconds(3f);

        FadeInCuadro.SetActive(false);
        conteoRegresivo.gameObject.SetActive(true);
        while (Conteo > 0)
        {
            SistemaSonido.instancia.PlayAudioBeep();

            Conteo--;
            yield return new WaitForSeconds(1f);
            conteoRegresivo.text = Conteo.ToString();
        }

        conteoRegresivo.gameObject.SetActive(false);
        GameOver = false;
        Fallo = false;

        SistemaSonido.instancia.PlayAudioGo();
    }
	
	void Update ()
    {
        if (Reinicar && (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.DownArrow)) || Input.GetKeyDown("a") || Input.GetKeyDown("b"))
        {
            SceneManager.LoadScene("main");
        }

        if (Continuar && (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.DownArrow)) || Input.GetKeyDown("a") || Input.GetKeyDown("b"))
        {
            Continuar = false;
            SistemaSonido.instancia.ReinicarMusica();
            Final4.SetActive(false);
            GameOver = false;
            return;
        }

        if (!GameOver)
        {
            // spawn bombs
            if (SoltarBomba)
            {
                SoltarBomba = false;

                BN = UnityEngine.Random.Range(0, 4);
                int X = UnityEngine.Random.Range(-2, 3);
                Bomba = Instantiate(MisBombas[BN], new Vector2(3 * X, 6), Quaternion.identity); // apply a rotation to bomb
                Bomba.GetComponent<Rigidbody2D>().gravityScale = IncrementarGravedad();

                Fallo = false;
                SistemaSonido.instancia.PlayBombaCayendo();
            }

            // check if we pressed the correct rotation as the bomb dropping
            if (!Fallo && (Input.anyKeyDown || TouchedBtn >= 0))
            {
                Debug.Log("TouchBtn: " + TouchedBtn);
                 // identify bomb key
                switch (BN)
                {
                    case 0:
                        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.DownArrow) || TouchedBtn == 2 || Input.GetKeyDown("b"))
                        {
                            DestruirBomba();
                        }
                        else
                        {
                            Fallaste();
                        }
                        break;
                    case 1:
                        if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.RightArrow) || TouchedBtn == 1 || Input.GetKeyDown("a"))
                        {
                            DestruirBomba();
                        }
                        else
                        {
                            Fallaste();
                        }
                        break;
                    case 2:
                        if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.LeftArrow) || TouchedBtn == 3 || Input.GetKeyDown("y"))
                        {
                            DestruirBomba();
                        }
                        else
                        {
                            Fallaste();
                        }
                        break;
                    case 3:
                        if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.UpArrow) || TouchedBtn == 0 || Input.GetKeyDown("x"))
                        {
                            DestruirBomba();
                        }
                        else
                        {
                            Fallaste();
                        }
                        break;
                }
            }
        }
	}

    private void Fallaste()
    {
        SistemaSonido.instancia.PlayDisparoFallido();
        Bomba.GetComponent<SpriteRenderer>().sprite = BombaRoja;
        Fallo = true;
        TouchedBtn = -1; // reset
    }

    private float IncrementarGravedad()
    {
        if (Gravedad < 3)
        {
            Gravedad += 0.05f;
        }

        return Gravedad;
    }

    public void DestruirBomba()
    {
        SistemaSonido.instancia.PlayDisparoAcertado();
        FX = Instantiate(FXBomba, Bomba.transform.position, Quaternion.identity);

        Destroy(Bomba.gameObject);
        Puntuacion++;
        GameName = (Convert.ToInt32(GameName) + 1).ToString();
        ActualizarPuntuacion();
        SoltarBomba = true;

        TouchedBtn = -1; // reset

        StartCoroutine(DestruirFX());
    }

    private IEnumerator DestruirFX()
    {
        yield return new WaitForSeconds(1f);
        Destroy(FX.gameObject);
    }

    public void SalvarPuntuacion()
    {
        if (Convert.ToInt32(GameName) == Puntuacion)
        {
            if (Puntuacion > PuntuacionMax)
            {
                PuntuacionMax = Puntuacion;
                PlayerPrefs.SetInt("Puntuacion", Puntuacion);
                PlayerPrefs.Save();
            }
        }
    }
}
