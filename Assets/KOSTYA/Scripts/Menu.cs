using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [SerializeField] private Slider[] sliders;
    [SerializeField] private TMP_Text[] slidersText;
    [SerializeField] private float[] sensRange;

    [SerializeField] private AudioMixer audioMixer;
    
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private string languagePrefName = "locale";
    [SerializeField] private Language[] languages;



    void Start(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

        slidersText[0].text = "" + PlayerPrefs.GetFloat("PlayerVolume", 1).ToString("F1");
        sliders[0].value = PlayerPrefs.GetFloat("PlayerVolume", 1);

        slidersText[1].text = "" + PlayerPrefs.GetFloat("PlayerSens", 3).ToString("F1");
        sliders[1].value = InverseLerp(sensRange[0], sensRange[1], PlayerPrefs.GetFloat("PlayerSens", 3));

        
        dropdown.AddOptions(languages.Select(language => new TMP_Dropdown.OptionData(language.name)).ToList());
        if (PlayerPrefs.HasKey(languagePrefName))
        {
            var language = PlayerPrefs.GetString(languagePrefName);
            for (var i = 0; i < languages.Length; i++)
                if (languages[i].tag == language)
                {
                    dropdown.SetValueWithoutNotify(i);
                    break;
                }
        }

        dropdown.onValueChanged.AddListener(ChangeLang);
    }

    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
            return Mathf.Clamp01((value - a) / (b - a));
        else
            return 0f;
    }

    public void changeSens(){
        float sens = Mathf.Lerp(sensRange[0], sensRange[1], sliders[1].value);
        PlayerPrefs.SetFloat("PlayerSens", sens);
        slidersText[1].text = "" + sens.ToString("F1");
    }

    public void changeVolume(){
        PlayerPrefs.SetFloat("PlayerVolume", sliders[0].value);
        slidersText[0].text = "" + sliders[0].value.ToString("F1");

        audioMixer.SetFloat("Volume", Mathf.Log10(sliders[0].value)*20);
    }
    
    public void ChangeLang(int state){
        PlayerPrefs.SetString(languagePrefName, languages[state].tag);
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
        PlayerPrefs.DeleteKey("CompressedGas");
        SceneManager.LoadScene("FACTORY");
    }
}

[Serializable]
public class Language
{
    public string name;
    public string tag;
}
