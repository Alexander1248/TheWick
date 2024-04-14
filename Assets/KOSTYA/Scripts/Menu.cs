using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private Vector3 startingPos;
    [SerializeField] private float radius;
    [SerializeField] private float speed;
    [SerializeField] private Transform cam;
    private float t;

    [SerializeField] private Animator animatorFade;

    private float yEndBlack;
    [SerializeField] private Transform black;
    [SerializeField] private TMP_Text[] buttons;
    private int currentSelected = -1;

    private bool cliked;
    private float[] fontSizes_def;
    private float[] fontSizes_hov;
    private float[] fontSizes_click;

    [SerializeField] private GameObject[] menus;

    [SerializeField] private AudioClip[] clipsUI;
    [SerializeField] private AudioSource audioSource;

    void Start(){
        fontSizes_def = new float[buttons.Length];
        fontSizes_hov = new float[buttons.Length];
        fontSizes_click = new float[buttons.Length];
        for(int i = 0; i < buttons.Length; i++){
            fontSizes_def[i] = buttons[i].fontSize;
            fontSizes_hov[i] = buttons[i].fontSize + 15;
            fontSizes_click[i] = buttons[i].fontSize + 30;
        }

        yEndBlack = black.localPosition.y; 

        startingPos = cam.position;
    }

    void Update()
    {
        t += Time.deltaTime * speed;
        Vector3 offset = new Vector3(0, Mathf.Sin(t), Mathf.Cos(t)) * radius;
        cam.transform.position = startingPos + offset;

        black.localPosition = Vector3.MoveTowards(black.localPosition,
                                new Vector3(black.localPosition.x, yEndBlack, black.localPosition.z), Time.deltaTime * 5000);
    }

    public void onHover(int id){
        if (cliked) return;
        audioSource.clip = clipsUI[0];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
        if (currentSelected != -1) buttons[currentSelected].fontSize = fontSizes_def[currentSelected];

        buttons[id].fontSize = fontSizes_hov[id];
        yEndBlack = buttons[id].transform.localPosition.y; 

        currentSelected = id;
    }

    public void onClick(int id){
        if (cliked) return;
        audioSource.clip = clipsUI[1];
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
        buttons[id].fontSize = fontSizes_click[id];
        Invoke("processClick", 0.1f);
        cliked = true;
    }

    void processClick(){
        buttons[currentSelected].fontSize = fontSizes_def[currentSelected];
        cliked = false;

        if (currentSelected == 0){
            //PlayerPrefs.SetInt("SaveStage", 0);
            animatorFade.Play("FadeIn", 0, 0);
            Invoke("StartGame", 2f);
            cliked = true;
        }
        else if (currentSelected == 1){
            menus[0].SetActive(false);
            menus[1].SetActive(true);
        }
        else if (currentSelected == 2){
            Application.Quit();
        }
        else if (currentSelected == 3){
            menus[0].SetActive(true);
            menus[1].SetActive(false);
            onHover(0);
        }
    }

    public void onExit(int id){
        if (cliked) return;
        if (currentSelected != -1) buttons[currentSelected].fontSize = fontSizes_def[currentSelected];
        currentSelected = -1;
    }

    void StartGame(){
        SceneManager.LoadScene("FACTORY");
    }
}
