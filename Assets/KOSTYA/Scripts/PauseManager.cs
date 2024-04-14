using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject[] menus;

    [SerializeField] private TMP_Text[] buttons;
    private float[] defaultSize;

    [SerializeField] private Slider[] sliders;
    [SerializeField] private TMP_Text[] slidersText;

    [SerializeField] private float[] sensRange;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Animator animationFade;

    [SerializeField] private AudioSource audioSourceButtons;
    [SerializeField] private AudioClip[] buttonClips;

    [SerializeField] private FirstPersonController2 controller2;


    private bool paused;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            paused = !paused;
            if (paused) pause();
            else unPause();
        }
    }

    private float _timeScaleBuffer;
    void pause(){
        controller2.NoRotate();

        _timeScaleBuffer = Time.timeScale;
        if (_timeScaleBuffer > 0.015f) 
            Time.timeScale = 0.015f;
        
        pauseMenu.SetActive(true);
        menus[0].SetActive(true);
        menus[1].SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void unPause(){
        controller2.enablePlayer();
        Time.timeScale = _timeScaleBuffer;
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void hoverButton(int id){
        audioSourceButtons.clip = buttonClips[0];
        audioSourceButtons.pitch = Random.Range(0.7f, 1.2f);
        audioSourceButtons.Play();
        for(int i = 0; i < buttons.Length; i++)
            buttons[i].fontSize = defaultSize[i];
        
        buttons[id].fontSize += 20;
    }

    void Start(){
        defaultSize = new float[buttons.Length];
        for(int i = 0; i < buttons.Length; i++)
            defaultSize[i] = buttons[i].fontSize;

        slidersText[1].text = "" + PlayerPrefs.GetFloat("PlayerSens", 3).ToString("F1");
        sliders[1].value = InverseLerp(sensRange[0], sensRange[1], PlayerPrefs.GetFloat("PlayerSens", 3));

        slidersText[0].text = "" + PlayerPrefs.GetFloat("PlayerVolume", 1).ToString("F1");
        sliders[0].value = PlayerPrefs.GetFloat("PlayerVolume", 1);
    }

    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
            return Mathf.Clamp01((value - a) / (b - a));
        else
            return 0f;
    }

    void changeScene(){
        SceneManager.LoadScene("MENU");
    }

    public void clickButton(int id){
        audioSourceButtons.clip = buttonClips[1];
        audioSourceButtons.pitch = Random.Range(0.7f, 1.2f);
        audioSourceButtons.Play();
        buttons[id].fontSize = defaultSize[id] + 30;
        Invoke("resetButtons", 0.3f);

        if (id == 0){
            unPause();
        }
        else if (id == 1){
            menus[0].SetActive(false);
            menus[1].SetActive(true);
        }
        else if (id == 2){
            unPause();
            animationFade.Play("FadeIn", 0, 0);
            Invoke("changeScene", 3);
        }
        else if (id == 3){
            menus[1].SetActive(false);
            menus[0].SetActive(true);
        }
    }

    public void resetButtons(){
        for(int i = 0; i < buttons.Length; i++)
            buttons[i].fontSize = defaultSize[i];
    }

    public void changeSens(){
        float sens = Mathf.Lerp(sensRange[0], sensRange[1], sliders[1].value);
        if (controller2 != null) controller2.mouseSensitivity = sens;
        PlayerPrefs.SetFloat("PlayerSens", sens);
        slidersText[1].text = "" + sens.ToString("F1");
    }

    public void changeVolume(){
        PlayerPrefs.SetFloat("PlayerVolume", sliders[0].value);
        slidersText[0].text = "" + sliders[0].value.ToString("F1");

        audioMixer.SetFloat("Volume", Mathf.Log10(sliders[0].value)*20);
    }

    public void resetButton(int id){
        buttons[id].fontSize = defaultSize[id];
    }
}
