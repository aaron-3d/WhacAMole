using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{    
    public static GameController instance;
    public GameObject mainMenu, inGameUI,endScreen,recordPanel;
    public GameObject noRecordPanel;
    public Transform molesParent;
    private MoleBehaviour[] moles;

    public bool playing = false;

    public float gameDuration = 60f;
    public float timePlayed;
    public float tiempoRestante;

    public int points = 0;
    public float clicks = 0;
    public float failedClicks = 0;
    public float topoClicks = 0;
    public int score = 0;
    public int record = 0;

    float recordActual;

    float porcentajeClicks;

    public TMP_InputField nameField;
    string playerName;

    bool isRecord = false;

    public TextMeshProUGUI infoGame;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI recordText;
    public TextMeshProUGUI noRecordtext;

    void Awake()
    {
        if (GameController.instance == null)
        {
            ConfigureInstance();
        }
        else
        {
            Destroy(this);
        }
    }

    void ConfigureInstance()
    {
        //Configura acceso a moles
        moles = new MoleBehaviour[molesParent.childCount];
        for (int i = 0; i < molesParent.childCount; i++)
        {
            moles[i] = molesParent.GetChild(i).GetComponent<MoleBehaviour>();
        }

        //Inicia los puntos
        points = 0;
        clicks = -1;
        failedClicks = -1;

        //Activa la UI inicial
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (score > record)   //Actualiza el record si la puntuación de la partida es mayor.
        {
            record = score;
        }

        pointsText.text = "Puntos: " + score;    
        timerText.text = ("Tiempo: " + Mathf.Floor(tiempoRestante));
        noRecordtext.text = "Record actual por " + playerName + " con " + record + " puntos.";
        
    
        

        if (playing == true)
        {
            timePlayed += Time.deltaTime;
            tiempoRestante = gameDuration - timePlayed;

            failedClicks = clicks - topoClicks;  //Calcula los clicks fallados en función de los clicks totales y los clicks a topos o powerUps.
            

            if (timePlayed >= gameDuration)
            {
                
                ShowEndScreen();
                playing = false;
                for (int i = 0; i < moles.Length; i++)
                {
                    moles[i].StopMole();
                }

                if (tiempoRestante < 0)
                {
                    tiempoRestante = 0;
                    gameDuration = 0;
                }
            }
            else
            {
                CheckClicks();
            }
            
        }
        porcentajeClicks = topoClicks / clicks * 100;  //Calcula el porcentaje de clicks acertados operando los clicks a topos o powerUps entre los totales y multiplicandolo por 100.
    }


    void ShowEndScreen()
    {
        endScreen.SetActive(true);
        infoGame.text =
            " Total points : " + score
            + "\n Record: " + record + " " + playerName
            + "\n" + Mathf.Floor(porcentajeClicks) + "% de acierto." 
            + "\n" + failedClicks +" fallos.";

        GetRecord();
        CheckScore();
    }

        //si hay nuevo record mostrar el panel recordPanel


    /// <summary>
    /// Function called from End Screen when players hits Retry button
    /// </summary>
    public void Retry()
    {
        //Guardar record si es necesario

        //Acceso al texto escrito
        playerName = nameField.text;
        Debug.Log("Record de " + playerName);

        //Reinicia información del juego
        ResetGame();
        SetRecord();
        //Cambia las pantallas
        inGameUI.SetActive(true);
        mainMenu.SetActive(false);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
        noRecordPanel.SetActive(false);

        score = 0;

        //Activa juego
        playing = true;

        if (nameField.text == "")
        {
            playerName = "Aarón";
        }

        //Reinicia moles
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole();
        }
    }

    /// <summary>
    /// Restarts all info game
    /// </summary>
    void ResetGame()
    {
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].StopMole();
        }
        gameDuration = 60f;
        timePlayed = 0.0f;
        points = 0;
        clicks = -1f;
        failedClicks = -1f;
        porcentajeClicks = 0f;
        topoClicks = 0f;

        if (nameField.text == "")
        {
            playerName = "Aarón";
        }
    }

    public void EnterMainScreen()
    {
        playerName = "Aarón";
        //Reinicia información del juego+
        SetRecord();
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
    }

    /// <summary>
    /// Used to check if players hits or not the moles/powerups
    /// </summary>
    public void CheckClicks()
    {
        if ((Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
            if (playing == true)
            {
                clicks++; //Añade 1 a clicks cada vez que se clica durante la partida.
            }

            Vector3 pos = Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android)
            {
                pos = Input.GetTouch(0).position;
            }

            Ray rayo = Camera.main.ScreenPointToRay(pos);
            RaycastHit hitInfo;
            if (Physics.Raycast(rayo, out hitInfo))
            {
                //Debug.Log("Porcentaje de clicks: " + porcentajeClicks);

                if (hitInfo.collider.tag.Equals("Mole"))
                {
                    MoleBehaviour mole = hitInfo.collider.GetComponent<MoleBehaviour>();
                    if (mole != null)
                    {
                        mole.OnHitMole();
                        score += 100;    //Suma 100 puntos y añade 1 al contador de clicks a topos o powerUps.
                        topoClicks++;
                    }
                }
               else if (hitInfo.collider.tag.Equals("PowerUp"))
                {
                    MoleBehaviour mole = hitInfo.collider.GetComponent<MoleBehaviour>();
                    if (mole != null)
                    {
                        //mole.OnHitMole();
                        //score += 100;
                        topoClicks++;
                    }
                }
            }
        }
    }

    public void OnGameStart()
    {
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);
        points = 0;
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole(moles[i].initTimeMin, moles[i].initTimeMax);
        }
        playing = true;

        SetRecord();
        Debug.Log("El record es: " + record);     
    }

    /// <summary>
    /// Funcion para entrar en pausa, pone playing en false y muestra la pantalla de pausa.
    /// </summary>
    public void EnterOnPause()
    { 
    
    
    }

    public void SetRecord()
    {
        if (score > PlayerPrefs.GetInt("highScore", record))
        {           
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.SetString("playerName", playerName);
            PlayerPrefs.Save();
            recordText.text = playerName + record.ToString();
        }      
    }
    public void GetRecord()
    {
        if (score > PlayerPrefs.GetInt("highScore", record))
        {
            PlayerPrefs.SetString("playerName", playerName);
        }
    }

    public void CheckScore()
    {
    if (score >= record) // Cambia isRecord a true si la puntuación es mayor al record.
        {
            isRecord = true;

            if (isRecord == true)
            {
                recordPanel.SetActive(isRecord);
            }

        }

        else
        {
            noRecordPanel.SetActive(true);
        }
    }

}
